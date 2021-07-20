using System;
using System.Collections.Generic;
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
    Dictionary<DateTime, RestShift> ShiftReqCache { get; set; } = new();
    DateTime ShiftReqCacheLastUpdate { get; set; }

    private async Task<ApiResult> UpdateShiftReqCache()
    {
      if (!IsLoggedIn)
        return new(false, ApiResultCodes.Not_Logged_In);//ログイン中しか使用できない

      if (DateTime.Now > ShiftReqCacheLastUpdate.AddSeconds(5)) //最後の受信から5秒間は更新を行わない
      {
        var res = await Sv.GetCurrentUserSingleShiftRequestsAsync();
        var resCode = ToApiRes(res.Response.StatusCode);
        if (resCode != ApiResultCodes.Success)
          return new(false, resCode);

        if (!(res.Content?.Length > 0))
          return new(false, ApiResultCodes.Data_Not_Found);

        ShiftReqCache = res.Content.Where(i => i.work_date is not null).ToDictionary(i => i.work_date?.Date ?? default);
      }
      return new(true, ApiResultCodes.Success);
    }

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
      var updRes = await UpdateShiftReqCache();

      if (!updRes.IsSuccess)
        return updRes;

      ServerResponse<RestShift> res;
      DateTime targetDate = singleShiftData.WorkDate.Date;
      var req = new RestShift().FromSingleShiftData(singleShiftData, 0, CurrentUserData?.StoreID.Value ?? string.Empty, true);
      if (ShiftReqCache.TryGetValue(targetDate, out var shift))
      { // 既にシフトが存在する場合 => IDを取得して更新扱い
        req.id = shift.id;
        res = await Sv.UpdateShiftAsync(req);

        if (res.Content is not null)
          ShiftReqCache[targetDate] = res.Content;
      }
      else
      { // サーバー側にシフトが存在しない場合 => 新規追加扱い
        res = await Sv.CreateSingleShiftAsync(req);

        if (res.Content is not null)
          ShiftReqCache.Add(targetDate, res.Content);
      }
      var resCode = ToApiRes(res.Response.StatusCode);

      return new(resCode == ApiResultCodes.Success, resCode);
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
      var updRes = await UpdateShiftReqCache();

      if (!updRes.IsSuccess)
        return new(false, updRes.ResultCode, null);

      return (ShiftReqCache.TryGetValue(date.Date, out var res) && res is not null)
        ? new(true, ApiResultCodes.Success, res.ToSingleShiftData())
        : new(false, ApiResultCodes.Target_Date_Not_Found, null);
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
