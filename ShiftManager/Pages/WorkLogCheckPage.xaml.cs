using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

using AutoNotify;

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

    public async void hoge()
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

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
      hoge();
    }
  }
}
