using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record WorkLog(UserID UserID, SortedDictionary<DateTime, SingleWorkLog> WorkLogDictionary);

  public record SingleWorkLog(DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary);

  public interface IWorkLog
  {
    SortedDictionary<DateTime, SingleWorkLog> WorkLogDictionary { get; }
  }
}
