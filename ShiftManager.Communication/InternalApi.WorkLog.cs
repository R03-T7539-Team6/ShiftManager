using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface InternalApi_WorkLog
  {
    Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID);
  }

  public partial class InternalApi : InternalApi_WorkLog
  {
    public Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }
  }
}
