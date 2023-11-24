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
        private List<Data> messages;

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

        public List<Data> Messages
        {
            get { return messages; }
            set { messages = value; }
        }

        public Chat(string name, Data message)
        {
            messages = new List<Data>();
            messages.Add(message);
            this.Name = name;
            this.Id = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.Date = DateTime.Now;
        }
    }
}