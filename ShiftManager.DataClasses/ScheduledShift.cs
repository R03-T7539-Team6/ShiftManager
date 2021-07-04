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
  public record ScheduledShift(DateTime TargetDate, DateTime StartOfSchedule, DateTime EndOfSchedule, ShiftSchedulingState SchedulingState, Dictionary<UserID, ISingleShiftData> ShiftDictionary, List<int> RequiredWorkerCountList) : IScheduledShift
  {
    public ScheduledShift(IScheduledShift i) : this(i?.TargetDate ?? new(), i?.StartOfSchedule ?? new(), i?.EndOfSchedule ?? new(), i?.SchedulingState ?? ShiftSchedulingState.None, i?.ShiftDictionary ?? new(), i?.RequiredWorkerCountList ?? new()) { }
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
  public enum ShiftSchedulingState
  {
    None,
    NotStarted,
    Working,
    FinalVersion
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
