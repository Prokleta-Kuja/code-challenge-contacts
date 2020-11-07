using MediatR;
using PublicContacts.App.Models;

namespace PublicContacts.App.Notifications
{
    public class ContactRemovedNotification : INotification
    {
        public ContactRemovedNotification(ContactViewModel contact)
        {
            Contact = contact;
        }
        public ContactViewModel Contact { get; set; }
    }
}