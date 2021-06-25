using System;
using System.Collections.Generic;
using System.ComponentModel;

using AutoNotify;

namespace ShiftManager.DataClasses
{
  public record UserSetting(IUserID UserID, NotificationPublishTimings NotificationPublishTiming, List<IClientData> ClientDataList) : IUserSetting
  {
    public UserSetting(IUserSetting i) : this(i?.UserID ?? new UserID(), i?.NotificationPublishTiming?? NotificationPublishTimings.None, i?.ClientDataList ?? new()) { }
  }

  public record ClientData(string Name, string EndPoint, string UserPublicKey, string UserAuthToken) : IClientData
  {
    public ClientData() : this(string.Empty, string.Empty, string.Empty, string.Empty) { }
    public ClientData(IClientData i) : this(i?.Name ?? string.Empty, i?.EndPoint ?? string.Empty, i?.UserPublicKey ?? string.Empty, i?.UserAuthToken ?? string.Empty) { }
  }

  public partial class UserSetting_NotifyPropertyChanged : IUserSetting, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private IUserID _UserID = new UserID();
    [AutoNotify]
    private NotificationPublishTimings _NotificationPublishTiming;
    [AutoNotify]
    private List<IClientData> _ClientDataList = new();
  }

  public partial class ClientData_NotifyPropertyChanged : IClientData, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    [AutoNotify]
    private string _Name = string.Empty;
    [AutoNotify]
    private string _EndPoint = string.Empty;
    [AutoNotify]
    private string _UserPublicKey = string.Empty;
    [AutoNotify]
    private string _UserAuthToken = string.Empty;
  }

  [Flags]
  public enum NotificationPublishTimings
  {
    None = 0,
    Before1H = 1 << 1,
    Before3H = 1 << 2,
    Before6H = 1 << 3,
    Before9H = 1 << 4,
    Before12H = 1 << 5,
    Before24H = 1 << 6,
    Before48H = 1 << 7,
    Before72H = 1 << 8,
    TwoDaysBefore_9 = 1 << 9,
    TwoDaysBefore_15 = 1 << 10,
    TwoDaysBefore_21 = 1 << 11,
    DayBeforeYesterday_9 = 1 << 12,
    DayBeforeYesterday_15 = 1 << 13,
    DayBeforeYesterday_21 = 1 << 14,
    SameDay_9 = 1 << 15,
    SameDay_15 = 1 << 16,
    SameDay_21 = 1 << 17,
  }

  public interface IUserSetting
  {
    IUserID UserID { get; }
    NotificationPublishTimings NotificationPublishTiming { get; }
    List<IClientData> ClientDataList { get; }
  }
  public interface IClientData
  {
    string Name { get; }
    string EndPoint { get; }
    string UserPublicKey { get; }
    string UserAuthToken { get; }
  }
}
