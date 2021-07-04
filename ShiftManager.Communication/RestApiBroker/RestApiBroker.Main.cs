using System.Net;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Authenticators;

using ShiftManager.DataClasses;


namespace ShiftManager.Communication
{
  public partial class RestApiBroker
  {
    public IUserData? CurrentUserData { get; private set; } = null;
    private StoreData TestD { get; set; } = new SampleDataForStandaloneTest().StoreData;//To Debug

    private string Token { get; set; } = string.Empty;


    const string TargetURL = "https://immense-harbor-69424.herokuapp.com";

    const int TimeOut = 5000;

    internal async Task<ApiResult<T>> GetDataAsync<T>(string path) where T : class
    {
      var result = await GetJsonDataAsync(path);

      return result.StatusCode == HttpStatusCode.OK ?
        new(true, ApiResultCodes.Success, SimpleJson.DeserializeObject<T>(result.Content))
        : new(false, ToApiRes(result.StatusCode), null);
    }
    internal async Task<(HttpStatusCode StatusCode, string Content)> GetJsonDataAsync(string path)
    {
      RestRequest request = new(path, Method.GET);

      var result = await ExecuteApi(request);

      return (result.StatusCode, result.Content);
    }

    internal async Task<ApiResult<TResult>> ExecuteWithDataAsync<TPost, TResult>(string path, TPost data)
      where TPost : class
      where TResult : class
    {
      RestRequest request = new(path, Method.POST);
      request.AddJsonBody(data);

      var result = await ExecuteApi(request);

      return result.StatusCode == HttpStatusCode.OK ?
        new(true, ApiResultCodes.Success, SimpleJson.DeserializeObject<TResult>(result.Content))
        : new(false, ToApiRes(result.StatusCode), null);
    }
    internal async Task<ApiResult> ExecuteWithDataAsync<T>(string path, T data) where T : class
    {
      RestRequest request = new(path, Method.POST);
      request.AddJsonBody(data);

      var result = await ExecuteApi(request);

      return new(result.StatusCode == HttpStatusCode.OK, ToApiRes(result.StatusCode));
    }

    internal Task<IRestResponse> ExecuteApi(IRestRequest request)
    {
      RestClient client = new(TargetURL);
      client.Timeout = TimeOut;

      if (!string.IsNullOrWhiteSpace(Token))
        client.Authenticator = new JwtAuthenticator(Token);

      return client.ExecuteAsync(request);
    }

    static ApiResultCodes ToApiRes(in HttpStatusCode statusCode) => statusCode switch
    {
      HttpStatusCode.OK => ApiResultCodes.Success,
      HttpStatusCode.NoContent => ApiResultCodes.E204_No_Content,
      HttpStatusCode.BadRequest => ApiResultCodes.E400_Bad_Request,
      HttpStatusCode.Forbidden => ApiResultCodes.E403_Forbidded,
      HttpStatusCode.NotFound => ApiResultCodes.E404_Not_Found,
      _ => ApiResultCodes.Unknown_Error
    };
  }
}
