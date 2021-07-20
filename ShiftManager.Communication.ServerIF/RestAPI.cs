using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Authenticators;

namespace ShiftManager.Communication
{
  public class RestAPI
  {
    public event EventHandler? UnauthorizedDetected;

    /// <summary>現在使用しているトークン</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>接続先URL</summary>
    public string TargetURL { get; init; } = "https://immense-harbor-69424.herokuapp.com";

    /// <summary>タイムアウト時間 [ms]</summary>
    public int TimeOut { get; init; } = 10 * 1000;
    const string application_json = "application/json";


    private static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
      DateTimeZoneHandling = DateTimeZoneHandling.Local,
      NullValueHandling = NullValueHandling.Ignore,
    };

    /// <summary>任意のデータをJsonに変換します</summary>
    /// <typeparam name="T">変換元データ型</typeparam>
    /// <param name="obj">変換元データ</param>
    /// <returns>Json文字列</returns>
    public static string ToJson<T>(in T obj) => JsonConvert.SerializeObject(obj, jsonSerializerSettings);

    /// <summary>Json文字列を任意のデータ型に変換します</summary>
    /// <typeparam name="T">変換先データ型</typeparam>
    /// <param name="json">変換元Json</param>
    /// <returns>変換結果</returns>
    public static T? FromJson<T>(in string json) => JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);

    /// <summary>サーバにリクエストを打ちます</summary>
    /// <typeparam name="TPost">送るデータの型</typeparam>
    /// <typeparam name="TResult">受け取るデータの型</typeparam>
    /// <param name="path">エンドポイント</param>
    /// <param name="data">送るデータ</param>
    /// <param name="reqType">リクエストタイプ</param>
    /// <returns>レスポンスの情報, 受け取ったデータ</returns>
    public async Task<ServerResponse<TResult>> ExecuteWithDataAsync<TPost, TResult>(string path, TPost data, Method reqType = Method.POST)
      where TPost : class
      where TResult : class
    {
      var result = await ExecuteWithDataAsync(path, data, reqType);

      return new(result.Response, FromJson<TResult>(result.Response.Content));
    }

    /// <summary>サーバにリクエストを打ちます</summary>
    /// <typeparam name="T">送るデータの型</typeparam>
    /// <param name="path">エンドポイント</param>
    /// <param name="data">送るデータ</param>
    /// <param name="reqType">リクエストタイプ</param>
    /// <returns>レスポンスの情報</returns>
    public async Task<ServerResponse> ExecuteWithDataAsync<T>(string path, T data, Method reqType = Method.POST) where T : class
      => await ExecuteAsync(new RestRequest(path, reqType).AddParameter(application_json, ToJson(data), ParameterType.RequestBody));
    
    public async Task<ServerResponse<TRes>> ExecuteAsync<TRes>(string path, Method reqType = Method.GET) where TRes : class
    {
      var res = await ExecuteAsync(path, reqType);

      return new(res.Response, FromJson<TRes>(res.Response.Content));
    }

    /// <summary>サーバにリクエストを打ちます</summary>
    /// <param name="path">エンドポイント</param>
    /// <param name="reqType">リクエストタイプ</param>
    /// <returns>レスポンスの情報</returns>
    public Task<ServerResponse> ExecuteAsync(string path, Method reqType = Method.GET) => ExecuteAsync(new RestRequest(path, reqType));

    /// <summary>サーバにリクエストを打ちます</summary>
    /// <param name="request">リクエストの情報</param>
    /// <returns>レスポンスの情報</returns>
    public async Task<ServerResponse> ExecuteAsync(IRestRequest request)
    {
      RestClient client = new(TargetURL);
      client.Timeout = TimeOut;

      if (!string.IsNullOrWhiteSpace(Token))
        client.Authenticator = new JwtAuthenticator(Token);

      request.AddHeader("Content-Type", "application/json");

      var res = await client.ExecuteAsync(request);

      if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(Token))
      {
        Token = string.Empty; //未認証状態を受け取ったらトークンを削除する
        UnauthorizedDetected?.Invoke(this, EventArgs.Empty);
      }

      return new(res);
    }
  }
}
