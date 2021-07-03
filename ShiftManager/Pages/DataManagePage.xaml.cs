using System.Windows.Controls;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for DataManagePage.xaml
  /// </summary>
  public partial class DataManagePage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }

    public DataManagePage()
    {
      InitializeComponent();
    }
  }
}
