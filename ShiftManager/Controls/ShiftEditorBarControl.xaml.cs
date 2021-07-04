using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class ShiftEditorBarControl : Control
  {
    static ShiftEditorBarControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorBarControl), new FrameworkPropertyMetadata(typeof(ShiftEditorBarControl)));

  }
}
