using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class PasswordStrengthVisualizerControl : Control
  {
    public string PasswordText { get => (string)GetValue(PasswordTextProperty); set => SetValue(PasswordTextProperty, value); }
    public static readonly DependencyProperty PasswordTextProperty = DependencyProperty.Register(nameof(PasswordText), typeof(string), typeof(PasswordStrengthVisualizerControl), new(PWChanged));

    public double PasswordStrength { get => (double)GetValue(PasswordStrengthProperty); protected set => SetValue(PasswordStrengthPropertyKey, value); }
    private static readonly DependencyPropertyKey PasswordStrengthPropertyKey = DependencyProperty.RegisterReadOnly(nameof(PasswordStrength), typeof(double), typeof(PasswordStrengthVisualizerControl), new());
    public static readonly DependencyProperty PasswordStrengthProperty = PasswordStrengthPropertyKey.DependencyProperty;

    /// <summary>パスワードの強度における"良い"の閾値</summary>
    public static double PWStrength_Good { get; } = 0.7;
    /// <summary>パスワードの強度における"最低限"の閾値</summary>
    public static double PWStrength_Least { get; } = 0.4;

    /*******************************************
* specification ;
* name = PWChanged ;
* Function = パスワード入力が更新された際に実行する ;
* note = パスワードの強度の更新が行われる ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 対象のUI要素 ;
* output = N/A ;
* end of specification ;
*******************************************/
    static void PWChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as PasswordStrengthVisualizerControl).PasswordStrength = GetPasswordStrength(e.NewValue as string);

    /*******************************************
* specification ;
* name = PasswordStrengthVisualizerControl ;
* Function = クラスコンストラクタです.  デフォルトスタイル設定を更新します ;
* note = N/A ;
* date = 06/16/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    static PasswordStrengthVisualizerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordStrengthVisualizerControl), new FrameworkPropertyMetadata(typeof(PasswordStrengthVisualizerControl)));

    /*******************************************
* specification ;
* name = GetPasswordStrength;
* Function = パスワード強度を測定します ;
* note = Zxcvbnを使用して強度を測定しています ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 測定対象のパスワード文字列 ;
* output = パスワード強度 (0.0 ~ 1.0) ;
* end of specification ;
*******************************************/
    static public double GetPasswordStrength(in string s) => string.IsNullOrWhiteSpace(s) ? 0.1 : 0.2 + (double)Zxcvbn.Core.EvaluatePassword(s as string).Score / 5;//Scoreの最大値が4のため
  }
}
