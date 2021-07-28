using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Reactive.Bindings;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ScheduledShiftCheckPage.xaml
  /// </summary>
  public partial class ScheduledShiftCheckPage : Page, IContainsApiHolder, IContainsIsProcessing
  {
    static readonly int DayPerPage = 28;
    public IApiHolder ApiHolder { get; set; }
    public ReactivePropertySlim<bool> IsProcessing { get; set; }

    ScheduledShiftManagePageViewModel VM = new();

    DateTime currentShowingDate = default;

    public ScheduledShiftCheckPage()
    {
      InitializeComponent();
      VM.ShiftRequestArray = new();
      DataContext = VM;
    }

    bool IsDataLoading = false;
/*******************************************
* specification ;
* name = main ;
* Function = Apiを呼び出して予定シフトを取得する ;
* note = 補足説明 ;
* date = 07/03/2021 ;
* author = 佐藤真通 ;
* History = N/A ;
* input = N/A ;
* output = N/A ;
* end of specification ;
*******************************************/
    public async void main()
    {
      if (IsDataLoading)
        return; //二重読み込みの防止

      if (VM.TargetDate == currentShowingDate) //既に同じ日付を表示済みなら再度実行しない
        return;

      try
      {
        IsDataLoading = true;
        if (IsProcessing is not null)
          IsProcessing.Value = true;

        VM.ShiftRequestArray.Clear();

        DateTime selectday = VM.TargetDate.Date;
        currentShowingDate = selectday;

        if (string.IsNullOrWhiteSpace(ApiHolder.CurrentUserID.Value))
        {
          _ = MessageBox.Show("サインインされていません (不正なユーザID)", "ShiftManager", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        List<Task<ApiResult<SingleShiftData>>> taskList = new();

        for (int i = 0; i < DayPerPage; i++)
        {
          DateTime targetDate = selectday.AddDays(i);
          VM.ShiftRequestArray.Add(new SingleShiftData(ApiHolder.CurrentUserID, targetDate, false, targetDate, targetDate, new()));
          taskList.Add(ApiHolder.Api.GetScheduledShiftByIDAsync(targetDate, ApiHolder.CurrentUserID));
        }

        var results = await Task.WhenAll(taskList);

        for (int i = 0; i < results.Length; i++)
          if (results[i].IsSuccess && results[i].ReturnData is not null)
            VM.ShiftRequestArray[i] = results[i].ReturnData;
      }
      finally
      {
        IsDataLoading = false;
        if (IsProcessing is not null)
          IsProcessing.Value = false;

      }
    }

/*******************************************
* specification ;
* name = DatePicker_SelectedDateChanged ;
* Function = 選択する日付が変更された時に予定シフト表の内容を更新する ;
* note = 補足説明 ;
* date = 07/03/2021 ;
* author = 佐藤真通 ;
* History = 更新履歴 ;
* input = 選択する日付が変わったことを知らせるイベントハンドラ ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => main();

  }
}
