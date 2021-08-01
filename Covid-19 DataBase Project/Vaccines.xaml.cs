using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
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

namespace Covid_19_DataBase_Project
{
    /// <summary>
    /// Interaction logic for Vaccines.xaml
    /// </summary>
    public partial class Vaccines : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        

        public List<string> dayRangeSelectList_1 { get; set; }
        public List<string> dayRangeSelectList_2 { get; set; }
        public List<string> _Labels { get; set; }

        private bool isUpdate;

        public List<string> Labels
        {
            get { return _Labels; }
            set
            {
                _Labels = value;
                OnPropertyChanged("Labels");
            }
        }

        private BL.BL_class bl;

        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public Vaccines(BL.BL_class bl)
        {
            InitializeComponent();
            
            this.bl = bl;

            isUpdate = false;

            dayRangeSelectList_1 = new List<string>() {"Until now",  "Last year", "Last 6 month", "Last 3 month", "Last month"};
            dayRangeSelectList_2 = new List<string>() {"Last 2 month", "Last month", "Last week"};

            XFormatter = val => new DateTime((long)val).ToString("dd-MM");
            YFormatter = val => val.ToString("N");

            cmbVaccTotal.ItemsSource = dayRangeSelectList_1;
            cmbVaccDay.ItemsSource = dayRangeSelectList_2;
            cmbVaccCity_day_range.ItemsSource = dayRangeSelectList_1;

            cmbVaccCity.ItemsSource = bl.get_city_names();

            DataContext = this;

            cmbVaccTotal.SelectedIndex = 0;
            cmbVaccDay.SelectedIndex = 1;
            cmbVaccCity.SelectedIndex = 116;            
            cmbVaccCity_day_range.SelectedIndex = 2;

            isUpdate = true;
            updateDataAsync();
        }

        private void refresh()
        {
            updateVaccineTotal();
            updateVaccinseByDayTable();
            updateVaccinseByCityTable();

            int second = int.Parse(bl.getTotalVaccineByDay(1)[2].Last().Value.ToString());
            int first = int.Parse(bl.getTotalVaccineByDay(1)[1].Last().Value.ToString()) + second;

            if (second > 1000000)
                txtFirst.Text = String.Format("{0:0.00}", (double)first / 1000000) + "M";
            else if (second > 1000)
                txtFirst.Text = String.Format("{0:0.00}", (double)first / 1000) + "K";


            if (second > 1000000)
                txtSecond.Text = String.Format("{0:0.00}", (double)second / 1000000 ) + "M";
            else if (second > 1000)
                txtSecond.Text = String.Format("{0:0.00}", (double)second / 1000) + "K";
        }

        private void updateVaccineTotal()
        {
            int numOfDays = -1;

            switch (cmbVaccTotal.SelectedIndex)
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

            var VaccineByDay = bl.getTotalVaccineByDay(numOfDays);
            var first_by_day = VaccineByDay[1];
            var second_by_day = VaccineByDay[2];

            VaccinseChart.Series = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "Second",
                    Values = toChartValues(second_by_day)
                },
                new StackedAreaSeries
                {
                    Title = "First",
                    Values = toChartValues(first_by_day)
                }
            };
        }


        private void updateVaccinseByDayTable()
        {
            int numOfDays = 30;

            switch (cmbVaccDay.SelectedIndex)
            {
                case 0:
                    numOfDays = 60;
                    break;
                case 1:
                    numOfDays = 30;
                    break;
                case 2:
                    numOfDays = 7;
                    break;
            }


            var byDay = bl.getNewVaccinesByDay();

            var first = (from day in byDay where day.Key > DateTime.Now.AddDays(-numOfDays) select new { k = day.Key, val = day.Value[0] }).ToDictionary(t => t.k, t => t.val);
            var second = (from day in byDay where day.Key > DateTime.Now.AddDays(-numOfDays) select new { k = day.Key, val = day.Value[1] }).ToDictionary(t => t.k, t => t.val);

            var firstChartValues = (from day in first select day.Value).AsChartValues();
            var secondChartValues = (from day in second select day.Value).AsChartValues();

            Labels = (from day in first select day.Key.ToString("dd-MM")).ToList();

            DayChart.Series = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Values = firstChartValues,
                    Title = "First",
                },
                new StackedColumnSeries
                {
                   Values = secondChartValues,
                   Title = "Second",
                }
            };
        }


        private ChartValues<DateTimePoint> toChartValues(Dictionary<DateTime,int> dictionary)
        {
            ChartValues<DateTimePoint> ret = new ChartValues<DateTimePoint>();

            foreach (var day in dictionary)
                ret.Add(new DateTimePoint(day.Key, day.Value));

            return ret;
        }


        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;

            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        private async void btnVaccRef_Click(object sender, RoutedEventArgs e)
        {
            await updateDataAsync();
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

            await bl.download_data();
            refresh();

            thread.Abort();
        }

        private void cmbVaccTotal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(isUpdate)
                updateVaccineTotal();
        }

        private void cmbVaccDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdate)
                updateVaccinseByDayTable();
        }

        private void cmbVaccCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdate)
                updateVaccinseByCityTable();
        }

        private void updateVaccinseByCityTable()
        {
            string city = cmbVaccCity.SelectedItem.ToString();

            int numOfDays = -1;

            switch (cmbVaccCity_day_range.SelectedIndex)
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

            var VaccineByDay = bl.getTotalVaccineByDay(numOfDays, city);
            var first_by_day = VaccineByDay[1];
            var second_by_day = VaccineByDay[2];

            CityChart.Series = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "Second",
                    Values = toChartValues(second_by_day)
                },
                new StackedAreaSeries
                {
                    Title = "First",
                    Values = toChartValues(first_by_day)
                }
            };
        }

        private void cmbVaccCity_day_range_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdate)
                updateVaccinseByCityTable();
        }
    }
}
