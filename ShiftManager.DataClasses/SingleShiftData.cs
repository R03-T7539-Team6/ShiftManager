using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  /// <summary>勤務予定/実績用クラス</summary>
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
  public record SingleShiftData(IUserID UserID, DateTime WorkDate, bool IsPaidHoliday, DateTime AttendanceTime, DateTime LeavingTime, Dictionary<DateTime, int> BreakTimeDictionary) : ISingleShiftData
  {
    public SingleShiftData(ISingleShiftData i) : this(i?.UserID ?? new UserID(), i?.WorkDate ?? new(), i?.IsPaidHoliday ?? false, i?.AttendanceTime ?? new(), i?.LeavingTime ?? new(), new(i?.BreakTimeDictionary ?? new())) { }
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
  public partial class SingleShiftData_NotifyPropertuChanged : ISingleShiftData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    public SingleShiftData_NotifyPropertuChanged() { }
    public SingleShiftData_NotifyPropertuChanged(in ISingleShiftData i)
    {
      if (i is null)
        return;

      UserID = i.UserID;
      WorkDate = i.WorkDate;
      IsPaidHoliday = i.IsPaidHoliday;
      AttendanceTime = i.AttendanceTime;
      LeavingTime = i.LeavingTime;
      BreakTimeDictionary = i.BreakTimeDictionary; //コピーコンストラクタを使うかどうかは要検討
    }

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
