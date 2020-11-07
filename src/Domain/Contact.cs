using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PublicContacts.Domain
{
    public class Contact
    {
        private string name = string.Empty;
        private string address = string.Empty;

        public int Id { get; set; }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                ComputeHash();
            }
        }
        public string Address
        {
            get => address;
            set
            {
                address = value;
                ComputeHash();
            }
        }
        public DateTime DateOfBirth { get; set; }
        public string Hash { get; private set; } = string.Empty;

        public ICollection<PhoneNumber>? PhoneNumbers { get; set; }
        private void ComputeHash()
        {
            var props = new List<string>();
            props.Add(Name.Trim().ToUpperInvariant());
            props.Add(Address.Trim().ToUpperInvariant());

            var stringToHash = string.Join("^", props);
            var valueToHash = Encoding.UTF8.GetBytes(stringToHash);

            using var hasher = MD5CryptoServiceProvider.Create();
            var hashValue = hasher.ComputeHash(valueToHash);
            var hashString = BitConverter.ToString(hashValue);

            Hash = hashString.Replace("-", string.Empty);
        }
    }
}