using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class PasswordStrengthVisualizerControl : Control
  {
    public string PasswordText { get => (string)GetValue(PasswordTextProperty); set => SetValue(PasswordTextProperty, value); }
    static public DependencyProperty PasswordTextProperty = DependencyProperty.Register(nameof(PasswordText), typeof(string), typeof(PasswordStrengthVisualizerControl), new(PWChanged));

    public double PasswordStrength { get => (double)GetValue(PasswordStrengthProperty); protected set => SetValue(PasswordStrengthPropertyKey, value); }
    static private DependencyPropertyKey PasswordStrengthPropertyKey = DependencyProperty.RegisterReadOnly(nameof(PasswordStrength), typeof(double), typeof(UserSettingControl), new());
    static public DependencyProperty PasswordStrengthProperty = PasswordStrengthPropertyKey.DependencyProperty;

    static PropertyChangedCallback PWChanged { get; } = (d, e) =>
    {

    };

    static PasswordStrengthVisualizerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordStrengthVisualizerControl), new FrameworkPropertyMetadata(typeof(PasswordStrengthVisualizerControl)));
    
  }
}
