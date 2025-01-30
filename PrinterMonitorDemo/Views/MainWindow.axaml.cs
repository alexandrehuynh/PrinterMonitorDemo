using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PrinterMonitorDemo.ViewModels;

namespace PrinterMonitorDemo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}