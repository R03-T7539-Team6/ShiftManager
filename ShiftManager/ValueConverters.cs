using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using ShiftManager.DataClasses;

namespace ShiftManager
{
  public class IntEquallyCheckConverter : IntEquallyCheckConverterBase, IValueConverter { }

  public class BooleanReverseConverter : BooleanReverseConverterBase, IValueConverter { }

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
  public class MultiDoubleValueMultiplConverter : MultiDoubleValueMultiplConverterBase, IMultiValueConverter { }

  public class HasFlagConverter : HasFlagConverterBase, IValueConverter { }

  public class DateTimeTo24HOverStringConverter : DateTimeTo24HOverStringConverterBase, IValueConverter { }

  public class TimeSpanTo24HOverStringConverter : TimeSpanTo24HOverStringConverterBase, IValueConverter { }

  public class DoubleValueAddConverter : DoubleValueAddConverterBase, IValueConverter { }

  public class UserIDToNameStringConverter : UserIDToNameStringConverterBase, IMultiValueConverter { }
}
