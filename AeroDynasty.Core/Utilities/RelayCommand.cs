using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AeroDynasty.Core.Utilities
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        // Constructor for parameterized commands
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructor for parameterless commands
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(
                _ => execute(),
                canExecute == null ? (Func<object, bool>)null : _ => canExecute()
            )
        {
        }

        // Simplified CanExecute check without CommandManager
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        // Execute the command
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        // Removed the CanExecuteChanged event
        public event EventHandler CanExecuteChanged;
    }

    // Generic version of RelayCommand for parameterized commands
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        // Constructor for generic parameterized commands
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructor for parameterless generic commands
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(
                _ => execute(),
                canExecute == null ? (Func<T, bool>)null : _ => canExecute()
            )
        {
        }

        // Simplified CanExecute check without CommandManager
        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
            {
                throw new InvalidOperationException($"Command parameter is null but expected type is {typeof(T).Name}.");
            }

            return _canExecute == null || _canExecute((T)parameter);
        }

        // Execute the command
        public void Execute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
            {
                throw new InvalidOperationException($"Command parameter is null but expected type is {typeof(T).Name}.");
            }

            _execute((T)parameter);
        }

        // Removed the CanExecuteChanged event
        public event EventHandler CanExecuteChanged;
    }
}
