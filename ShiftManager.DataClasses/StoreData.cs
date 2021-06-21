using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record StoreID(string Value) : IStoreID;
  public record StoreData(IStoreID StoreID, Dictionary<IUserID, IUserData> UserDataDictionary, Dictionary<IUserID, IShiftRequest> ShiftRequestsDictionary, Dictionary<DateTime, IScheduledShift> ScheduledShiftDictionary) : IStoreData;

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
    private Dictionary<IUserID, IUserData> _UserDataDictionary;
    [AutoNotify]
    private Dictionary<IUserID, IShiftRequest> _ShiftRequestsDictionary;
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
    Dictionary<IUserID, IUserData> UserDataDictionary { get; }
    Dictionary<IUserID, IShiftRequest> ShiftRequestsDictionary { get; }
    Dictionary<DateTime, IScheduledShift> ScheduledShiftDictionary { get; }
  }
}
