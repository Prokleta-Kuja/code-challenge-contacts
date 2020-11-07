using MediatR;
using PublicContacts.App.Models;

namespace PublicContacts.App.Notifications
{
    public class PhoneNumberRemovedNotification : INotification
    {
        public PhoneNumberRemovedNotification(ContactViewModel contact, PhoneNumberViewModel phoneNumber)
        {
            Contact = contact;
            PhoneNumber = phoneNumber;
        }
        public ContactViewModel Contact { get; set; }
        public PhoneNumberViewModel PhoneNumber { get; set; }
    }
}