using System;
using System.Collections;
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

    /// <summary>休憩時間の総量 [min]</summary>
    public TimeSpan TotalBreakTimeLength
    {
      get
      {
        if (!(BreakTimeList?.Count > 0))
          return TimeSpan.Zero;

        TimeSpan ret = TimeSpan.Zero;
        foreach (var i in BreakTimeList)
          ret += i.TimeLen;

        return ret;
      }
    }
    public event EventHandler BreakTimeLenChanged;

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

    /// <summary>休憩リスト表示の行選択が変更された際に, SelectionTextを更新したり, 削除のために選択中要素情報を更新します.</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /*******************************************
* specification ;
* name = OnSelectionChanged ;
* Function = 休憩リスト表示の行選択が変更された際に, SelectionTextを更新したり, 削除のために選択中要素情報を更新します. ;
* note = ListView.SelectionChangedイベントのイベントハンドラです ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = TargetのListView, イベント発火の状況 ;
* output = N/A ;
* end of specification ;
*******************************************/
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

    /// <summary>最後に選択された休憩情報UI要素に関するテキストを, SelectionTextにセットします</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /*******************************************
* specification ;
* name = LastSelectionTextObject_PropertyChanged ;
* Function = 最後に選択された休憩情報UI要素に関するテキストをSelectionTextにセットする ;
* note = 主に削除が行われた後等に実行されます ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0新規作成 ;
* input = イベント発行者 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void LastSelectionTextObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (sender is BreakTimeDataSource newV)
        SelectionText = $"[{newV.Index + 1:D2}/{BreakTimeDictionary.Count:D2}] {newV.StartTime:HH:mm} ~ {newV.EndTime:HH:mm}";
    }

    /*******************************************
* specification ;
* name = BreakTimeEditorControl ;
* Function = デフォルトスタイルを変更する ;
* note = プロセスで初めて使用された際の1度のみ呼び出される ;
* date = 06/29/2021 ;
* author = 藤田一範 ;
* History = v1.0新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    static BreakTimeEditorControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(BreakTimeEditorControl), new FrameworkPropertyMetadata(typeof(BreakTimeEditorControl)));

    /*******************************************
* specification ;
* name = OnApplyTemplate ;
* Function = テンプレート反映後に実行され, BreakTimeListViewのインスタンスを取得する ;
* note = N/A ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (Template.FindName("BreakTimeListView", this) is ListView lv)
        lv.SelectionChanged += _OnSelectionChanged;
    }

    /*******************************************
* specification ;
* name = BreakTimeDictionaryUpdated ;
* Function = 休憩時間情報のソースとなるインスタンスが変更された際に, ListViewの表示も変更する ;
* note = N/A ;
* date = 07/4/2021 ;
* author = 藤田一範 ;
* History = v1.0新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    public void BreakTimeDictionaryUpdated()
    {
      BreakTimeList = new();
      foreach (var i in new Dictionary<DateTime, int>(BreakTimeDictionary))
      {
        BreakTimeDataSource btds = new(i, BreakTimeDictionary, TargetDate);
        BreakTimeList.Add(btds);
        btds.TimeLenChanged += OnBreakTimeLenChanged;
      }
    }

    /*******************************************
* specification ;
* name = AddBreakTime ;
* Function = 休憩情報追加ボタン押下時に呼ばれ, 休憩情報を追加する ;
* note = N/A ;
* date = 07/4/2021 ;
* author = 藤田一範 ;
* History = v1.0新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void AddBreakTime()
    {
      BreakTimeDictionary ??= new(); //BreakTimeDicがNULLなら, 新規インスタンスを割り当てる
      if (!BreakTimeDictionary.ContainsKey(TargetDate.Date)) //TargetDateの年月日の0時0分0秒を割り当てる
      {
        KeyValuePair<DateTime, int> kvp = new(TargetDate.Date, 0);
        BreakTimeDataSource btds = new(kvp, BreakTimeDictionary, TargetDate);
        BreakTimeList.Add(btds);
        btds.TimeLenChanged += OnBreakTimeLenChanged;
      }

      foreach (var i in BreakTimeList)
        i.UpdateIndex();
    }

    private void OnBreakTimeLenChanged(object sender, EventArgs e) => BreakTimeLenChanged?.Invoke(this, e);

    /*******************************************
* specification ;
* name = RemoveBreakTIme ;
* Function = 休憩情報削除ボタン押下時に呼ばれ, 選択された休憩情報を削除する ;
* note = N/A ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void RemoveBreakTime()
    {
      if (SelectedBreakTime is null || SelectedBreakTime.Count <= 0)
        return;

      foreach (var i in new List<BreakTimeDataSource>(SelectedBreakTime))
      {
        _ = BreakTimeDictionary.Remove(i.StartTime); //Keyが存在しなかったときはfalseが返るだけ
        _ = BreakTimeList.Remove(i); //tmpが存在しなかったときはfalseが返るだけ
        i.TimeLenChanged -= OnBreakTimeLenChanged;
      }

      SelectedBreakTime.Clear();

      foreach (var i in BreakTimeList)
        i.UpdateIndex();

      BreakTimeLenChanged?.Invoke(this, EventArgs.Empty);
    }
  }
  public partial class BreakTimeDataSource : INotifyPropertyChanged, INotifyDataErrorInfo
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    public event EventHandler TimeLenChanged;

    /*******************************************
* specification ;
* name = OnPropertyChanged ;
* Function = PropertyChangedイベントを発行するためのメソッド ;
* note = N/A ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = プロパティ名 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void OnPropertyChanged(in string propName) => PropertyChanged?.Invoke(this, new(propName));
    public Dictionary<DateTime, int> BreakTimeDictionary { get; }

    /*******************************************
* specification ;
* name = BreakTimeDataSource ;
* Function = 引数で指定された情報を用いてインスタンスを初期化する ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = 設定するプロパティ ;
* output = N/A ;
* end of specification ;
*******************************************/
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

    /*******************************************
* specification ;
* name = UpdateIndex ;
* Function = 要素の削除等によりIndexの変更が必要になった際に, Index情報を修正する ;
* note = N/A ;
* date = 06/30/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    public void UpdateIndex() => Index = BreakTimeDictionary.Keys.ToList().IndexOf(StartTime);

    private Dictionary<string, List<string>> ErrorsDic = new() { { nameof(StartTime), new() } };
    public IEnumerable GetErrors(string propertyName) => ErrorsDic.TryGetValue(propertyName, out var value) && value is not null ? value : new List<string>();

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

    const string SAME_DATE_ERROR = "Error: Same Date is already existing";
    private DateTime _StartTime;
    public DateTime StartTime
    {
      get => _StartTime;
      set
      {
        if (StartTime == value)
          return;

        DateTime newValue = new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);

        if (BreakTimeDictionary.ContainsKey(newValue)) //既に同じキーが存在するかどうかを確認
        {
          if (!ErrorsDic[nameof(StartTime)].Contains(SAME_DATE_ERROR))
          {
            ErrorsDic[nameof(StartTime)].Add(SAME_DATE_ERROR);
            ErrorsChanged?.Invoke(this, new(nameof(StartTime)));
          }
          return;
        }
        else if (ErrorsDic[nameof(StartTime)].Contains(SAME_DATE_ERROR))
        {
          ErrorsDic[nameof(StartTime)].Remove(SAME_DATE_ERROR);
          ErrorsChanged?.Invoke(this, new(nameof(StartTime)));
        }

        if (BreakTimeDictionary.TryGetValue(StartTime, out int timeLen))
          _ = BreakTimeDictionary.Remove(StartTime);

        _StartTime = newValue;

        _EndTime = StartTime + TimeLen;

        BreakTimeDictionary.Add(StartTime, timeLen);

        OnPropertyChanged(nameof(StartTime));
        OnPropertyChanged(nameof(EndTime));
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
        TimeLenChanged?.Invoke(this, EventArgs.Empty);
        BreakTimeDictionary[StartTime] = (int)TimeLen.TotalMinutes;
      }
    }

    public bool HasErrors { get => ErrorsDic[nameof(StartTime)].Count > 0; }
  }

}
