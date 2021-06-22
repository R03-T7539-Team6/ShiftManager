using System;
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
    public IUserData? CurrentUserData { get; private set; } = null;

    public Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword)
    {
      throw new NotImplementedException();
    }
  }
}
