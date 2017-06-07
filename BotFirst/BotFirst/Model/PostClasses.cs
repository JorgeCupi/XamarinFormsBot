using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFirst.Model
{
    public class MessageRequest
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public int expires_in { get; set; }
        public string streamUrl { get; set; }
    }

    public class ActivityToPost
    {
        public string type { get; set; }
        public User from { get; set; }
        public string text { get; set; }
        public string locale { get; set; }
    }
    public class User
    {
        public string id { get; set; }
    }

    public class PostResult
    {
        public string id { get; set; }
    }
}
