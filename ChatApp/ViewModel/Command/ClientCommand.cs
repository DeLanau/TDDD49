using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class ClientCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private MainViewModel _main;

        public MainViewModel Main
        {
            get { return _main; }
            set { _main = value; }
        }

        public ClientCommand(MainViewModel main)
        {
            this.Main = main;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Main.ConnectListener();
        }

    }
}
