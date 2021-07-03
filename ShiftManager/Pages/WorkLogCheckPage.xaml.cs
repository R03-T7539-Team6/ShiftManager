using System;
using System.Windows.Controls;
using ShiftManager.DataClasses;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for WorkLogCheckPage.xaml
  /// </summary>
  public partial class WorkLogCheckPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public WorkLogCheckPage()
    {
      InitializeComponent();
    }

    public async void hoge()
    {
      var res = await ApiHolder.Api.GetWorkLogAsync();
      WorkLog wl  =  res.ReturnData;

      if (wl.WorkLogDictionary.TryGetValue(new DateTime(2021, 5, 1, 12, 0, 3).Date, out var output))
        hogehoge(); //値の取得に成功したとき
      else
        hugahuga();//値の取得に失敗したとき => outputにはNULLが入るので注意!
    }
  }
}
