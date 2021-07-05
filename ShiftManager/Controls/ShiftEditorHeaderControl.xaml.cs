using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class ShiftEditorHeaderControl : ContentControl
  {
    public ShiftEditorElements VisibleElements { get => (ShiftEditorElements)GetValue(VisibleElementsProperty); set => SetValue(VisibleElementsProperty, value); }
    public static readonly DependencyProperty VisibleElementsProperty = DependencyProperty.Register(nameof(VisibleElements), typeof(ShiftEditorElements), typeof(ShiftEditorHeaderControl));

    static ShiftEditorHeaderControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorHeaderControl), new FrameworkPropertyMetadata(typeof(ShiftEditorHeaderControl)));

  }
}
