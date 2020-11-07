using System;

namespace PublicContacts.App.Exceptions
{
    public class RequestException : Exception
    {
        public string? Key { get; set; }
        public string? KeyMessage { get; set; }
        public RequestException(string message) : base(message) { }
        public RequestException(string key, string keyMessage) : base(keyMessage)
        {
            Key = key;
            KeyMessage = keyMessage;
        }
        public RequestException(string message, string key, string keyMessage)
        {
            Key = key;
            KeyMessage = keyMessage;
        }
    }
}