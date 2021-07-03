using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{

  public partial class RestApiBroker : InternalApi_ShiftRequest
  {
    /// <summary>シフト希望を追加します  初めての追加の場合は, コレクションにユーザのデータが生成されたうえで追加されます</summary>
    /// <param name="singleShiftData">追加する単一シフトデータ</param>
    /// <returns>実行結果</returns>
    public ApiResult AddShiftRequest(ISingleShiftData singleShiftData)
    {
      UserID targetUserID = new(singleShiftData.UserID);
      if (!TestD.ShiftRequestsDictionary.TryGetValue(targetUserID, out IShiftRequest? shiftRequest) || shiftRequest is null)
      {
        TestD.ShiftRequestsDictionary.Add(targetUserID, new ShiftRequest(targetUserID, DateTime.Now, new() { { singleShiftData.WorkDate, singleShiftData } }));

        return new(true, ApiResultCodes.UserID_Not_Found);
      }
      else
      {
        try
        {
          TestD.ShiftRequestsDictionary[targetUserID] = new ShiftRequest(shiftRequest) with
          {
            LastUpdate = DateTime.Now,
            RequestsDictionary = new(shiftRequest.RequestsDictionary) { { singleShiftData.WorkDate, singleShiftData } }
          };

          return new(true, ApiResultCodes.Success);
        }
        catch (ArgumentNullException)
        {
          return new(false, ApiResultCodes.NewData_Is_NULL);
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
          return new(false, ApiResultCodes.Target_Date_Not_Found);
        }
      } 
    }
    public Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData)
      => Task.Run(() => AddShiftRequest(singleShiftData));

    public Task<ApiResult<SingleShiftData>> GetShiftRequestByDateAsync(DateTime date) => Task.Run<ApiResult<SingleShiftData>>(() =>
    {
      if (CurrentUserData is null)
        return new(false, ApiResultCodes.Not_Logged_In, null);//ログイン中しか使用できない

      if (TestD.ShiftRequestsDictionary.TryGetValue(new(CurrentUserData.UserID), out IShiftRequest? shiftRequest) || shiftRequest is null)
        return new(false, ApiResultCodes.UserID_Not_Found, null);

      if (shiftRequest.RequestsDictionary.TryGetValue(date, out ISingleShiftData? singleShiftData) || singleShiftData is null)
        return new(false, ApiResultCodes.Target_Date_Not_Found, null);
      else
        return new(true, ApiResultCodes.Success, new(singleShiftData));
    });

    [Obsolete("Please use AddShiftRequestAsync Method")]
    public Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData)
      => AddShiftRequestAsync(singleShiftData);
  }
}
