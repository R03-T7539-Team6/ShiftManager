using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace ShiftManager.Controls
{
  public class ShiftEditorBarControl : Control
  {
    public DateTime TargetDate { get => (DateTime)GetValue(TargetDateProperty); set => SetValue(TargetDateProperty, value); }
    public static readonly DependencyProperty TargetDateProperty = DependencyProperty.Register(nameof(TargetDate), typeof(DateTime), typeof(ShiftEditorBarControl));
    public Brush SeparatorBrush { get => (Brush)GetValue(SeparatorBrushProperty); set => SetValue(SeparatorBrushProperty, value); }
    public static readonly DependencyProperty SeparatorBrushProperty = DependencyProperty.Register(nameof(SeparatorBrush), typeof(Brush), typeof(ShiftEditorBarControl));
    public Brush WorkTimeBrush { get => (Brush)GetValue(WorkTimeBrushProperty); set => SetValue(WorkTimeBrushProperty, value); }
    public static readonly DependencyProperty WorkTimeBrushProperty = DependencyProperty.Register(nameof(WorkTimeBrush), typeof(Brush), typeof(ShiftEditorBarControl));
    public Brush BreakTimeBrush { get => (Brush)GetValue(BreakTimeBrushProperty); set => SetValue(BreakTimeBrushProperty, value); }
    public static readonly DependencyProperty BreakTimeBrushProperty = DependencyProperty.Register(nameof(BreakTimeBrush), typeof(Brush), typeof(ShiftEditorBarControl));

    public DateTime StartTime { get => (DateTime)GetValue(StartTimeProperty); set => SetValue(StartTimeProperty, value); }
    public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register(nameof(StartTime), typeof(DateTime), typeof(ShiftEditorBarControl),
      new((i, _) =>
      {
        if (i is not ShiftEditorBarControl c)
          return;

        if (c.StartTime != c.LocalStartTime)
          c.OnTimeValuesChanged();
      }));
    private DateTime LocalStartTime;

    public DateTime EndTime { get => (DateTime)GetValue(EndTimeProperty); set => SetValue(EndTimeProperty, value); }
    public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register(nameof(EndTime), typeof(DateTime), typeof(ShiftEditorBarControl),
      new((i, _) =>
      {
        if (i is not ShiftEditorBarControl c)
          return;

        if (c.EndTime != c.LocalEndTime)
          c.OnTimeValuesChanged();
      }));
    private DateTime LocalEndTime;

    public Dictionary<DateTime, int> BreakTimeDictionary { get => (Dictionary<DateTime, int>)GetValue(BreakTimeDictionaryProperty); set => SetValue(BreakTimeDictionaryProperty, value); }
    public static readonly DependencyProperty BreakTimeDictionaryProperty = DependencyProperty.Register(nameof(BreakTimeDictionary), typeof(Dictionary<DateTime, int>), typeof(ShiftEditorBarControl),
      new(new Dictionary<DateTime, int>(), (i, _) =>
      {
        if (i is not ShiftEditorBarControl c || !c.IsInitialized)
          return;

        c.BreakTimeDictionary ??= new(); //NULLは不可

        if (!c.BreakTimeDictionary.SequenceEqual(c.LocalBreakTimeDicRec))
          c.OnTimeValuesChanged();
      }));
    private Dictionary<DateTime, int> _LocalBreakTimeDicRec = new();
    private Dictionary<DateTime, int> LocalBreakTimeDicRec { get => _LocalBreakTimeDicRec; set => _LocalBreakTimeDicRec = new(value); }

    private void OnTimeValuesChanged()
    {
      if (BreakTimeDictionary is null)
        return;

      foreach (var i in WorkTimes.Values)
      {
        if (i.Center is not null)
          TargetGrid.Children.Remove(i.Center);
        if (i.Left is not null)
          TargetGrid.Children.Remove(i.Left);
        if (i.Right is not null)
          TargetGrid.Children.Remove(i.Right);
      }

      WorkTimes.Clear();

      if (StartTime == EndTime)
        return;

      // 長さゼロの休憩は除外する
      var rests = BreakTimeDictionary.Where(i => i.Value <= 0).ToArray();
      foreach (var i in rests)
        BreakTimeDictionary.Remove(i.Key);

      if (BreakTimeDictionary.Count <= 0)
      {
        WorkTimes.Add(new(StartTime - TargetDate, EndTime - TargetDate), new(null, null, null));
      }
      else
      {
        var SortedBreakTimeDic = BreakTimeDictionary.OrderBy(v => v.Key);

        TimeSpan From = StartTime - TargetDate;
        TimeSpan To = SortedBreakTimeDic.FirstOrDefault().Key - TargetDate;
        WorkTimes.Add(new(From, To), new(null, null, null));

        for (int i = 0; i < BreakTimeDictionary.Count - 1; i++)
        {
          var tmp = SortedBreakTimeDic.ElementAt(i + 1).Key - TargetDate;

          if (tmp == TimeSpan.Zero)
            continue;

          WorkTimes.Add(new(To.Add(new(0, SortedBreakTimeDic.ElementAt(i).Value, 0)), tmp), new(null, null, null));
          To = tmp;
        }

        WorkTimes.Add(new(To.Add(new(0, SortedBreakTimeDic.Last().Value, 0)), EndTime - TargetDate), new(null, null, null));
      }

      LocalStartTime = StartTime;
      LocalEndTime = EndTime;
      LocalBreakTimeDicRec = BreakTimeDictionary;

      if (TargetGrid is null)
        return;

      foreach (var i in FittingWorkTimeRectangles())
        TargetGrid.Children.Add(i);
    }

    static ShiftEditorBarControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ShiftEditorBarControl), new FrameworkPropertyMetadata(typeof(ShiftEditorBarControl)));

    #region Cell Settings
    private const double BASE_WIDTH = 32;

    /// <summary>1時間ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_1H_MAX = BASE_WIDTH * 1 * 24; //これ以上は30分間隔 これ未満なら1H間隔

    /// <summary>30分ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_30MIN_MAX = CELLWIDTH_1H_MAX * 1.5; //これ以上は15分間隔
    /// <summary>15分ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_15MIN_MAX = CELLWIDTH_30MIN_MAX * 2; //これ以上は10分間隔
    /// <summary>10分ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_10MIN_MAX = CELLWIDTH_30MIN_MAX * 3; //これ以上は5分間隔
    /// <summary>5分ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_05MIN_MAX = CELLWIDTH_10MIN_MAX * 2; //これ以上は1分間隔

    /// <summary>2時間ごとにセルを表示する最大値 [px]</summary>
    private const double CELLWIDTH_2H_MAX = CELLWIDTH_1H_MAX * 0.5; //これ以上は1時間間隔 これ未満なら2H間隔

    public enum CellMode
    {
      Hour01,
      Hour02,
      Minute01,
      Minute05,
      Minute10,
      Minute15,
      Minute30,
    }

    private static CellMode GetCellMode(in Size newSize) => GetCellMode(newSize.Width);
    private static CellMode GetCellMode(in double newWidth) =>
      newWidth < CELLWIDTH_2H_MAX ? CellMode.Hour02 :
      newWidth < CELLWIDTH_1H_MAX ? CellMode.Hour01 :
      newWidth < CELLWIDTH_30MIN_MAX ? CellMode.Minute30 :
      newWidth < CELLWIDTH_15MIN_MAX ? CellMode.Minute15 :
      newWidth < CELLWIDTH_10MIN_MAX ? CellMode.Minute10 :
      newWidth < CELLWIDTH_05MIN_MAX ? CellMode.Minute05 :
      CellMode.Minute01;

    private int GetCellCount() => GetCellCount(CurrentCellMode);
    private static int GetCellCount(in CellMode cellMode) => cellMode switch
    {
      CellMode.Hour02 => 24 / 2,
      CellMode.Hour01 => 24 / 1,
      CellMode.Minute30 => 24 * 2,
      CellMode.Minute10 => 24 * 6,
      CellMode.Minute15 => 24 * 4,
      CellMode.Minute05 => 24 * 12,
      CellMode.Minute01 => 24 * 60,
      _ => throw new ArgumentOutOfRangeException()
    };
    private double GetCellStep() => GetCellStep(CurrentCellMode);
    private static double GetCellStep(in CellMode cellMode) => 1.0 / GetCellCount(cellMode);
    private TimeSpan GetCellMinuteStep() => GetCellMinuteStep(CurrentCellMode);
    private static TimeSpan GetCellMinuteStep(in CellMode cellMode) => cellMode switch
    {
      CellMode.Hour02 => new(2, 0, 0),
      CellMode.Hour01 => new(1, 0, 0),
      CellMode.Minute30 => new(0, 30, 0),
      CellMode.Minute15 => new(0, 15, 0),
      CellMode.Minute10 => new(0, 10, 0),
      CellMode.Minute05 => new(0, 5, 0),
      CellMode.Minute01 => new(0, 1, 0),
      _ => throw new ArgumentOutOfRangeException()
    };

    public CellMode CurrentCellMode { get; private set; } = CellMode.Hour01;
    #endregion

    private Grid TargetGrid { get; set; }

    record TimeSpanFromTo(TimeSpan From, TimeSpan To);
    record TripleRectangle(Grid Left, Grid Center, Grid Right);
    SortedDictionary<TimeSpanFromTo, TripleRectangle> WorkTimes { get; } = new(new TimeSpanFromToComparer());

    public event EventHandler BreakTimeDictionaryUpdated;

    private void OnWorkTimesUpdate()
    {
      if (!IsInitialized)
        return;
      LocalStartTime = TargetDate.Date.AddMinutes(WorkTimes.FirstOrDefault().Key.From.TotalMinutes);
      StartTime = LocalStartTime;

      LocalEndTime = TargetDate.Date.AddMinutes(WorkTimes.LastOrDefault().Key.To.TotalMinutes);
      EndTime = LocalEndTime;

      if (BreakTimeDictionary is null)
        return;

      BreakTimeDictionary.Clear();

      if (WorkTimes.Count > 1)
        for (int i = 0; i < WorkTimes.Count - 1; i++)
        {
          var key1 = WorkTimes.ElementAt(i).Key;
          var key2 = WorkTimes.ElementAt(i + 1).Key;
          DateTime dt = TargetDate.Date.AddMinutes(key1.To.TotalMinutes);
          BreakTimeDictionary.Add(dt, (int)(key2.From - key1.To).TotalMinutes);
        }

      LocalBreakTimeDicRec = BreakTimeDictionary;
      BreakTimeDictionaryUpdated?.Invoke(this, EventArgs.Empty);
    }

    class TimeSpanFromToComparer : IComparer<TimeSpanFromTo>
    {
      public int Compare(TimeSpanFromTo x, TimeSpanFromTo y) => TimeSpan.Compare(x.From, y.From);
    }

    #region Tooltip
    private Popup TimeTooltip { get; }
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
    #endregion

    IReadOnlyDictionary<int, Grid> TimeTBDictionary { get; }

    private const int ZIndex_TimeTextBlock = 3; //大きいほど前に出る
    private const int ZIndex_Separator = 2; //大きいほど前に出る
    private const int ZIndex_BreakTime = 1; //大きいほど前に出る
    private const int ZIndex_WorkTime = 0; //大きいほど前に出る

    private const double SEPARATOR_WIDTH_BOLD = 4;
    private const double SEPARATOR_WIDTH_NORMAL = 2;
    private const double SEPARATOR_WIDTH_LIGHT = 1;

    public ShiftEditorBarControl()
    {
      #region 時間ラベル用のTextBlock初期化
      Dictionary<int, Grid> tbDic = new();
      for (int i = 0; i < 24; i++)
      {

        tbDic.Add(i, new()
        {
          Children =
          {
            new TextBlock()
            {
              Text = i.ToString(),
              FontWeight = FontWeights.Bold,
              Foreground = Brushes.White,
              Effect = new BlurEffect()
              {
                Radius = 1,
                KernelType = KernelType.Box
              }
            },
            new TextBlock()
            {
              Text = i.ToString(),
              FontWeight = FontWeights.Bold,
            },
          },
          HorizontalAlignment = HorizontalAlignment.Center,
          VerticalAlignment = VerticalAlignment.Center,
        });
      }
      TimeTBDictionary = tbDic;

      foreach (var t in TimeTBDictionary.Values)
        Panel.SetZIndex(t, ZIndex_TimeTextBlock);
      #endregion

      #region Tooltip用の初期設定
      TimeTooltipTB = new()
      {
        Text = "--:-- ~ --:--",
      };
      TimeTooltip = new()
      {
        Child = new Border()
        {
          Child = TimeTooltipTB,
          BorderBrush = Brushes.Black,
          BorderThickness = new(1),
          Padding = new(3),
          Background = Brushes.White,
          CornerRadius = new(2),
          Margin = new(0, 0, 16, 16), //影用
          Effect = new DropShadowEffect()
          {
            BlurRadius = 8,
            ShadowDepth = 1,
          },
        },
        IsOpen = false,
        Placement = PlacementMode.Absolute,
        AllowsTransparency = true
      };
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
        TargetGrid.MouseEnter += (_, _) => TimeTooltip.IsOpen = true;
        TargetGrid.MouseLeave += (_, _) => TimeTooltip.IsOpen = false;
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

      TargetGrid.Children.Clear();////////////////////////////////////////////

      TargetGrid.Children.Add(TimeTooltip);

      int cellCount = GetCellCount();

      int cellSubResult = TargetGrid.ColumnDefinitions.Count - cellCount;

      if (cellSubResult < 0) //現状足りない (24個存在に対して48個必要 => -24)
        for (int i = 0; i < cellSubResult * -1; i++)
          TargetGrid.ColumnDefinitions.Add(new()); //足りない分を追加
      else //現状余分なのがある (48個存在に対して24個必要 => +24)
        TargetGrid.ColumnDefinitions.RemoveRange(0, cellSubResult); //余分な部分を削除

      if (CurrentCellMode == CellMode.Hour02)
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

      ReputSeparator();

      _ = FittingWorkTimeRectangles();

      foreach (var i in WorkTimes.Values) //WorkTimeの四角形を配置する
      {
        if (i.Left is not null)
          _ = TargetGrid.Children.Add(i.Left);
        if (i.Center is not null)
          _ = TargetGrid.Children.Add(i.Center);
        if (i.Right is not null)
          _ = TargetGrid.Children.Add(i.Right);
      }
    }

    int lastCell = -1;
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

      var screenPoint = grid.PointToScreen(e.GetPosition(grid));
      TimeTooltip.HorizontalOffset = 10 + screenPoint.X;
      TimeTooltip.VerticalOffset = 10 + screenPoint.Y;

      if (!IsEnabled || e.LeftButton != MouseButtonState.Pressed)
        return;

      if (currentCell == lastCell)
        return;

      _ = TurnWorkTime(currentCellTime, cellMinuteStep);

      OptimizeWorkTimes();

      foreach (var i in FittingWorkTimeRectangles())
        _ = TargetGrid.Children.Add(i);

      lastCell = currentCell;
    }

    private void TargetGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!IsEnabled || sender is not Grid grid)
        return;
      // 押されたところが「勤務」設定である => 「勤務」設定解除
      // 押されたところが「勤務」設定ではない=>「勤務」設定を行う

      double pos_0to1 = e.GetPosition(grid).X / grid.ActualWidth;
      double cell_step = GetCellStep();

      //Step:0.1でPos=0.2ならCurrentCell=2
      int currentCell = (int)(pos_0to1 / cell_step);
      TimeSpan cellMinuteStep = GetCellMinuteStep();
      TimeSpan currentCellTime = new(0, (int)cellMinuteStep.TotalMinutes * currentCell, 0);

      _ = TurnWorkTime(currentCellTime, cellMinuteStep);

      OptimizeWorkTimes();

      foreach (var i in FittingWorkTimeRectangles())
        _ = TargetGrid.Children.Add(i);

      lastCell = currentCell;
    }

    /// <summary></summary>
    /// <param name="From"></param>
    /// <param name="Step"></param>
    /// <returns>再配置が必要かどうか</returns>
    private bool TurnWorkTime(in TimeSpan From, in TimeSpan Step)
    {
      TimeSpan To = From + Step;
      List<TimeSpanFromTo> KeysToRemove = new();

      foreach (var i in WorkTimes) //ソート済み
      {
        if (i.Key.To <= From) //反転させる場所がまだ含まれていない
          continue;
        else if (i.Key.From >= To) //反転させる場所がもう含まれることはない
          break;
        else if (From <= i.Key.From && i.Key.To <= To) //完全一致 OR 内側にWorkTimeが存在する => 該当KeyValuePairを除去
        {
          if (i.Value.Left is not null)
            TargetGrid.Children.Remove(i.Value.Left);
          if (i.Value.Center is not null)
            TargetGrid.Children.Remove(i.Value.Center);
          if (i.Value.Right is not null)
            TargetGrid.Children.Remove(i.Value.Right);

          if(From == i.Key.From && i.Key.To == To) //完全一致ならここで終了
          {
            WorkTimes.Remove(i.Key);
            return false;
          }

          KeysToRemove.Add(i.Key);

          continue; //最終的にはセルを塗りつぶすけど, 他にも内側に存在するかもしれないから再探索
        }
        else if (i.Key.From == From && i.Key.To > To) //始端一致 かつ WorkTime終端がTargetCellよりも右にある : 一部除去 (短縮: 始端を右にずらす)
          return ReplaceWorkTimesKey(i.Key, i.Key with { From = To });
        else if (i.Key.To == To && i.Key.From < From) //終端一致 かつ WorkTime始端がTargetCellよりも左にある : 一部除去 (短縮: 終端を左にずらす)
          return ReplaceWorkTimesKey(i.Key, i.Key with { To = From });
        else if(i.Key.From < From && To < i.Key.To) //該当セルとその両隣がWorkTime => 中抜き
        {
          ReplaceWorkTimesKey(i.Key, i.Key with { To = From }); //左側
          WorkTimes.Add(new(To, i.Key.To), new(null, null, null)); //右側

          return true;
        }
      }

      foreach (var i in KeysToRemove) //空リストでも大丈夫
        WorkTimes.Remove(i);

      WorkTimes.Add(new(From, To), new(null, null, null));

      return true;
    }
    private bool ReplaceWorkTimesKey(in TimeSpanFromTo Old, in TimeSpanFromTo New)
    {
      if (WorkTimes.TryGetValue(Old, out var value))
        WorkTimes.Remove(Old);

      WorkTimes.Add(New, value);

      return true;
    }

    private void OptimizeWorkTimes()
    {
      if (WorkTimes.Count > 1) //2つ以上存在する場合のみ結合チェックを行う
        for (int i = 1; i < WorkTimes.Count; i++)
        {
          var currentKey = WorkTimes.Keys.ElementAt(i);
          var prevKey = WorkTimes.Keys.ElementAt(i - 1);
          if (currentKey.From <= prevKey.To) // Prev:1400, From:1300など
          {
            var rects = WorkTimes[prevKey];
            var rects2 = WorkTimes[currentKey];

            TargetGrid.Children.Remove(rects.Right);
            TargetGrid.Children.Remove(rects2.Left);
            TargetGrid.Children.Remove(rects2.Center);

            WorkTimes.Remove(currentKey);
            WorkTimes.Remove(prevKey);

            WorkTimes.Add(prevKey with { To = currentKey.To }, rects with { Right = rects2.Right });

            i--; //再度確認する必要があるため
          }
        }

      OnWorkTimesUpdate();
    }

    private IReadOnlyList<UIElement> FittingWorkTimeRectangles()
    {
      TimeSpan cellMinuteStep = GetCellMinuteStep();
      List<UIElement> addList = new();
      Dictionary<TimeSpanFromTo, TripleRectangle> TmpDic = new();
      foreach(var i in WorkTimes)
      {
        var (leftGrid, centerGrid, rightGrid) = i.Value;

        var workLen = i.Key.To - i.Key.From;

        if (workLen < TimeSpan.Zero)
          continue;

        int leftCol = (int)(i.Key.From.TotalMinutes / cellMinuteStep.TotalMinutes); //端数は切り捨て
        int rightCol = (int)(i.Key.To.TotalMinutes / cellMinuteStep.TotalMinutes); //端数は切り捨て

        double leftFraction = i.Key.From.TotalMinutes % cellMinuteStep.TotalMinutes; //左のSeparatorから四角形左端までの時間
        double rightFraction = i.Key.To.TotalMinutes % cellMinuteStep.TotalMinutes; //左のSeparatorから四角形右端までの時間

        bool NeededLeftGrid = leftFraction != 0;
        bool NeededRightGrid = rightFraction != 0;
        bool IsSameCell = leftCol == rightCol || (NeededLeftGrid && !NeededRightGrid && rightCol == leftCol + 1); //右端に端数はないが, 左に1セル未満の勤務が存在する場合も含める

        if ((!NeededLeftGrid && !NeededRightGrid) || IsSameCell) //端数が必要ない OR 同じセル内で完結する
        {
          if(centerGrid is null)
          {
            Rectangle rect = new() { DataContext = this };
            rect.SetBinding(Shape.FillProperty, nameof(WorkTimeBrush));
            Grid.SetColumn(rect, 1);

            centerGrid = new()
            {
              ColumnDefinitions = { new(), new(), new() },
              Children = { rect }
            };

            Panel.SetZIndex(centerGrid, ZIndex_WorkTime);

            addList.Add(centerGrid);
          }

          centerGrid.ColumnDefinitions[0].Width = IsSameCell ? new(leftFraction, GridUnitType.Star) : new(0);
          centerGrid.ColumnDefinitions[1].Width = new(workLen.TotalMinutes, GridUnitType.Star);
          centerGrid.ColumnDefinitions[2].Width = IsSameCell ? new(cellMinuteStep.TotalMinutes - rightFraction, GridUnitType.Star) : new(0);

          if (leftGrid is not null)
          {
            TargetGrid.Children.Remove(leftGrid);
            leftGrid = null;
          }
          if(rightGrid is not null)
          {
            TargetGrid.Children.Remove(rightGrid);
            rightGrid = null;
          }

          Grid.SetColumn(centerGrid, leftCol);
          Grid.SetColumnSpan(centerGrid, Math.Max(rightCol - leftCol, 1)); //少なくともSpan=1は確保する
        }
        else //端数専用Gridが必要である
        {
          #region 左の端数Gridの処理
          if (NeededLeftGrid) //左端に端数あり
          {
            if (leftGrid is null) //端数用のGridが準備されていない
            {
              Rectangle rect = new() { DataContext = this };
              rect.SetBinding(Shape.FillProperty, nameof(WorkTimeBrush));

              Grid.SetColumn(rect, 1);

              leftGrid = new()
              {
                ColumnDefinitions = { new(), new() },
                Children = { rect }
              };

              Panel.SetZIndex(leftGrid, ZIndex_WorkTime);

              addList.Add(leftGrid);
            }

            leftGrid.ColumnDefinitions[0].Width = new(leftFraction, GridUnitType.Star);
            leftGrid.ColumnDefinitions[1].Width = new(cellMinuteStep.TotalMinutes - leftFraction, GridUnitType.Star);

            Grid.SetColumn(leftGrid, leftCol);
            Grid.SetColumnSpan(leftGrid, 1);
          }
          else if (leftGrid is not null) //左端に端数がないのに端数用Gridが存在する => 除去する
          {
            TargetGrid.Children.Remove(leftGrid);
            leftGrid = null;
          }
          #endregion

          #region 右の端数Gridの処理
          if (NeededRightGrid) //右端に端数あり
          {
            if (rightGrid is null) //端数用のGridが準備されていない
            {
              Rectangle rect = new() { DataContext = this };
              rect.SetBinding(Shape.FillProperty, nameof(WorkTimeBrush));

              Grid.SetColumn(rect, 0);

              rightGrid = new()
              {
                ColumnDefinitions = { new(), new() },
                Children = { rect }
              };

              Panel.SetZIndex(rightGrid, ZIndex_WorkTime);

              addList.Add(rightGrid);
            }

            rightGrid.ColumnDefinitions[0].Width = new(rightFraction, GridUnitType.Star);
            rightGrid.ColumnDefinitions[1].Width = new(cellMinuteStep.TotalMinutes - rightFraction, GridUnitType.Star);

            Grid.SetColumn(rightGrid, rightCol);
            Grid.SetColumnSpan(rightGrid, 1);
          }
          else if (rightGrid is not null) //右端に端数がないのに端数用Gridが存在する => 除去する
          {
            TargetGrid.Children.Remove(rightGrid);
            rightGrid = null;
          }
          #endregion

          #region CenterGridの設定
          if (rightCol - leftCol >= 2) //centerGridが必要である場合のみ処理を行う
          {
            if (centerGrid is null)
            {
              Rectangle rect = new() { DataContext = this };
              rect.SetBinding(Shape.FillProperty, nameof(WorkTimeBrush));

              Grid.SetColumn(rect, 1);

              centerGrid = new()
              {
                ColumnDefinitions = { new(), new(), new() },
                Children = { rect }
              };

              Panel.SetZIndex(centerGrid, ZIndex_WorkTime);

              addList.Add(centerGrid);
            }

            centerGrid.ColumnDefinitions[0].Width = new(0);
            centerGrid.ColumnDefinitions[1].Width = new(1, GridUnitType.Star); //いっぱいいっぱいを使用する
            centerGrid.ColumnDefinitions[2].Width = new(0);

            int LeftPad = NeededLeftGrid ? 1 : 0;

            Grid.SetColumn(centerGrid, leftCol + LeftPad); //左端Gridを配置しているなら, その分一つ右から開始する
            Grid.SetColumnSpan(centerGrid, rightCol - leftCol - LeftPad);
          }
          else if (centerGrid is not null) //必要ないのに存在しているなら削除する
          {
            TargetGrid.Children.Remove(centerGrid);
            centerGrid = null;
          }
          #endregion
        }

        TmpDic.Add(i.Key, new(leftGrid, centerGrid, rightGrid));
      }

      foreach (var i in TmpDic)
        WorkTimes[i.Key] = i.Value;

      return addList;
    }

    private void ReputSeparator()
    {
      int CellCount = GetCellCount();
      for(int i = 1; i < CellCount; i++)
      {
        Line line = new()
        {
          StrokeThickness = CurrentCellMode switch
          {
            CellMode.Minute01 =>
              (i % 60) == 0 ? SEPARATOR_WIDTH_BOLD :
              (i % 15) == 0 ? SEPARATOR_WIDTH_NORMAL
              : SEPARATOR_WIDTH_LIGHT, // 1HはBold, 15minはNormal, 5min/1minはLight

            CellMode.Minute05 =>
              (i % 12) == 0 ? SEPARATOR_WIDTH_BOLD :
              (i % 6) == 0 ? SEPARATOR_WIDTH_NORMAL
              : SEPARATOR_WIDTH_LIGHT, // 1HはBold, 30minはNormal, 10min/5minはLight

            CellMode.Minute10 =>
              (i % 6) == 0 ? SEPARATOR_WIDTH_BOLD :
              (i % 2) == 0 ? SEPARATOR_WIDTH_NORMAL
              : SEPARATOR_WIDTH_LIGHT, // 1HはBold, 30minはNormal, 10minはLight

            CellMode.Minute15 =>
              (i % 4) == 0 ? SEPARATOR_WIDTH_BOLD :
              (i % 2) == 0 ? SEPARATOR_WIDTH_NORMAL
              : SEPARATOR_WIDTH_LIGHT, // 1HはBold, 30minはNormal, 15minはLight

            CellMode.Minute30 => (i % 2) == 0 ? SEPARATOR_WIDTH_BOLD : SEPARATOR_WIDTH_NORMAL, // 1HはBold, 30minはNormal

            CellMode.Hour01 or CellMode.Hour02 => SEPARATOR_WIDTH_NORMAL,

            _ => 0
          },

          DataContext = this
        };

        if (line.StrokeThickness <= 0)
          return; //次回以降に回復することは考えられないため処理中断

        if (CurrentCellMode == CellMode.Minute01 && (i % 5) != 0) //1分ごとのモードのとき, 5の倍数以外は破線にする
        {
          line.StrokeDashArray = new() { 2 };
          line.StrokeDashCap = PenLineCap.Round;
        }
        else if (CurrentCellMode == CellMode.Minute05 && (i % 2) != 0) //5分ごとのモードのとき, 10の倍数以外は破線にする
        {
          line.StrokeDashArray = new() { 2 };
          line.StrokeDashCap = PenLineCap.Round;
        }

        line.Y1 = 0;
        line.Y2 = TargetGrid.ActualHeight;
        line.SetBinding(Shape.StrokeProperty, nameof(SeparatorBrush));
        line.HorizontalAlignment = HorizontalAlignment.Center;

        Panel.SetZIndex(line, ZIndex_Separator);
        Grid.SetColumn(line, i - 1);
        Grid.SetColumnSpan(line, 2);

        TargetGrid.Children.Add(line);
      }
    }
  }
}
