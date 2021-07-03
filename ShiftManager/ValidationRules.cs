using System.Globalization;
using System.Windows.Controls;

namespace ShiftManager
{
  public class HHMMValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if (value is not string s)
        return new(false, "valueにはstringを使用してください");

      if (string.IsNullOrWhiteSpace(s))
        return new(false, "valueが空か空白のみです");

      if (!s.Contains(':'))
        return new(false, "valueにコロン':'が含まれていません");

      string[] res = s.Split(':');
      if (res.Length != 2)
        return new(false, "コロンが多すぎます");

      if (!int.TryParse(res[0], out int HH))
        return new(false, "'時'情報の変換に失敗しました");
      if (HH < 0)
        return new(false, "'時'情報に負の数は使用できません");

      if (!int.TryParse(res[1], out int MM))
        return new(false, "'分'情報の変換に失敗しました");
      if (MM < 0 || MM >= 60)
        return new(false, "'分'情報に負の数や60以上の数は使用できません");


      return new(true, string.Empty);
    }
  }
}
