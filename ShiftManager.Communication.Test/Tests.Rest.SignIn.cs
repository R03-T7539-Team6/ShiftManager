using System.Threading.Tasks;

using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.RestApiTest
{
  public class SignInTests
  {
    [Test]
    public async Task SignInTest()
    {
      RestApiBroker api = new();
      var result = await api.SignInAsync(new UserID("19t4065l"), new HashedPassword("1234567890", string.Empty, 0));
      Assert.IsTrue(result.IsSuccess);
    }
  }
}
