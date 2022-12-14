using ClientWPF.Commands.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientWPF.Commands
{
    public class RelayCommand : BaseCommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
