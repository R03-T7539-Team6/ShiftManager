using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class ShiftEditorControl : Control
  {
    static ShiftEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorControl), new FrameworkPropertyMetadata(typeof(ShiftEditorControl)));

  }
}
