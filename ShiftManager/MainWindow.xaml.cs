using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

using Reactive.Bindings;

using ShiftManager.DataClasses;

namespace ShiftManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, IContainsApiHolder
  {
    // public IApiHolder ApiHolder { get; set; } = new ApiHolder() { Api = new Communication.RestApiBroker() };
    public IApiHolder ApiHolder { get; set; } = new ApiHolder() { Api = new Communication.InternalApi() };
    private MainWindowViewModel MWVM { get; }

    ThicknessAnimation MenuCloseThicknessAnimation { get; } = new(new(-300,0,0,0), TimeSpan.FromMilliseconds(500));
    bool IsMenuOpenAnimationRunning { get; set; } = false;
    ThicknessAnimation MenuOpenThicknessAnimation { get; } = new(new(0,0,0,0), TimeSpan.FromMilliseconds(500));

    /*******************************************
* specification ;
* name = MainWindow ;
* Function = MainWindowのインスタンスを初期化します ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = ListViewインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    public MainWindow()
    {
      InitializeComponent();
      MWVM = new() { MainFramePageChanger = new(MainFrame) };
      DataContext = MWVM;
      SignInPageElem.ApiHolder = this.ApiHolder;

      MenuCloseThicknessAnimation.Completed += (_, _) =>MenuGrid.Visibility = Visibility.Collapsed;
      MenuOpenThicknessAnimation.Completed += (_, _) => IsMenuOpenAnimationRunning = false;

      MenuCloseThicknessAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
      MenuOpenThicknessAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
    }

    /// <summary>SignInがSuccessした際に実行される</summary>
    /// <param name="sender">呼び出し元</param>
    /// <param name="e">イベント引数</param>
    /*******************************************
* specification ;
* name = SignInPageElem_Login ;
* Function = サインイン成功時に実行され, 表示の更新を行います ;
* note = N/A ;
* date = 07/04/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = SignInコントロールインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void SignInPageElem_Login(object sender, EventArgs e)
    {
      MWVM.IsSignedIn.Value = true;
      MWVM.UserName.Value = new(ApiHolder.CurrentUserName);
    }

    /*******************************************
* specification ;
* name = MainFrame_Navigating ;
* Function = ページ移動時に呼び出され, API参照用インスタンスをセットします ;
* note = N/A ;
* date = 06/29/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Frameのインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
    private void MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
      if (e.Content is IContainsApiHolder i)
        i.ApiHolder = ApiHolder;
    }

    /*******************************************
* specification ;
* name = SignOutClicked ;
* Function = サインアウトボタン押下時に実行され, サインアウト処理を実行します ;
* note = N/A ;
* date = 07/03/2021 ;
* author = 藤田一範 ;
* History = v1.0:新規作成 ;
* input = Buttonのインスタンス, イベント情報 ;
* output = N/A ;
* end of specification ;
*******************************************/
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

    private void MenuOpenCloseClicked(object sender, RoutedEventArgs e)
    {
      if (IsMenuOpenAnimationRunning || MenuGrid.Visibility == Visibility.Visible)
      {
        IsMenuOpenAnimationRunning = false;

        MenuGrid.BeginAnimation(Grid.MarginProperty, MenuCloseThicknessAnimation);
      }
      else
      {
        MenuGrid.Visibility = Visibility.Visible;

        MenuGrid.BeginAnimation(Grid.MarginProperty, MenuOpenThicknessAnimation);
        IsMenuOpenAnimationRunning = true;
      }
    }

    private void TitleStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (sender is FrameworkElement elem)
        MenuCloseThicknessAnimation.To = new(-1 * elem.ActualWidth, 0, 0, 0);
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
