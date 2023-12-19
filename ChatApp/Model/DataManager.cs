using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChatApp.Model
{
    public class DataManager
    {

        public void Init()
        {
            if (!Directory.Exists("ChatData"))
                Directory.CreateDirectory("ChatData");
        }

        public void UpdateChat(Chat chat)
        {
            string jsonObj = JsonConvert.SerializeObject(chat, Formatting.Indented); //format with indented for human-readable
            File.WriteAllText($"ChatData/chat_{chat.Id}.json", jsonObj);
        }
        public List<Chat> GetHistory()
        {
            var chatFiles = Directory.GetFiles("ChatData/", "*.json");
            List<Chat> sortedchats = new List<Chat>();

            foreach (string file in chatFiles)
            {
                var jsonString = File.ReadAllText(file);

                Chat jsonObj = JsonConvert.DeserializeObject<Chat>(jsonString);
                sortedchats.Add(jsonObj);
            }
            
            return sortedchats.OrderByDescending(o => o.Date).ToList();
        }
    }
}