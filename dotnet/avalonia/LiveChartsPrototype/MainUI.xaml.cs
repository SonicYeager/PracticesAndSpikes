using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using LiveChartsCore.Defaults;
using System.Collections.Generic;
using Avalonia.Media;
using LiveChartsCore;
using LiveChartsCore.Themes;
using Avalonia.Styling;
using System.Windows.Input;
using Protoype;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Avalonia.Controls.Selection;

namespace Prototype
{
    public class DataSetItem
    {
        public DataSetItem(string path)
        {
            Path = path;
        }
        public string Path { get; }
    }

    public class MainUI : Window, INotifyPropertyChanged
    {
        ObservableCollection<DataSetItem> referenceDataSets = new ObservableCollection<DataSetItem>() { new ("D://Performancetests/Data/log20092021.txt"), new("D://Performancetests/Data/log21092021.txt"), new("D://Performancetests/Data/log22092021.txt") };

        public ObservableCollection<DataSetItem> ReferenceDataSets
        {
            get => referenceDataSets;
            set => this.RaiseAndSetIfChanged(ref referenceDataSets, value);
        }

        ObservableCollection<DataSetItem> latestDataSets = new ObservableCollection<DataSetItem>() { new("D://Performancetests/Data/log23092021.txt")};

        public ObservableCollection<DataSetItem> LatestDataSets
        {
            get => latestDataSets;
            set => this.RaiseAndSetIfChanged(ref latestDataSets, value);
        }

        ObservableCollection<DataSetItem> regressiveMethods = new ObservableCollection<DataSetItem>() { new("GetFrameFromGPUCache, Frame 7, 12.18 ms"), new("GetFrameFromGPUCache, Frame 9, 13.67 ms") };

        public ObservableCollection<DataSetItem> RegressiveMethods
        {
            get => regressiveMethods;
            set => this.RaiseAndSetIfChanged(ref regressiveMethods, value);
        }

        public List<Axis> LineChartXAxis { get; set; } = new List<Axis>{ new Axis{ Name = "Frames", Labeler = (value) => "Frame " + value } };
        public List<Axis> LineChartYAxis { get; set; } = new List<Axis>{ new Axis{ Name = "Elapsed Time", Labeler = (value) => value + " ms" } };

        public AddFileDialog FileDialog { get; set; }

        DataSetItem selectedLatestSource;

        public DataSetItem SelectedLatestSource
        {
            get => selectedLatestSource;
            set => this.RaiseAndSetIfChanged(ref selectedLatestSource, value);
        }

        DataSetItem selectedRegression;

        public DataSetItem SelectedRegression
        {
            get => selectedRegression;
            set => this.RaiseAndSetIfChanged(ref selectedRegression, value);
        }
        public SelectionModel<DataSetItem> SelectionReferences { get; set; } = new() { SingleSelect = false };

        public CartesianChart LineChart { get; set; }
        public PieChart PieChart { get; set; }
        public TabControl ChartWrapper { get; set; }

        System.Random RandomGen { get; set; } = new System.Random();

        public MainUI()
        {
            AvaloniaXamlLoader.Load(this);

            Title = "Prototype";
            DataContext = this;

            LineChart = this.FindControl<CartesianChart>("LineChart");
            LineChart.Series = new ObservableCollection<LineSeries<double>>() 
            {
                new LineSeries<double>() { Values = new double[] { 10.11, 12.13, 9.77, 9.89, 10.07, 11.99, 12.18, 13.44, 13.67}, Name = "Latest", Fill=null, GeometrySize=10 },
                new LineSeries<double>() { Values = new double[] { 11.09, 13.13, 10.75, 9.90, 10.09, 14.01, 11.78, 13.45, 13.66 }, Name = "Reference", Fill=null, GeometrySize=10 }
            };

            PieChart = this.FindControl<PieChart>("PieChart");
            PieChart.Series = new ObservableCollection<PieSeries<int>>() {
                new PieSeries<int>() { Values = new int[] { 45 }, Name = "GetGPUFrameFromCache" },
                new PieSeries<int>() { Values = new int[] { 38 }, Name = "ShrinkFrameToFit" },
                new PieSeries<int>() { Values = new int[] { 17 }, Name = "GetRequiredInputRes" }
            };

            ChartWrapper = this.FindControl<TabControl>("ChartWrapper");

            PropertyChanged += PropertyChangedHandler;
            SelectionReferences.SelectionChanged += SelectedReferenceChangedHandler;

            SetLight();

            this.AttachDevTools();
        }

