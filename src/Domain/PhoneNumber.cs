namespace PublicContacts.Domain
{
    public class PhoneNumber
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string Number { get; set; } = null!;

        public Contact? Contact { get; set; }
    }
}