using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    public static readonly DependencyProperty WorkDateProperty = DependencyProperty.Register(nameof(WorkDate), typeof(DateTime), typeof(ShiftEditorControl), new(OnWorkDateChanged));
    public DateTime AttendanceTime { get => (DateTime)GetValue(AttendanceTimeProperty); set => SetValue(AttendanceTimeProperty, value); }
    public static readonly DependencyProperty AttendanceTimeProperty = DependencyProperty.Register(nameof(AttendanceTime), typeof(DateTime), typeof(ShiftEditorControl), new((s, _) => (s as ShiftEditorControl)?.ChangeWorkTimeLen()));
    public DateTime LeavingTime { get => (DateTime)GetValue(LeavingTimeProperty); set => SetValue(LeavingTimeProperty, value); }
    public static readonly DependencyProperty LeavingTimeProperty = DependencyProperty.Register(nameof(LeavingTime), typeof(DateTime), typeof(ShiftEditorControl), new((s, _) => (s as ShiftEditorControl)?.ChangeWorkTimeLen()));
    public TimeSpan WorkTimeLength { get => (TimeSpan)GetValue(WorkTimeLengthProperty); set => SetValue(WorkTimeLengthProperty, value); }
    public static readonly DependencyProperty WorkTimeLengthProperty = DependencyProperty.Register(nameof(WorkTimeLength), typeof(TimeSpan), typeof(ShiftEditorControl), new((s, e) => (s as ShiftEditorControl)?.OnWorkTimeLenChanged((TimeSpan)e.NewValue)));

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

    private BreakTimeEditorControl BreakTimeEditor;
    private TimeSpan TotalBreakTimeLength => BreakTimeEditor?.TotalBreakTimeLength ?? TimeSpan.Zero;

    /*******************************************
* specification ;
* name = ShiftEditorControl ;
* Function = コンストラクタです.  引数なしでインスタンスを生成できるようにするために置いています ;
* note = N/A ;
* date = 06/29/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    public ShiftEditorControl() => SetBindingsProperty();

    /*******************************************
* specification ;
* name = ShiftEditorControl ;
* Function = コンストラクタです.  指定のインスタンスに含まれる情報を用いてインスタンスを初期化します ;
* note = N/A ;
* date = 06/29/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 情報源となるインスタンス ;
* output = N/A ;
* end of specification ;
*******************************************/
    public ShiftEditorControl(ISingleShiftData i)
    {
      SingleShiftData = i;
      SetBindingsProperty();
    }

    public ISingleShiftData SingleShiftData
    {
      get => new SingleShiftData(this);
      set
      {
        var i = value ?? new SingleShiftData(null);
        UserID = i.UserID;

        if (i.WorkDate != default)
          WorkDate = i.WorkDate.Date;

        AttendanceTime = i.AttendanceTime == default ? WorkDate : i.AttendanceTime;
        LeavingTime = i.LeavingTime == default ? WorkDate : i.LeavingTime;

        IsPaidHoliday = i.IsPaidHoliday;

        BreakTimeDictionary = new(i.BreakTimeDictionary);

        int BreakTimeLen = 0;
        foreach (var item in BreakTimeDictionary)
          BreakTimeLen += item.Value;
        WorkTimeLength = LeavingTime - AttendanceTime - TimeSpan.FromMinutes(BreakTimeLen);
      }
    }
    private Binding AttendanceTimeBinding { get; } = new(nameof(AttendanceTime)) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
    private Binding LeavingTimeBinding { get; } = new(nameof(LeavingTime)) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

    private void SetBindingsProperty()
    {
      AttendanceTimeBinding.Source = this;
      AttendanceTimeBinding.Converter = new DateTimeTo24HOverStringConverter() { BaseDate = WorkDate };
      AttendanceTimeBinding.ValidationRules.Add(new HHMMValidationRule());

      LeavingTimeBinding.Source = this;
      LeavingTimeBinding.Converter = new DateTimeTo24HOverStringConverter() { BaseDate = WorkDate };
      LeavingTimeBinding.ValidationRules.Add(new HHMMValidationRule());

    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (Template.FindName("AttendanceTimeTextBox", this) is TextBox tba)
        _ = tba.SetBinding(TextBox.TextProperty, AttendanceTimeBinding);
      if (Template.FindName("LeavingTimeTextBox", this) is TextBox tbl)
        _ = tbl.SetBinding(TextBox.TextProperty, LeavingTimeBinding);
      if (Template.FindName(nameof(BreakTimeEditor), this) is BreakTimeEditorControl btec)
      {
        BreakTimeEditor = btec;
        BreakTimeEditor.BreakTimeLenChanged += (_, _) => ChangeWorkTimeLen();
      }
      if (Template.FindName("BarEditor", this) is ShiftEditorBarControl sebc)
      {
        sebc.BreakTimeDictionaryUpdated += (_, _) => BreakTimeEditor.BreakTimeDictionaryUpdated();
        sebc.BreakTimeDictionaryUpdated += (_, _) => ChangeWorkTimeLen();
        BreakTimeEditor.BreakTimeLenChanged += (_, _) =>
        {
          if (sebc.BreakTimeDictionary == BreakTimeEditor.BreakTimeDictionary)
            sebc.OnTimeValuesChanged(); //同じインスタンスなら値の更新を通知する
          else
            sebc.BreakTimeDictionary = BreakTimeEditor.BreakTimeDictionary; //違うインスタンスならインスタンスを更新する
        };
      }
    }

    static private void OnWorkDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is ShiftEditorControl s)
      {
        if (s.AttendanceTimeBinding.Converter is DateTimeTo24HOverStringConverter c1)
          c1.BaseDate = s.WorkDate.Date;

        if (s.LeavingTimeBinding.Converter is DateTimeTo24HOverStringConverter c2)
          c2.BaseDate = s.WorkDate.Date;
      }
    }

    /*******************************************
* specification ;
* name = BreakTimePopupOpenButtonClicked ;
* Function = 休憩情報表示ポップアップを表示します. ;
* note = 休憩時間表示ボタン押下時に実行されます ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void BreakTimePopupOpenButtonClicked()
    {
      if (BreakTimePopupState) //既に開いているのであれば
        BreakTimePopupState = false; //一旦Falseにする

      BreakTimePopupState //表示 == TRUEにするのは, 
        = VisibleElements.HasFlag(ShiftEditorElements.BreakTime); //休憩時間コントロールが可視状態である場合のみ
    }

    /*******************************************
* specification ;
* name = DeleteButtinClicked ;
* Function = 情報削除ボタン押下時に実行され, シフト情報を初期化します ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void DeleteButtonClicked() => SingleShiftData = new SingleShiftData(null);

    /*******************************************
* specification ;
* name = ChangeWorkTimeLen ;
* Function = 勤務時間長を更新します ;
* note = 退勤時間から出勤時間を引いているだけです ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void ChangeWorkTimeLen() => WorkTimeLength = LeavingTime - AttendanceTime - TotalBreakTimeLength;

    /*******************************************
* specification ;
* name = OnWorkTimeLenChanged ;
* Function = 勤務時間長が更新された際に実行されます.  勤務時間長が負の値になった際に赤字にする機能が含まれます ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void OnWorkTimeLenChanged(TimeSpan newWorkTimeLen)
    {
      if (newWorkTimeLen != (LeavingTime - AttendanceTime - TotalBreakTimeLength))
        LeavingTime = AttendanceTime + newWorkTimeLen - TotalBreakTimeLength;

      WorkTimeLenForeground = newWorkTimeLen < TimeSpan.Zero ? Brushes.Red : Brushes.Black;
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
