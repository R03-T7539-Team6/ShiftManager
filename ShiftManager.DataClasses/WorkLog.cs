using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record WorkLog(UserID UserID, SortedDictionary<DateTime, SingleWorkLog> WorkLogDictionary) : IWorkLog;

  public record SingleWorkLog(DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleWorkLog;

  public partial class WorkLog_NotifyPropertyChanged : IWorkLog, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private UserID _UserID;
    [AutoNotify]
    private SortedDictionary<DateTime, SingleWorkLog> _WorkLogDictionary;
  }

  public partial class SingleWorkLog_NoifyPropertyChanged : ISingleWorkLog, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private DateTime _AttendanceTime;
    [AutoNotify]
    private DateTime _LeavingTime;
    [AutoNotify]
    private Dictionary<DateTime, int> _BreakTimeDictionary;
  }

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
