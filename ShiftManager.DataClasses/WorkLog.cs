using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record WorkLog(UserID UserID, SortedDictionary<DateTime, SingleWorkLog> WorkLogDictionary) : IWorkLog;

  public record SingleWorkLog(DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleWorkLog;

  public interface IWorkLog
  {
    UserID UserID { get; }
    SortedDictionary<DateTime, SingleWorkLog> WorkLogDictionary { get; }
  }

  public interface ISingleWorkLog
  {
    DateTime AttendanceTime { get; }
    DateTime LeavingTime { get; }
    Dictionary<DateTime, int> BreakTimeDictionary { get; }
  }
}
