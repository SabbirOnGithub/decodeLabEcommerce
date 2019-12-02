using System.Collections.Generic;

namespace Nop.Plugin.Widgets.MobileLogin.Models
{
    public class SmsResponseResultModel
    {
        public SmsResponseResultModel()
        {
            messages = new List<Message>();
        }
        public List<Message> messages { get; set; }
    }

    public class Message
    {
        public Message()
        {
            status = new Status();
        }
        public string to { get; set; }
        public Status status { get; set; }
        public int smsCount { get; set; }
        public string messageId { get; set; }
    }


    public class Status
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
