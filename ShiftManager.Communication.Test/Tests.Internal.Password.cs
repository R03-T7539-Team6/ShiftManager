﻿using NUnit.Framework;

using ShiftManager.DataClasses;

namespace ShiftManager.Communication.InternalApiTest
{
  public class PasswordTests
  {
    static PasswordTests()
    {
      PasswordHashingTest_TestCases = new object[PasswordHashingTest_TestCasesCount];
      for (int i = 0; i < PasswordHashingTest_TestCasesCount; i++)
        PasswordHashingTest_TestCases[i] = new object[] { Password_Samples[i], Salt_Samples[i], STRETCH_COUNT, HashedPasswordExpectedResults[i] };
    }
    InternalApi TestTarget { get; } = new();

    [SetUp]
    public void Setup()
    {
    }

    static object[] PasswordHashingTest_TestCases;
    const int STRETCH_COUNT = 10000;
    const int PasswordHashingTest_TestCasesCount = 5;
    static readonly string[] Password_Samples = new string[PasswordHashingTest_TestCasesCount]
    {
      "0000",
      "0000",
      "12345678",
      "HWRnwOCy4HMiGPTA",
      "i1KgfuhDy41yGy8x"
    };
    //Saltは16Bytes(128bit = 22char)とする.  <- https://ja.wikipedia.org/wiki/PBKDF2
    static readonly string[] Salt_Samples = new string[PasswordHashingTest_TestCasesCount]
    {
      "30/DmISxGM+mLG0kfnbF1Q==",
      "ROa+FuvyOtq8w5w6LC92PA==",
      "5fRI5RQ+9eVSCMQ5g1z9LA==",
      "mdTM8HTo96Ba3kV77N9MSQ==",
      "LkDfl7iv6fO5bShQoru4Iw=="
    };
    static readonly string[] HashedPasswordExpectedResults = new string[PasswordHashingTest_TestCasesCount]
    {
      "Mk0Lu/PAI+aHFF9PiR+6NFiNnzR8CDbDaNPvqdB+Dh/aHUcJMTCsBE7K9/uMWtgu7FqLcnsyxsu7fToHU1dfjA==",
      "YVfGOEf8r8a+S7TioAWNN0u3seOby1d5SLOzhIymJwf0wdougMuJYmU2Ws1PkTEcr70Flu2ReqmSI7WaiovvKg==",
      "XaXu/kodppLFbME+9y470G+WPaCqxziAiB5w7ZwYq95zA3gbEjLkxz+HJ6qUWUJ15CI/hd1cAtwXSVRnvpH+4g==",
      "xMIjIuiIPYrmBoQqskJHHYlL2hc0TvKsdjbifXICxPzvUkh5/weTbWCoFECQabYZeVP+awQ9Cv+txfWzLtFxQQ==",
      "bXdYV4Mtv5udvHw/uI68hVBPufcxD0bdeamIXhj2jkvQkW4tb4vOrbnQkKwwaDhFjzJdrprJkzyRK9rHId2spg=="
    };


    [TestCaseSource(nameof(PasswordHashingTest_TestCases))]
    public void PasswordHashingTest_Record(string rawPW, string saltBASE64, int stretchCount, string expected)
      => PasswordHashingTest(rawPW, new HashedPassword(string.Empty, saltBASE64, stretchCount), expected);
    [TestCaseSource(nameof(PasswordHashingTest_TestCases))]
    public void PasswordHashingTest_Interface(string rawPW, string saltBASE64, int stretchCount, string expected)
      => PasswordHashingTest(rawPW, new HashedPassword_NotifyPropertyChanged() { Salt = saltBASE64, StretchCount = stretchCount }, expected);

    private void PasswordHashingTest(string rawPW, IHashedPassword hashedPassword, string expected)
      => Assert.AreEqual(expected, InternalApi.HashedPasswordGetter(rawPW, hashedPassword).Hash);
  }
}