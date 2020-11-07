using System;
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
using PublicContacts.Domain;

namespace PublicContacts.App.Actions
{
    public class UpdateContactCommand : IRequest<ContactViewModel>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
    public class UpdateContactValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);

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
    public class UpdateContactHandler : IRequestHandler<UpdateContactCommand, ContactViewModel>
    {
        private readonly ILogger<UpdateContactHandler> _logger;
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;
        public UpdateContactHandler(ILogger<UpdateContactHandler> logger, IAppDbContext db, IMediator mediator)
        {
            _logger = logger;
            _db = db;
            _mediator = mediator;
        }
        public async Task<ContactViewModel> Handle(UpdateContactCommand request, CancellationToken cancellationToken = default)
        {
            var contact = await _db.Contacts.SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (contact == null)
                throw new RequestException(nameof(request.Id), "Contact with specified Id not found");

            contact.Name = request.Name;
            contact.Address = request.Address;
            contact.DateOfBirth = request.DateOfBirth;

            if (_db.Contacts.Any(c => c.Hash == contact.Hash && c.Id != contact.Id))
                throw new RequestException("Contact with that name and address already exists");

            await _db.SaveChangesAsync(cancellationToken);

            var result = new ContactViewModel(contact);
            _logger.LogDebug("Contact {@Contact} updated", result);

            await _mediator.Publish(new ContactUpdatedNotification(result));

            return result;
        }
    }
}