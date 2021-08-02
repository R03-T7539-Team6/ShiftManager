using System;
using System.Collections.Generic;
using System.Linq;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.RestData
{
  public static class RestDataConverter
  {
    #region UserData
    public static UserData ToUserData(this RestUser i)
    {
      UserID id = new(i.user_id ?? string.Empty);
      return new UserData(id, new HashedPassword(), new NameData(i.firstname ?? string.Empty, i.lastname ?? string.Empty), new StoreID(i.store_id ?? string.Empty), GroupStringToValue(i.user_group), StatusStringToValue(i.user_state), new WorkLog(id, new()), new UserSetting(id, NotificationPublishTimings.None, new()));
    }
    public static RestUser FromUserData(this RestUser dst, IUserData i)
    {
      dst.user_id = i.UserID.Value;
      dst.store_id = i.StoreID.Value;
      dst.firstname = i.FullName.FirstName;
      dst.lastname = i.FullName.LastName;
      dst.user_state = i.UserState.ToString();
      dst.user_group = i.UserGroup.ToString();

      return dst;
    }

    public static RestUser GenerateFromUserData(in IUserData i) => new RestUser().FromUserData(i);
    #endregion

    #region SingleShiftData
    public static SingleShiftData ToSingleShiftData(this RestShift i)
    {
      Dictionary<DateTime, int> breakTDic = new();
      if (i.start_break_time is not null && i.end_break_time is not null)
        breakTDic = new() { [i.start_break_time ?? default] = (int?)(i.end_break_time - i.start_break_time)?.TotalMinutes ?? 0 };

      return new(new UserID(i.user_id ?? string.Empty), i.work_date?.Date ?? default, i.is_paid_holiday ?? false, i.attendance_time ?? default, i.leaving_time ?? default, breakTDic);
    }

    public static RestShift FromSingleShiftData(this RestShift dst, in ISingleShiftData i)
    {
      dst.user_id = i.UserID.Value;
      dst.work_date = i.WorkDate.Date;
      dst.is_paid_holiday = i.IsPaidHoliday;

      dst.attendance_time = i.AttendanceTime;
      dst.leaving_time = i.LeavingTime;

      if (i.BreakTimeDictionary.Count >= 1)
      {
        var v = i.BreakTimeDictionary.First();
        dst.start_break_time = v.Key;
        dst.end_break_time = v.Key + new TimeSpan(0, v.Value, 0);
      }

      return dst;
    }

    public static RestShift FromSingleShiftData(this RestShift dst, in ISingleShiftData i, in uint? id, in string store_id, in bool is_request)
    {
      dst.id = id;
      dst.store_id = store_id;
      dst.is_request = is_request;

      return dst.FromSingleShiftData(i);
    }

    public static RestShift GenerateFromSingleShiftData(in ISingleShiftData i, in uint? id, in string store_id, in bool is_request)
      => new RestShift().FromSingleShiftData(i, id, store_id, is_request);
    #endregion

    #region Shift Request
    public static ShiftRequest ToShiftRequest(this RestShiftRequest i) => new(new UserID(i.user_id ?? string.Empty), i.last_update ?? default, i.shifts?.Select(i => i.ToSingleShiftData() as ISingleShiftData).ToDictionary(i => i.WorkDate) ?? new());

    public static RestShiftRequest FromShiftRequest(this RestShiftRequest dst, in IShiftRequest i, string store_id)
    {
      dst.user_id = i.UserID.Value;
      dst.store_id = store_id;
      dst.last_update = i.LastUpdate;
      dst.shifts = i.RequestsDictionary.Values.Select((i, count) => new RestShift().FromSingleShiftData(i, (uint)count, store_id, true)).ToArray();

      return dst;
    }

    public static RestShiftRequest GenerateFromShiftRequest(in IShiftRequest i, string store_id)
      => new RestShiftRequest().FromShiftRequest(i, store_id);

    #endregion

    #region ShiftSchedule
    public static ScheduledShift ToScheduledShift(this RestShiftSchedule i)
      => new(i.target_date?.Date ?? default, i.start_of_schedule ?? default, i.end_of_schedule ?? default, ShiftStateStringToValue(i.shift_state),
        i.shifts?.Select(i => i.ToSingleShiftData() as ISingleShiftData).ToDictionary(i => new UserID(i.UserID)) ?? new(), new() { (int)(i.worker_num ?? 0) });

    public static RestShiftSchedule FromScheduledShift(this RestShiftSchedule dst, in IScheduledShift i, string store_id)
    {
      dst.store_id = store_id;
      dst.target_date = i.TargetDate.Date;
      dst.start_of_schedule = i.StartOfSchedule;
      dst.end_of_schedule = i.EndOfSchedule;
      dst.shift_state = i.SchedulingState.ToString();
      dst.shifts = i.ShiftDictionary.Values.Select((i, count) => new RestShift().FromSingleShiftData(i, (uint)count, store_id, false)).ToArray();
      dst.worker_num = (uint)i.RequiredWorkerCountList.FirstOrDefault();
      return dst;
    }

    public static RestShiftSchedule GenerateFromScheduledShift(in IScheduledShift i, in string store_id)
      => new RestShiftSchedule().FromScheduledShift(i, store_id);
    #endregion

    #region StoreData
    public static StoreData ToStoreData(this RestStore i)
      => new(new StoreID(i.store_id ?? string.Empty),
        i.worker_lists?.Select(i => i.ToUserData() as IUserData).ToDictionary(i => new UserID(i.UserID)) ?? new(),
        i.shift_requests?.Select(i => i.ToShiftRequest() as IShiftRequest).ToDictionary(i => new UserID(i.UserID)) ?? new(),
        i.shift_schedules?.Select(i => i.ToScheduledShift() as IScheduledShift).ToDictionary(i => i.TargetDate) ?? new()
      );

    public static RestStore FromStoreData(this RestStore dst, in IStoreData i)
    {
      dst.store_id = i.StoreID.Value;
      dst.worker_lists = i.UserDataDictionary.Values.Select((i, _) => new RestUser().FromUserData(i)).ToArray();
      dst.shift_requests = i.ShiftRequestsDictionary.Values.Select((i, _) => new RestShiftRequest().FromShiftRequest(i, dst.store_id)).ToArray();
      dst.shift_schedules = i.ScheduledShiftDictionary.Values.Select((i, _) => new RestShiftSchedule().FromScheduledShift(i, dst.store_id)).ToArray();

      return dst;
    }

    public static RestStore GenerateFromStoreData(in IStoreData i) => new RestStore().FromStoreData(i);
    #endregion

    #region SingleWorkLog
    public static SingleWorkLog ToSingleWorkLog(this RestWorkLog i)
    {
      Dictionary<DateTime, int> breakT = new();
      if (i.start_break_time is not null || i.end_break_time is not null)
        breakT = new() { [i.start_break_time ?? default] = (int?)((i.end_break_time - i.start_break_time)?.TotalMinutes) ?? 0 };

      return new(i.attendance_time ?? default, i.leaving_time ?? default, breakT);
    }

    public static RestWorkLog FromSingleWorkLog(this RestWorkLog dst, in ISingleWorkLog i, in IUserID userID)
    {
      dst.user_id = userID.Value;
      dst.attendance_time = i.AttendanceTime;
      dst.leaving_time = i.LeavingTime;
      if (i.BreakTimeDictionary.Count >= 1)
      {
        var v = i.BreakTimeDictionary.First();
        dst.start_break_time = v.Key;
        dst.end_break_time = dst.start_break_time + new TimeSpan(0, v.Value, 0);
      }

      return dst;
    }
    public static RestWorkLog GenerateFromSingleWorkLog(in ISingleWorkLog i, in IUserID userID)
      => new RestWorkLog().FromSingleWorkLog(i, userID);
    #endregion

    #region String To Enum
    public static UserGroup GroupStringToValue(in string? value) => value switch
    {
      RestDataConstants.Group.SystemAdmin => UserGroup.SystemAdmin,
      RestDataConstants.Group.SuperUser => UserGroup.SuperUser,
      RestDataConstants.Group.NormalUser => UserGroup.NormalUser,
      RestDataConstants.Group.ForTimeRecordTerminal => UserGroup.ForTimeRecordTerminal,
      _ => UserGroup.None
    };
    public static UserState StatusStringToValue(in string? value) => value switch
    {
      RestDataConstants.Status.Normal => UserState.Normal,
      RestDataConstants.Status.InLeaveOfAbsence => UserState.InLeaveOfAbsence,
      RestDataConstants.Status.Retired => UserState.Retired,
      RestDataConstants.Status.NotHired => UserState.NotHired,
      _ => UserState.Others
    };
    public static ShiftSchedulingState ShiftStateStringToValue(in string? value) => value switch
    {
      RestDataConstants.ShiftStatus.NotStarted => ShiftSchedulingState.NotStarted,
      RestDataConstants.ShiftStatus.Working => ShiftSchedulingState.Working,
      RestDataConstants.ShiftStatus.FinalVersion => ShiftSchedulingState.FinalVersion,
      _ => ShiftSchedulingState.None
    };
    #endregion
  }
}
