using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatApp.Model;
using ChatApp.ViewModel.Command;

namespace ChatApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private string _outMessage;
        private string _name;
        private string status; //handle status

        //changeble
        private string _changePort;
        private string _changeAddress;

        private string searchText;

        private ConnectionManager Connection { get; set; }
        private DataManager DataManager { get; set; }

        private MessageInfo _latestMsg;
        private Chat _activeChat = null; //start with zero active chats
        private Thread thread;

        //commands 
        public ICommand ListenerCmd { get; set; }
        public ICommand ClientCmd { get; set; }
        public ICommand SendMsgCmd { get; set; }
        public ICommand SendBuzzCmd { get; set; }
        public ICommand ShowChatCmd { get; set; }
        public ICommand SearchCmd { get; set; }

        //update UI
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //auto update ui elements
        public ObservableCollection<MessageInfo>? ObservableMessage { get; set; }
        public List<Chat>? OldChat { get; set; }
        public ObservableCollection<Chat>? ObservableSearchChat { get; set; }
        public ObservableCollection<MessageInfo>? ObservableOldChat { get; set; }

        private int _viewTabId = 0;
        public int ViewTabId
        {
            get { return _viewTabId; }
            set
            {
                _viewTabId = value;
                OnPropertyChanged(nameof(ViewTabId));
            }
        }

        private Visibility _displayOldChat = Visibility.Collapsed;
        public Visibility DisplayOldChat
        {
            get { return _displayOldChat; }
            set
            {
                _displayOldChat = value;
                OnPropertyChanged(nameof(DisplayOldChat));
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }
        public string OutMessage
        {
            get { return _outMessage; }
            set { _outMessage = value; }
        }

        public string Port
        {
            get { return _changePort; }
            set
            {
                _changePort = value;
                Connection.Port = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get { return _changeAddress; }
            set
            {
                _changeAddress = value;
                Connection.Address = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Connection.Name = value;
                OnPropertyChanged(nameof(value));
            }
        }

        public Chat ActiveChat
        {
            get { return _activeChat; }
            set { _activeChat = value; }
        }
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; }
        }

        public MainViewModel()
        {

            if (!Directory.Exists("ChatData"))
                Directory.CreateDirectory("ChatData");

            Status = "Not Connected";

            DataManager = new DataManager();
            Connection = new ConnectionManager();
           
            InitCommands();

            InitObservables();

            Connection.PropertyChanged += updateProperty;

            Port = "3000";
            Address = "127.0.0.1";

            if (OldChat.Count >= 0)
                DisplayChat(DataManager.GetHistory().First());
        }

        //startup
        private void InitCommands()
        {
            ListenerCmd = new ListenerCommand(this);
            ClientCmd = new ClientCommand(this);
            SendMsgCmd = new SendMessageCommand(this);
            SendBuzzCmd = new SendBuzzCommand(this);
            ShowChatCmd = new ShowChatCommand(this);
            SearchCmd = new SearchCommand(this);
        }

        private void InitObservables()
        {
            ObservableMessage = new ObservableCollection<MessageInfo>();
            OldChat = new List<Chat>(GetHistory());
            ObservableOldChat = new ObservableCollection<MessageInfo>();
            ObservableSearchChat = new ObservableCollection<Chat>(OldChat);
        }

        //listeners

        public void ConnectListener()
        {
            ObservableMessage.Clear();
            Status = "Connecting";

            if (!check_port())
            {
                Status = "Not Connected";
                MessageBox.Show("No server on this port");
                return;
            }

            thread = new Thread(new ThreadStart(Connection.ConnectListener));
            thread.IsBackground = true;
            thread.Start();
        }

        private bool check_port()
        {
            try
            {
                TcpListener l = new TcpListener(IPAddress.Parse(Address), int.Parse(Port));
                l.Start();
                l.Stop();
                return false;
            } catch (SocketException e)
            {
                return true;
            }
        }

        public void InitListener()
        {
            if(Connection.connected)
            {
                MessageBox.Show("Already Listening");
                return;
            }

            Status = "Listening";
            ObservableMessage.Clear();
            thread = new Thread(new ThreadStart(Connection.InitListener));
            thread.IsBackground = true;
            thread.Start();
        }

        //chat

        private void InitiateChat(MessageInfo msg)
        {
            _activeChat = new Chat(Connection.Otheruser, msg);
            DataManager.UpdateChat(_activeChat);
        }

        private void UpdateChat(MessageInfo msg)
        {
            _activeChat.Messages.Add(msg);
            DataManager.UpdateChat(_activeChat);
        }

        private List<Chat> GetHistory()
        {
            return DataManager.GetHistory();
        }

        public void DisplayChat(Chat chat)
        {
            if (OldChat.Count == 0 || chat == null) return;

            ObservableOldChat.Clear();

            App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    foreach (var msg in chat.Messages)
                    {
                        ObservableOldChat.Add(msg);
                    }
                });
        }

        public void SearchChat()
        {
            IEnumerable<Chat> chatList = OldChat
                .Where(c => c.Name.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(c => c.Date);
            
            ObservableSearchChat.Clear();

            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                foreach (Chat c in chatList)
                {
                    ObservableSearchChat.Add(c);
                }
            });

        }

        //messages
        public void SendMessage()
        {
            if (!Connection.Connected) return;

            MessageInfo msg = Connection.SendMessage(OutMessage);
            DisplayMessageOnScreen(msg);
            OutMessage = string.Empty;
            OnPropertyChanged(nameof(OutMessage));
        }

        private void DisplayMessageOnScreen(MessageInfo msg)
        {
            App.Current.Dispatcher.Invoke((System.Action)delegate
            {
                ObservableMessage.Add(msg);
            });

            if (_activeChat == null)
            {
                InitiateChat(msg);
            }
            else
            {
                UpdateChat(msg);
            }

        }

        public void SendBuzz()
        {
            Connection.SendBuzz();
        }

        public void ReceivedMessage()
        {
            _latestMsg = Connection.ReceivedMessage;
            DisplayMessageOnScreen(Connection.ReceivedMessage);
        }

        //connection
        public void AcceptConnection()
        {
            thread = new Thread(new ThreadStart(Connection.AcceptConnection));
            thread.IsBackground = true;
            thread.Start();
            Status = "Connected";
            ViewTabId = 1;
        }

        public void DeclineConnection()
        {
            Connection.DeclineConnection();
            Status = "Not connected";
        }

        public void DisconnectConnection()
        {
            Connection.DisconnectConnection();
            Status = "Disconnected";
        }

        //update property
        private void updateProperty(object sender, PropertyChangedEventArgs e)
        {
            var prop = e.PropertyName;
            
            if (prop == "ReceivedMessage")
            {
               // System.Diagnostics.Debug.WriteLine("in switch");
                ReceivedMessage();
                Status = "Connected";
            }
            else if (prop == "InConnection")
            {
                MessageBoxResult result = MessageBox.Show("Accept incoming connection from " + Connection.otheruser + "?", "Incoming Connection", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    AcceptConnection();
                }
                else
                {
                    DeclineConnection();
                }
            }
            else if (prop == "Disconnected" && !Connection.connected)
            {
               // System.Diagnostics.Debug.WriteLine(check);            
                MessageBox.Show("Client has been disconnected");
    
                //  MessageBox.Show("You disconnected");
                OldChat = DataManager.GetHistory();
                Status = "Not connected";
            }
            else if (prop == "buzz")
            {
                SystemSounds.Beep.Play();
                    //fix buzz, lägger den i dispatcher kö istället first second -> second disconect | first <- second har blivit disc | first diconected  | <- second e discont
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var window = Application.Current.MainWindow;

                    if (window != null)
                    {
                        const int shakeIntensity = 5; // Intensitet av skakning

                        Random random = new Random();
                        const int shakeCount = 20; // Antal skakningar
                        const int shakeDuration = 20; // Tid i millisekunder för varje skakning

                        double originalLeft = window.Left;
                        double originalTop = window.Top;

                        for (int i = 0; i < shakeCount; i++)
                        {
                            double offsetX = random.Next(-shakeIntensity, shakeIntensity + 1);
                            double offsetY = random.Next(-shakeIntensity, shakeIntensity + 1);

                            window.Left = originalLeft + offsetX;
                            window.Top = originalTop + offsetY;

                            System.Threading.Thread.Sleep(shakeDuration);
                        }

                        // Återställ fönstrets position efter skakningen
                        window.Left = originalLeft;
                        window.Top = originalTop;
                    }
                });
            }
            else if (prop == "Declined")
            {
                MessageBox.Show("Listener declined your invitation");
                Status = "Not connected";
            }
            else if (prop == "Connected")
            {
                Status = "Connected";
                ViewTabId = 1;
            }
        }
    }
}
