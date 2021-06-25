using System.Threading.Tasks;

using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.InternalApiTest
{
  public class SignInTests
  {
    InternalApi TestTarget { get; } = new();

    [SetUp]
    public void Setup()
    {
    }

    static object[] GetPasswordHashingDataAsyncTest_TestCases { get; } =
    {
      new object[]{new UserID("ID0001"), //Input
        new ApiResult<HashedPassword>(true, ApiResultCodes.Success,
          new HashedPassword(string.Empty, "mdTM8HTo96Ba3kV77N9MSQ==", 10000)) },

      new object[]{new UserID("ID0002"), //Input
        new ApiResult<HashedPassword>(true, ApiResultCodes.Success,
          new HashedPassword(string.Empty, "LkDfl7iv6fO5bShQoru4Iw==", 10000)) },

      /**************************************************************************/
      new object[]{new UserID("InvalidUserID"), //Invalid Input
        new ApiResult<HashedPassword>(false, ApiResultCodes.UserID_Not_Found, //実行に失敗した旨, およびユーザIDが見当たらなかったために実行に失敗したという理由を返答します
          null) //実行に失敗した場合, NULLが返ります.  注意してください
      },
    };

    [TestCaseSource(nameof(GetPasswordHashingDataAsyncTest_TestCases))]
    public async Task GetPasswordHashingDataAsyncTest(UserID userID, ApiResult<HashedPassword> expected)
    //ハッシュ化に必要な情報は, サインイン前に要求できます
      => Assert.AreEqual(expected,
          await //非同期処理に対応しています.  詳細は=> https://qiita.com/rawr/items/5d49960a4e4d3823722f
            TestTarget.GetPasswordHashingDataAsync(userID));//UserID型でユーザIDを送信します


  }
}
