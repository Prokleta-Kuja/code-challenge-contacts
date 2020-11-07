using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PublicContacts.App.Notifications;

namespace PublicContacts.Api.Hubs.Dispatchers
{
    public class PhoneNumberAddedDispatcher : INotificationHandler<PhoneNumberAddedNotification>
    {
        private readonly IHubContext<ContactEventsHub> _hubContext;
        public PhoneNumberAddedDispatcher(IHubContext<ContactEventsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(PhoneNumberAddedNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync(nameof(PhoneNumberAddedNotification), notification, cancellationToken);
        }
    }
}