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

  }
}
