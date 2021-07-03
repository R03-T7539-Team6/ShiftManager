using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
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

  public class DateTimeTo24HOverStringConverter : IMultiValueConverter
  {
    public DateTime BaseDate { get; set; } //受け取ったデータをそのまま保管する=>ConvertBackで返す時のため
    public string LastView { get; private set; }
    public int LastHH { get; private set; } = -1;
    public int LastMM { get; private set; } = -1;

    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
      BaseDate = (DateTime)value[1];

      return Convert((DateTime)value[0]);
    }


    /// <summary>DateTimeを24時以降も25時等と表記するタイプのstringに変換します</summary>
    /// <param name="value">表現する時間</param>
    /// <returns>HH:MM形式の文字列</returns>
    public string Convert(in DateTime value)
    {
      int HH = value.Hour + (int)(value.Date - BaseDate.Date).TotalHours;
      int MM = value.Minute;

      if (!string.IsNullOrWhiteSpace(LastView) && LastHH == HH && LastMM == MM) //更新の必要がないなら更新しない
        return LastView;

      LastHH = HH;
      LastMM = MM;

      LastView = $"{HH:D2}:{MM:D2}";
      return LastView;
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) => new object[] { ConvertBack(value as string), BaseDate };
    /// <summary>25時以降が存在する時刻表現文字列をDateTimeに変換します</summary>
    /// <param name="value">変換する時刻表現(HH:MM)</param>
    /// <param name="parameter">基準日</param>
    /// <returns>変換結果</returns>
    public DateTime ConvertBack(in string value)
    {
      LastView = value;

      var v = HHMMStrToInt(value);

      LastHH = v.HH;
      LastMM = v.MM;

      return BaseDate.Date + new TimeSpan(v.HH, v.MM, 0);
    }

    public static (int HH, int MM) HHMMStrToInt(in string value)
    {
      if (value is null)
        throw new ArgumentNullException("string value is null");

      if (!value.Contains(':'))
        throw new FormatException("Time Expression String must have char ':'");

      string[] HHMM = value.Split(':');
      if (HHMM.Length != 2)
        throw new FormatException("Time Expression String must have only one ':'");

      if (HHMM[0].Length <= 0 || HHMM[1].Length <= 0)
        throw new FormatException("Too short TimeExpressionString");

      if (!int.TryParse(HHMM[0], out int HH))
        throw new FormatException("Parse failed (HH)");
      if (HH < 0)
        throw new ArgumentOutOfRangeException("HH < 0");

      if (!int.TryParse(HHMM[1], out int MM))
        throw new FormatException("Parse failed (MM)");
      if (MM < 0)
        throw new ArgumentOutOfRangeException("MM < 0");
      if (MM >= 60)
        throw new ArgumentOutOfRangeException("MM >= 60");

      return (HH, MM);
    }
  }

  public class TimeSpanTo24HOverStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      TimeSpan ts = (TimeSpan)value;
      return $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}";
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => TimeSpan.Parse(value as string);
  }

  public class DoubleValueAddConverter : IValueConverter
  {
    public double? Value { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (double)value + (Value ?? double.Parse(parameter as string));
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
