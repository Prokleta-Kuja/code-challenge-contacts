using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PublicContacts.App.Actions;
using Xunit;

namespace PublicContacts.App.Tests
{
    public class GetContactsTests
    {
        private ILogger<GetContactsHandler> _logger = new Mock<ILogger<GetContactsHandler>>().Object;

        [Fact]
        public async Task Filtered_Results()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new GetContactsHandler(_logger, context);
            var contact = context.Contacts.First();
            var cmd = new GetContactsQuery { Term = contact.Name };

            // Act
            var results = await sut.Handle(cmd);

            // Assert  
            Assert.Contains(results, r => r.Name == contact.Name);
            Assert.True(results.Count() == 1);
        }

        [Fact]
        public async Task Sorted_Results()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new GetContactsHandler(_logger, context);
            var contact = context.Contacts.First();
            var cmd = new GetContactsQuery { SortBy = nameof(contact.Name), Ascending = false };

            // Act
            var results = await sut.Handle(cmd);

            // Assert  
            Assert.True(results.Last().Name == contact.Name);
            Assert.True(results.Count() == 2);
        }

        [Fact]
        public async Task Paged_Results()
        {
            using var factory = new AppDbContextFactory();
            using var context = factory.CreateContext(true);

            // Arrange
            var sut = new GetContactsHandler(_logger, context);
            var contact = context.Contacts.First();
            var cmd = new GetContactsQuery { Skip = 1, Take = 1 };

            // Act
            var results = await sut.Handle(cmd);

            // Assert  
            Assert.DoesNotContain(results, r => r.Name == contact.Name);
            Assert.True(results.Count() == 1);
        }
    }
}