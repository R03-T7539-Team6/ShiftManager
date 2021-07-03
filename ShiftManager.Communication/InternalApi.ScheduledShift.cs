using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface InternalApi_ScheduledShift
  {
    Task<ApiResult<SingleShiftData>> GetScheduledShiftByIDAsync(DateTime targetDate, IUserID userID);
    Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState);
    Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas);
    Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> singleShiftDatas);
  }

  public partial class InternalApi : InternalApi_ScheduledShift
  {
    /// <summary>指定日の予定シフトを, ユーザID指定で取得します</summary>
    /// <param name="targetDate">取得する予定シフトの対象日</param>
    /// <param name="userID">取得する予定シフトのユーザID</param>
    /// <returns>実行結果</returns>
    public Task<ApiResult<SingleShiftData>> GetScheduledShiftByIDAsync(DateTime targetDate, IUserID userID) => Task.Run<ApiResult<SingleShiftData>>(() =>
    {
      if (!TestD.ScheduledShiftDictionary.TryGetValue(targetDate, out IScheduledShift? scheduledShift) || scheduledShift is null)
        return new(false, ApiResultCodes.Target_Date_Not_Found, null);

      if (!scheduledShift.ShiftDictionary.TryGetValue(new(userID), out ISingleShiftData? singleShiftData) || singleShiftData is null)
        return new(false, ApiResultCodes.UserID_Not_Found_In_Scheduled_Shift, null);
      else
      {
        ApiResultCodes resultCode = scheduledShift.SchedulingState switch
        {
          ShiftSchedulingState.FinalVersion => ApiResultCodes.Success,
          ShiftSchedulingState.Working => ApiResultCodes.Scheduling_Is_Still_In_Working,
          ShiftSchedulingState.NotStarted => ApiResultCodes.Scheduling_Is_Not_Started,
          _ => ApiResultCodes.Unknown_Error,
        };
        return new(resultCode != ApiResultCodes.Unknown_Error, resultCode, new(singleShiftData));
      }
    });

    public Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> requiredWorkerCountList)
      => Task.Run(() => UpdateScheduledShift(targetDate, (i) => new ScheduledShift(i) with { RequiredWorkerCountList = new(requiredWorkerCountList) }));

    public Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState)
      => Task.Run(() => UpdateScheduledShift(targetDate, (i) => new ScheduledShift(i) with { SchedulingState = shiftSchedulingState }));

    public Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas)
      => Task.Run(() => UpdateScheduledShift(targetDate, (i) => new ScheduledShift(i) with { ShiftDictionary = singleShiftDatas.ToDictionary(i => new UserID(i.UserID)) }));


    private ApiResult UpdateScheduledShift(DateTime targetDate, Func<IScheduledShift, ScheduledShift> DataUpdater)
    {
      if (!TestD.ScheduledShiftDictionary.TryGetValue(targetDate, out IScheduledShift? scheduledShift) || scheduledShift is null)
        return new(false, ApiResultCodes.Target_Date_Not_Found);
      var newData = DataUpdater.Invoke(scheduledShift);
      try
      {
        TestD.ScheduledShiftDictionary[targetDate] = newData;
        return new(true, ApiResultCodes.Success);
      }
      catch (KeyNotFoundException)
      {
        return new(false, ApiResultCodes.Target_Date_Not_Found);
      }
      catch (ArgumentNullException)
      {
        return new(false, ApiResultCodes.NewData_Is_NULL);
      }
    }
  }
}
