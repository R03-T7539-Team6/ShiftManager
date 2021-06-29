using System.Windows.Controls;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for SignUpPage.xaml
  /// </summary>
  public partial class SignUpPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public SignUpPage()
    {
      InitializeComponent();
    }
  }
}
