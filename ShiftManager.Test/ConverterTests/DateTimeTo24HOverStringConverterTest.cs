using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using ShiftManager;

namespace ShiftManager.Test
{
	public class DateTimeTo24HOverStringConverterTests
  {
    DateTimeTo24HOverStringConverter target { get; } = new();

    [SetUp]
		public void Setup()
		{
		}

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

    [TestCaseSource(nameof(Convert_TestCases))]
    public string ConvertTest(in DateTime value, in DateTime parameter) => target.Convert(new object[] { value, parameter }, null, null, null) as string;

    /*[TestCaseSource(nameof(ConvertBack_TestCases))]
    public DateTime ConvertBackTest(in string value, in DateTime parameter) => (DateTime)target.ConvertBack(value, null, null, null)[0];*/

    [TestCaseSource(nameof(ConvertBack_InvalidInput_TestCases))]
    public void ConvertBackInvalidInputTest(string value, DateTime parameter, Type exception) => Assert.Throws(exception, () => target.ConvertBack(value, null, null, null));
    
	}
}
