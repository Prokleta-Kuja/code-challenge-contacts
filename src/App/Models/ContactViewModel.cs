using System;
using System.Collections.Generic;
using System.Linq;
using PublicContacts.Domain;

namespace PublicContacts.App.Models
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IEnumerable<PhoneNumberViewModel>? PhoneNumbers { get; set; }

        public ContactViewModel(Contact c)
        {
            Id = c.Id;
            Name = c.Name;
            Address = c.Address;
            DateOfBirth = c.DateOfBirth;

            if (c.PhoneNumbers != null)
                PhoneNumbers = c.PhoneNumbers
                    .Select(pn => new PhoneNumberViewModel(pn));
        }
    }
}