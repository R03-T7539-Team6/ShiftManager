using System.Windows;
using System.Windows.Controls;

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
  public class ShiftEditorBarControl : Control
  {
    static ShiftEditorBarControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorBarControl), new FrameworkPropertyMetadata(typeof(ShiftEditorBarControl)));

  }
}
