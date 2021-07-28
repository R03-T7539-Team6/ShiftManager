using System;
using System.Collections.Generic;
using System.Globalization;

using ShiftManager.DataClasses;

namespace ShiftManager
{
  public class IntEquallyCheckConverterBase
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      => (int)value
         == int.Parse(parameter as string);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }

  public class BooleanReverseConverterBase
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
  }

  public class MultiDoubleValueMultiplConverterBase
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      List<double> l = new();
      foreach (var v in values)
        l.Add(v is string s ? double.Parse(s) : (double)v);

      double ret = 1;
      foreach (var v in l)
        ret *= v;
      return ret;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }

  public class HasFlagConverterBase
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value as Enum)?.HasFlag((Enum)parameter) ?? false;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }

  public class TimeSpanTo24HOverStringConverterBase
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      TimeSpan ts = (TimeSpan)value;
      return $"{(int)ts.TotalHours:D2}:{ts:mm}";
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => TimeSpan.Parse(value as string);
  }

  public class DoubleValueAddConverterBase
  {
    public double? Value { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (double)value + (Value ?? double.Parse(parameter as string));
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }

  public class UserIDToNameStringConverterBase
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length >= 2 && values[0] is IContainsApiHolder api && values[1] is IUserID id)
      {
        var res = api.ApiHolder.Api.GetUserDataByIDAsync(id).Result;
        if (res.IsSuccess && res.ReturnData is not null)
          return res.ReturnData.FullName.LastName + " " + res.ReturnData.FullName.FirstName;
      }
      return null;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
