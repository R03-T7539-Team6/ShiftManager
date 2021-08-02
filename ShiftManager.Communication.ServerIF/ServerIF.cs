using System;
using System.Threading.Tasks;

using ShiftManager.Communication.RestData;

namespace ShiftManager.Communication
{
  public class ServerIF
  {
    internal RestAPI Api { get; } = new();

    public bool IsLoggedIn { get => !string.IsNullOrWhiteSpace(Api.Token); }

    public void SignOut() => Api.Token = string.Empty;

    #region ログインとサインアップ
    public Task<ServerResponse<RestUser>> SignUp_WithNoTokenAsync(RestUser user) => Api.ExecuteWithDataAsync<RestUser, RestUser>("/signup", user, RestSharp.Method.POST);

    const string ERROR_RESPONSE_INVALID_JSON_FORMAT = "Invalid json provided";
    const string ERROR_RESPONSE_WRONG_USER_ID = "Wrong User ID";
    const string ERROR_RESPONSE_WRONG_PASSWORD = "Wrong Password";
    public async Task<ServerResponse<RestSignInResponse>> SignInAsync(RestUser user)
    {
      var res = await Api.ExecuteWithDataAsync<RestUser, RestSignInResponse>("/login", user, RestSharp.Method.POST);

      if (res.Response.StatusCode == System.Net.HttpStatusCode.OK && res.Content?.token is not null)
        Api.Token = res.Content.token;
      else if(res.Content is null)
      {
        ErrorType err = ErrorType.Unknown;
        string res_s = res.Response.Content;

        if (res_s.Contains(ERROR_RESPONSE_INVALID_JSON_FORMAT))
          err = ErrorType.Invalid_Json_Format;
        else if (res_s.Contains(ERROR_RESPONSE_WRONG_USER_ID))
          err = ErrorType.Wrong_ID;
        else if (res_s.Contains(ERROR_RESPONSE_WRONG_PASSWORD))
          err = ErrorType.Wrong_PW;

        res = new ServerErrorResponse<RestSignInResponse>(res.Response, res.Content, err);
      }

      return res;
    }
    #endregion

    #region ユーザに関する操作
    public Task<ServerResponse<RestUser>> GetCurrentUserDataAsync() => Api.ExecuteAsync<RestUser>("/users", RestSharp.Method.GET);

    public Task<ServerResponse<RestUser>> AddUserAsync(RestUser user) => Api.ExecuteWithDataAsync<RestUser, RestUser>("/users", user, RestSharp.Method.POST);

    public Task<ServerResponse<RestUser>> UpdateUserDataAsync(RestUser user) => Api.ExecuteWithDataAsync<RestUser, RestUser>("/users", user, RestSharp.Method.PUT); //変えたい情報だけが変わっていることを期待する

    public Task<ServerResponse> DeleteCurrentUserAsync() => Api.ExecuteAsync("/users", RestSharp.Method.DELETE);
    #endregion


    static string GetTargetDateQuery(in DateTime dt) => $"target_date={dt:yyyy-MM-dd}T00:00:00Z";
    #region シフトに関する操作
    public Task<ServerResponse<RestShift[]>> GetCurrentUserSingleShiftAsync(bool is_request) => Api.ExecuteAsync<RestShift[]>("/shifts?is_request=" + is_request.ToString().ToLower(), RestSharp.Method.GET);
    public Task<ServerResponse<RestShift[]>> GetCurrentUserSingleShiftAsync(DateTime targetDate) => Api.ExecuteAsync<RestShift[]>($"/shifts?" + GetTargetDateQuery(targetDate), RestSharp.Method.GET);
    public Task<ServerResponse<RestShift[]>> GetCurrentUserSingleShiftAsync(bool is_request, DateTime targetDate) => Api.ExecuteAsync<RestShift[]>($"/shifts?is_request=" + is_request.ToString().ToLower() + "&" + GetTargetDateQuery(targetDate), RestSharp.Method.GET);
    public Task<ServerResponse<RestShift[]>> GetCurrentUserSingleShiftRequestsAsync() => GetCurrentUserSingleShiftAsync(true);
    public Task<ServerResponse<RestShift[]>> GetCurrentUserSingleShiftRequestsAsync(DateTime targetDate) => GetCurrentUserSingleShiftAsync(targetDate);

