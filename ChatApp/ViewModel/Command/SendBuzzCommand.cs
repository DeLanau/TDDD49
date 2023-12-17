using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class SendBuzzCommand : BaseCommand
    {
        public SendBuzzCommand(MainViewModel main) : base(main) { }

        public override void Execute(object? parameter)
        {
            _main.SendBuzz();
        }
    }
}
