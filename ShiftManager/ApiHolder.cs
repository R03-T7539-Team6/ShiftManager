
using ShiftManager.Communication;

namespace ShiftManager
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
  public class ApiHolder : IApiHolder
  {
    public InternalApi Api { get; } = new();
  }
  public interface IApiHolder
  {
    InternalApi Api { get; }
  }

  public interface IContainsApiHolder
  {
    IApiHolder ApiHolder { get; set; }
  }
}
