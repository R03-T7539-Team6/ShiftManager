using System.Windows.Controls;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for UserSettingPage.xaml
  /// </summary>
  public partial class UserSettingPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public UserSettingPage()
    {
      InitializeComponent();
    }
  }
}
