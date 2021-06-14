using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record StoreID(string Value);
  public record StoreData(StoreID StoreID, Dictionary<UserID,UserData> UserDataDictionary, Dictionary<UserID, ShiftRequest> ShiftRequestsDictionary, Dictionary<DateTime, ScheduledShift> ScheduledShiftDictionary);
}
