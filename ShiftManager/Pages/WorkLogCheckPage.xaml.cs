using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using AutoNotify;

using Reactive.Bindings;

using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  public partial class WorkLogCheckViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [AutoNotify]
    private ObservableCollection<ISingleWorkLog> _WorkLogArray;

    private double _ShiftRequestListItemWidth = double.NaN;
    public double ShiftRequestListItemWidth
    {
      get => _ShiftRequestListItemWidth;
      set
      {
        _ShiftRequestListItemWidth = value > 16 ? value : value - 16;
        PropertyChanged?.Invoke(this, new(nameof(ShiftRequestListItemWidth)));
      }
    }

    [AutoNotify]
    private DateTime _TargetDate = DateTime.Now.Date;
  }

  /// <summary>
  /// Interaction logic for WorkLogCheckPage.xaml
  /// </summary>
  public partial class WorkLogCheckPage : Page, IContainsApiHolder, IContainsIsProcessing
  {
    const int DayPerPage = 28;
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();
    WorkLogCheckViewModel VM = new();
    public WorkLogCheckPage()
    {
      InitializeComponent();
      VM.WorkLogArray = new();
      DataContext = VM;
    }

    bool DataLoadingCompleted { get; set; } = true;
    DateTime lastTargetDate { get; set; } = default;
    public ReactivePropertySlim<bool> IsProcessing { get; set; }

    /*******************************************
        * specification ;
        * name = main ;
        * Function = 勤怠履歴を取得する関数を呼び出す ;
        * note = 補足説明 ;
        * date = 07/03/2021 ;
        * author = 佐藤真通 ;
        * History = 更新履歴 ;
        * input = N/A ;
        * output = N/A ;
        * end of specification ;
        *******************************************/
    public async void main()
    {
      if (!DataLoadingCompleted && lastTargetDate == VM.TargetDate)
        return;

      DateTime targetDate = VM.TargetDate;

      try
      {
        DataLoadingCompleted = false;

        if (IsProcessing is not null)
          IsProcessing.Value = true;

        lastTargetDate = targetDate;

        DateTime date_Max = targetDate.AddDays(1);
        DateTime date_Min = targetDate.AddDays(DayPerPage * -1 + 1);

        if (string.IsNullOrWhiteSpace(ApiHolder.CurrentUserID.Value))
        {
          _ = MessageBox.Show("ログインされていません", "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        VM.WorkLogArray.Clear();

        for (int i = 0; i < DayPerPage; i++)
        {
          DateTime dt = targetDate.AddDays(i * -1);
          VM.WorkLogArray.Add(new SingleWorkLog(dt, dt, new()));
        }

        var res = await ApiHolder.Api.GetWorkLogAsync();

        if (lastTargetDate != targetDate)
          return;

        if (!res.IsSuccess || res.ReturnData is null)
        {
          _ = MessageBox.Show($"データ取得に失敗しました\nErrorCode:{res.ResultCode}\nData:{res.ReturnData}", "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        if (res.ReturnData.WorkLogDictionary.Count <= 0)
        {
          _ = MessageBox.Show($"勤務履歴が存在しません", "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        foreach (var item in res.ReturnData.WorkLogDictionary.Values)
        {
          if (item.AttendanceTime < date_Min || date_Max <= item.AttendanceTime)
            continue;

          VM.WorkLogArray[(targetDate - item.AttendanceTime.Date).Days] = item;
        }
      }
      finally
      {
        if (lastTargetDate == targetDate)
          DataLoadingCompleted = true;

        if (IsProcessing is not null)
          IsProcessing.Value = false;
      }
    }

    /*******************************************
    * specification ;
    * name = OnLooaded ;
    * Function = 画面がロードされた時に勤怠履歴を更新する ;
    * note = 補足説明 ;
    * date = 07/03/2021 ;
    * author = 佐藤真通 ;
    * History = 更新履歴 ;
    * input = 画面がロードされたことを知らせるイベントハンドラ ;
    * output = N/A ;
    * end of specification ;
    *******************************************/
    private void OnLoaded(object sender, RoutedEventArgs e) => main();


    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => main();

  }
}
