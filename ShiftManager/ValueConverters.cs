using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShiftManager
{
  public class IntEquallyCheckConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      => (int)value
         == int.Parse(parameter as string);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }

  public class BooleanReverseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
  }

  //ref : https://oita.oika.me/2018/04/15/pilevalueconverter/
  public class ValueConverterGroup : IValueConverter
  {
    public List<IValueConverter> Converters { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (var cnv in Converters)
        value = cnv.Convert(value, targetType, parameter, culture);
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (var cnv in Converters)
        value = cnv.ConvertBack(value, targetType, parameter, culture);
      return value;
    }
  }

  public class LinearColorChangeConverter : IValueConverter
  {
    public SolidColorBrush ColorFrom { get; set; }
    public SolidColorBrush ColorTo { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double v = (double)value;

      byte GetColor(byte src, byte dst) => (byte)(dst - (dst - src) / v);
      Color src = ColorFrom.Color;
      Color dst = ColorTo.Color;

      return v switch
      {
        <= 0 => ColorFrom,
        >= 1 => ColorTo,
        _ => new SolidColorBrush(Color.FromArgb(
          GetColor(src.A, dst.A),
          GetColor(src.R, dst.R),
          GetColor(src.G, dst.G),
          GetColor(src.B, dst.B)))
      };
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }

  public class MultiDoubleValueMultiplConverter : IMultiValueConverter
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

  public class HasFlagConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value as Enum)?.HasFlag((Enum)parameter) ?? false;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
