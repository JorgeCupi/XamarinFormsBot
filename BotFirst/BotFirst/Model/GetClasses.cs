﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFirst.Model
{

    public class From
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Conversation
    {
        public string id { get; set; }
    }

    public class Activity
    {
        public string type { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public string channelId { get; set; }
        public From from { get; set; }
        public Conversation conversation { get; set; }
        public string text { get; set; }
        public List<Attachment> attachments { get; set; }
        public List<object> entities { get; set; }
        public string replyToId { get; set; }
    }

    public class GetResult
    {
        public List<Activity> activities { get; set; }
        public string watermark { get; set; }
    }

    public class Attachment
    {
        public Content content { get; set; }
    }

    public class Content
    {
        public string title { get; set; }
        public string text { get; set; }
        public List<Button> buttons { get; set; }
    }

    public class Button
    {
        public string type { get; set; }
        public string title { get; set; }
        public string value { get; set; }
    }

}
