using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public partial class InternalApi : IInternalApi_SignIn
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
    public Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword) => Task.Run<ApiResult>(() =>
    {
      if (string.IsNullOrWhiteSpace(userID?.Value))
        return new(false, ApiResultCodes.UserID_Not_Found);
      if (string.IsNullOrWhiteSpace(hashedPassword?.Hash))
        return new(false, ApiResultCodes.Password_Not_Match);

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userD))
        return new(false, ApiResultCodes.UserID_Not_Found);

      if (userD.HashedPassword.Hash == hashedPassword.Hash)
      {
        CurrentUserData = new UserData(userD) with { HashedPassword = new HashedPassword() };
        return new(true, ApiResultCodes.Success);//実際はここでトークンをキャッシュする
      }
      else
        return new(false, ApiResultCodes.Password_Not_Match);
    });

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
      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In);

      CurrentUserData = null;
      return new(true, ApiResultCodes.Success);
    });
  }
}
