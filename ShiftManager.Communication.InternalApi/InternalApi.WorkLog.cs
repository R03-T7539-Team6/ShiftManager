using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public partial class InternalApi : InternalApi_WorkLog
  {
    public ICurrentTimeProvider CurrentTimeProvider { get; init; } = new CurrentTimeProvider();

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
    public Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID) => Task.Run<ApiResult<DateTime>>(() =>
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, CurrentTime);

      var workLogDic = userData.WorkLog.WorkLogDictionary;

      if (workLogDic.Count <= 0) //勤務記録がない == 出勤打刻をしたことがない
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      ISingleWorkLog lastWorkLog = workLogDic.Values.Last();

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.AttendanceTime == default || lastWorkLog.LeavingTime != default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      KeyValuePair<DateTime, int> lastBreakLog = lastWorkLog.BreakTimeDictionary.LastOrDefault();
      if (lastBreakLog.Key == default || lastBreakLog.Value > 0) //休憩開始時刻の記録がない || 休憩終了時刻の記録がある
        return new(false, ApiResultCodes.BreakTime_Not_Started, CurrentTime);

      int breakTimeLen = SharedFuncs.GetBreakTimeLength(lastBreakLog.Key, CurrentTime);
      if (breakTimeLen <= 0)
        return new(false, ApiResultCodes.BreakTimeLen_Zero_Or_Less, CurrentTime);

      lastWorkLog.BreakTimeDictionary[lastBreakLog.Key] = breakTimeLen;

      return new(true, ApiResultCodes.Success, CurrentTime);
    });

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
    public Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID) => Task.Run<ApiResult<DateTime>>(() =>
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, CurrentTime);

      var workLogDic = userData.WorkLog.WorkLogDictionary;

      if (workLogDic.Count <= 0) //勤務記録がない == 出勤打刻をしたことがない
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      ISingleWorkLog lastWorkLog = workLogDic.Values.Last();

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.AttendanceTime == default || lastWorkLog.LeavingTime != default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);


      KeyValuePair<DateTime, int> lastBreakLog = lastWorkLog.BreakTimeDictionary.LastOrDefault();
      if (lastBreakLog.Key != default && lastBreakLog.Value <= 0) //休憩終了時刻の記録がない
        return new(false, ApiResultCodes.BreakTime_Not_Ended, CurrentTime);

      if (lastBreakLog.Key == default)
        lastWorkLog.BreakTimeDictionary.Remove(default); //デフォルト値のキーは念のため削除

      lastWorkLog.BreakTimeDictionary.Add(CurrentTime, 0);

      return new(true, ApiResultCodes.Success, CurrentTime);
    });

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
    public Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID) => Task.Run<ApiResult<DateTime>>(() =>
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, CurrentTime);

      var workLogDic = userData.WorkLog.WorkLogDictionary;

      if (workLogDic.Count <= 0) //勤務記録がない == 出勤打刻をしたことがない
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      ISingleWorkLog lastWorkLog = workLogDic.Values.Last();

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.AttendanceTime == default || lastWorkLog.LeavingTime != default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //休憩記録が存在し, かつ最後の記録に休憩時間長が記録されていない
      if (lastWorkLog.BreakTimeDictionary.Count > 0 && lastWorkLog.BreakTimeDictionary.Last().Value <= 0)
        return new(false, ApiResultCodes.BreakTime_Not_Ended, CurrentTime);

      DateTime attendT = lastWorkLog.AttendanceTime;
      if (new DateTime(attendT.Year, attendT.Month, attendT.Day, attendT.Hour, attendT.Minute, 0)
      == new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, CurrentTime.Hour, CurrentTime.Minute, 0))
        return new(false, ApiResultCodes.WorkTimeLen_Too_Short, CurrentTime);

      workLogDic[attendT] = new SingleWorkLog(lastWorkLog) with { LeavingTime = CurrentTime };//退勤打刻

      return new(true, ApiResultCodes.Success, CurrentTime);
    });

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
    public Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID) => Task.Run<ApiResult<DateTime>>(() =>
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, CurrentTime);

      var workLogDic = userData.WorkLog.WorkLogDictionary;

      ISingleWorkLog? lastWorkLog = workLogDic.Values.LastOrDefault();

      //最後の要素に出勤打刻が行われており, かつ退勤打刻が行われていない
      if (lastWorkLog is not null && lastWorkLog.AttendanceTime != default && lastWorkLog.LeavingTime == default)
        return new(false, ApiResultCodes.Work_Not_Ended, CurrentTime);

      workLogDic.Add(CurrentTime, new SingleWorkLog(CurrentTime, default, new())); //出勤打刻

      return new(true, ApiResultCodes.Success, CurrentTime);
    });
  }
}
