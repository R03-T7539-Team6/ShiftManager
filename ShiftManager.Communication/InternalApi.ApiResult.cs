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
    T ReturnData { get; }
  }

  public record ApiResult(bool IsSuccess, ApiResultCodes ResultCode) : IApiResult;
  public record ApiResult<T>(bool IsSuccess, ApiResultCodes ResultCode, T ReturnData) : IApiResult<T>;

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

    /// <summary>データ不足</summary>
    Not_Enough_Data,

    /// <summary>パスワード強度不足</summary>
    Password_Strength_Not_Enough,

  }
}
