using System;
using System.Windows.Input;

namespace ShiftManager.Controls
{
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public class CustomCommand<T> : ICommand
  {
    public event EventHandler CanExecuteChanged;
    public Action<T> OnExecute { get; }
    public CustomCommand(Action<T> action) => OnExecute = action;
    public bool CanExecute(object parameter) => parameter is T;
    /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
    public void Execute(object parameter)
    {
      if (parameter is T i)
        OnExecute?.Invoke(i);
    }
  }
}
