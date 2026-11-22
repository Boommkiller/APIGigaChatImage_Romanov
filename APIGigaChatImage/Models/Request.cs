using System.Collections.Generic;

namespace APIGigaChatImage.Models
{
    public class Request
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
        public string function_call { get; set; }
        public bool stream { get; set; }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}
