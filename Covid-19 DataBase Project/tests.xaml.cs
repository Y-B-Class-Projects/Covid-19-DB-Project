using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Covid_19_DataBase_Project
{
    /// <summary>
    /// Interaction logic for tests.xaml
    /// </summary>
    public partial class tests : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> dayRangeSelectList { get; set; }

        private bool isUpdate;

        private BL.BL_class bl;

        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public tests(BL.BL_class bl)
        {
            InitializeComponent();

            isUpdate = false;

            this.bl = bl;

            XFormatter = val => new DateTime((long)val).ToString("dd-MM");
            YFormatter = val => val.ToString("N");

            dayRangeSelectList = new List<string>() { "Until now", "Last year", "Last 6 month", "Last 3 month", "Last month" };

            cmbNumberOfTests.ItemsSource = dayRangeSelectList;
            cmbpercentage.ItemsSource = dayRangeSelectList;
            cmbTotal.ItemsSource = dayRangeSelectList;

            cmbNumberOfTests.SelectedIndex = 0;
            cmbpercentage.SelectedIndex = 0;
            cmbTotal.SelectedIndex = 0;

            DataContext = this;

            isUpdate = true;

            updateDataAsync();
        }


        private void refresh()
        {
            updateTestTotalChart();
            updateNumberOfTestsChart();
            updatePercentageOfPositiveChart();

            txtLastDay.Text = bl.get_last_day_tests().ToString();
        }

        private void updatePercentageOfPositiveChart()
        {
            int numOfDays = -1;

            switch (cmbpercentage.SelectedIndex)
            {
                case 1:
                    numOfDays = 360;
                    break;
                case 2:
                    numOfDays = 180;
                    break;
                case 3:
                    numOfDays = 90;
                    break;
                case 4:
                    numOfDays = 30;
                    break;
            }


            percentageOfPositiveChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Positive percentage",
                    Values = toChartValues(bl.get_percentage_of_positive(numOfDays)),
                    DataLabels = false,
                    LineSmoothness = 20,
                    PointGeometrySize= 0,
                    StrokeThickness = 3
                }
            };
        }

        private void updateNumberOfTestsChart()
        {
            int numOfDays = -1;

            switch (cmbNumberOfTests.SelectedIndex)
            {
                case 1:
                    numOfDays = 360;
                    break;
                case 2:
                    numOfDays = 180;
                    break;
                case 3:
                    numOfDays = 90;
                    break;
                case 4:
                    numOfDays = 30;
                    break;
            }

            var number_of_tests = bl.get_number_of_tests_per_day(numOfDays);

            numberOfTestChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Number of tests",
                    Values = toChartValues(number_of_tests),
                    DataLabels = false,
                    LineSmoothness = 20,
                    PointGeometrySize= 0,
                    StrokeThickness = 3
                }
            };
        }

        private void updateTestTotalChart()
        {
            int numOfDays = -1;

            switch (cmbTotal.SelectedIndex)
            {
                case 1:
                    numOfDays = 360;
                    break;
                case 2:
                    numOfDays = 180;
                    break;
                case 3:
                    numOfDays = 90;
                    break;
                case 4:
                    numOfDays = 30;
                    break;
            }

            var tests = bl.get_positive_tests(numOfDays);

            mainTestsChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Positives",
                    Values = toChartValues(tests),
                    DataLabels = false,
                    LineSmoothness = 20,
                    PointGeometrySize= 0,
                    StrokeThickness = 3
                },
                new LineSeries
                {
                    Title = "7 days avg",
                    Values = toChartValues(bl.get_last_7_days_avg(tests)),
                    DataLabels = false,
                    LineSmoothness = 20,
                    PointGeometrySize= 0,
                    StrokeThickness = 3
                }
            };
        }

        private ChartValues<DateTimePoint> toChartValues<T>(Dictionary<DateTime, T> dictionary)
        {
            ChartValues<DateTimePoint> ret = new ChartValues<DateTimePoint>();

            foreach (var day in dictionary)
                ret.Add(new DateTimePoint(day.Key, Convert.ToDouble(day.Value)));

            return ret;
        }

        private async Task updateDataAsync()
        {
            Thread thread = new Thread(() =>
            {
                wait w;
                w = new wait();
                w.ShowDialog();

                w.Closed += (sender2, e2) => w.Dispatcher.InvokeShutdown();

                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            refresh();

            thread.Abort();
        }


        private async void btnTestsRef_Click(object sender, RoutedEventArgs e)
        {
            await updateDataAsync();
        }

        private void cmbTotal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(isUpdate)
                updateTestTotalChart();
        }

        private void cmbNumberOfTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdate)
                updateNumberOfTestsChart();
        }

        private void cmbpercentage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdate)
                updatePercentageOfPositiveChart();
        }



        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;

            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
