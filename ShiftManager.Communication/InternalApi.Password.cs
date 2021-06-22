using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public interface IInternalApi_Password
  {
    Task<ApiResult<HashedPassword>> GetPasswordHashingData(IUserID userID);
  }

  public partial class InternalApi : IInternalApi_Password
  {
    public Task<ApiResult<HashedPassword>> GetPasswordHashingData(IUserID userID)
    {
      throw new NotImplementedException();
    }
  }
}
