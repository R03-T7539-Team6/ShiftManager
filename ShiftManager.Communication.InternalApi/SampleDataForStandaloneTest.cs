
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

    public static HashedPassword PW_0000 { get => new("Mk0Lu/PAI+aHFF9PiR+6NFiNnzR8CDbDaNPvqdB+Dh/aHUcJMTCsBE7K9/uMWtgu7FqLcnsyxsu7fToHU1dfjA==", "30/DmISxGM+mLG0kfnbF1Q==", 10000); }//PW: 0000

    public StoreData StoreData { get; }
    private static StoreID STORE_ID { get => new("STORE001"); }

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
        #endregion

        userDataList.Add(
          new UserData(
            id,
            PW_0000,
            new NameData($"FN{i:D6}", $"LN{i:D6}"),
            STORE_ID,
            UserGroup.SystemAdmin,
            UserState.NotHired,
            new WorkLog(id, new(dictionary: wLogList.ToDictionary(v => v.AttendanceTime))),
            new UserSetting(id, NotificationPublishTimings.None, new())
            )
          );

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

          singleShiftList.Add(new SingleShiftData(id, workDate, false, attendTime, leaveTime, new()));
        }
        shiftReqList.Add(new ShiftRequest(id, DateTime.Now, singleShiftList.ToDictionary(x => x.WorkDate)));
        #endregion
      }

      #region 勤務予定(予定シフト)
      for (int i = 0; i < MAX_SAMPLE_SCEDULEDSHIFT; i++)
      {
        DateTime workDate = DateTime.Today.AddDays(i);

        List<ISingleShiftData> shiftList = new();

        foreach (var u in userDataList)
        {
          int v = RandomNumberGenerator.GetInt32(5);
          if (v % 5 == 0)
            continue;

          DateTime attendTime = workDate.AddMinutes(RandomNumberGenerator.GetInt32(MinutesPerDay));
          TimeSpan maxWorkTimeLen = TimeSpan.FromDays(1) - attendTime.TimeOfDay;

          if (maxWorkTimeLen == TimeSpan.Zero)
            continue;

          DateTime leaveTime = attendTime.AddMinutes(RandomNumberGenerator.GetInt32((int)maxWorkTimeLen.TotalMinutes));

          shiftList.Add(new SingleShiftData(u.UserID, workDate, false, attendTime, leaveTime, new()));
        }

        scheduledShiftList.Add(new ScheduledShift(workDate, workDate.Date, workDate.Date.AddDays(1), ShiftSchedulingState.FinalVersion, shiftList.ToDictionary(v => new UserID(v.UserID)), new() { 1 }));
      }
      #endregion

      StoreData = new(STORE_ID, userDataList.ToDictionary(v => new UserID(v.UserID)), shiftReqList.ToDictionary(v => new UserID(v.UserID)), scheduledShiftList.ToDictionary(v => v.TargetDate));

    }
  }
}
