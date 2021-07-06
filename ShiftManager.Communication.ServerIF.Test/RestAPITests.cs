using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;


namespace ShiftManager.Communication.ServerIF.Test
{
  public class RestAPITests
  {
    /// <summary>タイムアウト設定時間が短すぎる場合のテスト</summary>
    /// <remarks>StatusCodeに0が入ることを確認しています</remarks>
    /// <returns>非同期処理用の情報</returns>
    [Test]
    public async Task TimeoutTooShortTest()
    {
      var res = await new RestAPI() { TimeOut = 1 }.ExecuteAsync("/", RestSharp.Method.GET);
      Assert.AreEqual(0, res.StatusCode);
    }

    /// <summary>RootにアクセスしてHttpStatusCode.OKが返ることを確認します</summary>
    /// <returns>非同期処理用の情報</returns>
    [Test]
    public async Task ExecuteAsyncTest()
    {
      var res = await new RestAPI().ExecuteAsync("/", RestSharp.Method.GET);
      Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
  }
}
