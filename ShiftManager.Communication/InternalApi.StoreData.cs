using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_StoreData
  {
    //自身が所属する店舗に対してのみ操作可能

    Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync(DateTime date);
    Task<ApiResult<UserData>> GetUserDataByIDAsync(IUserID userID);
    Task<ApiResult> SignUpAsync(IUserData userData);
    Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync();
    Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None);
    Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal);
    Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(IUserID userID);
    Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync();
    Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(IUserID userID);
    Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime);
    Task<ApiResult<ScheduledShift>> GenerateScheduledShiftAsync(DateTime dateTime);
    Task<ApiResult> DeleteUserDataAsync(IUserID userID);
  }

  /// <summary>内部で使用するAPI</summary>
  public partial class InternalApi : IInternalApi_StoreData
  {
    public Task<ApiResult> DeleteUserDataAsync(IUserID userID) => DeleteUserDataAsync(new(userID));//NULLが渡されるとぶっ壊れるので注意
    public Task<ApiResult> DeleteUserDataAsync(UserID userID)
      => Task.Run<ApiResult>(() => !string.IsNullOrWhiteSpace(userID?.Value) && TestD.UserDataDictionary.Remove(userID) //UserIDが指定されていた場合のみ実行
      ? new(true, ApiResultCodes.Success) : new(false, ApiResultCodes.UserID_Not_Found));

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

    public Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(IUserID userID) => GenerateShiftRequestAsync(new(userID));
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

    public Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync()
      => Task.Run<ApiResult<ImmutableArray<ShiftRequest>>>(() => new(true, ApiResultCodes.Success,
        TestD.ShiftRequestsDictionary.Values.Select(i => new ShiftRequest(i)).ToImmutableArray() //ShiftRequestが存在しない場合は「要素0」の配列が返る
        ));

    public Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync()
      => Task.Run<ApiResult<ImmutableArray<UserData>>>(() => new(true, ApiResultCodes.Success,
        //ShiftRequestが存在しない場合は「要素0」の配列が返る
        //安全のため, ハッシュ化に関わる情報は含めない
        TestD.UserDataDictionary.Values.Select(i => new UserData(i) with { HashedPassword = new HashedPassword(string.Empty, string.Empty, 0) }).ToImmutableArray()
        ));

    public Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync(DateTime date) => Task.Run<ApiResult<bool>>(() =>
    {
      if (!TestD.ScheduledShiftDictionary.TryGetValue(date.Date, out IScheduledShift? scheduledShift) || scheduledShift is null)
        return new(false, ApiResultCodes.Target_Date_Not_Found, false);

      return new(true, ApiResultCodes.Success, scheduledShift.SchedulingState == ShiftSchedulingState.FinalVersion);
    });

    public Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime) => Task.Run<ApiResult<ScheduledShift>>(() =>
    {
      if (TestD.ScheduledShiftDictionary.TryGetValue(dateTime.Date, out IScheduledShift? scheduledShift) && scheduledShift is not null)
        return new(true, ApiResultCodes.Success, new(scheduledShift));
      else
        return new(false, ApiResultCodes.Target_Date_Not_Found, null);
    });

    public Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(IUserID userID) => GetShiftRequestByIDAsync(new(userID));
    public Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(UserID userID) => Task.Run<ApiResult<ShiftRequest>>(() =>
    {
      if (!string.IsNullOrWhiteSpace(userID?.Value) && TestD.ShiftRequestsDictionary.TryGetValue(userID, out IShiftRequest? shiftRequest) && shiftRequest is not null)
        return new(true, ApiResultCodes.Success, new(shiftRequest));
      else
        return new(false, ApiResultCodes.UserID_Not_Found, null);
    });

    public Task<ApiResult<UserData>> GetUserDataByIDAsync(IUserID userID) => GetUserDataByIDAsync(new(userID));
    public Task<ApiResult<UserData>> GetUserDataByIDAsync(UserID userID) => Task.Run<ApiResult<UserData>>(() =>
    {
      if (string.IsNullOrWhiteSpace(userID?.Value) || !TestD.UserDataDictionary.TryGetValue(new UserID(userID), out IUserData? userData) || userData is null)
        return new(false, ApiResultCodes.UserID_Not_Found, null);

      //ユーザデータからはハッシュを削除して返す
      return new(true, ApiResultCodes.Success, null);
    });

    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None)
      => Task.Run<ApiResult<ImmutableArray<UserData>>>(() => new(true, ApiResultCodes.Success,
        TestD.UserDataDictionary.Values.Where(i => i.UserGroup == userGroup).Select(i => new UserData(i) with { HashedPassword = new HashedPassword(string.Empty, string.Empty, 0) }).ToImmutableArray()
        ));

    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal)
      => Task.Run<ApiResult<ImmutableArray<UserData>>>(() => new(true, ApiResultCodes.Success,
        TestD.UserDataDictionary.Values.Where(i => i.UserState == userState).Select(i => new UserData(i) with { HashedPassword = new HashedPassword(string.Empty, string.Empty, 0) }).ToImmutableArray()
        ));

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
