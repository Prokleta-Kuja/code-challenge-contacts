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
    public class RemoveContactCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class RemoveContactValidator : AbstractValidator<RemoveContactCommand>
    {
        public RemoveContactValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);
        }
    }
    public class RemoveContactHandler : IRequestHandler<RemoveContactCommand>
    {
        private readonly ILogger<RemoveContactHandler> _logger;
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;
        public RemoveContactHandler(ILogger<RemoveContactHandler> logger, IAppDbContext db, IMediator mediator)
        {
            _logger = logger;
            _db = db;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(RemoveContactCommand request, CancellationToken cancellationToken = default)
        {
            var contact = await _db.Contacts.SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (contact == null)
                throw new RequestException(nameof(request.Id), "Contact with specified Id not found");

            _db.Contacts.Remove(contact);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Contact {@Contact} removed", contact);

            var notification = new ContactRemovedNotification(new ContactViewModel(contact));
            await _mediator.Publish(notification);

            return Unit.Value;
        }
    }
}