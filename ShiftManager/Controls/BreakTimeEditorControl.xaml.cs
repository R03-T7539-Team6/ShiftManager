using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShiftManager.Controls
{
  public class BreakTimeEditorControl : Control, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public DateTime TargetDate { get => (DateTime)GetValue(TargetDateProperty); set => SetValue(TargetDateProperty, value); }
    public static readonly DependencyProperty TargetDateProperty = DependencyProperty.Register(nameof(TargetDate), typeof(DateTime), typeof(BreakTimeEditorControl));

    public Dictionary<DateTime, int> BreakTimeDictionary { get => (Dictionary<DateTime, int>)GetValue(BreakTimeDictionaryProperty); set => SetValue(BreakTimeDictionaryProperty, value); }
    public static readonly DependencyProperty BreakTimeDictionaryProperty = DependencyProperty.Register(nameof(BreakTimeDictionary), typeof(Dictionary<DateTime, int>), typeof(BreakTimeEditorControl), new((i, _) => (i as BreakTimeEditorControl)?.BreakTimeDictionaryUpdated()));

    private List<BreakTimeDataSource> SelectedBreakTime { get; } = new();

    public static readonly ICommand AddBreakTimeCommand = new CustomCommand<BreakTimeEditorControl>(i => i.AddBreakTime());
    public static readonly ICommand RemoveBreakTimeCommand = new CustomCommand<BreakTimeEditorControl>(i => i.RemoveBreakTime());

    public ObservableCollection<BreakTimeDataSource> BreakTimeList { get => (ObservableCollection<BreakTimeDataSource>)GetValue(BreakTimeListProperty); set => SetValue(BreakTimeListProperty, value); }
    public static readonly DependencyProperty BreakTimeListProperty = DependencyProperty.Register(nameof(BreakTimeList), typeof(ObservableCollection<BreakTimeDataSource>), typeof(BreakTimeEditorControl));

    public string SelectionText
    {
      get => _SelectionText;
      set
      {
        _SelectionText = value;
        PropertyChanged?.Invoke(this, new(nameof(SelectionText)));
      }
    }
    private string _SelectionText = "[--/--] --:-- ~ --:--";

    private BreakTimeDataSource LastSelectionTextObject;

    private void _OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      foreach (var r in e.RemovedItems)
      {
        if (r is BreakTimeDataSource d)
          _ = SelectedBreakTime.Remove(d);
      }

      foreach (var a in e.AddedItems)
      {
        if (a is BreakTimeDataSource d)
          SelectedBreakTime.Add(d);
      }

      if (BreakTimeDictionary is null)
      {
        SelectionText = "[--/--] --:-- ~ --:--";
        return;
      }

      BreakTimeDataSource firstSelectedItem = SelectedBreakTime.FirstOrDefault();

      if (firstSelectedItem != LastSelectionTextObject)
      {
        if (LastSelectionTextObject is not null)
          LastSelectionTextObject.PropertyChanged -= LastSelectionTextObject_PropertyChanged;

        if (firstSelectedItem is not null)
        {
          firstSelectedItem.PropertyChanged += LastSelectionTextObject_PropertyChanged;
          LastSelectionTextObject_PropertyChanged(firstSelectedItem, null);
        }
        else
        {
          SelectionText = $"[--/{BreakTimeDictionary.Count:D2}] --:-- ~ --:--";
        }

        LastSelectionTextObject = firstSelectedItem;
      }
    }

    private void LastSelectionTextObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (sender is BreakTimeDataSource newV)
        SelectionText = $"[{newV.Index + 1:D2}/{BreakTimeDictionary.Count:D2}] {newV.StartTime:HH:mm} ~ {newV.EndTime:HH:mm}";
    }

    static BreakTimeEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(BreakTimeEditorControl), new FrameworkPropertyMetadata(typeof(BreakTimeEditorControl)));

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (Template.FindName("BreakTimeListView", this) is ListView lv)
        lv.SelectionChanged += _OnSelectionChanged;
    }

    private void BreakTimeDictionaryUpdated()
    {
      BreakTimeList = new();
      foreach (var i in BreakTimeDictionary)
        BreakTimeList.Add(new(i, BreakTimeDictionary, TargetDate));
    }

    private void AddBreakTime()
    {
      BreakTimeDictionary ??= new(); //BreakTimeDicがNULLなら, 新規インスタンスを割り当てる

      if (!BreakTimeDictionary.ContainsKey(default))
      {
        KeyValuePair<DateTime, int> kvp = new(new(TargetDate.Year, TargetDate.Month, TargetDate.Day), 0);
        BreakTimeDictionary.Add(kvp.Key, kvp.Value);
        BreakTimeList.Add(new(kvp, BreakTimeDictionary, TargetDate));
      }

      foreach (var i in BreakTimeList)
        i.UpdateIndex();
    }

    private void RemoveBreakTime()
    {
      if (SelectedBreakTime is null || SelectedBreakTime.Count <= 0)
        return;

      foreach (var i in new List<BreakTimeDataSource>(SelectedBreakTime))
      {
        _ = BreakTimeDictionary.Remove(i.StartTime); //Keyが存在しなかったときはfalseが返るだけ
        _ = BreakTimeList.Remove(i); //tmpが存在しなかったときはfalseが返るだけ
      }

      SelectedBreakTime.Clear();

      foreach (var i in BreakTimeList)
        i.UpdateIndex();
    }
  }
  public partial class BreakTimeDataSource : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(in string propName) => PropertyChanged?.Invoke(this, new(propName));
    public Dictionary<DateTime, int> BreakTimeDictionary { get; }
    public BreakTimeDataSource(in KeyValuePair<DateTime, int> keyValuePair, in Dictionary<DateTime, int> breakTimeDic, in DateTime baseDate)
    {
      BreakTimeDictionary = breakTimeDic;
      BaseDate = baseDate;

      _StartTime = keyValuePair.Key;
      TimeLen = new(0, keyValuePair.Value, 0); //60分以上も入力可
      //EndTimeはTimeLenのsetterで決定される

      UpdateIndex();
    }

    private DateTime _BaseDate;
    public DateTime BaseDate
    {
      get => _BaseDate;
      set
      {
        _BaseDate = value;
        StartTime = BaseDate + StartTime.TimeOfDay;
        OnPropertyChanged(nameof(BaseDate));
      }
    }

    public void UpdateIndex() => Index = BreakTimeDictionary.Keys.ToList().IndexOf(StartTime);

    private int _Index;
    public int Index
    {
      get => _Index;
      set
      {
        _Index = value;
        OnPropertyChanged(nameof(Index));
      }
    }

    private DateTime _StartTime;
    public DateTime StartTime
    {
      get => _StartTime;
      set
      {
        if (StartTime == value)
          return;

        if (BreakTimeDictionary.TryGetValue(StartTime, out int timeLen))
          _ = BreakTimeDictionary.Remove(StartTime);

        _StartTime = new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);

        _EndTime = StartTime + TimeLen;

        OnPropertyChanged(nameof(StartTime));
        OnPropertyChanged(nameof(EndTime));

        BreakTimeDictionary.Add(StartTime, timeLen);
      }
    }

    private DateTime _EndTime;
    public DateTime EndTime
    {
      get => _EndTime;
      set
      {
        _EndTime = new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        _TimeLen = EndTime - StartTime;

        OnPropertyChanged(nameof(EndTime));
        OnPropertyChanged(nameof(TimeLen));

        BreakTimeDictionary[StartTime] = (int)(EndTime - StartTime).TotalMinutes;
      }
    }

    private TimeSpan _TimeLen;
    public TimeSpan TimeLen
    {
      get => _TimeLen;
      set
      {
        _TimeLen = value;
        _EndTime = StartTime + TimeLen;

        OnPropertyChanged(nameof(EndTime));
        OnPropertyChanged(nameof(TimeLen));

        BreakTimeDictionary[StartTime] = (int)TimeLen.TotalMinutes;
      }
    }
  }

}
