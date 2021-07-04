using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Reactive.Bindings;

using ShiftManager.DataClasses;

namespace ShiftManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, IContainsApiHolder
  {
    public IApiHolder ApiHolder { get; set; } = new ApiHolder();
    private MainWindowViewModel MWVM { get; }

    public MainWindow()
    {
      InitializeComponent();
      MWVM = new() { MainFramePageChanger = new(MainFrame) };
      DataContext = MWVM;
      SignInPageElem.ApiHolder = this.ApiHolder;
    }

    /// <summary>SignInがSuccessした際に実行される</summary>
    /// <param name="sender">呼び出し元</param>
    /// <param name="e">イベント引数</param>
    private void SignInPageElem_Login(object sender, EventArgs e)
    {
      MWVM.IsSignedIn.Value = true;
      MWVM.UserName.Value = new(ApiHolder.Api.CurrentUserData?.FullName);
    }

    private void MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
      if (e.Content is IContainsApiHolder i)
        i.ApiHolder = ApiHolder;
    }

    private async void SignOutClicked(object sender, RoutedEventArgs e)
    {
      _ = await ApiHolder.Api.SignOutAsync();
      MWVM.IsSignedIn.Value = false;
      MainFrame.Content = null;
    }

/*******************************************
* specification ;
* name = LicenseClicked ;
* Function = ライセンスボタンがクリックされた時にライセンス情報のテキストフォルダを読み込んでサブウィンドウに渡す ;
* note = 補足説明 ;
* date = 07/04/2021 ;
* author = 佐藤真通 ;
* History = 更新履歴 ;
* input = ライセンスボタンが押されたことを知らせるイベントハンドラ ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void LicenseClicked(object sender, RoutedEventArgs e)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = "ShiftManager.Resources.ThirdPartyLicense.md";
      string manualFileContent;
      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream != null)
        {
          using (var sr = new StreamReader(stream))
          {
            manualFileContent = sr.ReadToEnd();
            var window = new Window1(manualFileContent);
            window.Show();
          }
        }
      }
    }
  }

  internal class MainWindowViewModel
  {
    public FramePageChanger MainFramePageChanger { get; init; }
    public ReactivePropertySlim<bool> IsMenuExpanded { get; } = new(true);
    public ReactivePropertySlim<bool> IsSignedIn { get; }
#if DEBUG
      = new(true);
#else
      = new(false);
#endif
    public ReactivePropertySlim<NameData_PropertyChanged> UserName { get; } = new(new());
  }

  /// <summary>指定のFrameに, CommandParmeterで指定された型のPageを表示する</summary>
  internal class FramePageChanger : ICommand
  {
    public event EventHandler CanExecuteChanged;
    public Frame TargetFrame { get; }

    public FramePageChanger(Frame _TargetFrame) => TargetFrame = _TargetFrame;

    public bool CanExecute(object parameter) => TargetFrame is not null && parameter is Type;

    public void Execute(object parameter) => TargetFrame.Content = (parameter as Type)?.GetConstructor(new Type[0]).Invoke(null);
  }
}
