using System;
using System.Collections;

using NUnit.Framework;

namespace ShiftManager.Test
{
  public class DateTimeTo24HOverStringConverterTests
  {
    DateTimeTo24HOverStringConverterBase target { get; } = new();

    public static IEnumerable Convert_TestCases
    {
      get
      {
        yield return new TestCaseData(new DateTime(2021, 5, 1, 11, 0, 0), new DateTime(2021, 5, 1, 10, 0, 0)).Returns("11:00");
        yield return new TestCaseData(new DateTime(2021, 5, 1, 0, 0, 0), new DateTime(2021, 5, 1, 10, 0, 0)).Returns("00:00");
        yield return new TestCaseData(new DateTime(2021, 5, 2, 0, 0, 0), new DateTime(2021, 5, 1, 10, 0, 0)).Returns("24:00");
        yield return new TestCaseData(new DateTime(2021, 5, 2, 1, 5, 0), new DateTime(2021, 5, 1, 10, 0, 0)).Returns("25:05");
        yield return new TestCaseData(new DateTime(2021, 5, 3, 1, 5, 0), new DateTime(2021, 5, 1, 10, 0, 0)).Returns("49:05");
        yield return new TestCaseData(new DateTime(2021, 5, 6, 3, 45, 0), new DateTime(2021, 5, 1, 10, 0, 0)).Returns("123:45");
      }
    }

    public static IEnumerable ConvertBack_TestCases
    {
      get
      {
        yield return new TestCaseData("11:00", new DateTime(2021, 5, 1, 10, 0, 0)).Returns(new DateTime(2021, 5, 1, 11, 0, 0));
        yield return new TestCaseData("00:00", new DateTime(2021, 5, 1, 10, 0, 0)).Returns(new DateTime(2021, 5, 1, 0, 0, 0));
        yield return new TestCaseData("24:00", new DateTime(2021, 5, 1, 10, 0, 0)).Returns(new DateTime(2021, 5, 2, 0, 0, 0));
        yield return new TestCaseData("25:05", new DateTime(2021, 5, 1, 10, 0, 0)).Returns(new DateTime(2021, 5, 2, 1, 5, 0));
        yield return new TestCaseData("49:00", new DateTime(2021, 5, 1, 10, 0, 0)).Returns(new DateTime(2021, 5, 3, 1, 0, 0));
        yield return new TestCaseData("123:45", new DateTime(2021, 5, 1, 10, 0, 0)).Returns(new DateTime(2021, 5, 6, 3, 45, 0));
      }
    }

    public static IEnumerable ConvertBack_InvalidInput_TestCases
    {
      get
      {
        yield return new TestCaseData(null, new DateTime(2021, 5, 1, 10, 0, 0), typeof(ArgumentNullException));
        yield return new TestCaseData("0000", new DateTime(2021, 5, 1, 10, 0, 0), typeof(FormatException));
        yield return new TestCaseData(":00:00:", new DateTime(2021, 5, 1, 10, 0, 0), typeof(FormatException));
        yield return new TestCaseData(":", new DateTime(2021, 5, 1, 10, 0, 0), typeof(FormatException));
        yield return new TestCaseData("aX:00", new DateTime(2021, 5, 1, 10, 0, 0), typeof(FormatException));
        yield return new TestCaseData("-1:00", new DateTime(2021, 5, 1, 10, 0, 0), typeof(ArgumentOutOfRangeException));
        yield return new TestCaseData("00:aX", new DateTime(2021, 5, 1, 10, 0, 0), typeof(FormatException));
        yield return new TestCaseData("00:-1", new DateTime(2021, 5, 1, 10, 0, 0), typeof(ArgumentOutOfRangeException));
        yield return new TestCaseData("00:60", new DateTime(2021, 5, 1, 10, 0, 0), typeof(ArgumentOutOfRangeException));
      }
    }

    /*******************************************
  * specification ;
  * name = ConvertTest ;
  * Function = 時刻情報から文字列に正常に変換できるかどうかをテストする ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 変換対象の日時, 基準日 ;
  * output = 変換結果の文字列 ;
  * end of specification ;
  *******************************************/
    [TestCaseSource(nameof(Convert_TestCases))]
    public string ConvertTest(in DateTime value, in DateTime parameter) => target.Convert(new object[] { value, parameter }, null, null, null) as string;

    /*[TestCaseSource(nameof(ConvertBack_TestCases))]
    public DateTime ConvertBackTest(in string value, in DateTime parameter) => (DateTime)target.ConvertBack(value, null, null, null)[0];*/

    /*******************************************
  * specification ;
  * name = ConvertBackInvalidInputTest ;
  * Function = 文字列から時刻情報に変換する際, 異常値入力に対し正常に例外を吐くかをテストする ;
  * note = N/A ;
  * date = 07/05/2021 ;
  * author = 藤田一範 ;
  * History = v1.0:新規作成 ;
  * input = 変換対象の時刻表現文字列, 基準日情報, 期待される例外の型 ;
  * output = N/A ;
  * end of specification ;
  *******************************************/
    [TestCaseSource(nameof(ConvertBack_InvalidInput_TestCases))]
    public void ConvertBackInvalidInputTest(string value, DateTime parameter, Type exception) => Assert.Throws(exception, () => target.ConvertBack(value, null, null, null));

  }
}
