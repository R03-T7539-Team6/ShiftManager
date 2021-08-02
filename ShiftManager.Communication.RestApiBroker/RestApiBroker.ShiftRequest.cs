using System;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.Communication.RestData;
using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : InternalApi_ShiftRequest
  {
    /* キャッシュ機能は一旦オミット
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
    }*/

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
      /*var updRes = await UpdateShiftReqCache();

      if (!updRes.IsSuccess)
        return updRes;*/

      ServerResponse<RestShift> res;
      DateTime targetDate = singleShiftData.WorkDate.Date;
      var req = new RestShift().FromSingleShiftData(singleShiftData, null, CurrentUserData?.StoreID.Value ?? string.Empty, true);

      var reqRes = await Sv.GetCurrentUserSingleShiftRequestsAsync(targetDate);
      var targetData = reqRes.Content?.FirstOrDefault(i => i.work_date?.Date == targetDate.Date && i.user_id == CurrentUserData?.UserID.Value);

      var reqResCode = ToApiRes(reqRes.Response.StatusCode);

      if (reqRes.Content is null)
      {
        if (reqResCode != ApiResultCodes.Success)
          return new(false, reqResCode);

        // サーバー側にシフトが存在しない場合 => 新規追加扱い
        res = await Sv.CreateSingleShiftAsync(req);
      }
      else 
      { // 既にシフトが存在する場合 => IDを取得して更新扱い
        req.id = targetData?.id;

        if (req == targetData) //データが同じなら更新しない
          return new(true, ApiResultCodes.Success);

        if (req.id is null)
          res = await Sv.CreateSingleShiftAsync(req);
        else
          res = await Sv.UpdateShiftAsync(req);
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
      //var updRes = await UpdateShiftReqCache();
      var res = await Sv.GetCurrentUserSingleShiftRequestsAsync(date);
      var apiRes = ToApiRes(res.Response.StatusCode);

      if (apiRes != ApiResultCodes.Success || res.Content is null || res.Content.Length <= 0 ) //失敗し, かつその原因が「データが存在しない」場合
        return new(false, apiRes, null);

      var data = res.Content.FirstOrDefault(i => i.work_date?.Date == date.Date && i.user_id == CurrentUserData?.UserID.Value)?.ToSingleShiftData();

      return (data is not null)
        ? new(true, ApiResultCodes.Success, data)
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
