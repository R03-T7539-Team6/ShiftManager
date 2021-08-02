using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.Communication.RestData;
using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public partial class RestApiBroker : InternalApi_WorkLog
  {
    public ICurrentTimeProvider CurrentTimeProvider { get; init; } = new CurrentTimeProvider();

    Dictionary<UserID, RestWorkLogWithModel> WorkLogCache { get; } = new();

    /// <summary>指定のユーザについて, 休憩終了の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    /*******************************************
  * specification ;
  * name = DoBreakTimeEndLoggingAsync ;
  * Function = 休憩を終了する打刻を試行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = ユーザID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;
      UserID uid = new(userID);

      if (!WorkLogCache.TryGetValue(uid, out var lastWorkLog) || lastWorkLog is null)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.attendance_time is null || lastWorkLog.leaving_time is not null)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      if (lastWorkLog.start_break_time is null || lastWorkLog.end_break_time is not null) //休憩開始時刻の記録がない || 休憩終了時刻の記録がある
        return new(false, ApiResultCodes.BreakTime_Not_Started, CurrentTime);

      int breakTimeLen = SharedFuncs.GetBreakTimeLength(lastWorkLog.start_break_time ?? default, CurrentTime); //startBreakTimeはnullじゃないはずだけど, 一応
      if (breakTimeLen <= 0)
        return new(false, ApiResultCodes.BreakTimeLen_Zero_Or_Less, CurrentTime);

      lastWorkLog.end_break_time = CurrentTime;

      var res = await Sv.UpdateWorkLogAsync(lastWorkLog, lastWorkLog.ID);

      return new(res.Content is not null, ToApiRes(res.Response.StatusCode), CurrentTime);
    }

    /// <summary>指定のユーザについて, 休憩開始の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    /*******************************************
  * specification ;
  * name = DoBreakTimeStartLoggingAsync ;
  * Function = 休憩を開始する打刻を試行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = ユーザID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;
      UserID uid = new(userID);

      if (!WorkLogCache.TryGetValue(uid, out var lastWorkLog) || lastWorkLog is null)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.attendance_time is null || lastWorkLog.leaving_time is not null)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);


      if (lastWorkLog.start_break_time is not null && lastWorkLog.end_break_time is null) //休憩中なのに休憩終了時刻の記録がない
        return new(false, ApiResultCodes.BreakTime_Not_Ended, CurrentTime);

      lastWorkLog.start_break_time = CurrentTime;
      lastWorkLog.end_break_time = null; // 休憩時間は最後の一つのみ記録可能

      var res = await Sv.UpdateWorkLogAsync(lastWorkLog, lastWorkLog.ID);

      return new(res.Content is not null, ToApiRes(res.Response.StatusCode), CurrentTime);
    }

    /// <summary>指定のユーザについて, 退勤の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    /*******************************************
  * specification ;
  * name = DoWorkEndTimeLoggingAsync ;
  * Function = 退勤打刻を試行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = ユーザID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!WorkLogCache.TryGetValue(new UserID(userID), out var lastWorkLog) || lastWorkLog is null)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.attendance_time is null|| lastWorkLog.leaving_time is not null)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //休憩記録が存在し, かつ最後の記録に休憩時間長が記録されていない
      if (lastWorkLog.start_break_time is not null && lastWorkLog.end_break_time is null)
        return new(false, ApiResultCodes.BreakTime_Not_Ended, CurrentTime);

      lastWorkLog.leaving_time = CurrentTime;

      var res = await Sv.UpdateWorkLogAsync(lastWorkLog, lastWorkLog.ID);

      return new(res.Content is not null, ToApiRes(res.Response.StatusCode), CurrentTime);
    }

    /// <summary>指定のユーザについて, 出勤の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    /*******************************************
  * specification ;
  * name = DoWorkStartTimeLoggingAsync ;
  * Function = 出勤打刻を試行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = ユーザID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;
      UserID uID = new(userID);


      if (WorkLogCache.TryGetValue(uID, out var lastWorkLog)) //WorkLogがすでに存在する
      {
        //出勤時刻がセットされているのに退勤時刻がセットされていない
        if (lastWorkLog.attendance_time is not null && lastWorkLog.leaving_time is null)
          return new(false, ApiResultCodes.Work_Not_Ended, CurrentTime);
      }
      else
        WorkLogCache.Add(uID, new()); //新規に追加する(スペースの確保のみ)


      RestWorkLogWithModel newWorkLog = new()
      {
        user_id = uID.Value,
        attendance_time = CurrentTime
      };

      var res = await Sv.CreateWorkLogAsync(newWorkLog);
      var apiRes = ToApiRes(res.Response.StatusCode);

      if (res.Content is null)
      {
        WorkLogCache.Remove(uID); //出勤登録に失敗したら, ローカルでも保持しない
        return new(false, apiRes, CurrentTime);
      }
      else
      {
        WorkLogCache[uID] = res.Content; //キャッシュを行う

        return new(true, apiRes, CurrentTime);
      }
    }
  }
}
