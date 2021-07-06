using System.Globalization;
using System.Windows.Controls;

namespace ShiftManager
{
  public class HHMMValidationRule : ValidationRule
  {
    /*******************************************
* specification ;
* name = Validate ;
* Function = 入力値のバリデーションを行います  HH:MMの形式が期待されます ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 入力値, 地域情報 ;
* output = Validate結果 ;
* end of specification ;
*******************************************/
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      var result = HHMMStyleTextValidatior.Validate(value, cultureInfo);
      return new(result.Result, result.Message);
    }
  }
}
