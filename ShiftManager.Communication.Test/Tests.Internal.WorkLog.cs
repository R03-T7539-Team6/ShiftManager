using System;
using System.Collections;
using System.Threading.Tasks;

using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.InternalApiTest
{
  public class WorkLogTests
  {
    static IEnumerable GetBreakTimeLengthTest_TestCases
    {
      get
      {
        yield return new TestCaseData( //1時間 (=60分) の計算
          new DateTime(2021, 5, 1, 0, 0, 0),
          new DateTime(2021, 5, 1, 1, 0, 0))
          .Returns(60);

        yield return new TestCaseData( //0時間の計算
          new DateTime(2021, 5, 1, 0, 0, 0),
          new DateTime(2021, 5, 1, 0, 0, 0))
          .Returns(0);

        yield return new TestCaseData( //1日の計算
          new DateTime(2021, 5, 1, 0, 0, 0),
          new DateTime(2021, 5, 2, 0, 0, 0))
          .Returns(24 * 60);

        yield return new TestCaseData( //1秒 = 0分の計算
          new DateTime(2021, 5, 1, 0, 0, 0),
          new DateTime(2021, 5, 1, 0, 0, 1))
          .Returns(0);

        yield return new TestCaseData( //59秒 = 0分の計算
          new DateTime(2021, 5, 1, 0, 0, 0),
          new DateTime(2021, 5, 1, 0, 0, 59))
          .Returns(0);

        yield return new TestCaseData( //うるう年に2月29日を挟むテスト
          new DateTime(2020, 2, 28, 23, 30, 0),
          new DateTime(2020, 3, 1, 0, 30, 0))
          .Returns(25 * 60);

        yield return new TestCaseData( //うるう年に2月29日を挟むテスト
          new DateTime(2021, 2, 28, 23, 30, 0),
          new DateTime(2021, 3, 1, 0, 30, 0))
          .Returns(60);

        yield return new TestCaseData( //特大の値を入力するテスト
          new DateTime(2000, 1, 1, 0, 0, 0),
          new DateTime(2100, 1, 1, 0, 0, 0))
          .Returns((100/*60年*/* 365 + 25/*うるう年*/) * 24 * 60);

      }
    }

    [TestCaseSource(nameof(GetBreakTimeLengthTest_TestCases))]
    public int GetBreakTimeLengthTest(in DateTime start, in DateTime end)
      => InternalApi.GetBreakTimeLength(start, end);

    public class CurrentTimeMock : ICurrentTimeProvider
    {
      public DateTime CurrentTime { get; set; }
    }
    [Test]
    public async Task AttendLeaveTest()
    {
      CurrentTimeMock tp = new();
      InternalApi i = new() { CurrentTimeProvider = tp };
      tp.CurrentTime = new(2021, 5, 31, 9, 1, 2);

      Assert.AreEqual(new ApiResult<DateTime>(true, ApiResultCodes.Success, tp.CurrentTime), await i.DoWorkStartTimeLoggingAsync(new UserID("ID0000")));
      tp.CurrentTime = new(2021, 5, 31, 9, 2, 2);
      Assert.AreEqual(new ApiResult<DateTime>(true, ApiResultCodes.Success, tp.CurrentTime), await i.DoWorkEndTimeLoggingAsync(new UserID("ID0000")));
    }

    [Test]
    public async Task AttendBreakLeaveTest()
    {
      CurrentTimeMock tp = new();
      InternalApi i = new() { CurrentTimeProvider = tp };
      tp.CurrentTime = new(2021, 5, 31, 9, 1, 2);

      Assert.AreEqual(new ApiResult<DateTime>(true, ApiResultCodes.Success, tp.CurrentTime), await i.DoWorkStartTimeLoggingAsync(new UserID("ID0000")));
      tp.CurrentTime = new(2021, 5, 31, 9, 5, 2);
      Assert.AreEqual(new ApiResult<DateTime>(true, ApiResultCodes.Success, tp.CurrentTime), await i.DoBreakTimeStartLoggingAsync(new UserID("ID0000")));
      tp.CurrentTime = new(2021, 5, 31, 9, 6, 2);
      Assert.AreEqual(new ApiResult<DateTime>(true, ApiResultCodes.Success, tp.CurrentTime), await i.DoBreakTimeEndLoggingAsync(new UserID("ID0000")));
      tp.CurrentTime = new(2021, 5, 31, 9, 8, 2);
      Assert.AreEqual(new ApiResult<DateTime>(true, ApiResultCodes.Success, tp.CurrentTime), await i.DoWorkEndTimeLoggingAsync(new UserID("ID0000")));
    }
  }
}
