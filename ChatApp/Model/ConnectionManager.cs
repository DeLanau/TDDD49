using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
                CloseConnection();
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

        private bool declined;
        public bool Declined
        {
            get { return declined; }
            set
            {
                declined = value;
                OnPropertyChanged("Declined");
            }
        }

        public Data SendMessage(String msg)
        {
            Data j_msg = new Data()
            {
                RequestType = "message",
                Date = DateTime.Now,
                UserName = name,
                Message = msg
            };
            sendJsonMessage(j_msg);
            return j_msg;
        }

        void sendJsonMessage (Data msg) 
        {
            var j_msg = JsonSerializer.Serialize<Data>(msg);
            var bytes = System.Text.Encoding.ASCII.GetBytes(j_msg);

            if(stream != null)
                stream.Write(bytes, 0, bytes.Length);
        }

        private Data receivedMessage;
        public Data ReceivedMessage
        {
            get { return receivedMessage;}
            set
            {
                receivedMessage = value;
                if (otheruser == "")
                    Otheruser = receivedMessage.UserName;
                OnPropertyChanged("ReceivedMessage");
            }
        }

        public void ConnectListener()
        {
            try
            {
                client = new TcpClient(address, port);
                stream = client.GetStream();

                Data connect_done = new Data()
                {
                    RequestType = "connectDone",
                    Date = DateTime.Now,
                    UserName = name,
                    Message = ""
                };

                sendJsonMessage(connect_done);
            }catch (ArgumentNullException e)
            {
                otheruser = "";
            }catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally {
                connected = true;
                getMessage();
            }
        }

        private void getMessage()
        {
            try
            {
               while(connected)
                {
                    if (client == null)
                        break;

                    var datab = new Byte[256];
                    stream = client.GetStream();
                    Int32 bytes = stream.Read(datab);
                    var msg = System.Text.Encoding.ASCII.GetString(datab, 0, bytes);

                    if(bytes > 0)
                    {
                        receivedMessage = JsonSerializer.Deserialize<Data>(msg);
                        string request_type = receivedMessage.RequestType;

                        switch (request_type)
                        {

                            case "message":
                                ReceivedMessage = receivedMessage;
                                break;
                            case "connectDone":
                                otheruser = receivedMessage.UserName;
                                InConnection = true;
                                break;
                            case "connectDecline":
                                Declined = true;
                                break;
                            case "buzz":
                                OnPropertyChanged("buzz");
                                break;
                            case "connectAccept":
                                Connected = true;
                                break;
                        }
                        stream.Flush();
                        if (Declined)
                            break;
                    }
                }
            }catch
            {
                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    Disconnected = true;
                    CloseConnection();
                });
            }
        }

        public void DisconnectConnection()
        {
            CloseConnection();
        }

        private void CloseConnection()
        {
            if(client != null)
                client.Close();

            connected = false;
            otheruser = "";
        }
    }
}