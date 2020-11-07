using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PublicContacts.App.Notifications;

namespace PublicContacts.Api.Hubs.Dispatchers
{
    public class ContactRemovedDispatcher : INotificationHandler<ContactRemovedNotification>
    {
        private readonly IHubContext<ContactEventsHub> _hubContext;
        public ContactRemovedDispatcher(IHubContext<ContactEventsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(ContactRemovedNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync(nameof(ContactRemovedNotification), notification, cancellationToken);
        }
    }
}