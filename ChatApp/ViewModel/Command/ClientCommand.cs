using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class ClientCommand : BaseCommand
    {
   
        public ClientCommand(MainViewModel main) : base(main) { }

        public override void Execute(object? parameter)
        {
            _main.ConnectListener();
        }

    }
}
