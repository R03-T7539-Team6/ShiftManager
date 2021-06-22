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
  }

  public partial class InternalApi : IInternalApi_SignIn
  {
    /// <summary>非同期でサインインを試行します</summary>
    /// <param name="userID">試行するUserID</param>
    /// <param name="hashedPassword">試行するハッシュ化パスワード</param>
    /// <returns>試行結果</returns>
    public Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword) => Task.Run<ApiResult>(() =>
    {
      if (!TestD.UserDataDictionary.TryGetValue(userID, out IUserData? userD))
        return new(false, ApiResultCodes.UserID_Not_Found);

      if (userD.HashedPassword.Hash == hashedPassword.Hash)
        return new(true, ApiResultCodes.Success);
      else
        return new(false, ApiResultCodes.Password_Not_Match);
    });

  }
}
