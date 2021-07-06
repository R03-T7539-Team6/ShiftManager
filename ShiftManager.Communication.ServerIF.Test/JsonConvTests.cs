using System;

using NUnit.Framework;

namespace ShiftManager.Communication.ServerIF.Test
{
  public class JsonConvTests
  {
    #region ToJson
    [Test]
    public void ToJson_ValidInput() => Assert.AreEqual("{\"A\":\"xxx\",\"B\":3}", RestAPI.ToJson(new TestDataClass("xxx", 3)));

    [Test]
    public void ToJson_NullInput() => Assert.AreEqual("null", RestAPI.ToJson<TestDataClass>(null));

    [Test]
    public void ToJson_EmptyClassInput() => Assert.AreEqual("{}", RestAPI.ToJson<EmptyClass>(new()));
    #endregion

    #region FromJson
    [Test]
    public void FromJson_ValidInput() => Assert.AreEqual(new TestDataClass("xxx", 3), RestAPI.FromJson<TestDataClass>("{\"A\":\"xxx\",\"B\":3}"));

    [Test]
    public void FromJson_EmptyInput() => Assert.AreEqual(null, RestAPI.FromJson<TestDataClass>(string.Empty));
    

    [Test]
    public void FromJson_NullInput() => Assert.Throws(typeof(ArgumentNullException), () => RestAPI.FromJson<TestDataClass>(null));

    [Test]
    public void FromJson_WhiteSpaceInput() => Assert.AreEqual(null, RestAPI.FromJson<TestDataClass>("   "));

    [Test]
    public void FromJson_TooManyData() => Assert.AreEqual(new TestDataClass("xxx", 3), RestAPI.FromJson<TestDataClass>("{\"A\":\"xxx\",\"B\":3,\"C\":999}"));

    [Test]
    public void FromJson_TooLessData_Nullable() => Assert.AreEqual(new TestDataClass(default, 3), RestAPI.FromJson<TestDataClass>("{\"B\":3,\"C\":999}"));

    [Test]
    public void FromJson_TooLessData_NotNullable() => Assert.AreEqual(new TestDataClass("3", default), RestAPI.FromJson<TestDataClass>("{\"A\":\"3\",\"C\":999}"));

    [Test]
    public void FromJson_InvalidDataType_IntStringToInt() => Assert.AreEqual(new TestDataClass(default, 3), RestAPI.FromJson<TestDataClass>("{\"B\":\"3\"}"));
    [Test]
    public void FromJson_InvalidDataType_FloatStringToInt() => Assert.Throws(typeof(Newtonsoft.Json.JsonReaderException), () => RestAPI.FromJson<TestDataClass>("{\"B\":\"3.14\"}"));
    #endregion

    private record TestDataClass(string A, int B);
    private record EmptyClass();
  }
}
