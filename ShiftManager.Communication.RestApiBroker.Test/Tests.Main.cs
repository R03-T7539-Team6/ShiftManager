using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;

namespace ShiftManager.Communication.RestApiTest
{
  public class MainTests
  {
    /*******************************************
  * specification ;
  * name = GetJsonDataAsyncTest ;
  * Function = サーバから正常にデータを取得できるかどうかを確認する ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = N/A(非同期処理用情報) ;
  * end of specification ;
  *******************************************/
    /*[Test]
    public async Task GetJsonDataAsyncTest()
    {
      RestApiBroker Api = new();
      var result = await Api.GetJsonDataAsync("/");
      Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
      Assert.AreEqual("{\"message\":\"Hello ShiftManager\"}", result.Content);
    }*/

    /*******************************************
  * specification ;
  * name = GetDataAsyncTest ;
  * Function = サーバから正常にデータを取得でき, かつJsonからクラスに変換できるるかどうかを確認する ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = N/A(非同期処理用情報) ;
  * end of specification ;
  *******************************************/
    /*[Test]
    public async Task GetDataAsyncTest()
    {
      RestApiBroker Api = new();
      var result = await Api.Api.GetDataAsync<GetDataAsyncTestClass>("/");
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(ApiResultCodes.Success, result.ResultCode);
      Assert.AreEqual("Hello ShiftManager", result.ReturnData.message);
    }*/

    public class GetDataAsyncTestClass
    {
      public string message { get; set; }
    }
  }
}
