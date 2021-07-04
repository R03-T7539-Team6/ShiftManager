using System.Collections.Immutable;
using System.Threading.Tasks;

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
  public interface InternalApi_ManageData
  {
    Task<ApiResult<ImmutableDictionary<string, ImmutableArray<string>>>> GetDataListAsync();
    Task<ApiResult<T>> DownloadDataAsync<T>(string Category, string DataName);
    Task<ApiResult> UploadDataAsync<T>(string Category, string DataName, T Data);
  }

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
  public partial class InternalApi : InternalApi_ManageData
  {
    public Task<ApiResult<T>> DownloadDataAsync<T>(string Category, string DataName)
    {
      throw new System.NotImplementedException();
    }

    public Task<ApiResult<ImmutableDictionary<string, ImmutableArray<string>>>> GetDataListAsync()
    {
      throw new System.NotImplementedException();
    }

    public Task<ApiResult> UploadDataAsync<T>(string Category, string DataName, T Data)
    {
      throw new System.NotImplementedException();
    }
  }
}
