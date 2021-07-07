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
    #endregion

    #region ユーザに関する操作
    public Task<ServerResponse<RestUser>> GetCurrentUserDataAsync() => Api.ExecuteAsync<RestUser>("/users", RestSharp.Method.GET);

    public Task<ServerResponse<RestUser>> AddUserAsync(RestUser user) => Api.ExecuteWithDataAsync<RestUser, RestUser>("/users", user, RestSharp.Method.POST);

    //public Task<ServerResponse> UpdateUserDataAsync() => Api.ExecuteAsync("/users", RestSharp.Method.PUT); //変えたい情報だけをJsonに含める方法を検討する

    public Task<ServerResponse> DeleteCurrentUserAsync() => Api.ExecuteAsync("/users", RestSharp.Method.DELETE);
    #endregion



    #region シフトに関する操作
    public Task<ServerResponse<RestShift[]>> GetCurrentUserSingleShiftRequestsAsync() => Api.ExecuteAsync<RestShift[]>("/shifts?is_request=true", RestSharp.Method.GET);

    public Task<ServerResponse<RestShift>> CreateSingleShiftAsync(RestShift shift) => Api.ExecuteWithDataAsync<RestShift, RestShift>("/shifts", shift, RestSharp.Method.POST);

    //public Task<ServerResponse<RestShift>> UpdateShiftAsync(RestShift shift) => Api.ExecuteWithDataAsync<RestShift, RestShift>($"/shifts/{shift.id}",shift, RestSharp.Method.PUT); //変えたい情報だけをJsonに含める方法を検討する

    public Task<ServerResponse> DeleteSingleShiftAsync(int shiftID) => Api.ExecuteAsync($"/shifts/{shiftID}", RestSharp.Method.DELETE);

    public Task<ServerResponse<RestShiftRequest>> GetCurrentUserShiftRequestFileAsync() => Api.ExecuteAsync<RestShiftRequest>("/shifts/requests", RestSharp.Method.GET);

    public Task<ServerResponse<RestShiftRequest>> CreateCurrentUserShiftRequestFileAsync(RestShiftRequest shift) => Api.ExecuteWithDataAsync<RestShiftRequest, RestShiftRequest>("/shifts/requests", shift, RestSharp.Method.POST);

    //public Task<ServerResponse> DeleteCurrentUserShiftRequestFileAsync(int id); //Serverの実装待ち

    public Task<ServerResponse<RestShiftSchedule>> GetCurrentStoreShiftScheduleFileAsync(int storeID) => Api.ExecuteAsync<RestShiftSchedule>($"/shifts/schedule/{storeID}", RestSharp.Method.GET);

    //public Task<ServerResponse<RestShiftSchedule>> CreateStoreShiftScheduleFileAsync() //変えたい情報だけをJsonに含める
    #endregion

    #region お店情報に関する操作
    public Task<ServerResponse<RestStore>> GetStoreFileAsync(int storeID) => Api.ExecuteAsync<RestStore>($"/stores/{storeID}", RestSharp.Method.GET);

    public Task<ServerResponse<RestStore>> CreateStoreFileAsync(RestStore store) => Api.ExecuteWithDataAsync<RestStore, RestStore>("/stores", store, RestSharp.Method.POST);
    #endregion

    #region ログに関する操作
    public Task<ServerResponse<RestWorkLog[]>> GetCurrentUserWorkLogAsync(RestWorkLog log) => Api.ExecuteWithDataAsync<RestWorkLog, RestWorkLog[]>("/logs", log, RestSharp.Method.GET);

    public Task<ServerResponse<RestWorkLog>> CreateWorkLogAsync(RestWorkLog log) => Api.ExecuteWithDataAsync<RestWorkLog, RestWorkLog>("/logs", log, RestSharp.Method.POST);

    //public Task<ServerResponse<RestWorkLog>> UpdateWorkLogAsync(RestWorkLog log) => Api.ExecuteWithDataAsync<RestWorkLog, RestWorkLog>("/logs", log, RestSharp.Method.PUT); //変えたい情報だけをJsonに含める
    #endregion
  }
}
