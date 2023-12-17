using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class Chat
    {

        public string Name { get; private set; }
        public string Id { get; private set; }
        public DateTime Date { get; private set; }
        public List<MessageInfo> Messages { get; private set; }

        public Chat(string name, MessageInfo message)
        {
            Messages = new List<MessageInfo>() { message };
            Name = name;
            Id = name + DateTime.Now.ToString("yyyyMMddHHmmss");
            Date = DateTime.Now;
        }
    }
}