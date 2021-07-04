using System;
using System.Windows.Controls;

using ShiftManager.Communication;
using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ScheduledShiftCheckPage.xaml
  /// </summary>
  public partial class ScheduledShiftCheckPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    ScheduledShiftManagePageViewModel VM = new();
    public ScheduledShiftCheckPage()
    {
      InitializeComponent();
      VM.ShiftRequestArray = new();
      DataContext = VM;
    }

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
      DateTime selectday = VM.TargetDate.Date;
      for (int i = 0; i < 7; i++)
      {
        ApiResult<SingleShiftData> res = await ApiHolder.Api.GetScheduledShiftByIDAsync(selectday.AddDays(i), ApiHolder.CurrentUserID);
        if (!res.IsSuccess)
        {
          break;
        }
        else
        {
          VM.ShiftRequestArray.Add(res.ReturnData);
        }
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
    private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => OnLoaded(null, null);

    /*******************************************
    * specification ;
    * name = OnLoaded ;
    * Function = 画面がロードされた時シフト表の内容を表示する ;
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
      VM.ShiftRequestArray.Clear();
      main();
    }
  }
}
