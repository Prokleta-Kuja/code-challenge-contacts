using MediatR;
using PublicContacts.App.Models;

namespace PublicContacts.App.Notifications
{
    public class ContactCreatedNotification : INotification
    {
        public ContactCreatedNotification(ContactViewModel contact)
        {
            Contact = contact;
        }
        public ContactViewModel Contact { get; set; }
    }
}