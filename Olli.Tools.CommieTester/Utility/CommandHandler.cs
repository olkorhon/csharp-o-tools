using System;
using System.Windows.Input;

namespace Olli.Tools.CommieTester.Utility
{
    /// <summary>
    /// Command hanlder for action type commands.
    /// </summary>
    public class CommandHandler : ICommand
    {
        private readonly Action action;
        private readonly bool canExecute;

        /// <summary>
        /// <see cref="CommandHandler"/> constructor with can execute true.
        /// </summary>
        /// <param name="action"></param>
        public CommandHandler(Action action) : this(action, true) { }

        /// <summary>
        /// <see cref="CommandHandler"/> constructor.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        public CommandHandler(Action action, bool canExecute)
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
            action();
        }
    }
}
