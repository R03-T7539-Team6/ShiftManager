using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface InternalApi_ShiftRequest
  {
    ApiResult AddShiftRequest(ISingleShiftData singleShiftData);
    Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData);
    Task<ApiResult<SingleShiftData>> GetShiftRequestByDateAsync(DateTime date);
    Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData);
  }

  public partial class InternalApi : InternalApi_ShiftRequest
  {
    /// <summary>シフト希望を追加します  初めての追加の場合は, コレクションにユーザのデータが生成されたうえで追加されます</summary>
    /// <param name="singleShiftData">追加する単一シフトデータ</param>
    /// <returns>実行結果</returns>
    /*******************************************
  * specification ;
  * name = AddShiftRequest ;
  * Function = シフト希望を追加します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 追加する単一日シフト希望情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
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
            RequestsDictionary = new(shiftRequest.RequestsDictionary)
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

    /*******************************************
  * specification ;
  * name = AddShiftRequestAsync ;
  * Function = AddShiftRequestを非同期に実行します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = シフト希望情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    public Task<ApiResult> AddShiftRequestAsync(ISingleShiftData singleShiftData)
      => Task.Run(() => AddShiftRequest(singleShiftData));

    /*******************************************
  * specification ;
  * name = GetShiftRequestByDateAsync ;
  * Function = 日付を指定してシフト希望を取得します ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 指定する日付 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
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

    /*******************************************
  * specification ;
  * name = UpdateShiftRequestAsync ;
  * Function = シフト希望を更新します ;
  * note = AddShiftRequestAsyncに同一の機能があります. ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = シフト情報 ;
  * output = 実行結果 ;
  * end of specification ;
  *******************************************/
    [Obsolete("Please use AddShiftRequestAsync Method")]
    public Task<ApiResult> UpdateShiftRequestAsync(ISingleShiftData singleShiftData)
      => AddShiftRequestAsync(singleShiftData);
  }
}
