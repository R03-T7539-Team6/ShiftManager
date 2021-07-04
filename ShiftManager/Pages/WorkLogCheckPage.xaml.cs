using System;
using System.ComponentModel;
using System.Windows.Controls;
using ShiftManager.DataClasses;
using AutoNotify;
using System.Collections.ObjectModel;

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
  public partial class WorkLogCheckPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();
    WorkLogCheckViewModel VM = new();
    public WorkLogCheckPage()
    {
      InitializeComponent();
      VM.WorkLogArray = new();
      DataContext = VM;
    }

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
      var res = await ApiHolder.Api.GetWorkLogAsync();
      WorkLog wl = res.ReturnData;
      DateTime today = DateTime.Today;

      for (int i = 0; i > -30; i--)
      {
        if (wl.WorkLogDictionary.TryGetValue(today.AddDays(i), out var output))
        {
          VM.WorkLogArray.Add(output);
        }
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
    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
      main();
    }
  }
}
