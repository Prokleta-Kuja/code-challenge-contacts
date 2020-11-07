namespace PublicContacts.Domain
{
    public static class Constants
    {
        public static class MaxLengths
        {
            // Contact
            public const int ContactName = 64;
            public const int ContactAddress = 64;
            public const int ContactHash = 32;

            // PhoneNumber
            public const int PhoneNumberNumber = 16; // Guesstimate
        }
    }
}