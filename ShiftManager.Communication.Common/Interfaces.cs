using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IApi : InternalApi_ManageData, InternalApi_ScheduledShift, InternalApi_ShiftRequest, InternalApi_WorkLog, IInternalApi_SignIn, IInternalApi_StoreData, IInternalApi_UserData, IInternalApi_Password
  { }

  public interface InternalApi_ManageData
  {
    Task<ApiResult<ImmutableDictionary<string, ImmutableArray<string>>>> GetDataListAsync();
    Task<ApiResult<T>> DownloadDataAsync<T>(string Category, string DataName);
    Task<ApiResult> UploadDataAsync<T>(string Category, string DataName, T Data);
  }

  public interface IInternalApi_Password
  {
    Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID);
  }

  public interface InternalApi_ScheduledShift
  {
    Task<ApiResult<SingleShiftData>> GetScheduledShiftByIDAsync(DateTime targetDate, IUserID userID);
    Task<ApiResult> UpdateShiftSchedulingStateAsync(DateTime targetDate, ShiftSchedulingState shiftSchedulingState);
    Task<ApiResult> UpdateSingleScheduledShiftListAsync(DateTime targetDate, IReadOnlyCollection<ISingleShiftData> singleShiftDatas);
    Task<ApiResult> UpdateRequiredWorkerCountListAsync(DateTime targetDate, IReadOnlyCollection<int> singleShiftDatas);
  }

  public interface InternalApi_ShiftRequest
  {
    ApiResult AddShiftRequest(ISingleShiftData singleShiftData);
    Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData);
    Task<ApiResult<SingleShiftData>> GetShiftRequestByDateAsync(DateTime date);
    Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData);
  }

  public interface IInternalApi_SignIn
  {
    IUserData? CurrentUserData { get; }

    /// <summary>サインインを試行します.</summary>
    /// <param name="userID">ユーザID</param>
    /// <param name="HashedPasswordGetter">パスワードのハッシュ化に関する情報を受けてハッシュ化パスワードを返す関数</param>
    /// <returns>試行結果</returns>
    Task<ApiResult> SignInAsync(IUserID userID, IHashedPassword hashedPassword);

    /// <summary>サインアウトを実行します</summary>
    /// <returns>実行結果</returns>
    Task<ApiResult> SignOutAsync();
  }
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

  public interface IInternalApi_UserData
  {
    Task<ApiResult> UpdatePasswordAsync(IHashedPassword hashedPassword);
    Task<ApiResult> UpdatePasswordAsync(IUserID userID, INameData nameData, IHashedPassword hashedPassword);

    Task<ApiResult<WorkLog>> GetWorkLogAsync();
    Task<ApiResult<UserSetting>> GetUserSettingAsync();
  }

  public interface InternalApi_WorkLog
  {
    Task<ApiResult<DateTime>> DoWorkStartTimeLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoWorkEndTimeLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoBreakTimeStartLoggingAsync(IUserID userID);
    Task<ApiResult<DateTime>> DoBreakTimeEndLoggingAsync(IUserID userID);
  }
}
