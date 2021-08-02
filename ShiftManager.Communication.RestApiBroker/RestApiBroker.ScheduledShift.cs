using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.Communication.RestData;
using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : InternalApi_ScheduledShift
  {
    /// <summary>指定日の予定シフトを, ユーザID指定で取得します  (自身の予定シフトしか取得できません)</summary>
    /// <param name="targetDate">取得する予定シフトの対象日</param>
    /// <returns>実行結果</returns>
    /*******************************************
  * specification ;
  * name = GetScheduledShiftByIDAsync ;
  * Function = 予定シフトの取得を試行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 取得する日付, ユーザID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<SingleShiftData>> GetCurrentUserScheduledShiftAsync(DateTime targetDate)
    {
      if (!IsLoggedIn || CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, null);
      var resStore = await Sv.GetCurrentUserSingleShiftAsync(false, targetDate);

      if (resStore.Content is null)
        return new(false, ApiResultCodes.Unknown_Error, null);

      if (resStore.Content.Length <= 0)
        return new(false, ApiResultCodes.Target_Date_Not_Found, null);

      var tmp1 = resStore.Content.Where(i => i.work_date == targetDate && i.user_id == CurrentUserData.UserID.Value).FirstOrDefault();

      if (tmp1 is not null)
        return new(true, ApiResultCodes.Success, tmp1.ToSingleShiftData());

      return new(false, ApiResultCodes.Target_Date_Not_Found, null);
    }


    /*******************************************
  * specification ;
  * name = UpdateRequiredWorkerCountListAsync ;
  * Function = 予定シフトの, 計画人数情報を更新します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 更新対象の日付, 計画人数情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> requiredWorkerCountList)
      => Task.Run(() => new ApiResult(true, ApiResultCodes.Not_Supported));


    /*******************************************
  * specification ;
  * name = UpdateShiftSchedulingStateAsync ;
  * Function = 予定シフトの, 操作状態を更新します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 更新対象の日付, 操作状態 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState)
      => Task.Run(() => new ApiResult(true, ApiResultCodes.Not_Supported));


    /*******************************************
  * specification ;
  * name = UpdateSingleScheduledShiftListAsync ;
  * Function = 予定シフトの, 勤務予定リストを更新します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 更新対象の日付, 勤務予定リスト ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas)
    {
      targetDate = targetDate.Date; //時刻部分の入力をカット

      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In);

      string store_id = CurrentUserData.StoreID.Value;

      var res = await Sv.GetCurrentStoreShiftScheduleFileAsync(store_id);
      var schedule = res.Content;
      if (schedule is null) //サーバ上にデータが存在しなかった
      {
        //新規に作成する
        schedule = new()
        {
          shifts = null, // とりあえずnull (Requestに含めない)
          shift_state = RestDataConstants.ShiftStatus.Working,
          store_id = store_id,
          target_date = targetDate,
          worker_num = 1,
          start_of_schedule = targetDate,
          end_of_schedule = targetDate.AddDays(1)
        };

        var res_genTask = await Sv.CreateStoreShiftScheduleFileAsync(schedule);

        var resCode = ToApiRes(res_genTask.Response.StatusCode);
        if (resCode != ApiResultCodes.Success || res_genTask.Content is null)
          return new(false, resCode); //失敗したらここで終了 (更新を行わない)
      }

      var shifts = schedule?.shifts ?? Array.Empty<RestShift>();

      //UserID指定でサーバ上のShiftIDを取得して, それをもってデータ更新タスクを組む
      List<Task<ServerResponse<RestShift>>> Tasks = new();
      foreach (var i in singleShiftDatas)
      {
        var tmp = shifts.FirstOrDefault(arg => arg.work_date?.Date == targetDate && arg.user_id == i.UserID.Value); //日付とUserIDが一致するものを探索する
        if (tmp is RestShift s) //nullチェック
        {
          var newD = new RestShift(s).FromSingleShiftData(i);

          System.Diagnostics.Debug.WriteLine($"\tUpdateFrom=>{s}");
          System.Diagnostics.Debug.WriteLine($"\tUpdate To =>{newD}");

          if (s == newD)
            continue; //更新前後で同じデータなら更新しない (通信データ量削減)

          Tasks.Add(
            Sv.UpdateShiftAsync(newD) //サーバ上にシフトが存在するため, それを更新する
            );
        }
        else //サーバ上に予定シフトデータが存在しなかった
        {
          var newD = RestDataConverter.GenerateFromSingleShiftData(i, null, store_id, false);

          System.Diagnostics.Debug.WriteLine($"\tGenerated =>{newD}");
          System.Diagnostics.Debug.WriteLine(newD);

          Tasks.Add(
            Sv.CreateSingleShiftAsync(newD) //新規にシフトを作成する
            );
        }
      }

      //データ更新(アップロード)タスク実行
      var results = await Task.WhenAll(Tasks);
      foreach (var i in results)
      {
        System.Diagnostics.Debug.WriteLine($"UpdateSingleScheduledShiftListAsync => {i.Content}");
        var resCode = ToApiRes(i.Response.StatusCode);
        if (resCode != ApiResultCodes.Success)
          return new(false, resCode);
      }

      return new(true, ApiResultCodes.Success);
    }


  }
}
