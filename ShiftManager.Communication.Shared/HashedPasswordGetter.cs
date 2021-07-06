using System;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  static public class HashedPasswordGetter
  {
    const int HASH_SIZE = 64;

    //ref : https://qiita.com/Nossa/items/0af4429ceb7628d46909
    /// <summary>ハッシュ化されたパスワードを取得する</summary>
    /// <param name="rawPassword">パスワード文字列(PlaneText)</param>
    /// <param name="hashedPasswordInfo">パスワードのハッシュ化に使用する</param>
    /// <returns>ハッシュ化されたパスワード</returns>
    /*******************************************
  * specification ;
  * name = HashedPasswordGetter ;
  * Function = 指定の情報を用いてパスワードのハッシュ化を行います ;
  * note = ハッシュ化アルゴリズムはPBKDF2です ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 生パスワード, ハッシュ化に必要な情報 ;
  * output = ハッシュ化パスワードおよびソルト, ストレッチ回数 ;
  * end of specification ;
  *******************************************/
    static public HashedPassword Get(in string rawPassword, in IHashedPassword hashedPasswordInfo)
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

  }
}
