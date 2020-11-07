using MediatR;
using PublicContacts.App.Models;

namespace PublicContacts.App.Notifications
{
    public class ContactUpdatedNotification : INotification
    {
        public ContactUpdatedNotification(ContactViewModel contact)
        {
            Contact = contact;
        }
        public ContactViewModel Contact { get; set; }
    }
}