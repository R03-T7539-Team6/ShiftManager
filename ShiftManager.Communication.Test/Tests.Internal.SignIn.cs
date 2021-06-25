using System.Collections;
using System.Threading.Tasks;

using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.InternalApiTest
{
  public class SignInTests
  {
    [SetUp]
    public void Setup()
    {
    }
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

    [TestCaseSource(nameof(SignInAsyncTest_TestCase))]
    public async Task<ApiResult> SignInAsyncTest(UserID userID, HashedPassword hashedPassword) => await new InternalApi().SignInAsync(userID, hashedPassword);
  }
}
