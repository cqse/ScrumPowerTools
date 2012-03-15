using System;
using System.Windows.Input;

namespace ScrumPowerTools.Framework.Presentation
{
    internal class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> action;

        public DelegateCommand(Action<T> action)
        {
            this.action = action;
        }

        public void Execute(object parameter)
        {
            action((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return parameter is T;
        }

        public event EventHandler CanExecuteChanged;
    }
}
