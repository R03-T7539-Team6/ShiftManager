using System;
using System.Threading.Tasks;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication
{
  public delegate string HashedPasswordGetter(IHashedPassword hashedPassword);
  public interface IInternalApi_Password
  {
    Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID);
  }

  public partial class InternalApi : IInternalApi_Password
  {
    public Task<ApiResult<HashedPassword>> GetPasswordHashingDataAsync(IUserID userID)
    {
      throw new NotImplementedException();
    }
  }
}
