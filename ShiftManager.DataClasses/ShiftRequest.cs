using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record ShiftRequest(IUserID UserID, DateTime LastUpdate, Dictionary<DateTime, ISingleShiftData> RequestsDictionary) : IShiftRequest
  {
    public ShiftRequest(IShiftRequest i) : this(i.UserID, i.LastUpdate, i.RequestsDictionary) { }
  }

  public partial class ShiftRequest_NotifyPropertuChanged : IShiftRequest, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private IUserID _UserID;
    [AutoNotify]
    private DateTime _LastUpdate;
    [AutoNotify]
    private Dictionary<DateTime, ISingleShiftData> _RequestsDictionary;
  }

  public interface IShiftRequest
  {
    IUserID UserID { get; }
    DateTime LastUpdate { get; }
    Dictionary<DateTime, ISingleShiftData> RequestsDictionary { get; }
  }
}
