using System.Threading.Tasks;

using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.InternalApiTest
{
  public class SignInTests
  {
    IInternalApi_SignIn TestTarget { get; } = new InternalApi();

    [SetUp]
    public void Setup()
    {
    }

  }
}
