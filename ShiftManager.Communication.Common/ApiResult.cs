namespace ShiftManager.Communication
{
  /// <summary>APIの返答</summary>
  public interface IApiResult
  {
    /// <summary>実行に成功したかどうか</summary>
    bool IsSuccess { get; }

    /// <summary>実行結果コード</summary>
    ApiResultCodes ResultCode { get; }

    string ToString() => IsSuccess ? "SUCCESS:" : $"FAULT(ErrorCode.{(int)ResultCode}) : {ResultCode}";
  }

  /// <summary>APIの返答用インターフェイス(返り値付き)</summary>
  public interface IApiResult<T> : IApiResult
  {
    /// <summary>実行の結果得られたデータ</summary>
    T? ReturnData { get; }
  }

  public record ApiResult(bool IsSuccess, ApiResultCodes ResultCode) : IApiResult;

  public record ApiResult<T>(bool IsSuccess, ApiResultCodes ResultCode, T? ReturnData) : IApiResult<T>;

  public enum ApiResultCodes
  {
    /// <summary>情報が存在しない (不明なエラー)</summary>
    Unknown_Error,

    /// <summary>実行に成功した</summary>
    Success,

    /// <summary>未ログイン</summary>
    Not_Logged_In,

    /// <summary>店舗IDが存在しない</summary>
    StoreID_Not_Found,
    /// <summary>店舗IDが既に存在する</summary>
    StoreID_Already_Exists,

    /// <summary>ユーザIDが存在しない</summary>
    UserID_Not_Found,
    /// <summary>ユーザIDが既に存在する</summary>
    UserID_Already_Exists,

    /// <summary>パスワードが一致しない</summary>
    Password_Not_Match,

    /// <summary>データが存在しない</summary>
    Data_Not_Found,
    /// <summary>データが既に存在する</summary>
    Data_Already_Exists,
    /// <summary>新しいデータがNULLである</summary>
    NewData_Is_NULL,

    /// <summary>データ不足</summary>
    Not_Enough_Data,

    /// <summary>パスワード強度不足</summary>
    Password_Strength_Not_Enough,

    /// <summary>指定の日付のデータが見つからなかった</summary>
    Target_Date_Not_Found,

    /// <summary>シフト組みが完了していない</summary>
    Scheduling_Is_Still_In_Working,

    /// <summary>シフト組みが完了していない</summary>
    Scheduling_Is_Not_Started,

    /// <summary>不正な入力</summary>
    Invalid_Input,

    /// <summary>休憩開始打刻がされていない</summary>
    BreakTime_Not_Started,

    /// <summary>休憩終了打刻がされていない</summary>
    BreakTime_Not_Ended,

    /// <summary>勤務開始(出勤)打刻がされていない</summary>
    Work_Not_Started,

    /// <summary>勤務終了(退勤)打刻がされていない</summary>
    Work_Not_Ended,

    /// <summary>休憩時間が0分以下 (休憩時間は1分以上という制約がある)</summary>
    BreakTimeLen_Zero_Or_Less,

    /// <summary>氏名が一致しない</summary>
    FullName_Not_Match,

    /// <summary>予定シフトに指定のユーザIDが存在しない</summary>
    UserID_Not_Found_In_Scheduled_Shift,

    /// <summary>勤務時間が短すぎる(最低1分は必要)</summary>
    WorkTimeLen_Too_Short,

    /// <summary>ユーザIDの文字列長が不正</summary>
    Invalid_Length_UserID,

    /// <summary>サインインに失敗した</summary>
    SignIn_Failed,

    /// <summary>許可されていない操作 (権限不足)</summary>
    Not_Allowed_Control,

    Request_Time_Out,

    Delete_Success,

    E204_No_Content,
    E400_Bad_Request,
    E403_Forbidded,
    E404_Not_Found
  }
}
