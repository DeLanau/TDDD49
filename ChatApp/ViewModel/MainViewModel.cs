using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private string _sendMessage;
        private string _name;
        private string status;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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

        public string Name
        {
            get { return _name; } 
            set 
            {
                _name = value;
                OnPropertyChanged();
            } 
        }

        public MainViewModel()
        {
            Status = "Not Connected";
        }

    }
}
