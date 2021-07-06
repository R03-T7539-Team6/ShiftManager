using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public partial class InternalApi : IInternalApi_Password
  {
    /// <summary>パスワードのハッシュ化に必要な情報を取得します</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>パスワードのハッシュ化に必要な情報</returns>
    /*******************************************
  * specification ;
  * name = GetPasswordHashingDataAsync ;
  * Function = サーバよりパスワードのハッシュ化に必要なデータの取得を試行する ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = ユーザID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID) => Task.Run<ApiResult<HashedPassword>>(() =>
    {
      if (!TestD.UserDataDictionary.TryGetValue(new(userID), out IUserData? userD) || userD is null)
        return new(false, ApiResultCodes.UserID_Not_Found, null);
      else
        return new(true, ApiResultCodes.Success, new HashedPassword(userD.HashedPassword) with { Hash = string.Empty });
    });
  }
}
