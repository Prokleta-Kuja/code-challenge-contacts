using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PublicContacts.App.Notifications;

namespace PublicContacts.Api.Hubs.Dispatchers
{
    public class ContactCreatedDispatcher : INotificationHandler<ContactCreatedNotification>
    {
        private readonly IHubContext<ContactEventsHub> _hubContext;
        public ContactCreatedDispatcher(IHubContext<ContactEventsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(ContactCreatedNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync(nameof(ContactCreatedNotification), notification, cancellationToken);
        }
    }
}