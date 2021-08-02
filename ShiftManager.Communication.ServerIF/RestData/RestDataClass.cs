using System;

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

  public record RestShift
  {
    public RestShift(RestShift b)
    {
      id = b.id;
      user_id = b.user_id;
      store_id = b.store_id;
      work_date = b.work_date;
      is_paid_holiday = b.is_paid_holiday;
      is_request = b.is_request;
      attendance_time = b.attendance_time;
      leaving_time = b.leaving_time;
      start_break_time = b.start_break_time;
      end_break_time = b.end_break_time;
    }

    public uint? id { get; set; } = null;
    public string? user_id { get; set; } = null;
    public string? store_id { get; set; } = null;


    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? work_date { get => _work_date; set => _work_date = value?.Date; } //WorkDateは必ず00:00:00を入れなければならないため, 入力時に「時」以下を切る (但しnullでない場合のみ)
    private DateTime? _work_date = null;

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

  public record RestShiftRequest
  {
    public string? user_id { get; set; } = null;
    public string? store_id { get; set; } = null;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? last_update { get; set; } = null;
    public RestShift[]? shifts { get; set; } = null;
  }

  public record RestShiftRequestWithModel : RestShiftRequest, IRestModel
  {
    public uint ID { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public record RestShiftSchedule
  {
    public string? store_id { get; set; } = null;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? target_date { get => _target_date; set => _target_date = value?.Date; } //TargetDateは必ず00:00:00を入れなければならないため, 入力時に「時」以下を切る (但しnullでない場合のみ)
    private DateTime? _target_date = null;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? start_of_schedule { get; set; } = null;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? end_of_schedule { get; set; } = null;

    public string? shift_state { get; set; } = null;
    public RestShift[]? shifts { get; set; } = null;
    /// <summary>時間ごと必要人数</summary>
    public uint? worker_num { get; set; } = null;
  }

  public record RestShiftScheduleWithModel : RestShiftSchedule, IRestModel
  {
    public uint ID { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public record RestStore
  {
    public string? store_id { get; set; } = null;
    public RestUser[]? worker_lists { get; set; } = null;
    public RestShiftRequest[]? shift_requests { get; set; } = null;
    public RestShiftSchedule[]? shift_schedules { get; set; } = null;
  }

  public record RestStoreWithModel : RestStore, IRestModel
  {
    public uint ID { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public record RestWorkLog
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

  public record RestWorkLogWithModel : RestWorkLog, IRestModel
  {
    public uint ID { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime CreatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime DeletedAt { get; set; }
  }

  public record RestSignInResponse
  {
    public string? token { get; set; } = null;
    public RestUser? user { get; set; } = null;
  }
}
