using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class Chat
    {

        private string name;
        private string id;
        private DateTime date;
        private List<MessageInfo> messages;

        public string Name
        { 
            get { return name; } 
            set {  name = value; } 
        }

        public string Id
        {
            get { return id; }
            set {  id = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public List<MessageInfo> Messages
        {
            get { return messages; }
            set { messages = value; }
        }

        public Chat(string name, MessageInfo message)
        {
            messages = new List<MessageInfo>();
            messages.Add(message);
            this.Name = name;
            this.Id = name + DateTime.Now.ToString("yyyyMMddHHmmss");
            this.Date = DateTime.Now;
        }
    }
}