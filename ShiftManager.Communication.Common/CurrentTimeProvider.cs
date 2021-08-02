using System;

namespace ShiftManager.Communication
{
  public interface ICurrentTimeProvider
  {
    DateTime CurrentTime { get; }
  }

  public class CurrentTimeProvider : ICurrentTimeProvider { public DateTime CurrentTime => DateTime.Today + TimeSpan.FromMinutes((int)DateTime.Now.TimeOfDay.TotalMinutes); } //分未満を削除する
}
