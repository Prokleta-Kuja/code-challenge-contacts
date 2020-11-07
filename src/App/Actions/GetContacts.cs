using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublicContacts.App.Contexts;
using PublicContacts.App.Extensions;
using PublicContacts.App.Models;
using PublicContacts.Domain;

namespace PublicContacts.App.Actions
{
    public class GetContactsQuery : IRequest<IEnumerable<ContactViewModel>>, IFilterable, ISortable, IPageable
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 32;
        public string? Term { get; set; }
        public string? SortBy { get; set; }
        public bool Ascending { get; set; } = true;
    }
    public class GetContactsHandler : IRequestHandler<GetContactsQuery, IEnumerable<ContactViewModel>>
    {
        private readonly ILogger<GetContactsHandler> _logger;
        private readonly IAppDbContext _db;
        public GetContactsHandler(ILogger<GetContactsHandler> logger, IAppDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<IEnumerable<ContactViewModel>> Handle(GetContactsQuery request, CancellationToken cancellationToken = default)
        {
            var contacts = await _db.Contacts
                .AsNoTracking()
                .Include(c => c.PhoneNumbers)
                .Filter(request, nameof(Contact.Name), nameof(Contact.Address))
                .Sort(request, nameof(Contact.Name))
                .Page(request)
                .Select(c => new ContactViewModel(c))
                .ToListAsync(cancellationToken);

            return contacts;
        }
    }
}