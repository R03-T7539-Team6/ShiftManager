using System;
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

    public MainWindow()
    {
      InitializeComponent();
      MainWindowViewModel mwvm = new() { MainFramePageChanger = new(MainFrame) };
      DataContext = mwvm;
      
    }

    private void MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
      if (e.Content is IContainsApiHolder i)
        i.ApiHolder = ApiHolder;
    }
  }

  internal class MainWindowViewModel
  {
    public FramePageChanger MainFramePageChanger { get; init; }
    public ReactivePropertySlim<bool> IsMenuExpanded { get; } = new(true);
    public ReactivePropertySlim<NameData> UserName { get; } = new(new("FIRSTNAME", "LASTNAME"));
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
