using System.Windows.Controls;
using System;
using System.Collections.Generic;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ScheduledShiftCheckPage.xaml
  /// </summary>

  public partial class ScheduledShiftCheckPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public ScheduledShiftCheckPage()
    {
      InitializeComponent();

      var Datelist = new List<DateTime>();
      DateTime today = DateTime.Today;
      Datelist.Add(today);
      Datelist.Add(today.AddDays(1));
      Datelist.Add(today.AddDays(2));
      Datelist.Add(today.AddDays(3));
      Datelist.Add(today.AddDays(4));
      Datelist.Add(today.AddDays(5));
      Datelist.Add(today.AddDays(6));
      Datelist.Add(today.AddDays(7));

      ComboBox.ItemsSource = Datelist;

    }
  }
}
