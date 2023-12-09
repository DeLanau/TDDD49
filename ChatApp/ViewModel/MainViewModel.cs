using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Model;

namespace ChatApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        
        private string _sendMessage;
        private string _name;
        private string status; //handle status

        //changeble var
        private string _changePort;
        private string _changeAddress;

        private string searchText;

        private ConnectionManager _connectionManager;
        private Chat _activeChat = null; //start with zero active chats
        private Data _latestMsg;
        private Thread thread; 
        private DataManager _dataManager;
        
        //update UI
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //auto update ui elements
        public ObservableCollection<Data>? ObservableMessage { get; set; }
        public List<Chat>? OldChat { get; set; }
        public ObservableCollection<Chat>? ObservableSearchChat { get; set; }
        public ObservableCollection<Data>? ObservableOldConversation { get; set; }
        public ConnectionManager Connection
        {
            get { return _connectionManager; }
            set { _connectionManager = value; }
        }

        public string Status
        {
            get { return status; }
            set { 
                status = value; 
            
                OnPropertyChanged();
            }
        }
        public string SendMessage
        {
            get { return _sendMessage; }
            set { _sendMessage = value; }   
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
        }

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

    }
}
