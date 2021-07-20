using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace ShiftManager.Communication.RestData
{
  public interface IRestModel
  {
    public uint ID { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
  }

  public class RestUser
  {
    public string? user_id { get; set; } = null;
    public string? password { get; set; } = null;
    public string? store_id { get; set; } = null;
    public string? firstname { get; set; } = null;
    public string? lastname { get; set; } = null;
    public string? user_state { get; set; } = null;
    public string? user_group { get; set; } = null;
  }
  public class RestUserWithModel : RestUser, IRestModel
  {
    public uint ID { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public class RestShift
  {
    public uint? id { get; set; } = null;
    public string? user_id { get; set; } = null;
    public string? store_id { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? work_date { get; set; } = null;
    public bool? is_paid_holiday { get; set; } = null;
    public bool? is_request { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? attendance_time { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? leaving_time { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? start_break_time { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? end_break_time { get; set; } = null;
  }

  public class RestShiftRequest
  {
    public string? user_id { get; set; } = null;
    public string? store_id { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? last_update { get; set; } = null;
    public RestShift[]? shifts { get; set; } = null;
  }

  public class RestShiftRequestWithModel : RestShiftRequest, IRestModel
  {
    public uint ID { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public class RestShiftSchedule
  {
    public string? store_id { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? target_date { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? start_of_schedule { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? end_of_schedule { get; set; } = null;
    public string? shift_state { get; set; } = null;
    public RestShift[]? shifts { get; set; } = null;
    /// <summary>時間ごと必要人数</summary>
    public uint? worker_num { get; set; } = null;
  }

  public class RestShiftScheduleWithModel : RestShiftSchedule, IRestModel
  {
    public uint ID { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public class RestStore
  {
    public string? store_id { get; set; } = null;
    public RestUser[]? worker_lists { get; set; } = null;
    public RestShiftRequest[]? shift_requests { get; set; } = null;
    public RestShiftSchedule[]? shift_schedules { get; set; } = null;
  }

  public class RestStoreWithModel : RestStore, IRestModel
  {
    public uint ID { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public class RestWorkLog
  {
    public string? user_id { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? attendance_time { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? leaving_time { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? start_break_time { get; set; } = null;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? end_break_time { get; set; } = null;
  }

  public class RestWorkLogWithModel : RestWorkLog, IRestModel
  {
    public uint ID { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public class RestSignInResponse
  {
    public string? token { get; set; } = null;
    public RestUser? user { get; set; } = null;
  }
}
