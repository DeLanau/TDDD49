using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class Chat
    {

        public string Name { get; set; }
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public List<MessageInfo> Messages { get; set; }

        public Chat(string name, MessageInfo message)
        {
            Messages = new List<MessageInfo>() { message };
            Name = name;
            Id = name + DateTime.Now.ToString("yyyyMMddHHmmss");
            Date = DateTime.Now;
        }
    }
}