using System;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

using ShiftManager.Communication.RestData;
using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : InternalApi_ShiftRequest
  {
    /// <summary>送受信したデータをキャッシュします</summary>
    MemoryCache ShiftReqCache { get; } = new(nameof(ShiftReqCache)); //実装準備段階

    /// <summary>シフト希望を追加します  初めての追加の場合は, コレクションにユーザのデータが生成されたうえで追加されます</summary>
    /// <param name="singleShiftData">追加する単一シフトデータ</param>
    /// <returns>実行結果</returns>
    /*******************************************
  * specification ;
  * name = AddShiftRequest ;
  * Function = シフト希望を追加します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 追加する単一日シフト希望情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public ApiResult AddShiftRequest(ISingleShiftData singleShiftData)
      => AddShiftRequestAsync(singleShiftData).Result;

    /*******************************************
  * specification ;
  * name = AddShiftRequestAsync ;
  * Function = AddShiftRequestを非同期に実行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = シフト希望情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData)
    {
      if (!IsLoggedIn)
        return new(false, ApiResultCodes.Not_Logged_In);

      var req = RestShift.GenerateFromSingleShiftData(singleShiftData, 0, CurrentUserData?.StoreID.Value ?? string.Empty, true);

      var res = await Api.ExecuteWithDataAsync<RestShift, RestShift>("/shifts", req);

      return new(res.IsSuccess, res.ResultCode);
    }

    /*******************************************
  * specification ;
  * name = GetShiftRequestByDateAsync ;
  * Function = 日付を指定してシフト希望を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 指定する日付 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<SingleShiftData>> GetShiftRequestByDateAsync(DateTime date)
    {
      if (!IsLoggedIn)
        return new(false, ApiResultCodes.Not_Logged_In, null);//ログイン中しか使用できない

      var res = await Api.GetDataAsync<RestShift[]>("/shifts?is_request=true");

      if (res?.IsSuccess != true)
        return new(false, res.ResultCode, null);

      if (res.ReturnData is null || res.ReturnData.Length <= 0)
        return new(false, ApiResultCodes.Data_Not_Found, null);

      var tmp = res.ReturnData.Where(i => i.work_date.Date == date.Date);

      if (tmp is null || tmp.Count() <= 0)
        return new(false, ApiResultCodes.Target_Date_Not_Found, null);

      return new(true, ApiResultCodes.Success, tmp.FirstOrDefault()?.ToSingleShiftData());
    }


    /*******************************************
  * specification ;
  * name = UpdateShiftRequestAsync ;
  * Function = シフト希望を更新します ;
  * note = AddShiftRequestAsyncに同一の機能があります. ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = シフト情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    [Obsolete("Please use AddShiftRequestAsync Method")]
    public Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData)
      => AddShiftRequestAsync(singleShiftData);
  }
}
