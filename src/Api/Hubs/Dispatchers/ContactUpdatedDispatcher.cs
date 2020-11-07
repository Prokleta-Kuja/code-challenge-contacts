using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PublicContacts.App.Notifications;

namespace PublicContacts.Api.Hubs.Dispatchers
{
    public class ContactUpdatedDispatcher : INotificationHandler<ContactUpdatedNotification>
    {
        private readonly IHubContext<ContactEventsHub> _hubContext;
        public ContactUpdatedDispatcher(IHubContext<ContactEventsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(ContactUpdatedNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync(nameof(ContactUpdatedNotification), notification, cancellationToken);
        }
    }
}