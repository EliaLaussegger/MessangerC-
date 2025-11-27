using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagesNameSpace
{
    public class Message
    {
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string content { get; set; }
        public DateTime timestamp { get; set; }
        public Message(string senderId, string receiverId, string content)
        {
            this.senderId = senderId;
            this.receiverId = receiverId;
            this.content = content;
            this.timestamp = DateTime.Now;
        }
    }
}
