using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShiftManager.Controls
{
  public class ShiftEditorBarControl : Control
  {
    // public double Scale { get; set; } = 1.0; //Scale設定は, 親側でしか変更しないため親側で行う
    static ShiftEditorBarControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorBarControl), new FrameworkPropertyMetadata(typeof(ShiftEditorBarControl)));

    private const double BASE_WIDTH = 32;

    /// <summary>1時間ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_1H_MAX = BASE_WIDTH * 1 * 24; //これ以上は30分間隔 これ未満なら1H間隔

    /// <summary>30分ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_30MIN_MAX = CELLWIDTH_1H_MAX * 1.5; //これ以上は15分間隔
    /// <summary>15分ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_15MIN_MAX = CELLWIDTH_30MIN_MAX * 2; //これ以上は5分間隔
    /// <summary>5分ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_05MIN_MAX = CELLWIDTH_15MIN_MAX * 3; //これ以上は1分間隔

    /// <summary>2時間ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_2H_MAX = CELLWIDTH_1H_MAX * 0.5; //これ以上は1時間間隔 これ未満なら2H間隔

    public enum CellMode
    {
      None,
      Minute_01,
      Minute_05,
      Minute_15,
      Minute_30,
      Hour_1,
      Hour_2,
    }

    private static CellMode GetCellMode(in Size newSize) => GetCellMode(newSize.Width);
    private static CellMode GetCellMode(in double newWidth) =>
      newWidth < CELLWIDTH_2H_MAX ? CellMode.Hour_2 :
      newWidth < CELLWIDTH_1H_MAX ? CellMode.Hour_1 :
      newWidth < CELLWIDTH_30MIN_MAX ? CellMode.Minute_30 :
      newWidth < CELLWIDTH_15MIN_MAX ? CellMode.Minute_15 :
      newWidth < CELLWIDTH_05MIN_MAX ? CellMode.Minute_05 :
      CellMode.Minute_01;

    private int GetCellCount() => GetCellCount(CurrentCellMode);
    private static int GetCellCount(in CellMode cellMode) => cellMode switch
    {
      CellMode.Hour_2 => 24 / 2,
      CellMode.Hour_1 => 24 / 1,
      CellMode.Minute_30 => 24 * 2,
      CellMode.Minute_15 => 24 * 2 * 2,
      CellMode.Minute_05 => 24 * 2 * 2 * 3,
      CellMode.Minute_01 => 24 * 2 * 2 * 3 * 5,
      _ => throw new ArgumentOutOfRangeException()
    };
    private double GetCellStep() => GetCellStep(CurrentCellMode);
    private static double GetCellStep(in CellMode cellMode) => 1.0 / GetCellCount(cellMode);
    private TimeSpan GetCellMinuteStep() => GetCellMinuteStep(CurrentCellMode);
    private static TimeSpan GetCellMinuteStep(in CellMode cellMode) => cellMode switch
    {
      CellMode.Hour_2 => new(2, 0, 0),
      CellMode.Hour_1 => new(1, 0, 0),
      CellMode.Minute_30 => new(0, 30, 0),
      CellMode.Minute_15 => new(0, 15, 0),
      CellMode.Minute_05 => new(0, 5, 0),
      CellMode.Minute_01 => new(0, 1, 0),
      _ => throw new ArgumentOutOfRangeException()
    };

    public CellMode CurrentCellMode { get; private set; } = CellMode.None;

    public Brush SeparatorBrush { get; set; } = Brushes.Red;
    public Brush WorkTimeBrush { get; set; } = Brushes.Aqua;
    public Brush BreakTimeBrush { get; set; } = Brushes.Lime;

    private Grid TargetGrid;
    
    //private ToolTip TimeTooltip { get; }
    private TextBlock TimeTooltipTB { get; }
    private TimeSpan TimeTooltipTB_LastStart = TimeSpan.MinValue;
    private TimeSpan TimeTooltipTB_LastStep = TimeSpan.MinValue;
    private void TryUpdateTimeTooltipTB(in TimeSpan start, in TimeSpan step)
    {
      if (start == TimeTooltipTB_LastStart && step == TimeTooltipTB_LastStep)
        return;

      TimeTooltipTB_LastStart = start;
      TimeTooltipTB_LastStep = step;

      TimeTooltipTB.Text = $"{start:hh\\:mm}~{start + step:hh\\:mm}";
    }


    IReadOnlyDictionary<int, TextBlock> TimeTBDictionary { get; }

    private const int ZIndex_TimeTextBlock = 3; //大きいほど前に出る
    private const int ZIndex_Separator = 2; //大きいほど前に出る
    private const int ZIndex_BreakTime = 1; //大きいほど前に出る
    private const int ZIndex_WorkTime = 0; //大きいほど前に出る

    public ShiftEditorBarControl()
    {
      #region 時間ラベル用のTextBlock初期化
      Dictionary<int, TextBlock> tbDic = new();
      for (int i = 0; i < 24; i++)
        tbDic.Add(i, new()
        {
          Text = i.ToString(),
          HorizontalAlignment = HorizontalAlignment.Center,
          VerticalAlignment = VerticalAlignment.Center
        });
      TimeTBDictionary = tbDic;

      foreach (var t in TimeTBDictionary.Values)
        Panel.SetZIndex(t, ZIndex_TimeTextBlock);
      #endregion

      #region Tooltip用の初期設定
      TimeTooltipTB = new() { Text = "--:-- ~ --:--", Background = Brushes.White };
      /*TimeTooltip = new()
      {
        Content= TimeTooltipTB,
        IsOpen = false,
        Effect = new DropShadowEffect(),
        Placement = PlacementMode.Mouse,
      };*/
      #endregion
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      SizeChanged += ShiftEditorBarControl_SizeChanged;

      if (Template.FindName(nameof(TargetGrid), this) is Grid grid)
        TargetGrid = grid;

      if(TargetGrid is not null)
      {
        TargetGrid.MouseDown += TargetGrid_MouseDown;
        TargetGrid.MouseMove += TargetGrid_MouseMove;
        //TargetGrid.MouseEnter += (_, _) => TimeTooltip.IsOpen = true;
        //TargetGrid.MouseLeave += (_, _) => TimeTooltip.IsOpen = false;

        TargetGrid.ToolTip = TimeTooltipTB; //TimeTooltip;

        ToolTipService.SetToolTip(TargetGrid, TimeTooltipTB);
        ToolTipService.SetInitialShowDelay(TargetGrid, 200);
        ToolTipService.SetBetweenShowDelay(TargetGrid, 1);
        ToolTipService.SetHasDropShadow(TargetGrid, false);
        ToolTipService.SetShowDuration(TargetGrid, 1);
      }
    }

    private void ShiftEditorBarControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (TargetGrid is null || !e.WidthChanged)
        return;

      var newCellMode = GetCellMode(e.NewSize);

      if (CurrentCellMode == newCellMode) //セルの数が変わらないならセルの再配置も不要
        return;
      CurrentCellMode = newCellMode;

      TargetGrid.Children.Clear();

      //TargetGrid.Children.Add(TimeTooltip);

      int cellCount = GetCellCount();

      int cellSubResult = TargetGrid.ColumnDefinitions.Count - cellCount;

      if (cellSubResult < 0) //現状足りない (24個存在に対して48個必要 => -24)
        for (int i = 0; i < cellSubResult * -1; i++)
          TargetGrid.ColumnDefinitions.Add(new()); //足りない分を追加
      else //現状余分なのがある (48個存在に対して24個必要 => +24)
        TargetGrid.ColumnDefinitions.RemoveRange(0, cellSubResult); //余分な部分を削除

      if (CurrentCellMode == CellMode.Hour_2)
      {
        for(int i = 0; i < TimeTBDictionary.Count / 2; i++)
        {
          Grid.SetColumn(TimeTBDictionary[i * 2], i);
          _ = TargetGrid.Children.Add(TimeTBDictionary[i * 2]);
        }
      }
      else
      {
        int colSpan = cellCount / TimeTBDictionary.Count;
        for (int i = 0; i < TimeTBDictionary.Count; i++)
        {
          Grid.SetColumn(TimeTBDictionary[i], i * colSpan);
          Grid.SetColumnSpan(TimeTBDictionary[i], colSpan);
          _ = TargetGrid.Children.Add(TimeTBDictionary[i]);
        }
      }
      Debug.WriteLine(CurrentCellMode);
    }

    private void TargetGrid_MouseMove(object sender, MouseEventArgs e)
    {
      if (!IsEnabled || sender is not Grid grid)
        return;

      var pos = e.GetPosition(grid);
      double pos_0to1 = pos.X / grid.ActualWidth;
      double cell_step = GetCellStep();

      //Step:0.1でPos=0.2ならCurrentCell=2
      int currentCell = (int)(pos_0to1 / cell_step);
      TimeSpan cellMinuteStep = GetCellMinuteStep();
      TimeSpan currentCellTime = new(0, (int)cellMinuteStep.TotalMinutes * currentCell, 0);

      TryUpdateTimeTooltipTB(currentCellTime, cellMinuteStep);

    }

    private void TargetGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!IsEnabled || sender is not Grid grid)
        return;

    }
  }
}
