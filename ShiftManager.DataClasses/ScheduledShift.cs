using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record ScheduledShift(DateTime TargetDate, DateTime StartOfSchedule, DateTime EndOfSchedule, ShiftSchedulingState SchedulingState, Dictionary<UserID, ISingleShiftData> ShiftDictionary, List<int> RequiredWorkerCountList) : IScheduledShift
  {
    public ScheduledShift(IScheduledShift i) : this(i?.TargetDate ?? new(), i?.StartOfSchedule ?? new(), i?.EndOfSchedule ?? new(), i?.SchedulingState ?? ShiftSchedulingState.None, i?.ShiftDictionary ?? new(), i?.RequiredWorkerCountList ?? new()) { }
  }

  public partial class ScheduledShift_NotifyPropertyChanged : IScheduledShift, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    [AutoNotify]
    private DateTime _TargetDate;
    [AutoNotify]
    private DateTime _StartOfSchedule;
    [AutoNotify]
    private DateTime _EndOfSchedule;
    [AutoNotify]
    private ShiftSchedulingState _SchedulingState;
    [AutoNotify]
    private Dictionary<UserID, ISingleShiftData> _ShiftDictionary = new();
    [AutoNotify]
    private List<int> _RequiredWorkerCountList = new();

  }

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
    Dictionary<UserID, ISingleShiftData> ShiftDictionary { get; }
    List<int> RequiredWorkerCountList { get; }
  }
}
