using System.Threading.Tasks;

using ShiftManager.Communication.RestData;
using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : IInternalApi_SignIn
  {
    /// <summary>非同期でサインインを試行します</summary>
    /// <param name="userID">試行するUserID</param>
    /// <param name="hashedPassword">試行するハッシュ化パスワード</param>
    /// <returns>試行結果</returns>
    /*******************************************
  * specification ;
  * name = SignInAsync ;
  * Function = サインイン(ログイン)を試行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = ユーザID, ハッシュ化されたパスワード ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword)
    {
      if (string.IsNullOrWhiteSpace(userID?.Value))
        return new(false, ApiResultCodes.UserID_Not_Found);
      if (string.IsNullOrWhiteSpace(hashedPassword?.Hash))
        return new(false, ApiResultCodes.Password_Not_Match);

      if (userID.Value.Length != 8)
        return new(false, ApiResultCodes.Invalid_Length_UserID);

      var result = await Sv.SignInAsync(new()
      {
        user_id = userID.Value,
        password = hashedPassword.Hash
      });

      if (result is ServerErrorResponse<RestData.RestSignInResponse> e_res)
        return new(false, e_res.Error switch
        {
          ErrorType.Invalid_Json_Format => ApiResultCodes.Invalid_Input,
          ErrorType.Wrong_ID => ApiResultCodes.UserID_Not_Found,
          ErrorType.Wrong_PW => ApiResultCodes.Password_Not_Match,
          _ => ApiResultCodes.Unknown_Error
        });

      if(result.Content is null)
        return new(false, ApiResultCodes.Unknown_Error);

      CurrentUserData = result.Content.user.ToUserData();

      return new(true, ApiResultCodes.Success);
    }

    /// <summary>サインアウトを実行します</summary>
    /// <returns>実行結果</returns>
    /*******************************************
  * specification ;
  * name = SignOutAsync ;
  * Function = サインアウト(ログアウト)を行います ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult> SignOutAsync() => Task.Run<ApiResult>(() =>
    {
      Sv.SignOut();

      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In);

      CurrentUserData = null;
      return new(true, ApiResultCodes.Success);
    });

    public class UserIDPW
    {
      public UserIDPW() { }
      public UserIDPW(in string userid, in string pw) { user_id = userid; password = pw; }
      public string user_id { get; set; } = string.Empty;
      public string password { get; set; } = string.Empty;
    }
    public class SignInReturn
    {
      public string token { get; set; } = string.Empty;
    }
  }

}
