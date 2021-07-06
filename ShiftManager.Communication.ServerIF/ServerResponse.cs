using RestSharp;

namespace ShiftManager.Communication
{
  /// <summary>サーバからの返答を記録します</summary>
  public record ServerResponse(IRestResponse Response);

  /// <summary>サーバからの返答を記録します</summary>
  /// <typeparam name="T">返答データの型</typeparam>
  public record ServerResponse<T>(IRestResponse Response, T? Content) : ServerResponse(Response) where T : class;

  //エラーの場合, Contentに直接Error Messageが入るので, それをErrorMessageに書き込むようにする
  /// <summary>サーバからの返答を記録します</summary>
  /// <typeparam name="T">返答データの型</typeparam>
  public record ServerErrorResponse<T>(IRestResponse Response, T? Content, ErrorType Error) : ServerResponse<T>(Response, Content) where T : class;

  public enum ErrorType
  {
    None,
    Unknown,
    Unauthorized,
    Wrong_ID,
    Wrong_PW,
    Invalid_Json_Format,
  }
}
