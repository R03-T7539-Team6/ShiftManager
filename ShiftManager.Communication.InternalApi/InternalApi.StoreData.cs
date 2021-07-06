using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  /// <summary>内部で使用するAPI</summary>
  public partial class InternalApi : IInternalApi_StoreData
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
    public Task<ApiResult> DeleteUserDataAsync(UserID userID)
      => Task.Run<ApiResult>(() => !string.IsNullOrWhiteSpace(userID?.Value) && TestD.UserDataDictionary.Remove(userID) //UserIDが指定されていた場合のみ実行
      ? new(true, ApiResultCodes.Success) : new(false, ApiResultCodes.UserID_Not_Found));

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
    public Task<ApiResult<ScheduledShift>> GenerateScheduledShiftAsync(DateTime dateTime) => Task.Run<ApiResult<ScheduledShift>>(() =>
    {
      if (TestD.ScheduledShiftDictionary.TryGetValue(dateTime.Date, out IScheduledShift? scheduledShift) && scheduledShift is not null) //時分秒情報は除去する
        return new(false, ApiResultCodes.Data_Already_Exists, new(scheduledShift));

      //Remoteにデータが存在しない場合のみ新規作成
      ScheduledShift retD = new(dateTime.Date,
        dateTime.Date,//TargetDateの00:00から
        dateTime.Date.AddDays(1),//TargetDateの翌日の00:00まで
        ShiftSchedulingState.NotStarted, new(), new());

      //Remoteへの追加
      return TestD.ScheduledShiftDictionary.TryAdd(retD.TargetDate, retD)
      ? new(true, ApiResultCodes.Success, retD)
      : new(false, ApiResultCodes.Data_Already_Exists, new(TestD.ScheduledShiftDictionary[retD.TargetDate]));
    });

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
    public Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(UserID userID) => Task.Run<ApiResult<ShiftRequest>>(() =>
    {
      if (string.IsNullOrWhiteSpace(userID?.Value))
        return new(false, ApiResultCodes.UserID_Not_Found, null);

      if (TestD.ShiftRequestsDictionary.TryGetValue(userID, out IShiftRequest? shiftRequest) && shiftRequest is not null)
        return new(false, ApiResultCodes.Data_Already_Exists, new(shiftRequest));

      //Remoteにデータが存在しない場合のみ新規作成
      ShiftRequest retD = new(userID, DateTime.Now, new());

      //Remoteへの追加
      TestD.ShiftRequestsDictionary.Add(userID, retD);

      return new(true, ApiResultCodes.Success, retD);
    });

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
    public Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync()
      => Task.Run<ApiResult<ImmutableArray<ShiftRequest>>>(() => new(true, ApiResultCodes.Success,
        TestD.ShiftRequestsDictionary.Values.Select(i => new ShiftRequest(i)).ToImmutableArray() //ShiftRequestが存在しない場合は「要素0」の配列が返る
        ));

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
    public Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync()
      => Task.Run<ApiResult<ImmutableArray<UserData>>>(() => new(true, ApiResultCodes.Success,
        //ShiftRequestが存在しない場合は「要素0」の配列が返る
        //安全のため, ハッシュ化に関わる情報は含めない
        TestD.UserDataDictionary.Values.Select(i => new UserData(i) with { HashedPassword = new HashedPassword(string.Empty, string.Empty, 0) }).ToImmutableArray()
        ));

    /*******************************************
  * specification ;
  * name = GetIsScheduledShiftFinalVersionAsync ;
  * Function = 指定日の予定シフトが確定済みかどうかを取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 確認する日付 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync(DateTime date) => Task.Run<ApiResult<bool>>(() =>
    {
      if (!TestD.ScheduledShiftDictionary.TryGetValue(date.Date, out IScheduledShift? scheduledShift) || scheduledShift is null)
        return new(false, ApiResultCodes.Target_Date_Not_Found, false);

      return new(true, ApiResultCodes.Success, scheduledShift.SchedulingState == ShiftSchedulingState.FinalVersion);
    });

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
    public Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime) => Task.Run<ApiResult<ScheduledShift>>(() =>
    {
      if (TestD.ScheduledShiftDictionary.TryGetValue(dateTime.Date, out IScheduledShift? scheduledShift) && scheduledShift is not null)
        return new(true, ApiResultCodes.Success, new(scheduledShift));
      else
        return new(false, ApiResultCodes.Target_Date_Not_Found, null);
    });

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
    public Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(UserID userID) => Task.Run<ApiResult<ShiftRequest>>(() =>
    {
      if (!string.IsNullOrWhiteSpace(userID?.Value) && TestD.ShiftRequestsDictionary.TryGetValue(userID, out IShiftRequest? shiftRequest) && shiftRequest is not null)
        return new(true, ApiResultCodes.Success, new(shiftRequest));
      else
        return new(false, ApiResultCodes.UserID_Not_Found, null);
    });

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
    public Task<ApiResult<UserData>> GetUserDataByIDAsync(UserID userID) => Task.Run<ApiResult<UserData>>(() =>
    {
      if (string.IsNullOrWhiteSpace(userID?.Value) || !TestD.UserDataDictionary.TryGetValue(new UserID(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, null);

      //ユーザデータからはハッシュを削除して返す
      return new(true, ApiResultCodes.Success, new UserData(userData) with { HashedPassword = (new HashedPassword(userData.HashedPassword) with { Hash = string.Empty }) });
    });

    /*******************************************
  * specification ;
  * name = GetUsersByUserGroupAsync ;
  * Function = 指定のUserGroupに所属するユーザの情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 条件とするUserGroup ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None)
      => Task.Run<ApiResult<ImmutableArray<UserData>>>(() => new(true, ApiResultCodes.Success,
        TestD.UserDataDictionary.Values.Where(i => i.UserGroup == userGroup).Select(i => new UserData(i) with { HashedPassword = new HashedPassword(string.Empty, string.Empty, 0) }).ToImmutableArray()
        ));

    /*******************************************
  * specification ;
  * name = GetUsersByUserStateAsync ;
  * Function = 指定のUserStateに所属するのユーザ情報を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 条件とするUserState ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal)
      => Task.Run<ApiResult<ImmutableArray<UserData>>>(() => new(true, ApiResultCodes.Success,
        TestD.UserDataDictionary.Values.Where(i => i.UserState == userState).Select(i => new UserData(i) with { HashedPassword = new HashedPassword(string.Empty, string.Empty, 0) }).ToImmutableArray()
        ));

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
    public Task<ApiResult> SignUpAsync(IUserData userData) => Task.Run<ApiResult>(() =>
    {
      if (string.IsNullOrWhiteSpace(userData?.UserID?.Value))
        return new(false, ApiResultCodes.Not_Enough_Data); //最低限, ID文字列は必要

      UserID userID = new(userData.UserID); //DictionaryのKeyで使用できるように型変換を行う
      if (TestD.UserDataDictionary.ContainsKey(userID))
        return new(false, ApiResultCodes.UserID_Already_Exists);

      TestD.UserDataDictionary.Add(userID, userData);
      return new(true, ApiResultCodes.Success);
    });
  }
}
