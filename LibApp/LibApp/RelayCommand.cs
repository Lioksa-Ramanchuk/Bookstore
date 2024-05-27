using System;
using System.Windows.Input;

namespace LibApp
{
    public class RelayCommand : ICommand
    {
        readonly Action<object?> _executeAction;
        readonly Predicate<object?> _canExecuteAction;

        public RelayCommand(Action<object?> executeAction, Predicate<object?>? canExecuteAction = null)
        {
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction ?? (_ => true);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        public bool CanExecute(object? parameter) => _canExecuteAction(parameter);
        public void Execute(object? parameter) => _executeAction(parameter);
    }
}
