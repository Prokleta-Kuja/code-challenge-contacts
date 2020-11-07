using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhoneNumbers;
using PublicContacts.App.Contexts;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Models;
using PublicContacts.App.Notifications;
using PublicContacts.App.Validators;

namespace PublicContacts.App.Actions
{
    public class CreatePhoneNumberCommand : IRequest<PhoneNumberViewModel>
    {
        public int ContactId { get; set; }
        public string Number { get; set; } = null!;

    }
    public class CreatePhoneNumberValidator : AbstractValidator<CreatePhoneNumberCommand>
    {
        public CreatePhoneNumberValidator()
        {
            RuleFor(pn => pn.ContactId).GreaterThan(0);
            RuleFor(pn => pn.Number).PhoneNumber();
        }
    }
    public class CreatePhoneNumberHandler : IRequestHandler<CreatePhoneNumberCommand, PhoneNumberViewModel>
    {
        private readonly ILogger<CreatePhoneNumberHandler> _logger;
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;
        public CreatePhoneNumberHandler(ILogger<CreatePhoneNumberHandler> logger, IAppDbContext db, IMediator mediator)
        {
            _logger = logger;
            _db = db;
            _mediator = mediator;
        }
        public async Task<PhoneNumberViewModel> Handle(CreatePhoneNumberCommand request, CancellationToken cancellationToken = default)
        {
            var contact = await _db.Contacts.SingleOrDefaultAsync(c => c.Id == request.ContactId, cancellationToken);
            if (contact == null)
                throw new RequestException(nameof(request.ContactId), "Contact with specified Id not found");

            var lib = PhoneNumberUtil.GetInstance();
            var parsed = lib.Parse(request.Number, "HR");

            var number = new Domain.PhoneNumber
            {
                ContactId = request.ContactId,
                Number = lib.Format(parsed, PhoneNumberFormat.E164)
            };

            _db.PhoneNumbers.Add(number);
            await _db.SaveChangesAsync(cancellationToken);

            var result = new PhoneNumberViewModel(number);
            _logger.LogDebug("Phone number {@PhoneNumber} added to contact {ContactName} with id {ContactId}", number, contact.Name, contact.Id);

            var notification = new PhoneNumberAddedNotification(new ContactViewModel(contact), result);
            await _mediator.Publish(notification);

            return result;
        }
    }
}