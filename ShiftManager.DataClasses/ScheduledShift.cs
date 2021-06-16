using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record ScheduledShift(DateTime TargetDate, DateTime StartOfSchedule, DateTime EndOfSchedule, ShiftSchedulingState SchedulingState, Dictionary<UserID, SingleShiftData> ShiftDictionary, List<int> RequiredWorkerCountList) : IScheduledShift;

  public enum ShiftSchedulingState
  {
    None,
    NotStarted,
    Working,
    FinalVersion
  }

  public interface IScheduledShift
  {
    DateTime TargetDate { get; }
    DateTime StartOfSchedule { get; }
    DateTime EndOfSchedule { get; }
    ShiftSchedulingState SchedulingState { get; }
    Dictionary<UserID, SingleShiftData> ShiftDictionary { get; }
    List<int> RequiredWorkerCountList { get; }
  }
}
