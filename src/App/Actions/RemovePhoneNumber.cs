using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublicContacts.App.Contexts;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Models;
using PublicContacts.App.Notifications;

namespace PublicContacts.App.Actions
{
    public class RemovePhoneNumberCommand : IRequest
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
    }
    public class RemovePhoneNumberValidator : AbstractValidator<RemovePhoneNumberCommand>
    {
        public RemovePhoneNumberValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);
            RuleFor(c => c.ContactId).GreaterThan(0);
        }
    }
    public class RemovePhoneNumberHandler : IRequestHandler<RemovePhoneNumberCommand>
    {
        private readonly ILogger<RemovePhoneNumberHandler> _logger;
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;
        public RemovePhoneNumberHandler(ILogger<RemovePhoneNumberHandler> logger, IAppDbContext db, IMediator mediator)
        {
            _logger = logger;
            _db = db;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(RemovePhoneNumberCommand request, CancellationToken cancellationToken = default)
        {
            var contact = await _db.Contacts
                .Include(c => c.PhoneNumbers)
                .SingleOrDefaultAsync(c => c.Id == request.ContactId, cancellationToken);

            if (contact == null)
                throw new RequestException(nameof(request.ContactId), "Contact with specified Id not found");

            var number = contact.PhoneNumbers.SingleOrDefault(pn => pn.Id == request.Id);
            if (number == null)
                throw new RequestException(nameof(request.Id), "Contact does not contain number with specified Id");

            _db.PhoneNumbers.Remove(number);
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Phone number {@PhoneNumber} removed", number);

            var notification = new PhoneNumberRemovedNotification(new ContactViewModel(contact), new PhoneNumberViewModel(number));
            await _mediator.Publish(notification);

            return Unit.Value;
        }
    }
}