using System.Collections;
using System.Threading.Tasks;

using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.InternalApiTest
{
  public class SignInTests
  {
    static IEnumerable SignInAsyncTest_TestCase
    {
      get
      {
        yield return new TestCaseData(
          new UserID("ID0001"),

          //HashedPasswordについて, SaltとStretchCountの値は何でも構いません.
          //APIは当該フィールドを読み取りませんが, 念のため無意味なデータに置換してAPIに渡すことをお勧めします
          new HashedPassword("xMIjIuiIPYrmBoQqskJHHYlL2hc0TvKsdjbifXICxPzvUkh5/weTbWCoFECQabYZeVP+awQ9Cv+txfWzLtFxQQ==", string.Empty, 0)
        ).Returns(new ApiResult(true, ApiResultCodes.Success)).SetName(nameof(SignInAsyncTest) + " : Valid UserData (ID0001)");

        yield return new TestCaseData(
          new UserID("ID0002"),
          new HashedPassword("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", string.Empty, 0)
        ).Returns(new ApiResult(true, ApiResultCodes.Success)).SetName(nameof(SignInAsyncTest) + " : Valid UserData (ID0002)");

        /**********************************************************************/
        //Invalid Input Test
        yield return new TestCaseData(
          new UserID("INVALID_USER_ID"),
          new HashedPassword("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName(nameof(SignInAsyncTest) + " : Invalid UserID");

        yield return new TestCaseData(
          new UserID("ID0001"),
          new HashedPassword("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName(nameof(SignInAsyncTest) + " : Invalid Password Hash");


        //空白やNULL, EmptyなID/ハッシュが渡された場合は, 通信せずにエラーを返します
        yield return new TestCaseData(
          new UserID((string)null),
          new HashedPassword("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName(nameof(SignInAsyncTest) + " : Invalid UserID (UserID string is NULL)");

        yield return new TestCaseData(
          new UserID("ID0001"),
          new HashedPassword(null, string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName(nameof(SignInAsyncTest) + " : Invalid Password Hash (Password Hash is NULL)");

        yield return new TestCaseData(
          new UserID(string.Empty),
          new HashedPassword("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName(nameof(SignInAsyncTest) + " : Invalid UserID (UserID string is Empty String)");

        yield return new TestCaseData(
          new UserID("ID0001"),
          new HashedPassword(string.Empty, string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName(nameof(SignInAsyncTest) + " : Invalid Password Hash (Password Hash is Empty String)");

        yield return new TestCaseData(
          new UserID("           "),
          new HashedPassword("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName(nameof(SignInAsyncTest) + " : Invalid UserID (UserID string contains Only WhiteSpace(Hankaku))");

        yield return new TestCaseData(
          new UserID("　　　　　"),
          new HashedPassword("bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg==", string.Empty, 0)
        ).Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName(nameof(SignInAsyncTest) + " : Invalid UserID (UserID string contains Only WhiteSpace(Zenkaku))");

      }
    }

    /*******************************************
  * specification ;
  * name = SignInAsyncTest ;
  * Function = サインイン処理が正常に動作するかをテストする ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = テストケース ;
  * output = テスト結果 ;
  * end of specification ;
  *******************************************/
    [TestCaseSource(nameof(SignInAsyncTest_TestCase))]
    public async Task<ApiResult> SignInAsyncTest(UserID userID, HashedPassword hashedPassword) => await new InternalApi().SignInAsync(userID, hashedPassword);


    #region SignInAsyncTest WithIDAndPassword
    static IEnumerable SignInAsyncTest_WithIDAndPassword_TestCase
    {
      get
      {
        yield return new TestCaseData("ID0001", "HWRnwOCy4HMiGPTA").Returns(new ApiResult(true, ApiResultCodes.Success)).SetName("SignIn-ID/PW (Valid ID/PW) (ID0001)");
        yield return new TestCaseData("ID0002", "i1KgfuhDy41yGy8x").Returns(new ApiResult(true, ApiResultCodes.Success)).SetName("SignIn-ID/PW (Valid ID/PW) (ID0002)");

        /**********************************************************************/
        //Invalid Input Tests
        yield return new TestCaseData("INVALID_ID", "HWRnwOCy4HMiGPTA").Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName("SignIn-ID/PW (ID is Inalid)");
        yield return new TestCaseData("ID0002", "HWRnwOCy4HMiGPTA").Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName("SignIn-ID/PW (PW is Inalid)");

        yield return new TestCaseData(null, "HWRnwOCy4HMiGPTA").Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName("SignIn-ID/PW (ID is NULL)");
        yield return new TestCaseData("ID0002", null).Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName("SignIn-ID/PW (PW is NULL)");
        yield return new TestCaseData(string.Empty, "HWRnwOCy4HMiGPTA").Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName("SignIn-ID/PW (ID is Empty)");
        yield return new TestCaseData("ID0002", string.Empty).Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName("SignIn-ID/PW (PW is Empty)");

        yield return new TestCaseData("              ", "HWRnwOCy4HMiGPTA").Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName("SignIn-ID/PW (ID is WhiteSpace(Hankaku))");
        yield return new TestCaseData("ID0002", "              ").Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName("SignIn-ID/PW (PW is WhiteSpace(Hankaku))");
        yield return new TestCaseData("　　　　　　　　　　　　", "HWRnwOCy4HMiGPTA").Returns(new ApiResult(false, ApiResultCodes.UserID_Not_Found)).SetName("SignIn-ID/PW (ID is WhiteSpace(Zenkaku))");
        yield return new TestCaseData("ID0002", "　　　　　　　　　　").Returns(new ApiResult(false, ApiResultCodes.Password_Not_Match)).SetName("SignIn-ID/PW (PW is WhiteSpace(Zenkaku))");
      }
    }

    /// <summary>IDと生パスワードを用いたログインの流れをテストします</summary>
    /// <param name="userID">入力ID</param>
    /// <param name="rawPassword">入力パスワード</param>
    /// <returns>APIからの返り値</returns>
    /*******************************************
  * specification ;
  * name = SignInAsyncTest_WithIDAndPassword ;
  * Function = サインイン処理が正常に動作するかをテストする ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = テストケース(ユーザID文字列, 生パスワード) ;
  * output = テスト結果 ;
  * end of specification ;
  *******************************************/
    [TestCaseSource(nameof(SignInAsyncTest_WithIDAndPassword_TestCase))]
    public async Task<ApiResult> SignInAsyncTest_WithIDAndPassword(string userID, string rawPassword)
    {
      //テスト用にInternalApiのインスタンスをローカル変数に入れていますが, 実際はインスタンス変数として置きます
      InternalApi api = new();

      //ユーザID情報をAPIに渡すために, 型を変換します
      UserID targetUserID = new(userID);

      //パスワードをハッシュ化するために必要な情報を取得します
      ApiResult<HashedPassword> passwordHashingDataRequestResult = await api.GetPasswordHashingDataAsync(targetUserID);

      //もしもID不一致等で実行に失敗した場合は, IsSuccessプロパティに"false"が入ります.
      if (!passwordHashingDataRequestResult.IsSuccess)
        //実行結果の詳細はResultCodeプロパティに入っています
        return new(false, passwordHashingDataRequestResult.ResultCode);

      //ハッシュ化用情報を取得できていた場合のみ, 次に進みます

      //生パスワードを, ハッシュ化するために必要な情報と一緒にHashedPasswordGetterに投げます
      HashedPassword hashedPassword = InternalApi.HashedPasswordGetter(rawPassword, passwordHashingDataRequestResult.ReturnData);

      //ハッシュ化パスワードとともに, サインインを試行します
      //API側で削られる情報ではありますが, 念のためSaltとStretchCountの情報は削除しておきましょう.  削除せずに渡しても動作に問題はありません.
      return await api.SignInAsync(targetUserID, hashedPassword with { Salt = string.Empty, StretchCount = 0 });
    }
    #endregion
  }
}
