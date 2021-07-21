using System;
using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class Over24HTimeEditorControl : TextBox
  {
    public DateTime TargetDate { get => (DateTime)GetValue(TargetDateProperty); set => SetValue(TargetDateProperty, value); }
    public static readonly DependencyProperty TargetDateProperty = DependencyProperty.Register(nameof(TargetDate), typeof(DateTime), typeof(Over24HTimeEditorControl), new FrameworkPropertyMetadata(DateTime.UnixEpoch, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTargetDateChanged));

    public DateTime CurrentDateTime { get => (DateTime)GetValue(CurrentDateTimeProperty); set => SetValue(CurrentDateTimeProperty, value); }
    public static readonly DependencyProperty CurrentDateTimeProperty = DependencyProperty.Register(nameof(CurrentDateTime), typeof(DateTime), typeof(Over24HTimeEditorControl), new FrameworkPropertyMetadata(DateTime.UnixEpoch, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentDateTimeChanged));

    public TimeSpan CurrentTimeSpan { get; private set; }

    public bool IsValidValue { get; private set; }

    static Over24HTimeEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Over24HTimeEditorControl), new FrameworkPropertyMetadata(typeof(Over24HTimeEditorControl)));

    static void OnTargetDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is not Over24HTimeEditorControl c)
        return;
      if (c.CurrentDateTime == (DateTime)CurrentDateTimeProperty.DefaultMetadata.DefaultValue)
        return; //初期値設定前は実行しない

      DateTime oldDate = (DateTime)e.OldValue;
      TimeSpan ts = c.CurrentDateTime - oldDate;
      DateTime newDate = (DateTime)e.NewValue;
      c.CurrentDateTime = newDate + ts;
    }

    static void OnCurrentDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is not Over24HTimeEditorControl c)
        return;

      TimeSpan ts = (DateTime)e.NewValue - c.TargetDate;

      if (ts == c.CurrentTimeSpan)
        return;

      c.Text = $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}";
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
      base.OnTextChanged(e);
      IsValidValue = false;

      if (string.IsNullOrWhiteSpace(Text))
        return;

      var sarr = Text.Split(':');

      if (sarr.Length != 2 || !int.TryParse(sarr[0], out var hh) || !int.TryParse(sarr[1], out var mm))
        return;

      TimeSpan ts = new(hh, mm, 0);

      IsValidValue = true;

      if (ts == CurrentTimeSpan)
        return;

      CurrentTimeSpan = ts;
      CurrentDateTime = TargetDate + CurrentTimeSpan;
    }
  }
}
