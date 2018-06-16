using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MainApp
{
    public class CommandHandler : ICommand
    {
        public Action<object> ExecuteAction { get; set; }
        /// <summary>
        /// The Delegate, used to represent the method which defines criteria for the execution 
        /// </summary>
        public Predicate<object> CanExecuteAction { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteAction != null)
            {
                return CanExecuteAction(parameter);
            }
            return true;
        }

        /// <summary>
        /// The event which will be raised based upon the
        /// value set for the command.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction != null)
            {
                ExecuteAction(parameter);
            }
        }


    }
}
