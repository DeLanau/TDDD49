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


        private object? _oldParameter = null;
        private int _parameterPressCount = 0;
        public void Execute(object? parameter)
        {
            if (parameter != null)
            {
                if (_oldParameter != null && _oldParameter.Equals(parameter))
                {
                    _parameterPressCount++;
                    if (_parameterPressCount >= 3)
                    {
                        _parameterPressCount = 0;
                        Main.DisplayChat((Chat)parameter);
                        Main.DisplayOldChat = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        Main.DisplayOldChat = System.Windows.Visibility.Collapsed;
                    }
                }
                else
                {
                    _parameterPressCount = 1;
                    Main.DisplayChat((Chat)parameter);
                    Main.DisplayOldChat = System.Windows.Visibility.Visible;
                }
                _oldParameter = parameter;
            }

        }
    }
}
