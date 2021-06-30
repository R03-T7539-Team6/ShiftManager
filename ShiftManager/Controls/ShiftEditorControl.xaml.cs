using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ShiftManager.DataClasses;

namespace ShiftManager.Controls
{
  public class ShiftEditorControl : ContentControl, ISingleShiftData
  {
    #region Properties
    public Elements VisibleElements { get => (Elements)GetValue(VisibleElementsProperty); set => SetValue(VisibleElementsProperty, value); }
    public static readonly DependencyProperty VisibleElementsProperty = DependencyProperty.Register(nameof(VisibleElements), typeof(Elements), typeof(ShiftEditorControl));

    public Elements EditableElements { get => (Elements)GetValue(EditableElementsProperty); set => SetValue(EditableElementsProperty, value); }
    public static readonly DependencyProperty EditableElementsProperty = DependencyProperty.Register(nameof(EditableElements), typeof(Elements), typeof(ShiftEditorControl));

    public DateTime WorkDate { get => (DateTime)GetValue(WorkDateProperty); set => SetValue(WorkDateProperty, value); }
    public static readonly DependencyProperty WorkDateProperty = DependencyProperty.Register(nameof(WorkDate), typeof(DateTime), typeof(ShiftEditorControl));
    public DateTime AttendanceTime { get => (DateTime)GetValue(AttendanceTimeProperty); set => SetValue(AttendanceTimeProperty, value); }
    public static readonly DependencyProperty AttendanceTimeProperty = DependencyProperty.Register(nameof(AttendanceTime), typeof(DateTime), typeof(ShiftEditorControl));
    public DateTime LeavingTime { get => (DateTime)GetValue(LeavingTimeProperty); set => SetValue(LeavingTimeProperty, value); }
    public static readonly DependencyProperty LeavingTimeProperty = DependencyProperty.Register(nameof(LeavingTime), typeof(DateTime), typeof(ShiftEditorControl));

    public Dictionary<DateTime, int> BreakTimeDictionary { get => (Dictionary<DateTime, int>)GetValue(BreakTimeDictionaryProperty); set => SetValue(BreakTimeDictionaryProperty, value); }
    public static readonly DependencyProperty BreakTimeDictionaryProperty = DependencyProperty.Register(nameof(BreakTimeDictionary), typeof(Dictionary<DateTime, int>), typeof(ShiftEditorControl));

    public Brush AttendingTimeBrush { get => (Brush)GetValue(AttendingTimeBrushProperty); set => SetValue(AttendingTimeBrushProperty, value); }
    public static readonly DependencyProperty AttendingTimeBrushProperty = DependencyProperty.Register(nameof(AttendingTimeBrush), typeof(Brush), typeof(ShiftEditorControl));
    public Brush BreakingTimeBrush { get => (Brush)GetValue(BreakingTimeBrushProperty); set => SetValue(BreakingTimeBrushProperty, value); }
    public static readonly DependencyProperty BreakingTimeBrushProperty = DependencyProperty.Register(nameof(BreakingTimeBrush), typeof(Brush), typeof(ShiftEditorControl));
    public Brush ShiftEditBarBackground { get => (Brush)GetValue(ShiftEditBarBackgroundProperty); set => SetValue(ShiftEditBarBackgroundProperty, value); }
    public static readonly DependencyProperty ShiftEditBarBackgroundProperty = DependencyProperty.Register(nameof(ShiftEditBarBackground), typeof(Brush), typeof(ShiftEditorControl));

    public double ShiftEditBarScale { get => (double)GetValue(ShiftEditBarScaleProperty); set => SetValue(ShiftEditBarScaleProperty, value); }
    public static readonly DependencyProperty ShiftEditBarScaleProperty = DependencyProperty.Register(nameof(ShiftEditBarScale), typeof(double), typeof(ShiftEditorControl));

    public bool BreakTimePopupState { get => (bool)GetValue(BreakTimePopupStateProperty); set => SetValue(BreakTimePopupStateProperty, value); }
    public static readonly DependencyProperty BreakTimePopupStateProperty = DependencyProperty.Register(nameof(BreakTimePopupState), typeof(bool), typeof(ShiftEditorControl));

    public IUserID UserID { get; private set; }
    public bool IsPaidHoliday { get; private set; }

    #endregion

    public static readonly ICommand BreakTimePopupOpenButtonClickedCommand = new CustomCommand<ShiftEditorControl>(i => i.BreakTimePopupOpenButtonClicked());


    static ShiftEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorControl), new FrameworkPropertyMetadata(typeof(ShiftEditorControl)));

    public ShiftEditorControl() { }
    public ShiftEditorControl(ISingleShiftData i) => SingleShiftData = i;

    public ISingleShiftData SingleShiftData
    {
      get => new SingleShiftData(this);
      set
      {
        UserID = value.UserID;
        WorkDate = value.WorkDate;
        IsPaidHoliday = value.IsPaidHoliday;
        AttendanceTime = value.AttendanceTime;
        LeavingTime = value.LeavingTime;
        BreakTimeDictionary = new(value.BreakTimeDictionary);
      }
    }

    private void BreakTimePopupOpenButtonClicked() => BreakTimePopupState //表示 == TRUEにするのは, 
      = VisibleElements.HasFlag(Elements.BraakTime); //休憩時間コントロールが可視状態である場合のみ

    public static readonly Elements FullBitsOfElements = (Elements)0b01111111;

    [Flags]
    public enum Elements
    {
      None = 0,
      /// <summary>テキスト</summary>
      Text = 1 << 1,

      /// <summary>出勤時刻</summary>
      AttendTime = 1 << 2,

      /// <summary>退勤時刻</summary>
      LeaveTime = 1 << 3,

      /// <summary>休憩時分</summary>
      BraakTime = 1 << 4,

      /// <summary>シフト表示UI (バー型シフト視覚化要素)</summary>
      ShiftBar = 1 << 5,

      /// <summary>勤務時間長</summary>
      WorkTimeLen = 1 << 6,

      /// <summary>削除ボタン</summary>
      DeleteButton = 1 << 7
    }
  }
}
