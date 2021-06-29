using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShiftManager.Controls
{
  public class BreakTimeEditorControl : Control
  {

    static BreakTimeEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(BreakTimeEditorControl), new FrameworkPropertyMetadata(typeof(BreakTimeEditorControl)));
    
  }
}
