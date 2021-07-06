using System.Runtime.CompilerServices;

using ShiftManager.DataClasses;

[assembly: InternalsVisibleTo("ShiftManager.Communication.InternalApi.Test")]

namespace ShiftManager.Communication
{
  public partial class InternalApi
  {
    public IUserData? CurrentUserData { get; private set; } = null;

    private StoreData TestD { get; set; } = new SampleDataForStandaloneTest().StoreData;//To Debug
  }
}
