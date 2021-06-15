using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class PasswordStrengthVisualizerControl : Control
  {
    static PasswordStrengthVisualizerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordStrengthVisualizerControl), new FrameworkPropertyMetadata(typeof(PasswordStrengthVisualizerControl)));
    
  }
}
