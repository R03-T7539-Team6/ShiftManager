using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using RestSharp;

using ShiftManager.DataClasses;


namespace ShiftManager.Communication
{
  public partial class RestApiBroker
  {
    public IUserData? CurrentUserData { get; private set; } = null;
    private StoreData TestD { get; set; } = new SampleDataForStandaloneTest().StoreData;//To Debug

    private string Token { get; set; } = string.Empty;

    static Task<ApiResult<T>> GetDataAsync<T>(string path)
    {
      return null;
    }
    static Task<ApiResult> PostDataAsync<T>(string path, T data)
    {
      return null;
    }
  }
}
