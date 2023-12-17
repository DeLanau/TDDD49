using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    public abstract class BaseCommand : ICommand
    {

        public event EventHandler? CanExecuteChanged;

        protected MainViewModel _main;
       
        protected BaseCommand(MainViewModel main)
        {
            _main = main;
        }

        public bool CanExecute(object? parameter) => true;

        public abstract void Execute(object? parameter);
        
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
