using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record WorkLog(IUserID UserID, SortedDictionary<DateTime, ISingleWorkLog> WorkLogDictionary) : IWorkLog;

  public record SingleWorkLog(DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleWorkLog;

  public partial class WorkLog_NotifyPropertyChanged : IWorkLog, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private IUserID _UserID;
    [AutoNotify]
    private SortedDictionary<DateTime, ISingleWorkLog> _WorkLogDictionary;
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
    IUserID UserID { get; }
    SortedDictionary<DateTime, ISingleWorkLog> WorkLogDictionary { get; }
  }

  public interface ISingleWorkLog
  {
    DateTime AttendanceTime { get; }
    DateTime LeavingTime { get; }
    Dictionary<DateTime, int> BreakTimeDictionary { get; }
  }
}
