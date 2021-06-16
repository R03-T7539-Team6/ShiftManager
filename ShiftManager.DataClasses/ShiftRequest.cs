using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record ShiftRequest(UserID UserID, DateTime LastUpdate, Dictionary<DateTime, SingleShiftData> RequestsDictionary) : IShiftRequest;

  public interface IShiftRequest
  {
    UserID UserID { get; }
    DateTime LastUpdate { get; }
    Dictionary<DateTime, SingleShiftData> RequestsDictionary { get; }
  }
}
