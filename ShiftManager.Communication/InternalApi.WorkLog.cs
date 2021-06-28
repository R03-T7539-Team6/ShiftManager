using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface InternalApi_WorkLog
  {
    Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID);
  }

  public partial class InternalApi : InternalApi_WorkLog
  {
    public Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID) => Task.Run<ApiResult<DateTime>>(() =>
    {
      DateTime CurrentTime = DateTime.Now;

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, CurrentTime);

      var workLogDic = userData.WorkLog.WorkLogDictionary;

      if (workLogDic.Count <= 0) //勤務記録がない == 出勤打刻をしたことがない
        return new(false, ApiResultCodes.Work_Not_Started, CurrentTime);

      ISingleWorkLog lastWorkLog = userData.WorkLog.WorkLogDictionary.Values.Last();

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

      return new(true, ApiResultCodes.Success, CurrentTime);
    });

    static internal int GetBreakTimeLength(in DateTime start, in DateTime end)
      => (int)(new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, 0) - new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0)).TotalMinutes;

    public Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }
  }
}
