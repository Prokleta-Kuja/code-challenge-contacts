using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PublicContacts.App.Actions;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Notifications;
using PublicContacts.App.Tests;
using Xunit;

namespace PublicContacty.App.Tests.Actions
{
    public class RemoveContactTests
    {
        private ILogger<RemoveContactHandler> _logger = new Mock<ILogger<RemoveContactHandler>>().Object;
        private Mock<IMediator> _mediator = new Mock<IMediator>();

        [Fact]
        public void Throws_ValidationException_Id()
        {
            // Arrange
            var sut = new RemoveContactValidator();
            var cmd = new RemoveContactCommand { Id = default };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Id));
        }

        [Fact]
        public async Task Throws_RequestException_Id()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext();

            // Arrange
            var sut = new RemoveContactHandler(_logger, context, _mediator.Object);
            var cmd = new RemoveContactCommand { Id = int.MaxValue };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequestException>(() => sut.Handle(cmd));
            Assert.True(ex.Key == nameof(cmd.Id));
            _mediator.Verify(m => m.Publish(It.IsAny<ContactRemovedNotification>(), default), Times.Never());
        }

        [Fact]
        public async Task OK()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new RemoveContactHandler(_logger, context, _mediator.Object);
            var contacts = context.Contacts.ToList();
            var cmd = new RemoveContactCommand { Id = contacts[0].Id };

            // Act
            await sut.Handle(cmd);

            // Assert  
            Assert.True(contacts.Count > context.Contacts.Count());
            _mediator.Verify(m => m.Publish(It.IsAny<ContactRemovedNotification>(), default), Times.Once());
        }
    }
}