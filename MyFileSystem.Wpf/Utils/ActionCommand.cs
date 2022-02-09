﻿using System;
using System.Windows.Input;

namespace MyFileSystem.Wpf.Utils
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;

        public event EventHandler CanExecuteChanged;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
