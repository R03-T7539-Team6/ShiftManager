using System.Net;
using System.Runtime.CompilerServices;

using ShiftManager.DataClasses;

[assembly: InternalsVisibleTo("ShiftManager.Communication.RestApiBroker.Test")]


namespace ShiftManager.Communication
{
  public partial class RestApiBroker : IApi
  {
    internal ServerIF Sv { get; } = new();
    public IUserData? CurrentUserData { get; private set; } = null;

    public bool IsLoggedIn { get => Sv.IsLoggedIn; }

    static ApiResultCodes ToApiRes(in HttpStatusCode statusCode) => statusCode switch
    {
      HttpStatusCode.OK => ApiResultCodes.Success,
      HttpStatusCode.NoContent => ApiResultCodes.Delete_Success,
      HttpStatusCode.BadRequest => ApiResultCodes.E400_Bad_Request,
      HttpStatusCode.Unauthorized => ApiResultCodes.Not_Logged_In,
      HttpStatusCode.Forbidden => ApiResultCodes.E403_Forbidded,
      HttpStatusCode.NotFound => ApiResultCodes.E404_Not_Found,
      0 => ApiResultCodes.Request_Time_Out,
      _ => ApiResultCodes.Unknown_Error
    };
  }
}
