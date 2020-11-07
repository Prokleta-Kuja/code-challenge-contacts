using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PublicContacts.App.Actions;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Notifications;
using PublicContacts.App.Tests;
using PublicContacts.Domain;
using Xunit;

namespace PublicContacty.App.Tests.Actions
{
    public class UpdateContactTests
    {
        private ILogger<UpdateContactHandler> _logger = new Mock<ILogger<UpdateContactHandler>>().Object;
        private Mock<IMediator> _mediator = new Mock<IMediator>();

        [Fact]
        public void Throws_ValidationException_Id()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand { Id = default };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Id));
        }

        [Fact]
        public void Throws_ValidationException_Name_Empty()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand
            {
                Id = int.MaxValue,
                Name = string.Empty,
                Address = "A",
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Name));
        }

        [Fact]
        public void Throws_ValidationException_Name_Too_Long()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand
            {
                Id = int.MaxValue,
                Name = new String('0', Constants.MaxLengths.ContactName + 1),
                Address = "A",
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Name));
        }

        [Fact]
        public void Throws_ValidationException_Address_Empty()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand
            {
                Id = int.MaxValue,
                Name = "A",
                Address = string.Empty,
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Address));
        }

        [Fact]
        public void Throws_ValidationException_Address_Too_Long()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand
            {
                Id = int.MaxValue,
                Name = "A",
                Address = new String('0', Constants.MaxLengths.ContactAddress + 1),
                DateOfBirth = DateTime.Now
            };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.Address));
        }

        [Fact]
        public void Throws_ValidationException_DateOfBirth_Default()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand { Id = int.MaxValue, Name = "A", Address = "A" };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.DateOfBirth));
        }

        [Fact]
        public void Throws_ValidationException_DateOfBirth_In_Future()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand
            {
                Id = int.MaxValue,
                Name = "A",
                Address = "A",
                DateOfBirth = DateTime.Now.AddDays(1)
            };

            // Act
            var result = sut.Validate(cmd);

            // Assert            
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.DateOfBirth));
        }

        [Fact]
        public void Validation_OK()
        {
            // Arrange
            var sut = new UpdateContactValidator();
            var cmd = new UpdateContactCommand
            {
                Id = int.MaxValue,
                Name = "A",
                Address = "A",
                DateOfBirth = DateTime.Now.AddDays(-1)
            };

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
            var sut = new UpdateContactHandler(_logger, context, _mediator.Object);
            var cmd = new UpdateContactCommand
            {
                Id = int.MaxValue,
                Name = "A",
                Address = "A",
                DateOfBirth = DateTime.Now.AddDays(-1)
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequestException>(() => sut.Handle(cmd));
            _mediator.Verify(m => m.Publish(It.IsAny<ContactUpdatedNotification>(), default), Times.Never());
        }

        [Fact]
        public async Task Throws_RequestException_Contact_Exists()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new UpdateContactHandler(_logger, context, _mediator.Object);
            var contacts = context.Contacts.ToList();
            var c1 = contacts.First();
            var c2 = contacts.Last();
            var cmd = new UpdateContactCommand
            {
                Id = c2.Id,
                Name = c1.Name.ToLower(),
                Address = c1.Address.ToUpper(),
                DateOfBirth = c1.DateOfBirth
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequestException>(() => sut.Handle(cmd));
            _mediator.Verify(m => m.Publish(It.IsAny<ContactUpdatedNotification>(), default), Times.Never());
        }

        [Fact]
        public async Task OK()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new UpdateContactHandler(_logger, context, _mediator.Object);
            var contact = context.Contacts.First();
            var cmd = new UpdateContactCommand
            {
                Id = contact.Id,
                Name = "C",
                Address = "C",
                DateOfBirth = DateTime.Now.AddDays(-1)
            };

            // Act
            var result = await sut.Handle(cmd);

            // Assert  
            Assert.True(contact.Name == "C");
            Assert.True(contact.Address == "C");
            _mediator.Verify(m => m.Publish(It.IsAny<ContactUpdatedNotification>(), default), Times.Once());
        }
    }
}