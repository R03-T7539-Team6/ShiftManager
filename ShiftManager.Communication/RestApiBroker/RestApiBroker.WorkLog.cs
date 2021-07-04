using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public partial class RestApiBroker : InternalApi_WorkLog
  {
    public ICurrentTimeProvider CurrentTimeProvider { get; init; } = new CurrentTimeProvider();

    Dictionary<UserID, ISingleWorkLog> WorkLogCache { get; } = new();

    /// <summary>指定のユーザについて, 休憩終了の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    public async Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!WorkLogCache.TryGetValue(new UserID(userID), out var lastWorkLog) || lastWorkLog is null || lastWorkLog.AttendanceTime == default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.AttendanceTime == default || lastWorkLog.LeavingTime != default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      KeyValuePair<DateTime, int> lastBreakLog = lastWorkLog.BreakTimeDictionary.LastOrDefault();
      if (lastBreakLog.Key == default || lastBreakLog.Value > 0) //休憩開始時刻の記録がない || 休憩終了時刻の記録がある
        return new(false, ApiResultCodes.BreakTime_Not_Started, CurrentTime);

      int breakTimeLen = GetBreakTimeLength(lastBreakLog.Key, CurrentTime);
      if (breakTimeLen <= 0)
        return new(false, ApiResultCodes.BreakTimeLen_Zero_Or_Less, CurrentTime);

      lastWorkLog.BreakTimeDictionary[lastBreakLog.Key] = breakTimeLen;

      var res = await Api.ExecuteWithDataAsync<RestData.RestWorkLog, RestData.RestWorkLog>("/logs", RestData.RestWorkLog.GenerateFromSingleWorkLog(lastWorkLog, userID), RestSharp.Method.PUT);

      return new(res.IsSuccess, res.ResultCode, CurrentTime);
    }

    /// <summary>休憩時刻の長さを計算します</summary>
    /// <param name="start">休憩開始時刻</param>
    /// <param name="end">休憩終了時刻</param>
    /// <returns>休憩時間長 [min]</returns>
    static internal int GetBreakTimeLength(in DateTime start, in DateTime end)
      => (int)(new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, 0) - new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0)).TotalMinutes;

    /// <summary>指定のユーザについて, 休憩開始の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    public async Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!WorkLogCache.TryGetValue(new UserID(userID), out var lastWorkLog) || lastWorkLog is null || lastWorkLog.AttendanceTime == default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.AttendanceTime == default || lastWorkLog.LeavingTime != default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);


      KeyValuePair<DateTime, int> lastBreakLog = lastWorkLog.BreakTimeDictionary.LastOrDefault();
      if (lastBreakLog.Key != default && lastBreakLog.Value <= 0) //休憩終了時刻の記録がない
        return new(false, ApiResultCodes.BreakTime_Not_Ended, CurrentTime);

      if (lastBreakLog.Key == default)
        lastWorkLog.BreakTimeDictionary.Remove(default); //デフォルト値のキーは念のため削除

      lastWorkLog.BreakTimeDictionary.Add(CurrentTime, 0);

      var res = await Api.ExecuteWithDataAsync<RestData.RestWorkLog, RestData.RestWorkLog>("/logs", RestData.RestWorkLog.GenerateFromSingleWorkLog(lastWorkLog, userID), RestSharp.Method.PUT);

      return new(res.IsSuccess, res.ResultCode, CurrentTime);
    }

    /// <summary>指定のユーザについて, 退勤の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    public async Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;

      if (!WorkLogCache.TryGetValue(new UserID(userID), out var lastWorkLog) || lastWorkLog is null || lastWorkLog.AttendanceTime == default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //最後の要素に出勤打刻が行われていない || 最後の要素に退勤打刻が終わっている
      if (lastWorkLog.AttendanceTime == default || lastWorkLog.LeavingTime != default)
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      //休憩記録が存在し, かつ最後の記録に休憩時間長が記録されていない
      if (lastWorkLog.BreakTimeDictionary.Count > 0 && lastWorkLog.BreakTimeDictionary.Last().Value <= 0)
        return new(false, ApiResultCodes.BreakTime_Not_Ended, CurrentTime);

      lastWorkLog = new SingleWorkLog(lastWorkLog) with { LeavingTime = CurrentTime };

      var res = await Api.ExecuteWithDataAsync<RestData.RestWorkLog, RestData.RestWorkLog>("/logs", RestData.RestWorkLog.GenerateFromSingleWorkLog(lastWorkLog, userID), RestSharp.Method.PUT);

      return new(res.IsSuccess, res.ResultCode, CurrentTime);
    }

    /// <summary>指定のユーザについて, 出勤の打刻を行います</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>実行結果と, 処理した時刻</returns>
    public async Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID)
    {
      DateTime CurrentTime = CurrentTimeProvider.CurrentTime;
      UserID uID = new(userID);
      if (!WorkLogCache.TryGetValue(uID, out var lastWorkLog))
      {
        if (lastWorkLog is null || lastWorkLog.LeavingTime == default)
          return new(false, ApiResultCodes.Work_Not_Ended, CurrentTime);
      }
      else
        WorkLogCache[uID] = new SingleWorkLog(CurrentTime, default, new());

      var res = await Api.ExecuteWithDataAsync<RestData.RestWorkLog, RestData.RestWorkLog>("/logs", RestData.RestWorkLog.GenerateFromSingleWorkLog(lastWorkLog, userID));

      return new(true, ApiResultCodes.Success, CurrentTime);
    }
  }
}
