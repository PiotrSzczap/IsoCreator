using System;
using System.Windows.Input;

namespace IsoCreator
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _action;
        public DelegateCommand(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action), $"Parameter {nameof(action)} cannot be null.");
        }

        public bool CanExecute(object parameter)
            => true;

        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }
}