using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_UserData
  {
    Task<ApiResult> CheckPasswordValid(HashedPasswordGetter hashedPasswordGetter);
    Task<ApiResult> UpdatePassword(HashedPasswordGetter hashedPasswordGetter);

    Task<ApiResult<IWorkLog>> GetWorkLog();
    Task<ApiResult<IUserSetting>> GetUserSetting();
  }

  public partial class InternalApi : IInternalApi_UserData
  {
    public Task<ApiResult> CheckPasswordValid(HashedPasswordGetter hashedPasswordGetter)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<IUserSetting>> GetUserSetting()
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<IWorkLog>> GetWorkLog()
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult> UpdatePassword(HashedPasswordGetter hashedPasswordGetter)
    {
      throw new NotImplementedException();
    }
  }
}
