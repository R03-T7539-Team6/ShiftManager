using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  /// <summary>勤務予定/実績用クラス</summary>
  public record SingleShiftData(IUserID UserID, DateTime WorkDate, bool IsPaidHoliday, DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleShiftData
  {
    public SingleShiftData(ISingleShiftData i) : this(i.UserID, i.WorkDate, i.IsPaidHoliday, i.AttendanceTime, i.LeavingTime, i.BreakTimeDictionary) { }
  }

  public partial class SingleShiftData_NotifyPropertuChanged : ISingleShiftData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private IUserID _UserID;
    [AutoNotify]
    private DateTime _WorkDate;
    [AutoNotify]
    private bool _IsPaidHoliday;
    [AutoNotify]
    private DateTime _AttendanceTime;
    [AutoNotify]
    private DateTime _LeavingTime;
    [AutoNotify]
    private Dictionary<DateTime, int> _BreakTimeDictionary;
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
