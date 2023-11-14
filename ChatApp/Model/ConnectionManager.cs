using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class ConnectionManager : INotifyPropertyChanged
    {
        TcpClient client;
        TcpListener listener;
        NetworkStream stream;

        int port = 3000;
        string address = "127.0.0.1";
        string name = "Elljo";

        public string otheruser = "";
        public bool connected = false;

        bool listening = false;
        bool inconnection = false;
        bool disconnected = true;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Port
        {
            get { return port.ToString(); }
            set { port = int.Parse(value); }
        }

        public string Address
        { 
            get { return address; } 
            set {  address = value; } 
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Otheruser
        {
            get { return otheruser; }
            set { otheruser = value; }
        }

        public bool InConnection
        {
            get { return InConnection; }
            set 
            {  
                inconnection = value;
                OnPropertyChanged("InConnection");
            }
        }

        public bool Disconnected
        {
            get { return disconnected; }
            set 
            {
                OnPropertyChanged("Disconnected");
                //CloseConnection();
            }
        }

        public bool Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                OnPropertyChanged("Connected");
            }
        }

        //TODO sendmsg, recievemsg, connect/start/disconnect/close connection
    }
}