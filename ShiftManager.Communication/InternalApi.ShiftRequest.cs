using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface InternalApi_ShiftRequest
  {
    Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData);
    Task<ApiResult<SingleShiftData>> GetShiftRequestByDateAsync(DateTime date);
    Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData);
  }

  public partial class InternalApi : InternalApi_ShiftRequest
  {
    public Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<SingleShiftData>> GetShiftRequestByDateAsync(DateTime date)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData)
    {
      throw new NotImplementedException();
    }
  }
}
