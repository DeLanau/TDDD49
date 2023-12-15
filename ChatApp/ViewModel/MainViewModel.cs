using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
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

        private ConnectionManager _connection;
        private DataManager _dataManager;

        private MessageInfo _latestMsg;
        private Chat _activeChat = null; //start with zero active chats
        private Thread thread;


        private ICommand _listenercmd;
        private ICommand _clientcmd;
        private ICommand _sendmsgcmd;
        private ICommand _sendbuzzcmd;
        private ICommand _showchatcmd;
        private ICommand _searchcmd;

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
        public ConnectionManager Connection
        {
            get { return _connection; }
            set { _connection = value; }
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

        public DataManager DataManager
        {
            get { return _dataManager; }
            set { _dataManager = value; }
        }

        public ICommand ListenerCmd
        {
            get { return _listenercmd; }
            set { _listenercmd = value; }
        }

        public ICommand ClientCmd
        { 
            get { return _clientcmd; } 
            set { _clientcmd = value; }
        }

        public ICommand SendMsgCmd
        {
            get { return _sendmsgcmd; }
            set { _sendmsgcmd = value; }
        }

        public ICommand SendBuzzCmd
        {
            get { return _sendbuzzcmd; }
            set { _sendbuzzcmd = value; }
        }

        public ICommand ShowChatCmd
        {
            get { return _showchatcmd; }
            set { _showchatcmd = value; }
        }

        public ICommand SearchCmd
        {
            get { return _searchcmd; }
            set { _searchcmd = value; }
        }

        public MainViewModel()
        {

            if (!Directory.Exists("ChatData"))
                Directory.CreateDirectory("ChatData");

            Status = "Not Connected";

            this.DataManager = new DataManager();
            this.Connection = new ConnectionManager();
            this.ListenerCmd = new ListenerCommand(this);
            this.ClientCmd = new ClientCommand(this);
            this.SendMsgCmd = new SendMessageCommand(this);
            this.SendBuzzCmd = new SendBuzzCommand(this);
            this.ShowChatCmd = new ShowChatCommand(this);
            this.SearchCmd = new SearchCommand(this);
            this.ObservableMessage = new ObservableCollection<MessageInfo>();
            this.OldChat = new List<Chat>(GetHistory());
            this.ObservableOldChat = new ObservableCollection<MessageInfo>();
            this.ObservableSearchChat = new ObservableCollection<Chat>(OldChat);
            this.Connection.PropertyChanged += updateProperty;
            Port = "3000";
            Address = "127.0.0.1";

            Debug.WriteLine("Test!!!");
            if (OldChat.Count > 0)
                DisplayChat(DataManager.GetHistory().First());

        }

        //listeners

        public void ConnectListener()
        {
            ObservableMessage.Clear();
            Status = "Connecting";
            thread = new Thread(new ThreadStart(Connection.ConnectListener));
            thread.IsBackground = true;
            thread.Start();
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
            if (OldChat.Count == 0) return;

            ObservableOldChat.Clear();

            Chat temp = chat;
            if (temp != null) return;

            App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    foreach (var msg in temp.Messages)
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
            System.Diagnostics.Debug.WriteLine("in update");

            var prop = e.PropertyName;

            System.Diagnostics.Debug.WriteLine(e.PropertyName);

            if (e.PropertyName == "ReceivedMessage")
            {
                System.Diagnostics.Debug.WriteLine("in switch");
                ReceivedMessage();
                Debug.WriteLine("after");
                Status = "Connected";
            }
            else if (e.PropertyName == "InConnection")
            {
                System.Diagnostics.Debug.WriteLine("in if connection");
                MessageBoxResult result = MessageBox.Show("Accept incoming connection from " + Connection.otheruser + "?", "Incoming Connection", MessageBoxButton.YesNo);

                System.Diagnostics.Debug.WriteLine("let's go");
                if (result == MessageBoxResult.Yes)
                {
                    AcceptConnection();
                }
                else
                {
                    DeclineConnection();
                }
            }
            else if (prop == "Disconnected")
            {
                MessageBox.Show(Connection.otheruser + " disconnected");
                OldChat = DataManager.GetHistory();
                Status = "Not connected";
            }
            else if (prop == "buzz")
            {
                SystemSounds.Beep.Play();

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

            }
            else if (prop == "connectDecline")
            {
                MessageBox.Show("Listener declined your invitation");
                Status = "Not connected";
            }
            else if (prop == "connectAccept")
            {
                Status = "Connected";
            }
        }
    }
}
