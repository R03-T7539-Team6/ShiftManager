using System;
using System.Windows.Input;

namespace ShiftManager.Controls
{
  public class CustomCommand<T> : ICommand
  {
    public event EventHandler CanExecuteChanged;
    public Action<T> OnExecute { get; }

    /*******************************************
* specification ;
* name = CustomCommand ;
* Function = インスタンスの初期化を行う ;
* note = CommandがExecuteされた際は, 引数に与えられたActionが実行されます ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Executeされた際に実行するメソッド ;
* output = N/A ;
* end of specification ;
*******************************************/
    public CustomCommand(Action<T> action) => OnExecute = action;

    /*******************************************
* specification ;
* name = CanExecute ;
* Function = Commandを実行可能かどうかを取得する ;
* note = N/A ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Command実行時に渡される引数 ;
* output = 実行可能性 ;
* end of specification ;
*******************************************/
    public bool CanExecute(object parameter) => parameter is T;

    /*******************************************
* specification ;
* name = Execute ;
* Function = コンストラクタで指定された処理を実行する ;
* note = N/A ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 関数に渡す引数 ;
* output = N/A ;
* end of specification ;
*******************************************/
    public void Execute(object parameter)
    {
      if (parameter is T i)
        OnExecute?.Invoke(i);
    }
  }
}
