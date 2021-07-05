using System;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

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

      var res = await Api.GetDataAsync<RestData.RestWorkLog[]>("/logs");

      if (!res.IsSuccess)
        return new(false, res.ResultCode, null);

      if (CurrentUserData is null || res.ReturnData is null)
        return new(false, ApiResultCodes.NewData_Is_NULL, null);

      return new(true, res.ResultCode, new WorkLog(CurrentUserData.UserID, new(res.ReturnData.Select(i => i.ToSingleWorkLog() as ISingleWorkLog).ToDictionary(i => i.AttendanceTime.Date))));
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
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = UserID, ユーザの氏名, パスワード情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult> UpdatePasswordAsync(IUserID userID, INameData nameData, IHashedPassword hashedPassword)
    {
      if (userID is null || nameData is null || hashedPassword is null)
        return new(false, ApiResultCodes.Invalid_Input); //引数NULLは許容できない

      if (string.IsNullOrWhiteSpace(hashedPassword.Hash) || string.IsNullOrWhiteSpace(hashedPassword.Salt) || hashedPassword.StretchCount <= 0)
        return new(false, ApiResultCodes.Invalid_Input); //ハッシュ情報の不足も許容できない

      if (userID.Value.Length != 8)
        return new(false, ApiResultCodes.Invalid_Length_UserID);

      throw new NotSupportedException();

      return await SignInAsync(userID, hashedPassword);
    }
  }
}
