using System.Threading.Tasks;

using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.RestApiTest
{
  public class SignInTests
  {
    /*******************************************
  * specification ;
  * name = SignInTest ;
  * Function = サインインのAPIが正常に動作するかどうかをテストする ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = N/A(非同期処理用情報) ;
  * end of specification ;
  *******************************************/
    [Test]
    public async Task SignInTest()
    {
      RestApiBroker api = new();
      var result = await api.SignInAsync(new UserID("19t4065l"), new HashedPassword("1234567890", string.Empty, 0));
      Assert.IsTrue(result.IsSuccess);
    }
  }
}
