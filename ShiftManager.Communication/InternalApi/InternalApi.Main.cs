using System.Runtime.CompilerServices;

using ShiftManager.DataClasses;

[assembly: InternalsVisibleTo("ShiftManager.Communication.Test")]

namespace ShiftManager.Communication
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
  public partial class InternalApi
  {
    public IUserData? CurrentUserData { get; private set; } = null;

    private StoreData TestD { get; set; } = new SampleDataForStandaloneTest().StoreData;//To Debug
  }
}
