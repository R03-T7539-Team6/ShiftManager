using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_StoreData
  {
    //自身が所属する店舗に対してのみ操作可能

    Task<ApiResult> GetIsScheduledShiftFinalVersionAsync();
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
    Task<ApiResult> DeleteUserDataAsync(UserID userID);
  }

  /// <summary>内部で使用するAPI</summary>
  public partial class InternalApi : IInternalApi_StoreData
  {
    public Task<ApiResult> DeleteUserDataAsync(UserID userID) => Task.Run<ApiResult>(() =>
    {
      return new(true, ApiResultCodes.Success);
    });

    public Task<ApiResult<ScheduledShift>> GenerateScheduledShiftAsync(DateTime dateTime) => Task.Run<ApiResult<ScheduledShift>>(() =>
    {
      return new(true, ApiResultCodes.Success, null);
    });

    public Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(IUserID userID) => Task.Run<ApiResult<ShiftRequest>>(() =>
    {
      return new(true, ApiResultCodes.Success, null);
    });

    public Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync() => Task.Run<ApiResult<ImmutableArray<ShiftRequest>>>(() =>
    {
      return new(true, ApiResultCodes.Success, default);
    });

    public Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync() => Task.Run<ApiResult<ImmutableArray<UserData>>>(() =>
    {
      return new(true, ApiResultCodes.Success, default);
    });

    public Task<ApiResult> GetIsScheduledShiftFinalVersionAsync() => Task.Run<ApiResult>(() =>
    {
      return new(true, ApiResultCodes.Success);
    });

    public Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime) => Task.Run<ApiResult<ScheduledShift>>(() =>
    {
      return new(true, ApiResultCodes.Success, null);
    });

    public Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(IUserID userID) => Task.Run<ApiResult<ShiftRequest>>(() =>
    {
      return new(true, ApiResultCodes.Success, null);
    });

    public Task<ApiResult<UserData>> GetUserDataByIDAsync(IUserID userID) => Task.Run<ApiResult<UserData>>(() =>
    {
      return new(true, ApiResultCodes.Success, null);
    });

    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None) => Task.Run<ApiResult<ImmutableArray<UserData>>>(() =>
    {
      return new(true, ApiResultCodes.Success, default);
    });

    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal) => Task.Run<ApiResult<ImmutableArray<UserData>>>(() =>
    {
      return new(true, ApiResultCodes.Success, default);
    });

    public Task<ApiResult> SignUpAsync(IUserData userData) => Task.Run<ApiResult>(() =>
    {
      return new(true, ApiResultCodes.Success);
    });
  }
}
