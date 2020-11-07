using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PublicContacts.App.Actions;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Notifications;
using PublicContacts.App.Tests;
using Xunit;

namespace PublicContacty.App.Tests.Actions
{
    public class CreatePhoneNumberTests
    {
        const string VALID_NUMBER = "+385911234567";
        private ILogger<CreatePhoneNumberHandler> _logger = new Mock<ILogger<CreatePhoneNumberHandler>>().Object;
        private Mock<IMediator> _mediator = new Mock<IMediator>();

        [Fact]
        public void Throws_ValidationException_ContactId()
        {
            // Arrange
            var sut = new CreatePhoneNumberValidator();
            var cmd = new CreatePhoneNumberCommand { ContactId = default, Number = VALID_NUMBER };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.ContactId));
        }

        [Fact]
        public void Throws_ValidationException_Number()
        {
            // Arrange
            var sut = new CreatePhoneNumberValidator();
            var cmd = new CreatePhoneNumberCommand { ContactId = int.MaxValue, Number = "123" };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Number));
        }

        [Fact]
        public void Validation_OK()
        {
            // Arrange
            var sut = new CreatePhoneNumberValidator();
            var cmd = new CreatePhoneNumberCommand { ContactId = int.MaxValue, Number = VALID_NUMBER };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Throws_RequestException_Contact_Not_Found()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext();

            // Arrange
            var sut = new CreatePhoneNumberHandler(_logger, context, _mediator.Object);
            var cmd = new CreatePhoneNumberCommand { ContactId = int.MaxValue, Number = VALID_NUMBER };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequestException>(() => sut.Handle(cmd));
            Assert.True(ex.Key == nameof(cmd.ContactId));
            _mediator.Verify(m => m.Publish(It.IsAny<PhoneNumberAddedNotification>(), default), Times.Never());
        }

        [Fact]
        public async Task OK()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new CreatePhoneNumberHandler(_logger, context, _mediator.Object);
            var contact = context.Contacts
                .Include(c => c.PhoneNumbers)
                .First();
            var initialCount = contact.PhoneNumbers!.Count;
            var cmd = new CreatePhoneNumberCommand { ContactId = contact.Id, Number = VALID_NUMBER };

            // Act
            var result = await sut.Handle(cmd);

            // Assert  
            Assert.True(initialCount < context.Contacts.SelectMany(c => c.PhoneNumbers).Count());
            _mediator.Verify(m => m.Publish(It.IsAny<PhoneNumberAddedNotification>(), default), Times.Once());
        }
    }
}