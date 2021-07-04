using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;

namespace ShiftManager.Communication.RestApiTest
{
  public class MainTests
  {
    [Test]
    public async Task GetJsonDataAsyncTest()
    {
      RestApiBroker Api = new();
      var result = await Api.Api.GetJsonDataAsync("/");
      Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
      Assert.AreEqual("{\"message\":\"Hello ShiftManager\"}", result.Content);
    }

    [Test]
    public async Task GetDataAsyncTest()
    {
      RestApiBroker Api = new();
      var result = await Api.Api.GetDataAsync<GetDataAsyncTestClass>("/");
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(ApiResultCodes.Success, result.ResultCode);
      Assert.AreEqual("Hello ShiftManager", result.ReturnData.message);
    }

    public class GetDataAsyncTestClass
    {
      public string message { get; set; }
    }
  }
}