    public Task<ServerResponse<RestShift>> CreateSingleShiftAsync(RestShift shift) => Api.ExecuteWithDataAsync<RestShift, RestShift>("/shifts", shift with { id = null }, RestSharp.Method.POST); //DataIDはクライアントから指定できないためnullを入れる

    public Task<ServerResponse<RestShift>> UpdateShiftAsync(RestShift shift)
      => Api.ExecuteWithDataAsync<RestShift, RestShift>($"/shifts/{shift.id}",
        shift with //更新できない情報にはnullを入れておく
        {
          id = null,
          is_request = null,
          store_id = null,
          user_id = null,
          work_date = null
        },
        RestSharp.Method.PUT); //変えたい情報だけが変わっていることを期待する

    public Task<ServerResponse> DeleteSingleShiftAsync(int shiftID) => Api.ExecuteAsync($"/shifts/{shiftID}", RestSharp.Method.DELETE);

    public Task<ServerResponse<RestShiftRequest>> GetCurrentUserShiftRequestFileAsync() => Api.ExecuteAsync<RestShiftRequest>("/shifts/requests", RestSharp.Method.GET);

    public Task<ServerResponse<RestShiftRequest>> CreateCurrentUserShiftRequestFileAsync(RestShiftRequest shift) => Api.ExecuteWithDataAsync<RestShiftRequest, RestShiftRequest>("/shifts/requests", shift, RestSharp.Method.POST);

    public Task<ServerResponse> DeleteCurrentUserShiftRequestFileAsync(int id) => throw new NotSupportedException(); //Serverの実装待ち

    public Task<ServerResponse<RestShiftSchedule>> GetCurrentStoreShiftScheduleFileAsync(string storeID) => Api.ExecuteAsync<RestShiftSchedule>($"/shifts/schedule/{storeID}", RestSharp.Method.GET);
    [Obsolete("バグがあり, 正常に取得できない (404 Not Foundが返される)")]
    public Task<ServerResponse<RestShiftSchedule>> GetCurrentStoreShiftScheduleFileAsync(string storeID, DateTime targetDate) => Api.ExecuteAsync<RestShiftSchedule>($"/shifts/schedule/{storeID}?" + GetTargetDateQuery(targetDate), RestSharp.Method.GET);

    public Task<ServerResponse<RestShiftSchedule>> CreateStoreShiftScheduleFileAsync(RestShiftSchedule data) => Api.ExecuteWithDataAsync<RestShiftSchedule, RestShiftSchedule>("/shifts/schedule", data, RestSharp.Method.POST); //変えたい情報だけがJsonに含まれることを期待
    #endregion

    #region お店情報に関する操作
    public Task<ServerResponse<RestStore>> GetStoreFileAsync(string storeID) => Api.ExecuteAsync<RestStore>($"/stores/{storeID}", RestSharp.Method.GET);

    public Task<ServerResponse<RestStore>> CreateStoreFileAsync(RestStore store) => Api.ExecuteWithDataAsync<RestStore, RestStore>("/stores", store, RestSharp.Method.POST);
    #endregion

    #region ログに関する操作
    public Task<ServerResponse<RestWorkLog[]>> GetCurrentUserWorkLogAsync(RestWorkLog log) => Api.ExecuteWithDataAsync<RestWorkLog, RestWorkLog[]>("/logs", log, RestSharp.Method.GET);

    public Task<ServerResponse<RestWorkLog>> CreateWorkLogAsync(RestWorkLog log) => Api.ExecuteWithDataAsync<RestWorkLog, RestWorkLog>("/logs", log, RestSharp.Method.POST);

    public Task<ServerResponse<RestWorkLog>> UpdateWorkLogAsync(RestWorkLog log) => Api.ExecuteWithDataAsync<RestWorkLog, RestWorkLog>("/logs", log, RestSharp.Method.PUT); //変えたい情報だけがJsonに含まれることを期待
    #endregion
  }
}
