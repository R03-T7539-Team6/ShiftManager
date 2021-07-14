using System;
using System.Collections.Generic;
using System.Linq;


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
  }

  public class RestShiftRequest : RestModel
  {
    public string user_id { get; set; } = string.Empty;
    public string store_id { get; set; } = string.Empty;
    public DateTime last_update { get; set; }
    public RestShift[] shifts { get; set; } = Array.Empty<RestShift>();
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
  }

  public class RestStore : RestModel
  {
    public string store_id { get; set; } = string.Empty;
    public RestUser[] worker_lists { get; set; } = Array.Empty<RestUser>();
    public RestShiftRequest[] shift_requests { get; set; } = Array.Empty<RestShiftRequest>();
    public RestShiftSchedule[] shift_schedules { get; set; } = Array.Empty<RestShiftSchedule>();
  }

  public class RestWorkLog : RestModel
  {
    public string user_id { get; set; } = string.Empty;
    public DateTime attendance_time { get; set; }
    public DateTime? leaving_time { get; set; }
    public DateTime? start_break_time { get; set; }
    public DateTime? end_break_time { get; set; }
  }

  public class RestSignInResponse
  {
    public string token { get; set; } = string.Empty;
    public RestUser user { get; set; } = new();
  }
}
