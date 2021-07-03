using System;
using System.Windows.Input;

namespace ShiftManager.Controls
{
  public class CustomCommand<T> : ICommand
  {
    public event EventHandler CanExecuteChanged;
    public Action<T> OnExecute { get; }
    public CustomCommand(Action<T> action) => OnExecute = action;
    public bool CanExecute(object parameter) => parameter is T;
    public void Execute(object parameter)
    {
      if (parameter is T i)
        OnExecute?.Invoke(i);
    }
  }
}
