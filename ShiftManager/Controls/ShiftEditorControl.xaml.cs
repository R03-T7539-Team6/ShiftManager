using System;
using System.Collections.Generic;
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
    public ShiftEditorElements VisibleElements { get => (ShiftEditorElements)GetValue(VisibleElementsProperty); set => SetValue(VisibleElementsProperty, value); }
    public static readonly DependencyProperty VisibleElementsProperty = DependencyProperty.Register(nameof(VisibleElements), typeof(ShiftEditorElements), typeof(ShiftEditorControl));

    public ShiftEditorElements EditableElements { get => (ShiftEditorElements)GetValue(EditableElementsProperty); set => SetValue(EditableElementsProperty, value); }
    public static readonly DependencyProperty EditableElementsProperty = DependencyProperty.Register(nameof(EditableElements), typeof(ShiftEditorElements), typeof(ShiftEditorControl));

    public DateTime WorkDate { get => (DateTime)GetValue(WorkDateProperty); set => SetValue(WorkDateProperty, value); }
    public static readonly DependencyProperty WorkDateProperty = DependencyProperty.Register(nameof(WorkDate), typeof(DateTime), typeof(ShiftEditorControl));
    public DateTime AttendanceTime { get => (DateTime)GetValue(AttendanceTimeProperty); set => SetValue(AttendanceTimeProperty, value); }
    public static readonly DependencyProperty AttendanceTimeProperty = DependencyProperty.Register(nameof(AttendanceTime), typeof(DateTime), typeof(ShiftEditorControl), new((s, _) => (s as ShiftEditorControl)?.ChangeWorkTimeLen()));
    public DateTime LeavingTime { get => (DateTime)GetValue(LeavingTimeProperty); set => SetValue(LeavingTimeProperty, value); }
    public static readonly DependencyProperty LeavingTimeProperty = DependencyProperty.Register(nameof(LeavingTime), typeof(DateTime), typeof(ShiftEditorControl), new((s, _) => (s as ShiftEditorControl)?.ChangeWorkTimeLen()));
    public TimeSpan WorkTimeLength { get => (TimeSpan)GetValue(WorkTimeLengthProperty); set => SetValue(WorkTimeLengthProperty, value); }
    public static readonly DependencyProperty WorkTimeLengthProperty = DependencyProperty.Register(nameof(WorkTimeLength), typeof(TimeSpan), typeof(ShiftEditorControl), new((s, _) => (s as ShiftEditorControl)?.OnWorkTimeLenChanged()));

    public Dictionary<DateTime, int> BreakTimeDictionary { get => (Dictionary<DateTime, int>)GetValue(BreakTimeDictionaryProperty); set => SetValue(BreakTimeDictionaryProperty, value); }
    public static readonly DependencyProperty BreakTimeDictionaryProperty = DependencyProperty.Register(nameof(BreakTimeDictionary), typeof(Dictionary<DateTime, int>), typeof(ShiftEditorControl));

    public Brush AttendingTimeBrush { get => (Brush)GetValue(AttendingTimeBrushProperty); set => SetValue(AttendingTimeBrushProperty, value); }
    public static readonly DependencyProperty AttendingTimeBrushProperty = DependencyProperty.Register(nameof(AttendingTimeBrush), typeof(Brush), typeof(ShiftEditorControl));
    public Brush BreakingTimeBrush { get => (Brush)GetValue(BreakingTimeBrushProperty); set => SetValue(BreakingTimeBrushProperty, value); }
    public static readonly DependencyProperty BreakingTimeBrushProperty = DependencyProperty.Register(nameof(BreakingTimeBrush), typeof(Brush), typeof(ShiftEditorControl));
    public Brush ShiftEditBarBackground { get => (Brush)GetValue(ShiftEditBarBackgroundProperty); set => SetValue(ShiftEditBarBackgroundProperty, value); }
    public static readonly DependencyProperty ShiftEditBarBackgroundProperty = DependencyProperty.Register(nameof(ShiftEditBarBackground), typeof(Brush), typeof(ShiftEditorControl));

    public Brush WorkTimeLenForeground { get => (Brush)GetValue(WorkTimeLenForegroundProperty); private set => SetValue(WorkTimeLenForegroundPropertyKey, value); }
    private static readonly DependencyPropertyKey WorkTimeLenForegroundPropertyKey = DependencyProperty.RegisterReadOnly(nameof(WorkTimeLenForeground), typeof(Brush), typeof(ShiftEditorControl), new(Brushes.Black));
    public static readonly DependencyProperty WorkTimeLenForegroundProperty = WorkTimeLenForegroundPropertyKey.DependencyProperty;

    public double ShiftEditBarScale { get => (double)GetValue(ShiftEditBarScaleProperty); set => SetValue(ShiftEditBarScaleProperty, value); }
    public static readonly DependencyProperty ShiftEditBarScaleProperty = DependencyProperty.Register(nameof(ShiftEditBarScale), typeof(double), typeof(ShiftEditorControl));

    public bool BreakTimePopupState { get => (bool)GetValue(BreakTimePopupStateProperty); set => SetValue(BreakTimePopupStateProperty, value); }
    public static readonly DependencyProperty BreakTimePopupStateProperty = DependencyProperty.Register(nameof(BreakTimePopupState), typeof(bool), typeof(ShiftEditorControl));

    public ISingleShiftData SingleShiftDataSetter { get => (ISingleShiftData)GetValue(SingleShiftDataSetterProperty); set => SetValue(SingleShiftDataSetterProperty, value); }
    public static readonly DependencyProperty SingleShiftDataSetterProperty = DependencyProperty.Register(nameof(SingleShiftDataSetter), typeof(ISingleShiftData), typeof(ShiftEditorControl),
      new((s, e) =>
      {
        if (s is ShiftEditorControl sec)
          sec.SingleShiftData = e.NewValue as SingleShiftData;
      }));

    public IUserID UserID { get; private set; } //使わない
    public bool IsPaidHoliday { get; private set; } //使わない

    #endregion

    public static readonly ICommand BreakTimePopupOpenButtonClickedCommand = new CustomCommand<ShiftEditorControl>(i => i.BreakTimePopupOpenButtonClicked());
    public static readonly ICommand DeleteButtonClickedCommand = new CustomCommand<ShiftEditorControl>(i => i.DeleteButtonClicked());

    static ShiftEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorControl), new FrameworkPropertyMetadata(typeof(ShiftEditorControl)));

    public ShiftEditorControl() { }
    public ShiftEditorControl(ISingleShiftData i) => SingleShiftData = i;
    /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
    public ISingleShiftData SingleShiftData
    {
      get => new SingleShiftData(this);
      set
      {
        var i = value ?? new SingleShiftData(null);
        UserID = value.UserID;
        WorkDate = value.WorkDate;
        IsPaidHoliday = value.IsPaidHoliday;
        AttendanceTime = value.AttendanceTime;
        LeavingTime = value.LeavingTime;
        BreakTimeDictionary = new(value.BreakTimeDictionary);
      }
    }

    /*******************************************
* specification ;
* name = メソッド名 ;
* Function = メソッドの説明 ;
* note = 補足説明 ;
* date = 最終更新(MM/DD/YYYY) ;
* author = 作成者 ;
* History = 更新履歴 ;
* input = 入力 ;
* output = 出力 ;
* end of specification ;
*******************************************/
    private void BreakTimePopupOpenButtonClicked()
    {
      if (BreakTimePopupState) //既に開いているのであれば
        BreakTimePopupState = false; //一旦Falseにする

      BreakTimePopupState //表示 == TRUEにするのは, 
        = VisibleElements.HasFlag(ShiftEditorElements.BreakTime); //休憩時間コントロールが可視状態である場合のみ
    }
    private void DeleteButtonClicked() => SingleShiftData = new SingleShiftData(null);

    private void ChangeWorkTimeLen() => WorkTimeLength = LeavingTime - AttendanceTime;

    private void OnWorkTimeLenChanged()
    {
      if (WorkTimeLength != (LeavingTime - AttendanceTime))
        LeavingTime = AttendanceTime + WorkTimeLength;

      WorkTimeLenForeground = WorkTimeLength < new TimeSpan(0) ? Brushes.Red : Brushes.Black;
    }

    public static readonly ShiftEditorElements FullBitsOfElements = (ShiftEditorElements)0b01111111;
  }

  [Flags]
  public enum ShiftEditorElements
  {
    None = 0,
    /// <summary>テキスト</summary>
    Text = 1 << 0,

    /// <summary>出勤時刻</summary>
    AttendTime = 1 << 1,

    /// <summary>退勤時刻</summary>
    LeaveTime = 1 << 2,

    /// <summary>休憩時分</summary>
    BreakTime = 1 << 3,

    /// <summary>シフト表示UI (バー型シフト視覚化要素)</summary>
    ShiftBar = 1 << 4,

    /// <summary>勤務時間長</summary>
    WorkTimeLen = 1 << 5,

    /// <summary>削除ボタン</summary>
    DeleteButton = 1 << 6
  }
}
