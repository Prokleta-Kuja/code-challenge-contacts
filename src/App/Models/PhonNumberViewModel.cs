using PublicContacts.Domain;

namespace PublicContacts.App.Models
{
    public class PhoneNumberViewModel
    {
        public int Id { get; set; }
        public string Number { get; set; }

        public PhoneNumberViewModel(PhoneNumber pn)
        {
            Id = pn.Id;
            Number = pn.Number;
        }
    }
}