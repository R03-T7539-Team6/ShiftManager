using System;
using System.Collections.Generic;
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

      var result = await Sv.DeleteCurrentUserAsync();

      return new(result.Response.StatusCode == HttpStatusCode.NoContent, ToApiRes(result.Response.StatusCode));
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

      var res = await Sv.CreateStoreShiftScheduleFileAsync(new() //サーバー側の実装を確認する
      {
        store_id = CurrentUserData.StoreID.Value,
        target_date = dateTime.Date,
        start_of_schedule = dateTime.Date,
        end_of_schedule = dateTime.Date,
        worker_num = 1
      });

      var apiRes = ToApiRes(res.Response.StatusCode);

      return new(apiRes == ApiResultCodes.Success, apiRes, res.Content?.ToScheduledShift());
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
  * Function = UserIDを指定してシフト希望ファイルを作成します 既存の場合は作成済みのものが返ります ;
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

      var shiftReq = await Sv.GetCurrentUserShiftRequestFileAsync(); //既存のものが無いか確認する

      if(shiftReq.Response.StatusCode != HttpStatusCode.OK) //存在しなかった場合のみ生成を行う
      {
        shiftReq = await Sv.CreateCurrentUserShiftRequestFileAsync(new()
        {
          store_id = CurrentUserData.StoreID.Value,
          user_id = CurrentUserData.UserID.Value,
          last_update = DateTime.Now,
          shifts = Array.Empty<RestShift>()
        });
      }


      return new(shiftReq.Response.StatusCode == HttpStatusCode.OK, ToApiRes(shiftReq.Response.StatusCode), shiftReq.Content?.ToShiftRequest());
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
      if (!IsLoggedIn || CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, new ImmutableArray<ShiftRequest>());

      var res = await Sv.GetStoreFileAsync(CurrentUserData.StoreID.Value); //非効率なような気もする

      var resCode = ToApiRes(res.Response.StatusCode);

      if (res.Response.StatusCode != HttpStatusCode.OK || res.Content?.shift_requests is null)
        return new(false, resCode, new ImmutableArray<ShiftRequest>());


      return new(true, resCode, res.Content.shift_requests.Select(i => i.ToShiftRequest()).ToImmutableArray());
    }

    Dictionary<UserID, UserData> UserDataDic { get; } = new();

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
      if (!IsLoggedIn || CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, new ImmutableArray<UserData>());

      var res = await Sv.GetStoreFileAsync(CurrentUserData.StoreID.Value); //非効率なような気もする
      var retCode = ToApiRes(res.Response.StatusCode);
      if (retCode != ApiResultCodes.Success)
        return new(false, retCode, Array.Empty<UserData>().ToImmutableArray());
      if (res.Content?.worker_lists is null)
        return new(false, ApiResultCodes.Data_Not_Found, Array.Empty<UserData>().ToImmutableArray());

      UserData[] udarr = res.Content.worker_lists.Select(i => i.ToUserData()).ToArray();
      foreach (var i in udarr) {
        UserID id = new(i.UserID);
        if (UserDataDic.ContainsKey(id))
          UserDataDic[new(i.UserID)] = i;
        else
          UserDataDic.Add(id, i);
      }

      return new(true, retCode, udarr.ToImmutableArray());
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
    public async Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync(DateTime date)
    {
      var res = await GetScheduledShiftByDateAsync(date);

      return new(res.IsSuccess, res.ResultCode, res.IsSuccess && res.ReturnData?.SchedulingState == ShiftSchedulingState.FinalVersion);
    }

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
      DateTime targetDate = dateTime.Date;
      if (!IsLoggedIn || CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, null);

      var res = await Sv.GetCurrentStoreShiftScheduleFileAsync(CurrentUserData.StoreID.Value); //ファイルは一つ.  shiftsにすべての日のやつが入る

      var retCode = ToApiRes(res.Response.StatusCode);

      if (res.Response.StatusCode != HttpStatusCode.OK || res.Content?.shifts is null)
        return new(false, retCode, null);

      var ret = res.Content.shifts
        .Where(i => i.work_date == targetDate) // 全日程のシフトのうち, 目的の日付のシフトのみを抽出
        .Select(i => i.ToSingleShiftData() as ISingleShiftData) // それぞれISいんｇぇShiftDataに変換
        .ToDictionary(i => new UserID(i.UserID)); // UserIDをKeyとする辞書型に変換

      return
        new(true, retCode,
          new ScheduledShift(targetDate, targetDate, targetDate.AddDays(1), ShiftSchedulingState.Working, ret, new() { 1 })
        );
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

    public NameData GetUserNameFromCacheByID(IUserID userID) => new(GetUserDataByID_Orig(userID).FullName);


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
    public async Task<ApiResult<UserData>> GetUserDataByIDAsync(UserID userID)
    {
      if (userID.Value?.Length != 8)
        return new(false, ApiResultCodes.Invalid_Length_UserID, null);

      var res = await GetAllUserAsync();
      if (res.ReturnData.IsDefaultOrEmpty)
        return new(false, res.ResultCode, null);

      var userData = res.ReturnData.FirstOrDefault(i => new UserID(i.UserID) == userID);

      return userData is null ? new(false, ApiResultCodes.UserID_Not_Found, null) : new(true, ApiResultCodes.Success, userData);
    }

    public ApiResult<UserData> GetUserDataByID(IUserID userID)
    {
      if (UserDataDic.TryGetValue(new(userID), out var val))
        return new(true, ApiResultCodes.Success, val);
      else
        return new(false, ApiResultCodes.Unknown_Error, null);
    }

    public UserData GetUserDataByID_Orig(IUserID userID) => UserDataDic.TryGetValue(new(userID), out var val) ? val : new(userID, new HashedPassword(), new NameData(), CurrentUserData?.StoreID ?? new StoreID(), UserGroup.None, UserState.Normal, new WorkLog(userID, new()), new UserSetting(userID, NotificationPublishTimings.None, new()));
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
    public async Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None)
    {
      var res = await GetAllUserAsync();
      if (res.ReturnData.IsDefaultOrEmpty)
        return new(false, res.ResultCode, new ImmutableArray<UserData>());

      var userData = res.ReturnData.Where(i => i.UserGroup == userGroup).ToImmutableArray();

      return userData.IsDefaultOrEmpty ? new(false, ApiResultCodes.Data_Not_Found, new ImmutableArray<UserData>()) : new(true, ApiResultCodes.Success, userData);
    }

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
    public async Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal)
    {
      var res = await GetAllUserAsync();
      if (res.ReturnData.IsDefaultOrEmpty)
        return new(false, res.ResultCode, new ImmutableArray<UserData>());

      var userData = res.ReturnData.Where(i => i.UserState == userState).ToImmutableArray();

      return userData.IsDefaultOrEmpty ? new(false, ApiResultCodes.Data_Not_Found, new ImmutableArray<UserData>()) : new(true, ApiResultCodes.Success, userData);
    }

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
      var res = await Sv.AddUserAsync(new RestUser().FromUserData(userData));
      var resCode = ToApiRes(res.Response.StatusCode);
      if (resCode != ApiResultCodes.Success)
        return new(false, resCode);

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
