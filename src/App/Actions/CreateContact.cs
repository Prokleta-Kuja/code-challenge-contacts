using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PublicContacts.App.Contexts;
using PublicContacts.App.Exceptions;
using PublicContacts.App.Models;
using PublicContacts.App.Notifications;
using PublicContacts.Domain;

namespace PublicContacts.App.Actions
{
    public class CreateContactCommand : IRequest<ContactViewModel>
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
    public class CreateContactValidator : AbstractValidator<CreateContactCommand>
    {
        public CreateContactValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(Constants.MaxLengths.ContactName);

            RuleFor(c => c.Address)
                .NotEmpty()
                .MaximumLength(Constants.MaxLengths.ContactAddress);

            RuleFor(c => c.DateOfBirth)
                .NotEqual(default(DateTime))
                .LessThan(DateTime.Now);
        }
    }
    public class CreateContactHandler : IRequestHandler<CreateContactCommand, ContactViewModel>
    {
        private readonly ILogger<CreateContactHandler> _logger;
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;
        public CreateContactHandler(ILogger<CreateContactHandler> logger, IAppDbContext db, IMediator mediator)
        {
            _logger = logger;
            _db = db;
            _mediator = mediator;
        }
        public async Task<ContactViewModel> Handle(CreateContactCommand request, CancellationToken cancellationToken = default)
        {
            var contact = new Contact
            {
                Name = request.Name,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
            };

            if (_db.Contacts.Any(c => c.Hash == contact.Hash))
                throw new RequestException("Contact with that name and address already exists");

            _db.Contacts.Add(contact);
            await _db.SaveChangesAsync(cancellationToken);

            var result = new ContactViewModel(contact);
            _logger.LogDebug("Contact {@Contact} added", result);

            await _mediator.Publish(new ContactCreatedNotification(result));

            return result;
        }
    }
}