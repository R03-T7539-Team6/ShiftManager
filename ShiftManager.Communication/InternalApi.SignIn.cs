using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_SignIn
  {
    IUserData? CurrentUserData { get; }

    /// <summary>サインインを試行します.</summary>
    /// <param name="userID">ユーザID</param>
    /// <param name="HashedPasswordGetter">パスワードのハッシュ化に関する情報を受けてハッシュ化パスワードを返す関数</param>
    /// <returns>試行結果</returns>
    Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword);

    /// <summary>サインアウトを実行します</summary>
    /// <returns>実行結果</returns>
    Task<ApiResult> SignOutAsync();
  }

  public partial class InternalApi : IInternalApi_SignIn
  {
    /// <summary>非同期でサインインを試行します</summary>
    /// <param name="userID">試行するUserID</param>
    /// <param name="hashedPassword">試行するハッシュ化パスワード</param>
    /// <returns>試行結果</returns>
    public Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword) => Task.Run<ApiResult>(() =>
    {
      if(string.IsNullOrWhiteSpace(userID?.Value))
        return new(false, ApiResultCodes.UserID_Not_Found);
      if(string.IsNullOrWhiteSpace(hashedPassword?.Hash))
        return new(false, ApiResultCodes.Password_Not_Match);

      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userD))
        return new(false, ApiResultCodes.UserID_Not_Found);

      if (userD.HashedPassword.Hash == hashedPassword.Hash)
        return new(true, ApiResultCodes.Success);//実際はここでトークンをキャッシュする
      else
        return new(false, ApiResultCodes.Password_Not_Match);
    });

    /// <summary>サインアウトを実行します</summary>
    /// <returns>実行結果</returns>
    public Task<ApiResult> SignOutAsync() => Task.Run<ApiResult>(() =>
    {
      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In);

      CurrentUserData = null;
      return new(true, ApiResultCodes.Success);
    });
  }
}
