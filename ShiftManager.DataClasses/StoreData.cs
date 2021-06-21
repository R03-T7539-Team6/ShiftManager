using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record StoreID(string Value) : IStoreID;
  public record StoreData(StoreID StoreID, Dictionary<UserID,UserData> UserDataDictionary, Dictionary<UserID, ShiftRequest> ShiftRequestsDictionary, Dictionary<DateTime, ScheduledShift> ScheduledShiftDictionary) : IStoreData;

  public partial class StoreID_NotifyPropertyChanged : IStoreID, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private string _Value;
  }

  public partial class StoreData_NotifyPropertyChanged : IStoreData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private StoreID _StoreID;
    [AutoNotify]
    private Dictionary<UserID, UserData> _UserDataDictionary;
    [AutoNotify]
    private Dictionary<UserID, ShiftRequest> _ShiftRequestsDictionary;
    [AutoNotify]
    private Dictionary<DateTime, ScheduledShift> _ScheduledShiftDictionary;
  }

  public interface IStoreID
  {
    string Value { get; }
  }

  public interface IStoreData
  {
    StoreID StoreID { get; }
    Dictionary<UserID, UserData> UserDataDictionary { get; }
    Dictionary<UserID, ShiftRequest> ShiftRequestsDictionary { get; }
    Dictionary<DateTime, ScheduledShift> ScheduledShiftDictionary { get; }
  }
}
