using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_UserData
  {
    Task<ApiResult> CheckPasswordValidAsync(IHashedPassword hashedPassword);
    Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword);

    Task<ApiResult<IWorkLog>> GetWorkLogAsync();
    Task<ApiResult<IUserSetting>> GetUserSettingAsync();
  }

  public partial class InternalApi : IInternalApi_UserData
  {
    public Task<ApiResult> CheckPasswordValidAsync(IHashedPassword hashedPassword)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<IUserSetting>> GetUserSettingAsync()
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<IWorkLog>> GetWorkLogAsync()
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword)
    {
      throw new NotImplementedException();
    }
  }
}
