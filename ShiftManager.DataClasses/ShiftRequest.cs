using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record ShiftRequest(IUserID UserID, DateTime LastUpdate, Dictionary<DateTime, ISingleShiftData> RequestsDictionary) : IShiftRequest
  {
    public ShiftRequest(IShiftRequest i) : this(i?.UserID ?? new UserID(), i?.LastUpdate ?? new(), i?.RequestsDictionary ?? new()) { }
  }

  public partial class ShiftRequest_NotifyPropertuChanged : IShiftRequest, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IUserID _UserID = new UserID();
    [AutoNotify]
    private DateTime _LastUpdate = new();
    [AutoNotify]
    private Dictionary<DateTime, ISingleShiftData> _RequestsDictionary = new();
  }

  public interface IShiftRequest
  {
    IUserID UserID { get; }
    DateTime LastUpdate { get; }
    Dictionary<DateTime, ISingleShiftData> RequestsDictionary { get; }
  }
}
