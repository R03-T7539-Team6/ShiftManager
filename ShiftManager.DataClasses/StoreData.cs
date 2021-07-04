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
  public record StoreID(string Value) : IStoreID
  {
    public StoreID() : this(string.Empty) { }
    public StoreID(IStoreID i) : this(i?.Value ?? string.Empty) { }
  }
  public record StoreData(IStoreID StoreID, Dictionary<UserID, IUserData> UserDataDictionary, Dictionary<UserID, IShiftRequest> ShiftRequestsDictionary, Dictionary<DateTime, IScheduledShift> ScheduledShiftDictionary) : IStoreData
  {
    public StoreData(IStoreData i) : this(i?.StoreID ?? new StoreID(), i?.UserDataDictionary ?? new(), i?.ShiftRequestsDictionary ?? new(), i?.ScheduledShiftDictionary ?? new()) { }
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
  public partial class StoreID_NotifyPropertyChanged : IStoreID, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private string _Value = string.Empty;
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
  public partial class StoreData_NotifyPropertyChanged : IStoreData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IStoreID _StoreID = new StoreID();
    [AutoNotify]
    private Dictionary<UserID, IUserData> _UserDataDictionary = new();
    [AutoNotify]
    private Dictionary<UserID, IShiftRequest> _ShiftRequestsDictionary = new();
    [AutoNotify]
    private Dictionary<DateTime, IScheduledShift> _ScheduledShiftDictionary = new();
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
  public interface IStoreID
  {
    string Value { get; }
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
  public interface IStoreData
  {
    IStoreID StoreID { get; }
    Dictionary<UserID, IUserData> UserDataDictionary { get; }
    Dictionary<UserID, IShiftRequest> ShiftRequestsDictionary { get; }
    Dictionary<DateTime, IScheduledShift> ScheduledShiftDictionary { get; }
  }
}
