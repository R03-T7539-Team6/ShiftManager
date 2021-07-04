using System;
using System.Collections.Generic;
using System.Linq;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.RestData
{
  public class RestModel
  {
    public uint ID { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
  }

  public class RestUser : RestModel
  {
    public string user_id { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string store_id { get; set; } = string.Empty;
    public string firstname { get; set; } = string.Empty;
    public string lastname { get; set; } = string.Empty;
    public string user_state { get; set; } = string.Empty;
    public string user_group { get; set; } = string.Empty;

    public UserData ToUserData()
    {
      UserID id = new(user_id);
      return new UserData(id, new HashedPassword(), new NameData(firstname, lastname), new StoreID(store_id), RestDataConstants.Group.ConvToValue(user_state), RestDataConstants.Status.ConvToValue(user_state), new WorkLog(id, new()), new UserSetting(id, NotificationPublishTimings.None, new()));
    }
    public RestUser FromUserData(in IUserData i)
    {
      user_id = i.UserID.Value;
      store_id = i.StoreID.Value;
      firstname = i.FullName.FirstName;
      lastname = i.FullName.LastName;
      user_state = i.UserState.ToString();
      user_group = i.UserGroup.ToString();

      return this;
    }

    public static RestUser GenerateFromUserData(in IUserData i) => new RestUser().FromUserData(i);
  }

  public class RestShift
  {
    public uint id { get; set; }
    public string user_id { get; set; } = string.Empty;
    public string store_id { get; set; } = string.Empty;
    public DateTime work_date { get; set; }
    public bool is_paid_holiday { get; set; }
    public bool is_request { get; set; }
    public DateTime attendance_time { get; set; }
    public DateTime leaving_time { get; set; }
    public DateTime? start_break_time { get; set; }
    public DateTime? end_break_time { get; set; }

    public SingleShiftData ToSingleShiftData()
    {
      Dictionary<DateTime, int> breakTDic = new();
      if (start_break_time is not null && end_break_time is not null)
        breakTDic = new() { [start_break_time ?? default] = (int?)(end_break_time - start_break_time)?.TotalMinutes ?? 0 };

      return new(new UserID(user_id), work_date.Date, is_paid_holiday, attendance_time, leaving_time, breakTDic);
    }

    public RestShift FromSingleShiftData(in ISingleShiftData i, in uint id, in string store_id, in bool is_request)
    {
      this.id = id;
      user_id = i.UserID.Value;
      this.store_id = store_id;
      work_date = i.WorkDate.Date;
      is_paid_holiday = i.IsPaidHoliday;
      this.is_request = is_request;
      attendance_time = i.AttendanceTime;
      leaving_time = i.LeavingTime;
      if (i.BreakTimeDictionary.Count >= 1)
      {
        var v = i.BreakTimeDictionary.First();
        start_break_time = v.Key;
        end_break_time = v.Key + new TimeSpan(0, v.Value, 0);
      }
      return this;
    }

    public static RestShift GenerateFromSingleShiftData(in ISingleShiftData i, in uint id, in string store_id, in bool is_request)
      => new RestShift().FromSingleShiftData(i, id, store_id, is_request);
  }

  public class RestShiftRequest : RestModel
  {
    public string user_id { get; set; } = string.Empty;
    public string store_id { get; set; } = string.Empty;
    public DateTime last_update { get; set; }
    public RestShift[] shifts { get; set; } = Array.Empty<RestShift>();

    public ShiftRequest ToShiftRequest() => new(new UserID(user_id), last_update, shifts.Select(i => i.ToSingleShiftData() as ISingleShiftData).ToDictionary(i => i.WorkDate));

    public RestShiftRequest FromShiftRequest(in IShiftRequest i, string store_id)
    {
      user_id = i.UserID.Value;
      this.store_id = store_id;
      last_update = i.LastUpdate;
      shifts = i.RequestsDictionary.Values.Select((i, count) => new RestShift().FromSingleShiftData(i, (uint)count, store_id, true)).ToArray();

      return this;
    }

    public static RestShiftRequest GenerateFromShiftRequest(in IShiftRequest i, string store_id)
      => new RestShiftRequest().FromShiftRequest(i, store_id);
  }

  public class RestShiftSchedule : RestModel
  {
    public string store_id { get; set; } = string.Empty;
    public DateTime target_date { get; set; }
    public DateTime start_of_schedule { get; set; }
    public DateTime end_of_schedule { get; set; }
    public string shift_state { get; set; } = string.Empty;
    public RestShift[] shifts { get; set; } = Array.Empty<RestShift>();
    /// <summary>時間ごと必要人数</summary>
    public uint worker_num { get; set; }

    public ScheduledShift ToScheduledShift()
      => new(target_date.Date, start_of_schedule, end_of_schedule, RestDataConstants.ShiftStatus.ConvToValue(shift_state),
        shifts.Select(i => i.ToSingleShiftData() as ISingleShiftData).ToDictionary(i => new UserID(i.UserID)), new() { (int)worker_num });

    public RestShiftSchedule FromScheduledShift(in IScheduledShift i, string store_id)
    {
      this.store_id = store_id;
      target_date = i.TargetDate.Date;
      start_of_schedule = i.StartOfSchedule;
      end_of_schedule = i.EndOfSchedule;
      shift_state = i.SchedulingState.ToString();
      shifts = i.ShiftDictionary.Values.Select((i, count) => new RestShift().FromSingleShiftData(i, (uint)count, store_id, false)).ToArray();
      worker_num = (uint)i.RequiredWorkerCountList.FirstOrDefault();
      return this;
    }

    public static RestShiftSchedule GenerateFromScheduledShift(in IScheduledShift i, in string store_id)
      => new RestShiftSchedule().FromScheduledShift(i, store_id);
  }

  public class RestStore : RestModel
  {
    public string store_id { get; set; } = string.Empty;
    public RestUser[] worker_lists { get; set; } = Array.Empty<RestUser>();
    public RestShiftRequest[] shift_requests { get; set; } = Array.Empty<RestShiftRequest>();
    public RestShiftSchedule[] shift_schedules { get; set; } = Array.Empty<RestShiftSchedule>();

    public StoreData ToStoreData()
      => new(new StoreID(store_id),
        worker_lists.Select(i => i.ToUserData() as IUserData).ToDictionary(i => new UserID(i.UserID)),
        shift_requests.Select(i => i.ToShiftRequest() as IShiftRequest).ToDictionary(i => new UserID(i.UserID)),
        shift_schedules.Select(i => i.ToScheduledShift() as IScheduledShift).ToDictionary(i => i.TargetDate)
      );

    public RestStore FromStoreData(in IStoreData i)
    {
      store_id = i.StoreID.Value;
      worker_lists = i.UserDataDictionary.Values.Select((i, _) => new RestUser().FromUserData(i)).ToArray();
      shift_requests = i.ShiftRequestsDictionary.Values.Select((i, _) => new RestShiftRequest().FromShiftRequest(i, store_id)).ToArray();
      shift_schedules = i.ScheduledShiftDictionary.Values.Select((i, _) => new RestShiftSchedule().FromScheduledShift(i, store_id)).ToArray();

      return this;
    }

    public static RestStore GenerateFromStoreData(in IStoreData i) => new RestStore().FromStoreData(i);
  }

  public class RestWorkLog : RestModel
  {
    public string user_id { get; set; } = string.Empty;
    public DateTime attendance_time { get; set; }
    public DateTime? leaving_time { get; set; }
    public DateTime? start_break_time { get; set; }
    public DateTime? end_break_time { get; set; }

    public SingleWorkLog ToSingleWorkLog()
    {
      Dictionary<DateTime, int> breakT = new();
      if (start_break_time is not null || end_break_time is not null)
        breakT = new() { [start_break_time ?? default] = (int?)((end_break_time - start_break_time)?.TotalMinutes) ?? 0 };

      return new(attendance_time, leaving_time ?? default, breakT);
    }

    public RestWorkLog FromSingleWorkLog(in ISingleWorkLog i, in IUserID userID)
    {
      user_id = userID.Value;
      attendance_time = i.AttendanceTime;
      leaving_time = i.LeavingTime;
      if (i.BreakTimeDictionary.Count >= 1)
      {
        var v = i.BreakTimeDictionary.First();
        start_break_time = v.Key;
        end_break_time = start_break_time + new TimeSpan(0, v.Value, 0);
      }

      return this;
    }

    public static RestWorkLog GenerateFromSingleWorkLog(in ISingleWorkLog i, in IUserID userID)
      => new RestWorkLog().FromSingleWorkLog(i, userID);
  }
}
