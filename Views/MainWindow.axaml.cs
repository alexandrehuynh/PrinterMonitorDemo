using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PrinterMonitorDemo.ViewModels;
using System;
using System.Linq;
using ScottPlot;
using ScottPlot.Avalonia;

namespace PrinterMonitorDemo.Views
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializePlots();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InitializePlots()
        {
            // Configure Temperature Plot
            var tempPlot = this.FindControl<AvaPlot>("TemperaturePlot");
            if (tempPlot != null)
            {
                tempPlot.Plot.Title("Temperature History");
                tempPlot.Plot.XLabel("Time (s)");
                tempPlot.Plot.YLabel("Temperature (°C)");
                tempPlot.Plot.SetAxisLimits(yMin: 45, yMax: 85);
                tempPlot.Plot.Style(figureBackground: System.Drawing.Color.Transparent,
                                  dataBackground: System.Drawing.Color.Transparent);
            }

            // Configure Position Plot
            var posPlot = this.FindControl<AvaPlot>("PositionPlot");
            if (posPlot != null)
            {
                posPlot.Plot.Title("Position History");
                posPlot.Plot.XLabel("X Position");
                posPlot.Plot.YLabel("Y Position");
                posPlot.Plot.SetAxisLimits(0, 100, 0, 100);
                posPlot.Plot.Style(figureBackground: System.Drawing.Color.Transparent,
                                 dataBackground: System.Drawing.Color.Transparent);
            }

            // Configure Ink Level Plot
            var inkPlot = this.FindControl<AvaPlot>("InkPlot");
            if (inkPlot != null)
            {
                inkPlot.Plot.Title("Ink Level History");
                inkPlot.Plot.XLabel("Time (s)");
                inkPlot.Plot.YLabel("Ink Level (%)");
                inkPlot.Plot.SetAxisLimits(yMin: 0, yMax: 100);
                inkPlot.Plot.Style(figureBackground: System.Drawing.Color.Transparent,
                                 dataBackground: System.Drawing.Color.Transparent);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdatePlots();
        }

        private void UpdatePlots()
        {
            // Update Temperature Plot
            var tempPlot = this.FindControl<AvaPlot>("TemperaturePlot");
            if (tempPlot != null)
            {
                var tempData = _viewModel.TemperatureHistory.ToArray();
                var tempXs = Enumerable.Range(0, tempData.Length).Select(i => (double)i).ToArray();
                tempPlot.Plot.Clear();
                tempPlot.Plot.AddScatter(tempXs, tempData, color: System.Drawing.Color.LightGreen);
                tempPlot.Refresh();
            }

            // Update Position Plot
            var posPlot = this.FindControl<AvaPlot>("PositionPlot");
            if (posPlot != null)
            {
                var posXs = _viewModel.PositionHistory.Select(p => p.x).ToArray();
                var posYs = _viewModel.PositionHistory.Select(p => p.y).ToArray();
                posPlot.Plot.Clear();
                posPlot.Plot.AddScatter(posXs, posYs, color: System.Drawing.Color.LightBlue);
                posPlot.Plot.AddPoint(_viewModel.PrinterHead.XPosition, _viewModel.PrinterHead.YPosition,
                                    color: System.Drawing.Color.Red, size: 10);
                posPlot.Refresh();
            }

            // Update Ink Level Plot
            var inkPlot = this.FindControl<AvaPlot>("InkPlot");
            if (inkPlot != null)
            {
                var inkData = _viewModel.InkHistory.ToArray();
                var inkXs = Enumerable.Range(0, inkData.Length).Select(i => (double)i).ToArray();
                inkPlot.Plot.Clear();
                inkPlot.Plot.AddScatter(inkXs, inkData, color: System.Drawing.Color.LightBlue);
                inkPlot.Refresh();
            }
        }
    }
}