using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Authenticators;

using ShiftManager.DataClasses;


namespace ShiftManager.Communication
{
  public partial class RestApiBroker
  {
    internal RemoteApi Api { get; } = new();
    public IUserData? CurrentUserData { get; private set; } = null;

    public bool IsLoggedIn { get => !string.IsNullOrWhiteSpace(Api.Token); }

    static ApiResultCodes ToApiRes(in HttpStatusCode statusCode) => statusCode switch
    {
      HttpStatusCode.OK => ApiResultCodes.Success,
      HttpStatusCode.NoContent => ApiResultCodes.E204_No_Content,
      HttpStatusCode.BadRequest => ApiResultCodes.E400_Bad_Request,
      HttpStatusCode.Forbidden => ApiResultCodes.E403_Forbidded,
      HttpStatusCode.NotFound => ApiResultCodes.E404_Not_Found,
      _ => ApiResultCodes.Unknown_Error
    };


    internal class RemoteApi
    {
      public string Token { get; set; } = string.Empty;


      const string TargetURL = "https://immense-harbor-69424.herokuapp.com";

      const int TimeOut = 5000;
      const string application_json = "application/json";

      private static readonly JsonSerializerSettings jsonSerializerSettings = new()
      {
        DateTimeZoneHandling = DateTimeZoneHandling.Local
      };
      private static string ToJson<T>(in T obj) => JsonConvert.SerializeObject(obj, jsonSerializerSettings);

      private static T? FromJson<T>(in string json) => JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);

      public async Task<ApiResult<T>> GetDataAsync<T>(string path) where T : class
      {
        var result = await GetJsonDataAsync(path);

        return result.StatusCode == HttpStatusCode.OK ?
          new(true, ApiResultCodes.Success, FromJson<T>(result.Content))
          : new(false, ToApiRes(result.StatusCode), null);
      }
      public async Task<(HttpStatusCode StatusCode, string Content)> GetJsonDataAsync(string path)
      {
        RestRequest request = new(path, Method.GET);

        var result = await ExecuteApiAsync(request);

        return (result.StatusCode, result.Content);
      }

      public async Task<ApiResult<TResult>> ExecuteWithDataAsync<TPost, TResult>(string path, TPost data, Method reqType = Method.POST)
        where TPost : class
        where TResult : class
      {
        RestRequest request = new(path, reqType);
        var s = ToJson(data);
        request.AddParameter(application_json, s, ParameterType.RequestBody);

        var result = await ExecuteApiAsync(request);

        return result.StatusCode == HttpStatusCode.OK ?
          new(true, ApiResultCodes.Success, FromJson<TResult>(result.Content))
          : new(false, ToApiRes(result.StatusCode), null);
      }
      public async Task<ApiResult> ExecuteWithDataAsync<T>(string path, T data) where T : class
      {
        RestRequest request = new(path, Method.POST);
        request.AddJsonBody(data);

        var result = await ExecuteApiAsync(request);

        return new(result.StatusCode == HttpStatusCode.OK, ToApiRes(result.StatusCode));
      }

      public Task<IRestResponse> ExecuteApiAsync(IRestRequest request)
      {
        RestClient client = new(TargetURL);
        client.Timeout = TimeOut;

        if (!string.IsNullOrWhiteSpace(Token))
          client.Authenticator = new JwtAuthenticator(Token);

        request.AddHeader("Content-Type", "application/json");

        return client.ExecuteAsync(request);
      }
    }
  }
}
