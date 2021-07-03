using System.Windows.Controls;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ScheduledShiftManagePage.xaml
  /// </summary>
  public partial class ScheduledShiftManagePage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public ScheduledShiftManagePage()
    {
      InitializeComponent();
    }
  }
}
