
using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager
{
  public class ApiHolder : IApiHolder
  {
    //public InternalApi Api { get; } = new();
    public RestApiBroker Api { get; } = new();
    public StoreID CurrentStoreID { get => new StoreID("0000"); }
    public UserID CurrentUserID { get => new(Api.CurrentUserData?.UserID); }
    public NameData CurrentUserName { get => new(Api.CurrentUserData?.FullName); }
  }
  public interface IApiHolder
  {
    //InternalApi Api { get; }
    RestApiBroker Api { get; }

    StoreID CurrentStoreID { get; }
    public UserID CurrentUserID { get; }
    public NameData CurrentUserName { get; }
  }

  public interface IContainsApiHolder
  {
    IApiHolder ApiHolder { get; set; }
  }
}
