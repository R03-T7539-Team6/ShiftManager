using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : IInternalApi_Password
  {
    /// <summary>パスワードのハッシュ化に必要な情報を取得します</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>パスワードのハッシュ化に必要な情報</returns>
    public Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID)
      => Task.Run(() => new ApiResult<HashedPassword>(true, ApiResultCodes.Success, new(string.Empty, "eiofjrueahrgheirugha", 33)));
  }
}
