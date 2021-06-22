using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface InternalApi_ScheduledShift
  {
    Task<ApiResult<SingleShiftData>> GetScheduledShiftByIDAsync(DateTime targetDate, UserID userID);
    Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState);
    Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas);
    Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> singleShiftDatas);
  }

  public partial class InternalApi : InternalApi_ScheduledShift
  {
    public Task<ApiResult<SingleShiftData>> GetScheduledShiftByIDAsync(DateTime targetDate, UserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> singleShiftDatas)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas)
    {
      throw new NotImplementedException();
    }
  }
}
