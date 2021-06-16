using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record StoreID(string Value) : IStoreID;
  public record StoreData(StoreID StoreID, Dictionary<UserID,UserData> UserDataDictionary, Dictionary<UserID, ShiftRequest> ShiftRequestsDictionary, Dictionary<DateTime, ScheduledShift> ScheduledShiftDictionary) : IStoreData;

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
