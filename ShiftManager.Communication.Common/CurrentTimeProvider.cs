using System;

namespace ShiftManager.Communication
{
  public interface ICurrentTimeProvider
  {
    DateTime CurrentTime { get; }
  }

  public class CurrentTimeProvider : ICurrentTimeProvider { public DateTime CurrentTime => DateTime.Now; }
}
