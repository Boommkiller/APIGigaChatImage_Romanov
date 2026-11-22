using System.Collections.Generic;

namespace APIGigaChatImage.Models.Response
{
    public class ResponseMessage
    {
        public List<Choice> choices { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public string @object { get; set; }

        public class Choice
        {
            public string finish_reason { get; set; }
            public int index { get; set; }
            public Request.Message message { get; set; }
        }
    }
}
