using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.Model;

namespace ChatApp.ViewModel.Command
{
    internal class ShowChatCommand : BaseCommand
    {

        public ShowChatCommand(MainViewModel main) : base(main) { }

        private object? _oldParameter = null;
        private int _parameterPressCount = 0;
        public override void Execute(object? parameter)
        {
            if (parameter != null)
            {
                if (_oldParameter != null && _oldParameter.Equals(parameter))
                {
                    _parameterPressCount++;
                    if (_parameterPressCount >= 3)
                    {
                        _parameterPressCount = 0;
                        _main.DisplayChat((Chat)parameter);
                        _main.DisplayOldChat = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        _main.DisplayOldChat = System.Windows.Visibility.Collapsed;
                    }
                }
                else
                {
                    _parameterPressCount = 1;
                    _main.DisplayChat((Chat)parameter);
                    _main.DisplayOldChat = System.Windows.Visibility.Visible;
                }
                _oldParameter = parameter;
            }
        }
    }
}
