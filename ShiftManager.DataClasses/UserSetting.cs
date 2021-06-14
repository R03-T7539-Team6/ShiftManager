using System;
using System.Collections.Generic;

namespace ShiftManager.DataClasses
{
  public record UserSetting(UserID UserID, NotificationPublishTimings NotificationPublishTiming, List<ClientData> ClientDataList);

  public record ClientData(string Name, string EndPoint, string UserPublicKey, string UserAuthToken);

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
}
