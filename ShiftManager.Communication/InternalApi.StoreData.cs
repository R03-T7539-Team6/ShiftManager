using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_StoreData
  {
    //自身が所属する店舗に対してのみ操作可能

    Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync();
    Task<ApiResult<UserData>> GetUserDataByIDAsync(IUserID userID);
    Task<ApiResult> SignUpAsync(IUserData userData);
    Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync();
    Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None);
    Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal);
    Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(IUserID userID);
    Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync();
    Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(IUserID userID);
    Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime);
    Task<ApiResult<ScheduledShift>> GenerateScheduledShift(DateTime dateTime);
    Task<ApiResult> DeleteUserData(UserID userID);
  }

  /// <summary>内部で使用するAPI</summary>
  public partial class InternalApi : IInternalApi_StoreData
  {
    public Task<ApiResult> DeleteUserData(UserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ScheduledShift>> GenerateScheduledShift(DateTime dateTime)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ShiftRequest>> GenerateShiftRequestAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ImmutableArray<ShiftRequest>>> GetAllShiftRequestAsync()
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ImmutableArray<UserData>>> GetAllUserAsync()
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<bool>> GetIsScheduledShiftFinalVersionAsync()
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ScheduledShift>> GetScheduledShiftByDateAsync(DateTime dateTime)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ShiftRequest>> GetShiftRequestByIDAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<UserData>> GetUserDataByIDAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserGroupAsync(UserGroup userGroup = UserGroup.None)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult<ImmutableArray<UserData>>> GetUsersByUserStateAsync(UserState userState = UserState.Normal)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResult> SignUpAsync(IUserData userData)
    {
      throw new NotImplementedException();
    }
  }
}
