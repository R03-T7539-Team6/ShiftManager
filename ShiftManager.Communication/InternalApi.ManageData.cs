using System.Collections.Immutable;
using System.Threading.Tasks;

namespace ShiftManager.Communication
{
  public interface InternalApi_ManageData
  {
    Task<ApiResult<ImmutableDictionary<string, ImmutableArray<string>>>> GetDataListAsync();
    Task<ApiResult<T>> DownloadDataAsync<T>(string Category, string DataName);
    Task<ApiResult> UploadDataAsync<T>(string Category, string DataName, T Data);
  }

  public partial class InternalApi : InternalApi_ManageData
  {
    public Task<ApiResult<T>> DownloadDataAsync<T>(string Category, string DataName)
    {
      throw new System.NotImplementedException();
    }

    public Task<ApiResult<ImmutableDictionary<string, ImmutableArray<string>>>> GetDataListAsync()
    {
      throw new System.NotImplementedException();
    }

    public Task<ApiResult> UploadDataAsync<T>(string Category, string DataName, T Data)
    {
      throw new System.NotImplementedException();
    }
  }
}
