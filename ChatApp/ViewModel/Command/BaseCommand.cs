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
        protected MainViewModel _main;
        
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;


        public abstract void Execute(object? parameter);
        
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        protected BaseCommand(MainViewModel main) 
        { 
            _main = main;
        }
    }
}
