using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using RestSharp;

using ShiftManager.Communication.RestData;
using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  /// <summary>内部で使用するAPI</summary>
  public partial class RestApiBroker : IInternalApi_StoreData
  {
    /*******************************************
  * specification ;
  * name = DeleteUserDataAsync ;
  * Function = ユーザを削除します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 削除対象のUserID (インターフェイス経由のアクセス) ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult> DeleteUserDataAsync(IUserID userID) => DeleteUserDataAsync(new(userID));//NULLが渡されるとぶっ壊れるので注意

    /*******************************************
  * specification ;
  * name = DeleteUserDataAsync ;
  * Function = ユーザを削除します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 削除対象のUserID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult> DeleteUserDataAsync(UserID userID)
    {
      if (userID?.Value?.Length != 8)
        return new ApiResult(false, ApiResultCodes.Invalid_Length_UserID);

      if (CurrentUserData?.UserGroup != UserGroup.SystemAdmin)
        return new(false, ApiResultCodes.Not_Allowed_Control);

      var result = await Api.ExecuteApiAsync(new RestRequest("/users", Method.DELETE));

      return new(result.StatusCode == HttpStatusCode.OK, ToApiRes(result.StatusCode));
    }


    /*******************************************
  * specification ;
  * name = GenerateScheduledShiftAsync ;
  * Function = 日付を指定して予定シフトを作成します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 作成する日付 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<ScheduledShift>> GenerateScheduledShiftAsync(DateTime dateTime)
    {
      if (!IsLoggedIn || CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, null);

      var res = await Api.ExecuteWithDataAsync<RestShiftSchedule, RestShiftSchedule>("/shifts/schedule", new()
      {
        store_id = CurrentUserData.StoreID.Value,
        target_date = dateTime.Date,
        start_of_schedule = dateTime.Date,
        end_of_schedule = dateTime.Date,
        worker_num = 1
      });

      return new(res.IsSuccess, res.ResultCode, res.ReturnData?.ToScheduledShift());
    }

    /*******************************************
  * specification ;
  * name = GenerateShiftRequestAsync ;
  * Function = UserIDを指定してシフト希望ファイルを作成します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 作成するユーザのID (インターフェイス経由のアクセス) ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(IUserID userID) => GenerateShiftRequestAsync(new(userID));

    /*******************************************
  * specification ;
  * name = GenerateShiftRequestAsync ;
  * Function = UserIDを指定してシフト希望ファイルを作成します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 作成するユーザのID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(UserID userID)
    {
      if (!IsLoggedIn || CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, null);

      var res = await Api.ExecuteWithDataAsync<GenShiftReqFilePOSTData, RestShiftRequest>("/shifts/requests", new()
      {
        user_id = CurrentUserData.UserID.Value,
        store_id = CurrentUserData.StoreID.Value,
        last_update = DateTime.Now,
        shift_request = null
      });

      return new(res.IsSuccess, res.ResultCode, res.ReturnData?.ToShiftRequest());
    }

    /*******************************************
  * specification ;
  * name = GetAllShiftRequestAsync ;
  * Function = 全ユーザのシフト希望を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = N/A ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync()
    {
      var res = await Api.GetDataAsync<RestStore>("/stores/0000");

      if (res.IsSuccess || res.ReturnData is null)
        return new(false, res.ResultCode, new ImmutableArray<ShiftRequest>());

      return new(true, res.ResultCode, res.ReturnData.shift_requests.Select(i => i.ToShiftRequest()).ToImmutableArray());
    }

    /*******************************************
  * specification ;
  * name = GetAllUserAsync ;
  * Function = 現在サインイン中のユーザが所属する店舗に所属する全従業員の情報を取得する ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input =  ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync()
    {
      var res = await Api.GetDataAsync<RestUser[]>("/users");
      if (!res.IsSuccess)
        return new(res.IsSuccess, res.ResultCode, Array.Empty<UserData>().ToImmutableArray());
      if (res.ReturnData is null)
        return new(false, ApiResultCodes.Data_Not_Found, Array.Empty<UserData>().ToImmutableArray());

      return new(res.IsSuccess, res.ResultCode, res.ReturnData.Select(i => i.ToUserData()).ToImmutableArray());
    }

    /*******************************************
  * specification ;
  * name = GetIsScheduledShiftFinalVersionAsync ;
  * Function = 指定日の予定シフトが確定済みかどうかを取得します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 確認する日付 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync(DateTime date)
      => throw new NotSupportedException();

    /*******************************************
  * specification ;
  * name = GetScheduledShiftByDateAsync ;
  * Function = 指定日の予定シフトを取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 取得する予定シフトの日付 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime)
    {
      var res = await Api.GetDataAsync<RestStore>("/stores/0000");

      if (res.IsSuccess || res.ReturnData is null)
        return new(false, res.ResultCode, null);

      var ret = res.ReturnData.shift_schedules.Select(i => i.ToScheduledShift()).Where(i => i.TargetDate == dateTime.Date).FirstOrDefault();

      return new(true, res.ResultCode, ret);
    }

    /*******************************************
  * specification ;
  * name = GetShiftRequestByIDAsync ;
  * Function = 指定ユーザの予定シフト情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 取得するユーザのID (インターフェイス経由のアクセス) ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(IUserID userID) => GetShiftRequestByIDAsync(new(userID));

    /*******************************************
  * specification ;
  * name = GetShiftRequestByIDAsync ;
  * Function = 指定ユーザの予定シフト情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 取得するユーザのID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(UserID userID)
    {
      var apires = await GetAllShiftRequestAsync();
      if (!apires.IsSuccess || apires.ReturnData.Length <= 0)
        return new(false, apires.ResultCode, null);

      var res = apires.ReturnData.Where(i => new UserID(i.UserID) == userID).FirstOrDefault();
      if (res is null)
        return new(false, ApiResultCodes.Unknown_Error, null);

      return new(true, ApiResultCodes.Success, res);
    }

    /*******************************************
  * specification ;
  * name = GetByIDAsync ;
  * Function = 指定ユーザのユーザ情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 取得するユーザのID (インターフェイス経由のアクセス) ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<UserData>> GetUserDataByIDAsync(IUserID userID) => GetUserDataByIDAsync(new(userID));

    /*******************************************
  * specification ;
  * name = GetByIDAsync ;
  * Function = 指定ユーザのユーザ情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 取得するユーザのID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<UserData>> GetUserDataByIDAsync(UserID userID) => Task.Run<ApiResult<UserData>>(async () =>
    {
      if (userID.Value?.Length != 8)
        return new(false, ApiResultCodes.Invalid_Length_UserID, null);

      var result = await Api.GetDataAsync<RestUser>("/users");
      if (!result.IsSuccess || result.ReturnData is null)
        return new(false, result.ResultCode, null);

      return new(true, ApiResultCodes.Success, result.ReturnData.ToUserData());
    });

    /*******************************************
  * specification ;
  * name = GetUsersByUserGroupAsync ;
  * Function = 指定のUserGroupに所属するユーザの情報を取得します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 条件とするUserGroup ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None)
      => throw new NotSupportedException();

    /*******************************************
  * specification ;
  * name = GetUsersByUserStateAsync ;
  * Function = 指定のUserStateに所属するのユーザ情報を取得します ;
  * note = v1.0では未対応 ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 条件とするUserState ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal)
      => throw new NotSupportedException();

    /*******************************************
  * specification ;
  * name = SignUpAsync ;
  * Function = 新規ユーザを追加します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 追加するユーザのID ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public async Task<ApiResult> SignUpAsync(IUserData userData)
    {
      var res = await Api.ExecuteWithDataAsync<RestUser, RestUser>("/signup", new RestUser().FromUserData(userData));

      if (!res.IsSuccess)
        return new(false, res.ResultCode);

      return await SignInAsync(userData.UserID, userData.HashedPassword);
    }
  }

  public class GenShiftReqFilePOSTData
  {
    public string user_id { get; set; } = string.Empty;
    public string store_id { get; set; } = string.Empty;
    public DateTime last_update { get; set; }
    public RestShift[]? shift_request { get; set; } = null;
  }

  public class GenSchedShiftFilePOSTData
  {
    public string store_id { get; set; } = string.Empty;
    public DateTime target_date { get; set; }
    public DateTime start_of_schedule { get; set; }
    public DateTime end_of_schedule { get; set; }
    public uint worker_num { get; set; }
  }
}
