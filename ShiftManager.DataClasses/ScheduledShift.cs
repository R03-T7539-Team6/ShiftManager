using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record ScheduledShift(DateTime TargetDate, DateTime StartOfSchedule, DateTime EndOfSchedule, ShiftSchedulingState SchedulingState, Dictionary<UserID, SingleShiftData> ShiftDictionary, List<int> RequiredWorkerCountList);

  public enum ShiftSchedulingState
  {
    None,
    NotStarted,
    Working,
    FinalVersion
  }
}
