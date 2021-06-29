using System.Windows.Controls;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ShiftRequestManagePage.xaml
  /// </summary>
  public partial class ShiftRequestManagePage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public ShiftRequestManagePage()
    {
      InitializeComponent();
    }
  }
}
