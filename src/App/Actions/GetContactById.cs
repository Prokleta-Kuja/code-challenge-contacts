using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublicContacts.App.Contexts;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Models;

namespace PublicContacts.App.Actions
{
    public class GetContactByIdQuery : IRequest<ContactViewModel>
    {
        public int Id { get; set; }
    }
    public class GetContactByIdValidator : AbstractValidator<GetContactByIdQuery>
    {
        public GetContactByIdValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);
        }
    }
    public class GetContactByIdHandler : IRequestHandler<GetContactByIdQuery, ContactViewModel>
    {
        private readonly ILogger<GetContactByIdHandler> _logger;
        private readonly IAppDbContext _db;
        public GetContactByIdHandler(ILogger<GetContactByIdHandler> logger, IAppDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<ContactViewModel> Handle(GetContactByIdQuery request, CancellationToken cancellationToken = default)
        {
            var contact = await _db.Contacts
                .AsNoTracking()
                .Include(c => c.PhoneNumbers)
                .SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (contact == null)
                throw new RequestException(nameof(request.Id), "Contact with specified Id not found");

            var result = new ContactViewModel(contact);
            return result;
        }
    }
}