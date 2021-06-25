using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_Password
  {
    Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID);
  }

  public partial class InternalApi : IInternalApi_Password
  {
    const int HASH_SIZE = 64;

    //ref : https://qiita.com/Nossa/items/0af4429ceb7628d46909
    /// <summary>ハッシュ化されたパスワードを取得する</summary>
    /// <param name="rawPassword">パスワード文字列(PlaneText)</param>
    /// <param name="hashedPasswordInfo">パスワードのハッシュ化に使用する</param>
    /// <returns>ハッシュ化されたパスワード</returns>
    static public HashedPassword HashedPasswordGetter(in string rawPassword, in IHashedPassword hashedPasswordInfo)
    {
      if (hashedPasswordInfo is null)
        return new(string.Empty, string.Empty, 0);//nullなら計算するまでもなく無効値を返す

      if (string.IsNullOrWhiteSpace(rawPassword) || string.IsNullOrWhiteSpace(hashedPasswordInfo.Salt) || hashedPasswordInfo.StretchCount <= 0)
        return new HashedPassword(hashedPasswordInfo) with { Hash = string.Empty };//ハッシュ化に必要な情報が不足しているのであれば, ハッシュに無効値を代入して返す

      return (hashedPasswordInfo is HashedPassword recordHashedPW ? recordHashedPW : new HashedPassword(hashedPasswordInfo))//引数で渡されたのがrecordならrecordで, そうでないならrecordに変換してから使用する
        with //recordの中身を変更する
        {
          Hash = Convert.ToBase64String(//Hashを変更する
            KeyDerivation.Pbkdf2(rawPassword, Convert.FromBase64String(hashedPasswordInfo.Salt), KeyDerivationPrf.HMACSHA256, hashedPasswordInfo.StretchCount, HASH_SIZE)
            )
        };
    }


    /// <summary>パスワードのハッシュ化に必要な情報を取得します</summary>
    /// <param name="userID">ユーザID</param>
    /// <returns>パスワードのハッシュ化に必要な情報</returns>
    public Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID) => Task.Run<ApiResult<HashedPassword>>(() =>
    {
      if (!TestD.UserDataDictionary.TryGetValue(userID, out IUserData? userD) || userD is null)
        return new(false, ApiResultCodes.UserID_Not_Found, null);
      else
        return new(true, ApiResultCodes.Success, new HashedPassword(userD.HashedPassword) with { Hash = string.Empty });
    });
  }
}
