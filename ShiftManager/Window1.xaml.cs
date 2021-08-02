
using System.Windows;

namespace ShiftManager
{
  /// <summary>
  /// Window1.xaml の相互作用ロジック
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1(string strData)
    {
      InitializeComponent();
      text.Text = strData;

    }
  }
}
