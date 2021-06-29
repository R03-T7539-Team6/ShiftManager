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

    public BreakTimeDataSource SelectedBreakTime { get => (BreakTimeDataSource)GetValue(SelectedBreakTimeProperty); set => SetValue(SelectedBreakTimeProperty, value); }
    public static readonly DependencyProperty SelectedBreakTimeProperty = DependencyProperty.Register(nameof(SelectedBreakTime), typeof(BreakTimeDataSource), typeof(BreakTimeEditorControl), new((s, e) => (s as BreakTimeEditorControl)?.SelectionChanged(e)));

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
      BreakTimeDictionary ??= new(); //BreakTimeDicがNULLなら, 新規インスタンスを割り当てる

      if (!BreakTimeDictionary.ContainsKey(default))
      {
        KeyValuePair<DateTime, int> kvp = new(new(TargetDate.Year, TargetDate.Month, TargetDate.Day), 0);
        BreakTimeDictionary.Add(kvp.Key, kvp.Value);
        BreakTimeList.Add(new(kvp, BreakTimeDictionary));
      }
    }

    private void RemoveBreakTime()
    {
      if (SelectedBreakTime is null)
        return;

      var tmp = SelectedBreakTime;
      SelectedBreakTime = null; //先に選択を解除する

      BreakTimeDictionary.Remove(tmp.StartTime); //Keyが存在しなかったときはfalseが返るだけ
      BreakTimeList.Remove(tmp); //tmpが存在しなかったときはfalseが返るだけ

      foreach (var i in BreakTimeList)
        i.UpdateIndex();
    }

    private BreakTimeDataSource lastSelectedItem;
    private void SelectionChanged(in DependencyPropertyChangedEventArgs? e)
    {
      if(BreakTimeDictionary is null)
      {
        SelectionText = "[--/--] --:-- ~ --:--";
        return;
      }

      if (e?.OldValue is BreakTimeDataSource v)
        v.PropertyChanged -= SelectedItemPropertyChanged;

      BreakTimeDataSource newV = e?.NewValue as BreakTimeDataSource ?? SelectedBreakTime;

      if (newV is not null) //選択されてる場合のみ更新
      {
        if (lastSelectedItem != newV) //選択要素の更新があればイベントの購読/解除も行う
        {
          if (lastSelectedItem is not null)
            lastSelectedItem.PropertyChanged -= SelectedItemPropertyChanged;

          newV.PropertyChanged += SelectedItemPropertyChanged;
          lastSelectedItem = newV;
        }

        SelectionText = $"[{newV.Index:D2}/{BreakTimeDictionary.Count:D2}] {newV.StartTime:HH:mm} ~ {newV.EndTime:HH:mm}";
      }

    }
    private void SelectedItemPropertyChanged(object s, PropertyChangedEventArgs e) => SelectionChanged(null);
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

      UpdateIndex();
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
          BreakTimeDictionary.Remove(StartTime);

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
