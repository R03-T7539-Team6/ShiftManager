
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  internal class SampleDataForStandaloneTest
  {
    static readonly int MinutesPerDay = (int)TimeSpan.FromDays(1).TotalMinutes;
    static readonly int SAMPLE_USER_COUNT = 100;
    static readonly int MAX_SAMPLE_WORKLOG = 64;
    static readonly int MAX_SAMPLE_SHIFTREQ_PerUser = 64;
    static readonly int MAX_SAMPLE_SCEDULEDSHIFT = 32;

    public SampleDataForStandaloneTest()
    {
      List<IUserData> userDataList = new();
      List<IShiftRequest> shiftReqList = new();
      List<IScheduledShift> scheduledShiftList = new();

      for(int i = 0; i < SAMPLE_USER_COUNT; i++)
      {
        UserID id = new UserID($"ID{i:D6}");

        #region 勤務実績
        List<ISingleWorkLog> wLogList = new();
        int rndInt = RandomNumberGenerator.GetInt32(MAX_SAMPLE_WORKLOG - 10);

        for (int d = 0; d < rndInt + 10; d++) {
          int v = RandomNumberGenerator.GetInt32(5);
          if (v % 5 == 0)
            continue;

          DateTime workDate = DateTime.Today.AddDays(d * -1);
          DateTime attendTime = workDate.AddMinutes(RandomNumberGenerator.GetInt32(MinutesPerDay));
          TimeSpan maxWorkTimeLen = TimeSpan.FromDays(1) - attendTime.TimeOfDay;

          if (maxWorkTimeLen == TimeSpan.Zero)
            continue;

          DateTime leaveTime = attendTime.AddMinutes(RandomNumberGenerator.GetInt32((int)maxWorkTimeLen.TotalMinutes));

          wLogList.Add(new SingleWorkLog(attendTime, leaveTime, new()));
        }

        userDataList.Add(
          new UserData(
            id,
            ID0000.HashedPW,
            new NameData($"FN{i:D6}", $"LN{i:06}"),
            STORE_ID,
            UserGroup.SystemAdmin,
            UserState.NotHired,
            new WorkLog(id, new(dictionary: wLogList.ToDictionary(v => v.AttendanceTime))),
            new UserSetting(id, NotificationPublishTimings.None, new())
            )
          );
        #endregion

        #region シフト希望
        rndInt = RandomNumberGenerator.GetInt32(MAX_SAMPLE_SHIFTREQ_PerUser - 10);
        List<ISingleShiftData> singleShiftList = new();
        for (int d = 0; d < rndInt + 10; d++)
        {
          int v = RandomNumberGenerator.GetInt32(5);
          if (v % 5 == 0)
            continue;

          DateTime workDate = DateTime.Today.AddDays(d);
          DateTime attendTime = workDate.AddMinutes(RandomNumberGenerator.GetInt32(MinutesPerDay));
          TimeSpan maxWorkTimeLen = TimeSpan.FromDays(1) - attendTime.TimeOfDay;

          if (maxWorkTimeLen == TimeSpan.Zero)
            continue;

          DateTime leaveTime = attendTime.AddMinutes(RandomNumberGenerator.GetInt32((int)maxWorkTimeLen.TotalMinutes));

          singleShiftList.Add(new SingleShiftData(id, workDate, false, attendTime, leaveTime, new())); ;
        }
        shiftReqList.Add(new ShiftRequest(id, DateTime.Now, singleShiftList.ToDictionary(x => x.WorkDate)));
        #endregion
      }

      #region 勤務予定(予定シフト)

      #endregion

      StoreData = new(STORE_ID, userDataList.ToDictionary(v => new UserID(v.UserID)), shiftReqList.ToDictionary(v => new UserID(v.UserID)), scheduledShiftList.ToDictionary(v => v.TargetDate));

    }

    public StoreData StoreData { get; }
    private static StoreID STORE_ID { get => new("STORE001"); }
    private static UserData[] USER_DATA_ARR
    {
      get => new[]{
        ID0001.UserData, ID0002.UserData, ID0000.UserData
    };
    }
    private static ScheduledShift[] SCHEDULED_SHIFT_ARR
    {
      get => new ScheduledShift[]
{
      new(new(2021, 5, 29), new(2021, 5, 29, 0, 0, 0), new(2021, 5, 30, 0, 0, 0), ShiftSchedulingState.FinalVersion,
        new()
        {
          { ID0001.UserID,
            new SingleShiftData(ID0001.UserID, new(2021, 5, 29), false, new(2021, 5, 29, 9, 0, 0), new(2021, 5, 29, 12, 0, 0),
              new()
            )
          },
          {
            ID0002.UserID,
            new SingleShiftData(ID0002.UserID, new(2021, 5, 29), false, new(2021, 5, 29, 8, 0, 0), new(2021, 5, 29, 18, 30, 0), new()
              {
                { new(2021, 5, 29, 9, 0, 0), 60 },
                { new(2021, 5, 29, 12, 12, 0), 18 }
              }
            )
          }
        },
        new(){1}),

      new(new(2021, 5, 30), new(2021, 5, 30, 12, 0, 0), new(2021, 5, 30, 18, 0, 0), ShiftSchedulingState.FinalVersion,
        new()
        {
          {
            ID0001.UserID,
            new SingleShiftData(ID0001.UserID, new(2021, 5, 30), false, new(2021, 5, 30, 12, 0, 0), new(2021, 5, 30, 18, 0, 0),
              new()
            )
          }
        },

        new(){1}),
      new(new(2021, 5, 31), new(2021, 5, 31, 0, 0, 0), new(2021, 6, 1, 0, 0, 0), ShiftSchedulingState.NotStarted, new(), new(){1}),
      new(new(2021, 7, 4), new(2021, 7, 4, 0, 0, 0), new(2021, 7, 5, 0, 0, 0), ShiftSchedulingState.Working,
        new()
        {
          {
            ID0000.UserID,
            new SingleShiftData(ID0000.UserID, new(2021, 7, 4), false, new(2021, 7, 4, 12, 0, 0), new(2021, 7, 4, 18, 0, 0),
              new()
            )
          }
        },
        new(){1}),
      new(new(2021, 7, 5), new(2021, 7, 5, 0, 0, 0), new(2021, 7, 6, 0, 0, 0), ShiftSchedulingState.NotStarted, new(), new(){1}),
    };
    }

    private static class ID0001
    {
      public static UserID UserID { get => new("ID0001"); }
      public static HashedPassword HashedPW { get => new("xMIjIuiIPYrmBoQqskJHHYlL2hc0TvKsdjbifXICxPzvUkh5/weTbWCoFECQabYZeVP+awQ9Cv+txfWzLtFxQQ==", "mdTM8HTo96Ba3kV77N9MSQ==", 10000); }//PW: HWRnwOCy4HMiGPTA
      public static NameData NameData { get => new("SuperUser", "Sample"); }
      public static UserGroup UserGroup { get => UserGroup.SuperUser; }
      public static UserState UserState { get => UserState.Normal; }
      public static WorkLog WorkLog { get => new(UserID, new()); }
      public static UserSetting UserSetting { get => new(UserID, NotificationPublishTimings.Before24H | NotificationPublishTimings.DayBeforeYesterday_21, new()); }

      public static UserData UserData { get => new(UserID, HashedPW, NameData, STORE_ID, UserGroup, UserState, WorkLog, UserSetting); }

      public static SingleShiftData[] SingleShiftDataArr
      {
        get => new SingleShiftData[]
{
        new(UserID, new(2021, 6, 1), false, new(2021, 6, 1, 12, 0, 0), new(2021, 6, 1, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 2), false, new(2021, 6, 2, 15, 0, 0), new(2021, 6, 2, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 3), false, new(2021, 6, 3,  9, 0, 0), new(2021, 6, 3, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 11), false, new(2021, 6, 11, 22, 0, 0), new(2021, 6, 12, 0, 0, 0), new()),
        new(UserID, new(2021, 6, 12), false, new(2021, 6, 12,  0, 0, 0), new(2021, 6, 12,  5, 0, 0), new()),
};
      }
      public static ShiftRequest ShiftRequest
      {
        get => new(UserID, new(2021, 5, 31, 12, 50, 39),
new()
{
  { SingleShiftDataArr[0].WorkDate, SingleShiftDataArr[0] },
  { SingleShiftDataArr[1].WorkDate, SingleShiftDataArr[1] },
  { SingleShiftDataArr[2].WorkDate, SingleShiftDataArr[2] },
  { SingleShiftDataArr[3].WorkDate, SingleShiftDataArr[3] },
  { SingleShiftDataArr[4].WorkDate, SingleShiftDataArr[4] },
});
      }
    }

    private static class ID0002
    {
      public static UserID UserID { get => new("ID0002"); }
      public static HashedPassword HashedPW { get => new("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", "LkDfl7iv6fO5bShQoru4Iw==", 10000); }//PW: i1KgfuhDy41yGy8x
      public static NameData NameData { get => new("SAMPLE", "NormalUser"); }
      public static UserGroup UserGroup { get => UserGroup.NormalUser; }
      public static UserState UserState { get => UserState.Normal; }
      public static WorkLog WorkLog { get => new(UserID, new()); }
      public static UserSetting UserSetting { get => new(UserID, NotificationPublishTimings.Before24H | NotificationPublishTimings.DayBeforeYesterday_21, new()); }

      public static UserData UserData { get => new(UserID, HashedPW, NameData, STORE_ID, UserGroup, UserState, WorkLog, UserSetting); }

      public static SingleShiftData[] SingleShiftDataArr
      {
        get => new SingleShiftData[]
{
        new(UserID, new(2021, 6, 1), false, new(2021, 6, 1, 9, 0, 0), new(2021, 6, 1, 12, 0, 0), new()),
        new(UserID, new(2021, 6, 2), true, new(2021, 6, 2, 0, 0, 0), new(2021, 6, 2, 0, 0, 0), new()),
        new(UserID, new(2021, 6, 5), false, new(2021, 6, 5,  9, 0, 0), new(2021, 6, 5, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 11), false, new(2021, 6, 11, 8, 0, 0), new(2021, 6, 12, 0, 0, 0), new()),
        new(UserID, new(2021, 6, 12), false, new(2021, 6, 12,  0, 0, 0), new(2021, 6, 12,  8, 0, 0), new()),
};
      }
      public static ShiftRequest ShiftRequest
      {
        get => new(UserID, new(2021, 5, 30, 8, 1, 9),
new()
{
  { SingleShiftDataArr[0].WorkDate, SingleShiftDataArr[0] },
  { SingleShiftDataArr[1].WorkDate, SingleShiftDataArr[1] },
  { SingleShiftDataArr[2].WorkDate, SingleShiftDataArr[2] },
  { SingleShiftDataArr[3].WorkDate, SingleShiftDataArr[3] },
  { SingleShiftDataArr[4].WorkDate, SingleShiftDataArr[4] }
});
      }
    }

    private static class ID0000
    {
      public static UserID UserID { get => new("ID0000"); }
      public static HashedPassword HashedPW { get => new("Mk0Lu/PAI+aHFF9PiR+6NFiNnzR8CDbDaNPvqdB+Dh/aHUcJMTCsBE7K9/uMWtgu7FqLcnsyxsu7fToHU1dfjA==", "30/DmISxGM+mLG0kfnbF1Q==", 10000); }//PW: 0000
      public static NameData NameData { get => new("SAMPLE", "SystemAdmin"); }
      public static UserGroup UserGroup { get => UserGroup.SystemAdmin; }
      public static UserState UserState { get => UserState.Normal; }
      public static WorkLog WorkLog { get => new(UserID, new()); }
      public static UserSetting UserSetting { get => new(UserID, NotificationPublishTimings.None, new()); }

      public static UserData UserData { get => new(UserID, HashedPW, NameData, STORE_ID, UserGroup, UserState, WorkLog, UserSetting); }

      public static SingleShiftData[] SingleShiftDataArr { get => Array.Empty<SingleShiftData>(); }
      public static ShiftRequest ShiftRequest { get => new(UserID, new(2021, 5, 20, 8, 1, 9), new()); }
    }
  }
}
