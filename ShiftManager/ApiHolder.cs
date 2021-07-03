
using ShiftManager.Communication;

namespace ShiftManager
{
  public class ApiHolder : IApiHolder
  {
    public InternalApi Api { get; } = new();
  }
  public interface IApiHolder
  {
    InternalApi Api { get; }
  }

  public interface IContainsApiHolder
  {
    IApiHolder ApiHolder { get; set; }
  }
}
