using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record WorkLog(IUserID UserID, SortedDictionary<DateTime, ISingleWorkLog> WorkLogDictionary) : IWorkLog
  {
    public WorkLog(IWorkLog i) : this(i?.UserID ?? new UserID(), i?.WorkLogDictionary ?? new()) { }
  }

  public record SingleWorkLog(DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleWorkLog
  {
    public SingleWorkLog(ISingleWorkLog i) : this(i?.AttendanceTime ?? new(), i?.LeavingTime ?? new(), i?.BreakTimeDictionary ?? new()) { }
  }

  public partial class WorkLog_NotifyPropertyChanged : IWorkLog, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IUserID _UserID = new UserID(string.Empty);
    [AutoNotify]
    private SortedDictionary<DateTime, ISingleWorkLog> _WorkLogDictionary = new();
  }

  public partial class SingleWorkLog_NoifyPropertyChanged : ISingleWorkLog, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private DateTime _AttendanceTime;
    [AutoNotify]
    private DateTime _LeavingTime;
    [AutoNotify]
    private Dictionary<DateTime, int> _BreakTimeDictionary = new();
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
