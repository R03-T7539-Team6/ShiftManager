using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record StoreID(string Value) : IStoreID;
  public record StoreData(IStoreID StoreID, Dictionary<UserID, IUserData> UserDataDictionary, Dictionary<UserID, IShiftRequest> ShiftRequestsDictionary, Dictionary<DateTime, IScheduledShift> ScheduledShiftDictionary) : IStoreData;

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
    private IStoreID _StoreID;
    [AutoNotify]
    private Dictionary<UserID, IUserData> _UserDataDictionary;
    [AutoNotify]
    private Dictionary<UserID, IShiftRequest> _ShiftRequestsDictionary;
    [AutoNotify]
    private Dictionary<DateTime, IScheduledShift> _ScheduledShiftDictionary;
  }

  public interface IStoreID
  {
    string Value { get; }
  }

  public interface IStoreData
  {
    IStoreID StoreID { get; }
    Dictionary<UserID, IUserData> UserDataDictionary { get; }
    Dictionary<UserID, IShiftRequest> ShiftRequestsDictionary { get; }
    Dictionary<DateTime, IScheduledShift> ScheduledShiftDictionary { get; }
  }
}
