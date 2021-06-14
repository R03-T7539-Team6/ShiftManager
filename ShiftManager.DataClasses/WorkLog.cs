using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record WorkLog(UserID UserID, );

  public record SingleWorkLog(DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary);
}
