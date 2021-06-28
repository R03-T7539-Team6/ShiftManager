using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_UserData
  {
    Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword);

    Task<ApiResult<WorkLog>> GetWorkLogAsync();
    Task<ApiResult<UserSetting>> GetUserSettingAsync();
  }

  public partial class InternalApi : IInternalApi_UserData
  {
    public Task<ApiResult<UserSetting>> GetUserSettingAsync() => Task.Run(() =>
    {
      return CurrentUserData is null
      ? new ApiResult<UserSetting>(false, ApiResultCodes.Not_Logged_In, null)
      : new ApiResult<UserSetting>(true, ApiResultCodes.Success, new(CurrentUserData.UserSetting));
    });

    public Task<ApiResult<WorkLog>> GetWorkLogAsync() => Task.Run(() =>
    {
      return CurrentUserData is null
      ? new ApiResult<WorkLog>(false, ApiResultCodes.Not_Logged_In, null)
      : new ApiResult<WorkLog>(true, ApiResultCodes.Success, new(CurrentUserData.WorkLog));
    });

    public Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword) => Task.Run<ApiResult>(() =>
    {
      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In);

      if (string.IsNullOrWhiteSpace(hashedPassword.Hash) || string.IsNullOrWhiteSpace(hashedPassword.Salt) || hashedPassword.StretchCount <= 0)
        return new(false, ApiResultCodes.Invalid_Input);

      CurrentUserData = new UserData(CurrentUserData) with { HashedPassword = new HashedPassword(hashedPassword) };
      return new(true, ApiResultCodes.Success);
    });
  }
}
