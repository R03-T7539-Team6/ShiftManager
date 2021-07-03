using System.Windows.Controls;

namespace ShiftManager.Pages
{
  /// <summary>
  /// Interaction logic for ShiftPrintPage.xaml
  /// </summary>
  public partial class ShiftPrintPage : Page, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; }
    public ShiftPrintPage()
    {
      InitializeComponent();
    }
  }
}
