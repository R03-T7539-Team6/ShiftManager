using System.Threading.Tasks;

using ShiftManager.Communication.RestData;

namespace ShiftManager.Communication
{
  public class ServerIF
  {
    internal RestAPI Api { get; } = new();

    public bool IsLoggedIn { get => !string.IsNullOrWhiteSpace(Api.Token); }

    #region ログインとサインアップ
    public Task<ServerResponse<RestUser>> SignUp_WithNoTokenAsync(RestUser user) => Api.ExecuteWithDataAsync<RestUser, RestUser>("/signup", user, RestSharp.Method.POST);

    public async Task<ServerResponse> SignInAsync(RestUser user)
    {
      var res = await Api.ExecuteWithDataAsync<RestUser, RestTokenResponse>("/login", user, RestSharp.Method.POST);
      if (res.Response.StatusCode == System.Net.HttpStatusCode.OK && res.Content is not null)
        Api.Token = res.Content.token;
      else if(res.Response.StatusCode== System.Net.HttpStatusCode.BadRequest)
      {
        res = new ServerErrorResponse<RestTokenResponse>(res.Response, res.Content, res.Response.Content switch
        {
          "Invalid json provided" => ErrorType.Invalid_Json_Format,
          "Wrong User ID" => ErrorType.Wrong_ID,
          "Wrong Password" => ErrorType.Wrong_PW,
          _ => ErrorType.Unknown
        });
      }

      return res;
    }

    public Task<ServerResponse<RestUser>> GetCurrentUserDataAsync() => Api.ExecuteAsync<RestUser>("/users", RestSharp.Method.GET);

    public Task<ServerResponse<RestUser>> AddUserAsync(RestUser user) => Api.ExecuteWithDataAsync<RestUser, RestUser>("/users", user, RestSharp.Method.POST);

    //public Task<ServerResponse> UpdateUserDataAsync() => Api.ExecuteAsync("/users", RestSharp.Method.PUT); //変えたい情報だけをJsonに含める方法を検討する

    public Task<ServerResponse> DeleteCurrentUserAsync() => Api.ExecuteAsync("/users", RestSharp.Method.DELETE);
    #endregion

    #region シフトに関する操作

    #endregion

    #region お店情報に関する操作
    #endregion

    #region ログに関する操作
    #endregion
  }
}
