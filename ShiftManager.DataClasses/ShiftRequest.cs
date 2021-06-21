using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record ShiftRequest(UserID UserID, DateTime LastUpdate, Dictionary<DateTime, SingleShiftData> RequestsDictionary) : IShiftRequest;

  public partial class ShiftRequest_NotifyPropertuChanged : IShiftRequest, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private UserID _UserID;
    [AutoNotify]
    private DateTime _LastUpdate;
    [AutoNotify]
    private Dictionary<DateTime, SingleShiftData> _RequestsDictionary;
  }

  public interface IShiftRequest
  {
    UserID UserID { get; }
    DateTime LastUpdate { get; }
    Dictionary<DateTime, SingleShiftData> RequestsDictionary { get; }
  }
}
