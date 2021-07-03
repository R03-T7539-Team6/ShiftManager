using System.Windows.Controls;
using System;
using System.Windows.Threading;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for WorkTimeRecordPage.xaml
  /// </summary>
  public partial class WorkTimeRecordPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public WorkTimeRecordPage()
    {
      InitializeComponent();
      DispatcherTimer timer = new DispatcherTimer();
      timer.Tick += timer_Tick;
      timer.Interval = new TimeSpan(0, 0, 1);
      timer.Start();
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      DateTime d = DateTime.Now;
      time.Text = string.Format("{0:00}:{1:00}:{2:00}", d.Hour, d.Minute, d.Second);
    }
  }
}
