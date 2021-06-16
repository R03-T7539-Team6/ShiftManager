using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  /// <summary>勤務予定/実績用クラス</summary>
  public record SingleShiftData(UserID UserID, DateTime WorkDate, bool IsPaidHoliday, DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleShiftData;

  public interface ISingleShiftData
  {
    UserID UserID { get; }
    DateTime WorkDate { get; }
    bool IsPaidHoliday { get; }
    DateTime AttendanceTime { get; }
    DateTime LeavingTime { get; }
    Dictionary<DateTime, int> BreakTimeDictionary { get; }
  }
}
