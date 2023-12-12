using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.Model;

namespace ChatApp.ViewModel.Command
{
    internal class ShowChatCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private MainViewModel _main;

        public MainViewModel Main
        {
            get { return _main; }
            set { _main = value; }
        }

        public ShowChatCommand(MainViewModel main)
        {
            this.Main = main;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter != null)
                Main.DisplayChat((Chat)parameter);
        }
    }
}
