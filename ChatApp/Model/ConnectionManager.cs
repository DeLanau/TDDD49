using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

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

        public MessageInfo SendMessage(String msg)
        {
            MessageInfo j_msg = new MessageInfo()
            {
                RequestType = "message",
                Date = DateTime.Now,
                UserName = name,
                Message = msg
            };
            sendJsonMessage(j_msg);
            return j_msg;
        }

        void sendJsonMessage (MessageInfo msg) 
        {
            var j_msg = JsonSerializer.Serialize<MessageInfo>(msg);
            var bytes = System.Text.Encoding.ASCII.GetBytes(j_msg);

            if(stream != null)
                stream.Write(bytes, 0, bytes.Length);
        }

        private MessageInfo receivedMessage;
        public MessageInfo ReceivedMessage
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

                MessageInfo connect_done = new MessageInfo()
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

        //listen stream of data for message. 
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
                        receivedMessage = JsonSerializer.Deserialize<MessageInfo>(msg);
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

        //init listener to default port 127.0.0.1
        public void InitListener()
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                client = listener.AcceptTcpClient();
                connected = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                if (otheruser != null)
                    otheruser = "";

                listener.Stop();
            }
            getMessage();
        }

        public void SendBuzz()
        {
            MessageInfo msg = new MessageInfo()
            {
                RequestType = "buzz",
                Date = DateTime.Now,
                UserName = name,
                Message = ""
            };
            sendJsonMessage(msg);
        }

        public void AcceptConnection()
        {
            Otheruser = receivedMessage.UserName;
            connected = true;
            inconnection = false;
            MessageInfo msg = new MessageInfo()
            {
                RequestType = "connectAccept",
                Date = DateTime.Now,
                UserName = name,
                Message = ""
            };
            sendJsonMessage(msg);
            getMessage();
        }

        public void DeclineConnection()
        {
            MessageInfo msg = new MessageInfo()
            {
                RequestType = "connectDecline",
                Date = DateTime.Now,
                UserName = name,
                Message = ""
            };
            sendJsonMessage(msg);
            CloseConnection();
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