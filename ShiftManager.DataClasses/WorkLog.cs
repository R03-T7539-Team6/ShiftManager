using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public record WorkLog(IUserID UserID, SortedDictionary<DateTime, ISingleWorkLog> WorkLogDictionary) : IWorkLog
  {
    public WorkLog(IWorkLog i) : this(i?.UserID ?? new UserID(), i?.WorkLogDictionary ?? new()) { }
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public record SingleWorkLog(DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleWorkLog
  {
    public SingleWorkLog(ISingleWorkLog i) : this(i?.AttendanceTime ?? new(), i?.LeavingTime ?? new(), i?.BreakTimeDictionary ?? new()) { }
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public partial class WorkLog_NotifyPropertyChanged : IWorkLog, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IUserID _UserID = new UserID(string.Empty);
    [AutoNotify]
    private SortedDictionary<DateTime, ISingleWorkLog> _WorkLogDictionary = new();
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
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
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public interface IWorkLog
  {
    IUserID UserID { get; }
    SortedDictionary<DateTime, ISingleWorkLog> WorkLogDictionary { get; }
  }
  /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
  public interface ISingleWorkLog
  {
    DateTime AttendanceTime { get; }
    DateTime LeavingTime { get; }
    Dictionary<DateTime, int> BreakTimeDictionary { get; }
  }
}
