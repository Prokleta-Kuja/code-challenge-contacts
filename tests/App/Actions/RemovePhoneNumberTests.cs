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
    public class RemovePhoneNumberTests
    {
        private ILogger<RemovePhoneNumberHandler> _logger = new Mock<ILogger<RemovePhoneNumberHandler>>().Object;
        private Mock<IMediator> _mediator = new Mock<IMediator>();

        [Fact]
        public void Throws_ValidationException_Id()
        {
            // Arrange
            var sut = new RemovePhoneNumberValidator();
            var cmd = new RemovePhoneNumberCommand { Id = default, ContactId = int.MaxValue };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Id));
        }

        [Fact]
        public void Throws_ValidationException_ContactId()
        {
            // Arrange
            var sut = new RemovePhoneNumberValidator();
            var cmd = new RemovePhoneNumberCommand { Id = int.MaxValue, ContactId = default };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.ContactId));
        }

        [Fact]
        public async Task Throws_RequestException_Contact_Not_Found()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new RemovePhoneNumberHandler(_logger, context, _mediator.Object);
            var contact = context.Contacts.First();
            var cmd = new RemovePhoneNumberCommand { Id = int.MaxValue, ContactId = contact.Id };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequestException>(() => sut.Handle(cmd));
            Assert.True(ex.Key == nameof(cmd.Id));
            _mediator.Verify(m => m.Publish(It.IsAny<PhoneNumberRemovedNotification>(), default), Times.Never());
        }

        [Fact]
        public async Task Throws_RequestException_PhoneNumber_Not_Found()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new RemovePhoneNumberHandler(_logger, context, _mediator.Object);
            var pn = context.PhoneNumbers.First();
            var cmd = new RemovePhoneNumberCommand { Id = pn.Id, ContactId = int.MaxValue };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequestException>(() => sut.Handle(cmd));
            Assert.True(ex.Key == nameof(cmd.ContactId));
            _mediator.Verify(m => m.Publish(It.IsAny<PhoneNumberRemovedNotification>(), default), Times.Never());
        }

        [Fact]
        public async Task OK()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new RemovePhoneNumberHandler(_logger, context, _mediator.Object);
            var contact = context.Contacts
                .Include(c => c.PhoneNumbers)
                .First();
            var initialCount = contact.PhoneNumbers!.Count;
            var cmd = new RemovePhoneNumberCommand { Id = contact.PhoneNumbers.First().Id, ContactId = contact.Id };

            // Act
            await sut.Handle(cmd);

            // Assert  
            Assert.True(initialCount > context.PhoneNumbers.Where(pn => pn.ContactId == contact.Id).Count());
            _mediator.Verify(m => m.Publish(It.IsAny<PhoneNumberRemovedNotification>(), default), Times.Once());
        }
    }
}