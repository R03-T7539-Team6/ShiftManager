using System;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;
using ShiftManager.Communication.RestData;

namespace ShiftManager.Communication
{
  public partial class RestApiBroker : IInternalApi_UserData
  {
    /*******************************************
  * specification ;
  * name = GetUserSettingAsync ;
  * Function = サインイン中のユーザのユーザ情報を取得します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<UserSetting>> GetUserSettingAsync()
      => throw new NotSupportedException();

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
    public async Task<ApiResult<WorkLog>> GetWorkLogAsync()
    {
      if (!IsLoggedIn)
        return new(false, ApiResultCodes.Not_Logged_In, null);

      var res = await Sv.GetCurrentUserWorkLogAsync(new() { user_id = CurrentUserData?.UserID.Value ?? string.Empty });

      var resCode = ToApiRes(res.Response.StatusCode);

      if (resCode != ApiResultCodes.Success)
        return new(false, resCode, null);

      if (CurrentUserData is null || res.Content is null)
        return new(false, ApiResultCodes.NewData_Is_NULL, null);

      return new(true, resCode, new WorkLog(CurrentUserData?.UserID ?? new UserID(), new(res.Content.Select(i => i.ToSingleWorkLog() as ISingleWorkLog).ToDictionary(i => i.AttendanceTime.Date))));
    }

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
    public async Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword)
    {
      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In);

      var res = await Sv.UpdateUserDataAsync(new() { password = hashedPassword.Hash }); //パスワード以外はNULL => JSONに入らない

      var apiRes = ToApiRes(res.Response.StatusCode);

      return new(apiRes == ApiResultCodes.Success, apiRes);
    }

    /*******************************************
  * specification ;
  * name = UpdatePasswordAsync ;
  * Function = ユーザIDとユーザの氏名の両方が一致するユーザのパスワード情報を更新します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = UserID, ユーザの氏名, パスワード情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public /*async*/ Task<ApiResult> UpdatePasswordAsync(IUserID userID, INameData nameData, IHashedPassword hashedPassword)
    {
      throw new NotSupportedException();
/*
      if (userID is null || nameData is null || hashedPassword is null)
        return new(false, ApiResultCodes.Invalid_Input); //引数NULLは許容できない

      if (string.IsNullOrWhiteSpace(hashedPassword.Hash) || string.IsNullOrWhiteSpace(hashedPassword.Salt) || hashedPassword.StretchCount <= 0)
        return new(false, ApiResultCodes.Invalid_Input); //ハッシュ情報の不足も許容できない

      if (userID.Value.Length != 8)
        return new(false, ApiResultCodes.Invalid_Length_UserID);

      return await SignInAsync(userID, hashedPassword);
*/
    }

    public async Task<ApiResult<UserData>> UpdateUserDataAsync(IUserData userData)
    {
      RestUser restUser = new();
      restUser.FromUserData(userData);

      if (string.IsNullOrWhiteSpace(restUser.password))
        restUser.password = null;

      var res = await Sv.UpdateUserDataAsync(restUser);

      var apiRes = ToApiRes(res.Response.StatusCode);

      if (apiRes != ApiResultCodes.Success || res.Content is null)
        return new(false, apiRes, null);

      UserData newUserData = res.Content.ToUserData();
      CurrentUserData = newUserData;

      return new(true, ApiResultCodes.Success, newUserData);
    }
  }
}
