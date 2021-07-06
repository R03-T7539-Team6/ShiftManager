using System.Collections.Immutable;
using System.Threading.Tasks;

namespace ShiftManager.Communication
{
  public partial class InternalApi : InternalApi_ManageData
  {
    /*******************************************
  * specification ;
  * name = DownloadDataAsync ;
  * Function = 指定のデータをダウンロードする ;
  * note = v1.0時点で未実装 ;
  * date = 07/03/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = カテゴリ, データ名 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<T>> DownloadDataAsync<T>(string Category, string DataName)
    {
      throw new System.NotImplementedException();
    }

    /*******************************************
  * specification ;
  * name = GetDataListAsync ;
  * Function = ダウンロード可能なデータのリストを取得する ;
  * note = v1.0時点で未実装 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<ImmutableDictionary<string, ImmutableArray<string>>>> GetDataListAsync()
    {
      throw new System.NotImplementedException();
    }
    /*******************************************
* specification ;
* name = UploadDataAsync ;
* Function = 指定のデータをアップロードする ;
* note = v1.0時点で未実装 ;
* date = 07/05/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = カテゴリ, データ名, データ ;
* output = 実行結果 ;
* end of specification ;
*******************************************/

    public Task<ApiResult> UploadDataAsync<T>(string Category, string DataName, T Data)
    {
      throw new System.NotImplementedException();
    }
  }
}
