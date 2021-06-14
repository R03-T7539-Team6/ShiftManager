using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  /// <summary>勤務予定/実績用クラス</summary>
  public record SingleShiftData(UserID UserID, DateTime WorkDate, bool IsPaidHoliday, DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary);
}
