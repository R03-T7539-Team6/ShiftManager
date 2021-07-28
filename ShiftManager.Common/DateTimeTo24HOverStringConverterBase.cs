using System;
using System.Globalization;

namespace ShiftManager
{
  public class DateTimeTo24HOverStringConverterBase
  {
    public DateTime BaseDate { get; set; } //受け取ったデータをそのまま保管する=>ConvertBackで返す時のため
    public string LastView { get; private set; }
    public int LastHH { get; private set; } = -1;
    public int LastMM { get; private set; } = -1;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert((DateTime)value);

    /// <summary>DateTimeを24時以降も25時等と表記するタイプのstringに変換します</summary>
    /// <param name="value">表現する時間</param>
    /// <returns>HH:MM形式の文字列</returns>
    public string Convert(in DateTime value)
    {
      LastHH = value.Hour + (int)(value.Date - BaseDate.Date).TotalHours;
      LastMM = value.Minute;

      LastView = $"{LastHH:D2}:{value:mm}";
      return LastView;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ConvertBack(value as string);
    /// <summary>25時以降が存在する時刻表現文字列をDateTimeに変換します</summary>
    /// <param name="value">変換する時刻表現(HH:MM)</param>
    /// <param name="parameter">基準日</param>
    /// <returns>変換結果</returns>
    public DateTime ConvertBack(in string value)
    {
      LastView = value;

      (LastHH, LastMM) = HHMMStrToInt(value);

      return BaseDate.Date + new TimeSpan(LastHH, LastMM, 0);
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


}
