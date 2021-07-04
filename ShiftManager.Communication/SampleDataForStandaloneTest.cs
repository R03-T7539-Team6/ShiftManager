
using System;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  internal class SampleDataForStandaloneTest
  {
    public StoreData StoreData { get; } =
      new(
        STORE_ID,

        new()
        {
          { new(USER_DATA_ARR[0].UserID), USER_DATA_ARR[0] },
          { new(USER_DATA_ARR[1].UserID), USER_DATA_ARR[1] },
          { new(USER_DATA_ARR[2].UserID), USER_DATA_ARR[2] }
        },

        new()
        {
          { ID0000.UserID, ID0000.ShiftRequest },
          { ID0001.UserID, ID0001.ShiftRequest },
          { ID0002.UserID, ID0002.ShiftRequest }
        },

        new()
        {
          { SCHEDULED_SHIFT_ARR[0].TargetDate, SCHEDULED_SHIFT_ARR[0] },
          { SCHEDULED_SHIFT_ARR[1].TargetDate, SCHEDULED_SHIFT_ARR[1] },
          { SCHEDULED_SHIFT_ARR[2].TargetDate, SCHEDULED_SHIFT_ARR[2] },
          { SCHEDULED_SHIFT_ARR[3].TargetDate, SCHEDULED_SHIFT_ARR[3] },
          { SCHEDULED_SHIFT_ARR[4].TargetDate, SCHEDULED_SHIFT_ARR[4] }
        }
      );

    private static StoreID STORE_ID { get; } = new("STORE001");
    private static UserData[] USER_DATA_ARR { get; } =
    {
      ID0001.UserData, ID0002.UserData, ID0000.UserData
    };
    private static ScheduledShift[] SCHEDULED_SHIFT_ARR { get; } =
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

    private static class ID0001
    {
      public static UserID UserID { get; } = new("ID0001");
      public static HashedPassword HashedPW { get; } = new("xMIjIuiIPYrmBoQqskJHHYlL2hc0TvKsdjbifXICxPzvUkh5/weTbWCoFECQabYZeVP+awQ9Cv+txfWzLtFxQQ==", "mdTM8HTo96Ba3kV77N9MSQ==", 10000);//PW: HWRnwOCy4HMiGPTA
      public static NameData NameData { get; } = new("SuperUser", "Sample");
      public static UserGroup UserGroup { get; } = UserGroup.SuperUser;
      public static UserState UserState { get; } = UserState.Normal;
      public static WorkLog WorkLog { get; } = new(UserID, new());
      public static UserSetting UserSetting { get; } = new(UserID, NotificationPublishTimings.Before24H | NotificationPublishTimings.DayBeforeYesterday_21, new());

      public static UserData UserData { get; } = new(UserID, HashedPW, NameData, STORE_ID, UserGroup, UserState, WorkLog, UserSetting);

      public static SingleShiftData[] SingleShiftDataArr { get; } =
      {
        new(UserID, new(2021, 6, 1), false, new(2021, 6, 1, 12, 0, 0), new(2021, 6, 1, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 2), false, new(2021, 6, 2, 15, 0, 0), new(2021, 6, 2, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 3), false, new(2021, 6, 3,  9, 0, 0), new(2021, 6, 3, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 11), false, new(2021, 6, 11, 22, 0, 0), new(2021, 6, 12, 0, 0, 0), new()),
        new(UserID, new(2021, 6, 12), false, new(2021, 6, 12,  0, 0, 0), new(2021, 6, 12,  5, 0, 0), new()),
      };
      public static ShiftRequest ShiftRequest { get; } = new(UserID, new(2021, 5, 31, 12, 50, 39),
        new()
        {
          { SingleShiftDataArr[0].WorkDate, SingleShiftDataArr[0] },
          { SingleShiftDataArr[1].WorkDate, SingleShiftDataArr[1] },
          { SingleShiftDataArr[2].WorkDate, SingleShiftDataArr[2] },
          { SingleShiftDataArr[3].WorkDate, SingleShiftDataArr[3] },
          { SingleShiftDataArr[4].WorkDate, SingleShiftDataArr[4] },
        });
    }

    private static class ID0002
    {
      public static UserID UserID { get; } = new("ID0002");
      public static HashedPassword HashedPW { get; } = new("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", "LkDfl7iv6fO5bShQoru4Iw==", 10000);//PW: i1KgfuhDy41yGy8x
      public static NameData NameData { get; } = new("SAMPLE", "NormalUser");
      public static UserGroup UserGroup { get; } = UserGroup.NormalUser;
      public static UserState UserState { get; } = UserState.Normal;
      public static WorkLog WorkLog { get; } = new(UserID, new());
      public static UserSetting UserSetting { get; } = new(UserID, NotificationPublishTimings.Before24H | NotificationPublishTimings.DayBeforeYesterday_21, new());

      public static UserData UserData { get; } = new(UserID, HashedPW, NameData, STORE_ID, UserGroup, UserState, WorkLog, UserSetting);

      public static SingleShiftData[] SingleShiftDataArr { get; } =
      {
        new(UserID, new(2021, 6, 1), false, new(2021, 6, 1, 9, 0, 0), new(2021, 6, 1, 12, 0, 0), new()),
        new(UserID, new(2021, 6, 2), true, new(2021, 6, 2, 0, 0, 0), new(2021, 6, 2, 0, 0, 0), new()),
        new(UserID, new(2021, 6, 5), false, new(2021, 6, 5,  9, 0, 0), new(2021, 6, 5, 18, 0, 0), new()),
        new(UserID, new(2021, 6, 11), false, new(2021, 6, 11, 8, 0, 0), new(2021, 6, 12, 0, 0, 0), new()),
        new(UserID, new(2021, 6, 12), false, new(2021, 6, 12,  0, 0, 0), new(2021, 6, 12,  8, 0, 0), new()),
      };
      public static ShiftRequest ShiftRequest { get; } = new(UserID, new(2021, 5, 30, 8, 1, 9),
        new()
        {
          { SingleShiftDataArr[0].WorkDate, SingleShiftDataArr[0] },
          { SingleShiftDataArr[1].WorkDate, SingleShiftDataArr[1] },
          { SingleShiftDataArr[2].WorkDate, SingleShiftDataArr[2] },
          { SingleShiftDataArr[3].WorkDate, SingleShiftDataArr[3] },
          { SingleShiftDataArr[4].WorkDate, SingleShiftDataArr[4] }
        });
    }

    private static class ID0000
    {
      public static UserID UserID { get; } = new("ID0000");
      public static HashedPassword HashedPW { get; } = new("Mk0Lu/PAI+aHFF9PiR+6NFiNnzR8CDbDaNPvqdB+Dh/aHUcJMTCsBE7K9/uMWtgu7FqLcnsyxsu7fToHU1dfjA==", "30/DmISxGM+mLG0kfnbF1Q==", 10000);//PW: 0000
      public static NameData NameData { get; } = new("SAMPLE", "SystemAdmin");
      public static UserGroup UserGroup { get; } = UserGroup.SystemAdmin;
      public static UserState UserState { get; } = UserState.Normal;
      public static WorkLog WorkLog { get; } = new(UserID, new());
      public static UserSetting UserSetting { get; } = new(UserID, NotificationPublishTimings.None, new());

      public static UserData UserData { get; } = new(UserID, HashedPW, NameData, STORE_ID, UserGroup, UserState, WorkLog, UserSetting);

      public static SingleShiftData[] SingleShiftDataArr { get; } = Array.Empty<SingleShiftData>();
      public static ShiftRequest ShiftRequest { get; } = new(UserID, new(2021, 5, 20, 8, 1, 9), new());
    }
  }
}
