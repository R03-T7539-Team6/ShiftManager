using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : IInternalApi_SignIn
  {
    /// <summary>非同期でサインインを試行します</summary>
    /// <param name="userID">試行するUserID</param>
    /// <param name="hashedPassword">試行するハッシュ化パスワード</param>
    /// <returns>試行結果</returns>
    public async Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword)
    {
      if (string.IsNullOrWhiteSpace(userID?.Value))
        return new(false, ApiResultCodes.UserID_Not_Found);
      if (string.IsNullOrWhiteSpace(hashedPassword?.Hash))
        return new(false, ApiResultCodes.Password_Not_Match);

      if (userID.Value.Length != 8)
        return new(false, ApiResultCodes.Invalid_Length_UserID);

      var result = await Api.ExecuteWithDataAsync<UserIDPW, SignInReturn>("/login", new(userID.Value, hashedPassword.Hash));

      if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.ReturnData?.token))
        return new(false, ApiResultCodes.SignIn_Failed);

      Api.Token = result.ReturnData.token;

      var userData = await GetUserDataByIDAsync(userID);
      if (!userData.IsSuccess)
        return new(false, ApiResultCodes.SignIn_Failed);

      CurrentUserData = userData.ReturnData;

      return new(true, ApiResultCodes.Success);
    }

    /// <summary>サインアウトを実行します</summary>
    /// <returns>実行結果</returns>
    public Task<ApiResult> SignOutAsync() => Task.Run<ApiResult>(() =>
    {
      Api.Token = string.Empty;

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
