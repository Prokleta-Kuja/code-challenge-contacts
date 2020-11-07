using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PublicContacts.App.Actions;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Tests;
using Xunit;

namespace PublicContacty.App.Tests.Actions
{
    public class GetContactByIdTests
    {
        private ILogger<GetContactByIdHandler> _logger = new Mock<ILogger<GetContactByIdHandler>>().Object;

        [Fact]
        public void Throws_ValidationException_Id()
        {
            // Arrange
            var sut = new GetContactByIdValidator();
            var cmd = new GetContactByIdQuery { Id = default };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Id));
        }

        [Fact]
        public void Validation_OK()
        {
            // Arrange
            var sut = new GetContactByIdValidator();
            var cmd = new GetContactByIdQuery { Id = int.MaxValue, };

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
            var sut = new GetContactByIdHandler(_logger, context);
            var cmd = new GetContactByIdQuery { Id = int.MaxValue, };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequestException>(() => sut.Handle(cmd));
        }

        [Fact]
        public async Task OK()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new GetContactByIdHandler(_logger, context);
            var contact = context.Contacts.First();
            var cmd = new GetContactByIdQuery { Id = contact.Id, };

            // Act
            var result = await sut.Handle(cmd);

            // Assert  
            Assert.True(contact.Name == "A");
            Assert.True(contact.Address == "A");
        }
    }
}