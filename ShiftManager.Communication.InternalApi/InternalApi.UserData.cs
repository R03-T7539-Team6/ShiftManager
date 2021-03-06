using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public partial class InternalApi : IInternalApi_UserData
  {
    /*******************************************
  * specification ;
  * name = GetUserSettingAsync ;
  * Function = サインイン中のユーザのユーザ情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<UserSetting>> GetUserSettingAsync() => Task.Run(() =>
    {
      return CurrentUserData is null
      ? new ApiResult<UserSetting>(false, ApiResultCodes.Not_Logged_In, null)
      : new ApiResult<UserSetting>(true, ApiResultCodes.Success, new(CurrentUserData.UserSetting));
    });

    /*******************************************
  * specification ;
  * name = GetWorkLogAsync ;
  * Function = サインイン中のユーザの勤怠情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<WorkLog>> GetWorkLogAsync() => Task.Run(() =>
    {
      return CurrentUserData is null
      ? new ApiResult<WorkLog>(false, ApiResultCodes.Not_Logged_In, null)
      : new ApiResult<WorkLog>(true, ApiResultCodes.Success, new(CurrentUserData.WorkLog));
    });

    /*******************************************
  * specification ;
  * name = UpdatePasswordAsync ;
  * Function = サインイン中のユーザのパスワード情報を更新します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = パスワード情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword) => Task.Run<ApiResult>(() =>
    {
      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In);

      return UpdatePasswordAsync(CurrentUserData.UserID, CurrentUserData.FullName, hashedPassword).Result;
    });

    /*******************************************
  * specification ;
  * name = UpdatePasswordAsync ;
  * Function = ユーザIDとユーザの氏名の両方が一致するユーザのパスワード情報を更新します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = UserID, ユーザの氏名, パスワード情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
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
    public Task<ApiResult<UserData>> UpdateUserDataAsync(IUserData userData) => Task.Run<ApiResult<UserData>>(() =>
    {
      UserID id = new(userData.UserID);
      if (!TestD.UserDataDictionary.TryGetValue(id, out var oldData) || oldData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, null);



      UserData newUserData = new UserData(userData) with { HashedPassword = oldData.HashedPassword };
      TestD.UserDataDictionary[new(userData.UserID)] = newUserData;

      return new(true, ApiResultCodes.Success, newUserData);
    });
  }
}
