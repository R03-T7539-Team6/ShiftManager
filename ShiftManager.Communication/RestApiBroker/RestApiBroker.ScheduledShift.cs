using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.Communication.RestData;
using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : InternalApi_ScheduledShift
  {
    /// <summary>指定日の予定シフトを, ユーザID指定で取得します</summary>
    /// <param name="targetDate">取得する予定シフトの対象日</param>
    /// <param name="userID">取得する予定シフトのユーザID</param>
    /// <returns>実行結果</returns>
    public async Task<ApiResult<SingleShiftData>> GetScheduledShiftByIDAsync(DateTime targetDate, IUserID userID)
    {
      if (!IsLoggedIn || CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, null);

      var res = await Api.GetDataAsync<RestStore>("/stores/0000");
      if (!res.IsSuccess || res.ReturnData is null)
        return new(false, ApiResultCodes.Unknown_Error, null);

      if (res.ReturnData.shift_schedules.Length <= 0)
        return new(false, ApiResultCodes.Data_Not_Found, null);

      var tmp1 = res.ReturnData.shift_schedules.Where(i => i.target_date == targetDate);
      foreach (var tmp2 in tmp1)
      {
        var tmp3 = tmp2.shifts.Where(i => i.user_id == userID.Value).FirstOrDefault();
        if (tmp3 != default)
          return new(true, ApiResultCodes.Success, tmp3.ToSingleShiftData());
      }

      return new(false, ApiResultCodes.Data_Not_Found, null);
    }

    public Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> requiredWorkerCountList)
      => Task.Run(() => UpdateScheduledShift(targetDate, (i) => new ScheduledShift(i) with { RequiredWorkerCountList = new(requiredWorkerCountList) }));

    public Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState)
      => Task.Run(() => UpdateScheduledShift(targetDate, (i) => new ScheduledShift(i) with { SchedulingState = shiftSchedulingState }));

    public Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas)
      => Task.Run(() => UpdateScheduledShift(targetDate, (i) => new ScheduledShift(i) with { ShiftDictionary = singleShiftDatas.ToDictionary(i => new UserID(i.UserID)) }));


    private ApiResult UpdateScheduledShift(DateTime targetDate, Func<IScheduledShift, ScheduledShift> DataUpdater)
    {
      return new(false, ApiResultCodes.Unknown_Error); //実装準備扱い
      /*
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
      }*/
    }
  }
}
