using System;

namespace ShiftManager.Communication.RestApiBroker.RestData
{
  public class RestModel
  {
    public uint ID { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
  }

  public class RestUser : RestModel
  {
    public string UserID { get; set; }
    public string StoreID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserState { get; set; }
    public string UserGroup { get; set; }
  }

  public class RestShift
  {
    public uint ID { get; set; }
    public string UserID { get; set; }
    public string StoreID { get; set; }
    public DateTime WokrDate { get; set; }
    public bool IsPaidHoliday { get; set; }
    public bool IsRequest { get; set; }
    public DateTime AttendanceTime { get; set; }
    public DateTime LeavingTime { get; set; }
    public DateTime StartBreakTime { get; set; }
    public DateTime EndBreakTime { get; set; }
  }

  public class RestShiftRequest : RestModel
  {
    public string UserID { get; set; }
    public string StoreID { get; set; }
    public DateTime LastUpdate { get; set; }
    public RestShift[] Shift { get; set; }
  }

  public class RestShiftSchedule : RestModel
  {
    public string StoreID { get; set; }
    public DateTime TargetDate { get; set; }
    public DateTime StartOfSchedule { get; set; }
    public DateTime EndOfSchedule { get; set; }
    public string ShiftSchedulingState { get; set; }
    public RestShift[] Shift { get; set; }
    public uint WorkerNum { get; set; }
  }

  public class RestStore : RestModel
  {
    public string StoreID { get; set; }
    public RestUser[] User { get; set; }
    public RestShiftRequest[] ShiftRequest { get; set; }
    public RestShiftSchedule[] ShiftSchedule { get; set; }
  }

  public class RestWorkLog : RestModel
  {
    public string UserID { get; set; }
    public DateTime AttendanceTime { get; set; }
    public DateTime LeavingTime { get; set; }
    public DateTime StartBreakTime { get; set; }
    public DateTime EndBreakTime { get; set; }
  }
}
