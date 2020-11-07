using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PublicContacts.App.Notifications;

namespace PublicContacts.Api.Hubs.Dispatchers
{
    public class PhoneNumberRemovedDispatcher : INotificationHandler<PhoneNumberRemovedNotification>
    {
        private readonly IHubContext<ContactEventsHub> _hubContext;
        public PhoneNumberRemovedDispatcher(IHubContext<ContactEventsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(PhoneNumberRemovedNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync(nameof(PhoneNumberRemovedNotification), notification, cancellationToken);
        }
    }
}