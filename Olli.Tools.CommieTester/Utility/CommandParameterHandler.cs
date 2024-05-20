using System;
using System.Windows.Input;

namespace Olli.Tools.CommieTester.Utility
{
    /// <summary>
    /// Command hanlder for action type commands with input parameter.
    /// </summary>
    public class CommandParameterHandler : ICommand
    {
        private readonly Action<object> action;
        private readonly bool canExecute;

        /// <summary>
        /// <see cref="CommandHandler"/> constructor with can execute true.
        /// </summary>
        /// <param name="action"></param>
        public CommandParameterHandler(Action<object> action) : this(action, true) { }

        /// <summary>
        /// <see cref="CommandHandler"/> constructor.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        public CommandParameterHandler(Action<object> action, bool canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        /// <inheritdoc/>
        event EventHandler? ICommand.CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <inheritdoc/>
        bool ICommand.CanExecute(object? parameter)
        {
            return canExecute;
        }

        /// <inheritdoc/>
        void ICommand.Execute(object? parameter)
        {
            if (parameter != null)
                action.Invoke(parameter);
        }
    }
}
