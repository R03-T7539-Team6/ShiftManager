using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

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
  }
}
