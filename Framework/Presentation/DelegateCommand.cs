using System;
using System.Windows.Input;

namespace ScrumPowerTools.Framework.Presentation
{
    internal class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> action;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action<T> action, Func<bool> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            action((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute();
            }

            return parameter is T;
        }

        public event EventHandler CanExecuteChanged;
    }
}
