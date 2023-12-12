using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using ChatApp.Model;

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

        private ConnectionManager _connectionManager;
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
            get { return _connectionManager; }
            set { _connectionManager = value; }
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
                OnPropertyChanged();
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

        public MainViewModel()
        {

            if (!Directory.Exists("ChatData"))
                Directory.CreateDirectory("ChatData");

            Status = "Not Connected";

            this.DataManager = new DataManager();
            this.Connection = new ConnectionManager();
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

    }
}
