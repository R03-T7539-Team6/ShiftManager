using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class MyProgressRing : Control
  {
    static MyProgressRing() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MyProgressRing), new FrameworkPropertyMetadata(typeof(MyProgressRing)));
  }
}
