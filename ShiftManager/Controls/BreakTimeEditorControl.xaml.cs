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
  public class BreakTimeEditorControl : Control
  {
    public Dictionary<DateTime, int> BreakTimeDictionary { get => (Dictionary<DateTime, int>)GetValue(BreakTimeDictionaryProperty); set => SetValue(BreakTimeDictionaryProperty, value); }
    public static readonly DependencyProperty BreakTimeDictionaryProperty = DependencyProperty.Register(nameof(BreakTimeDictionary), typeof(Dictionary<DateTime, int>), typeof(BreakTimeEditorControl), new((i, _) => (i as BreakTimeEditorControl)?.BreakTimeDictionaryUpdated()));

    public BreakTimeDataSource SelectedBreakTime { get => (BreakTimeDataSource)GetValue(SelectedBreakTimeProperty); set => SetValue(SelectedBreakTimeProperty, value); }
    public static readonly DependencyProperty SelectedBreakTimeProperty = DependencyProperty.Register(nameof(SelectedBreakTime), typeof(BreakTimeDataSource), typeof(BreakTimeEditorControl));

    public static readonly ICommand AddBreakTimeCommand = new CustomCommand<BreakTimeEditorControl>(i => i.AddBreakTime());
    public static readonly ICommand RemoveBreakTimeCommand = new CustomCommand<BreakTimeEditorControl>(i => i.RemoveBreakTime());

    public ObservableCollection<BreakTimeDataSource> BreakTimeList { get => (ObservableCollection<BreakTimeDataSource>)GetValue(BreakTimeListProperty); set => SetValue(BreakTimeListProperty, value); }
    public static readonly DependencyProperty BreakTimeListProperty = DependencyProperty.Register(nameof(BreakTimeList), typeof(ObservableCollection<BreakTimeDataSource>), typeof(BreakTimeEditorControl));

    

    static BreakTimeEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(BreakTimeEditorControl), new FrameworkPropertyMetadata(typeof(BreakTimeEditorControl)));
    
    private void BreakTimeDictionaryUpdated()
    {
      SelectedBreakTime = null;
      BreakTimeList = new();
      foreach (var i in BreakTimeDictionary)
        BreakTimeList.Add(new(i, BreakTimeDictionary));
    }

    private void AddBreakTime()
    {
      if (!BreakTimeDictionary.ContainsKey(default))
        BreakTimeDictionary.Add(default, 0);
    }

    private void RemoveBreakTime()
    {
      if (SelectedBreakTime is null || !BreakTimeDictionary.ContainsKey(SelectedBreakTime.StartTime))
        return;

      BreakTimeDictionary.Remove(SelectedBreakTime.StartTime);
    }
  }
  public partial class BreakTimeDataSource : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(in string propName) => PropertyChanged?.Invoke(this, new(propName));
    public Dictionary<DateTime, int> BreakTimeDictionary { get; }
    public BreakTimeDataSource(in KeyValuePair<DateTime,int> keyValuePair ,Dictionary<DateTime, int> breakTimeDic)
    {
      BreakTimeDictionary = breakTimeDic;

      _StartTime = keyValuePair.Key;
      TimeLen = new(0, keyValuePair.Value, 0); //60分以上も入力可
      //EndTimeはTimeLenのsetterで決定される

      Index = breakTimeDic.Keys.ToList().IndexOf(StartTime);
    }

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
          BreakTimeDictionary.Remove(StartTime);

        _StartTime = new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);

        EndTime = StartTime + TimeLen;

        OnPropertyChanged(nameof(StartTime));

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
        TimeLen = EndTime - StartTime;
        OnPropertyChanged(nameof(EndTime));

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
        EndTime = StartTime + TimeLen;
        OnPropertyChanged(nameof(TimeLen));

        BreakTimeDictionary[StartTime] = (int)TimeLen.TotalMinutes;
      }
    }
  }

}
