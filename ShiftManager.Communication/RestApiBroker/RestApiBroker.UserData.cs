using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public partial class RestApiBroker : IInternalApi_UserData
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

      return UpdatePasswordAsync(CurrentUserData.UserID, CurrentUserData.FullName, hashedPassword).Result;
    });

    public Task<ApiResult> UpdatePasswordAsync(IUserID userID, INameData nameData, IHashedPassword hashedPassword) => Task.Run<ApiResult>(() =>
    {
      if (userID is null || nameData is null || hashedPassword is null)
        return new(false, ApiResultCodes.Invalid_Input); //引数NULLは許容できない

      if (string.IsNullOrWhiteSpace(hashedPassword.Hash) || string.IsNullOrWhiteSpace(hashedPassword.Salt) || hashedPassword.StretchCount <= 0)
        return new(false, ApiResultCodes.Invalid_Input); //ハッシュ情報の不足も許容できない

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found);

      if (new NameData(userData.FullName) != new NameData(nameData))
        return new(false, ApiResultCodes.FullName_Not_Match);

      UserData newUserData = new UserData(userData) with { HashedPassword = new HashedPassword(hashedPassword) };
      TestD.UserDataDictionary[new(userData.UserID)] = newUserData;

      return SignInAsync(userID, hashedPassword).Result;
    });
  }
}
