using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  /// <summary>勤務予定/実績用クラス</summary>
  public record SingleShiftData(IUserID UserID, DateTime WorkDate, bool IsPaidHoliday, DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleShiftData
  {
    public SingleShiftData(ISingleShiftData i) : this(i?.UserID ?? new UserID(), i?.WorkDate ?? new(), i?.IsPaidHoliday ?? false, i?.AttendanceTime ?? new(), i?.LeavingTime ?? new(), new(i?.BreakTimeDictionary ?? new())) { }
  }

  public partial class SingleShiftData_NotifyPropertuChanged : ISingleShiftData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IUserID _UserID = new UserID();
    [AutoNotify]
    private DateTime _WorkDate = new();
    [AutoNotify]
    private bool _IsPaidHoliday = false;
    [AutoNotify]
    private DateTime _AttendanceTime = new();
    [AutoNotify]
    private DateTime _LeavingTime = new();
    [AutoNotify]
    private Dictionary<DateTime, int> _BreakTimeDictionary = new();
  }

  public interface ISingleShiftData
  {
    IUserID UserID { get; }
    DateTime WorkDate { get; }
    bool IsPaidHoliday { get; }
    DateTime AttendanceTime { get; }
    DateTime LeavingTime { get; }
    Dictionary<DateTime, int> BreakTimeDictionary { get; }
  }
}