        private void SelectedReferenceChangedHandler(object sender, SelectionModelSelectionChangedEventArgs<DataSetItem> e)
        {
            ((ObservableCollection<LineSeries<double>>)LineChart.Series)[1].Values = new double[] {
                    RandomGen.Next(8, 12)+0.45,
                    RandomGen.Next(8, 12) + 0.13,
                    RandomGen.Next(8, 12)+0.12,
                    RandomGen.Next(8, 12)+0.89,
                    RandomGen.Next(8, 12)+0.07,
                    RandomGen.Next(8, 12)+0.99,
                    RandomGen.Next(8, 12)+0.18,
                    RandomGen.Next(8, 12)+0.44,
                    RandomGen.Next(8, 12)+0.67
            };
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedLatestSource")
            {
               ((ObservableCollection<LineSeries<double>>)LineChart.Series)[0].Values = new double[] {
                    RandomGen.Next(8, 12)+0.45,
                    RandomGen.Next(8, 12) + 0.13,
                    RandomGen.Next(8, 12)+0.12,
                    RandomGen.Next(8, 12)+0.89,
                    RandomGen.Next(8, 12)+0.07,
                    RandomGen.Next(8, 12)+0.99,
                    RandomGen.Next(8, 12)+0.18,
                    RandomGen.Next(8, 12)+0.44,
                    RandomGen.Next(8, 12)+0.67
                };
            }
            else if (e.PropertyName == "SelectedRegression")
            {
                int sum = 100;
                ((ObservableCollection<PieSeries<int>>)PieChart.Series)[0].Values = new int[] { sum = sum - RandomGen.Next(1, sum) };
                ((ObservableCollection<PieSeries<int>>)PieChart.Series)[1].Values = new int[] { sum = sum - RandomGen.Next(1, sum) };
                ((ObservableCollection<PieSeries<int>>)PieChart.Series)[2].Values = new int[] { sum - RandomGen.Next(1, sum) };
                ChartWrapper.SelectedIndex = 1;
            }
            else
            {
                ((ObservableCollection<LineSeries<double>>)LineChart.Series).Remove(((ObservableCollection<LineSeries<double>>)LineChart.Series)[0]);
            }
        }

        public async Task<string> ShowFileDialogAsync() => await FileDialog.ShowDialog<string>(this);

        public async Task SelectRefFileCommand()
        {
            FileDialog = new AddFileDialog();
            var retVal = await ShowFileDialogAsync();
            referenceDataSets.Add(new(retVal));
            ReferenceDataSets = referenceDataSets;
        }

        public async Task SelectLatFileCommand()
        {
            FileDialog = new AddFileDialog();
            var retVal = await ShowFileDialogAsync();
            latestDataSets.Add(new(retVal));
            LatestDataSets = latestDataSets;
        }

        public void SetDark()
        {
            Background = new SolidColorBrush(new Color(120, 40, 40, 40));
            //HighlightedBackgound = new SolidColorBrush(new Color(255, 40, 40, 40));
            Foreground = new SolidColorBrush(new Color(255, 250, 250, 250));

            Application.Current.Styles.Insert(0, App.DefaultDark);

            LiveCharts.Configure(
                settings => settings
                    .AddDefaultMappers()
                    .AddSkiaSharp()
                    .AddDarkTheme(
                        theme =>
                        {
                            // you can add additional rules to the current theme
                            theme.Style
                                .HasRuleForLineSeries(lineSeries =>
                                {
                                    // this method will be called in the constructor of a line series instance

                                    lineSeries.LineSmoothness = 0.65;
                                    // ...
                                    // add more custom styles here ...
                                }).HasRuleForBarSeries(barSeries =>
                                {
                                    // this method will be called in the constructor of a column series instance
                                    // ...
                                });
                        }));
        }

        public void SetLight()
        {
            Background = new SolidColorBrush(new Color(120, 238, 238, 238));
            //HighlightedBackgound = new SolidColorBrush(new Color(255, 255, 255, 255));
            Foreground = new SolidColorBrush(new Color(255, 70, 70, 70));

            Application.Current.Styles.Insert(0, App.DefaultLight);

            LiveCharts.Configure(
                settings => settings
                    .AddDefaultMappers()
                    .AddSkiaSharp()
                    .AddLightTheme());
        }

        public override void Show()
        {
            base.Show();
        }

        new public event PropertyChangedEventHandler PropertyChanged;

        protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}