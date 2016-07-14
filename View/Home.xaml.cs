using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using timesheet.Model;
using timesheet.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using Windows.Storage;
using System.Collections;
using timesheet.Common;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace timesheet.View
{


    public sealed partial class Home : Page
    {
        #region Attributes

        bool _timerRunning = false;
        bool _timerStopped = false;
        
        TimeActivities currentAc;

        private TimeActivitiesViewModel vm_day;
        private TimeActivitiesViewModel vm_week;
        private TimeActivitiesViewModel vm_month;

        private readonly NavigationHelper navigationHelper;
        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        
        #endregion

        public Home()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.SaveState += this.SaveState;
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            Application.Current.Resuming += this.NavigationHelper_OnResume;
        }

        #region StateAppHandler

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private void NavigationHelper_OnResume(object sender, object e)
        {
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && currentAc != null && e.PageState.ContainsKey("savedActivity"))
            {
                this.currentAc = currentAc.DeserializeFromString(e.PageState["savedActivity"] as string);
                
                //UpdateTile("No activity", "running");
                txtNoRunning.Visibility = Visibility.Collapsed;
                spinningRing.IsActive = true;
                ChangeButtonState();
                abortTimerButton.IsEnabled = true;
                e.PageState.Remove("savedActivity");

            }else
            {
                txtNoRunning.Visibility = Visibility.Visible;
                spinningRing.IsActive = false;
                ChangeButtonState();
            }
 
        }
 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        
        private void SaveState(object sender, SaveStateEventArgs e)
        {
            if (_timerRunning)
            {
                string current = currentAc.SerializeToString();
                e.PageState.Add("savedActivity", current);
                //e.PageState["savedActivity"] = current;
            } 
        }

        #endregion

        #region LoadContent

        // Carica il contenuto della combobox dal database
        private void cmbBoxLoad(object sender, RoutedEventArgs e)
        {
            ReadAllModelsList models = new ReadAllModelsList();
            var modelsList = models.GetAllModels();


            if (modelsList.Count > 0)
            {
                cmbTypeActivity.ItemsSource = modelsList;

            }
            else
            {
                cmbTypeActivity.PlaceholderText = "No models found";

            }
        }

        #region ListViewDay

        public void DayListView_Load(object sender, RoutedEventArgs e)
        {
            
            vm_day = new TimeActivitiesViewModel();
            vm_day.GetTodayList();
            var day = vm_day.Activities;
            
            if (day != null && day.Count > 0)
            {
                listViewDay.ItemsSource = day;
                txtNoItemsFoundD.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoItemsFoundD.Visibility = Visibility.Visible;
            }

        }

        public async Task listViewDay_refresh(object s, EventArgs e)
        {
            if (vm_day != null)
            {
                vm_day.Clear();
            }else{
                vm_day = new TimeActivitiesViewModel();
            }

            vm_day.GetTodayList();
            var day = vm_day.Activities;

            if (day != null &&  day.Count > 0)
            {
                listViewDay.ItemsSource = day;
                txtNoItemsFoundD.Visibility = Visibility.Collapsed;
            }else{
                txtNoItemsFoundD.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region ListViewWeek

        public void WeekListView_Load(object sender, RoutedEventArgs e)
        {

            vm_week = new TimeActivitiesViewModel();
            vm_week.GetWeekList();
            var week = vm_week.Activities;

            if (week != null && week.Count > 0)
            {
                listViewWeek.ItemsSource = week;
                txtNoItemsFoundW.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoItemsFoundW.Visibility = Visibility.Visible;
            }

        }

        public async Task listViewWeek_refresh(object s, EventArgs e)
        {
            if (vm_week != null)
            {
                vm_week.Clear();
            }
            else
            {
                vm_week = new TimeActivitiesViewModel();
            }

            vm_week.GetWeekList();
            var week = vm_week.Activities;

            if (week != null && week.Count > 0)
            {
                listViewWeek.ItemsSource = week;
                txtNoItemsFoundW.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoItemsFoundW.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region ListViewMonth

        public void MonthListView_Load(object sender, RoutedEventArgs e)
        {

            vm_month = new TimeActivitiesViewModel();
            vm_month.GetMonthList();
            var month = vm_month.Activities;

            if (month != null && month.Count > 0)
            {
                listViewMonth.ItemsSource = month;
                txtNoItemsFoundM.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoItemsFoundM.Visibility = Visibility.Visible;
            }

        }

        public async Task listViewMonth_refresh(object s, EventArgs e)
        {
            if (vm_month != null)
            {
                vm_month.Clear();
            }
            else
            {
                vm_month = new TimeActivitiesViewModel();
            }

            vm_month.GetMonthList();
            var month = vm_month.Activities;

            if (month != null && month.Count > 0)
            {
                listViewMonth.ItemsSource = month;
                txtNoItemsFoundM.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoItemsFoundM.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Charts

        private IEnumerable LoadChart(string TypeOfResult, TimeActivitiesViewModel vm)
        {
            switch (TypeOfResult)
            {
                case "day":
                    vm.GetTodayList();
                    break;
        
                case "week":
                    vm.GetWeekList();
                    break;

                case "month":
                    vm.GetMonthList();
                    break;

                case "total":
                    vm.GetAllElements();
                    break;

                default:
                    vm.GetTodayList();
                    break;
            }

            var collection = vm.Activities;

            if (collection != null && collection.Count > 0)
            {
                var result = collection.GroupBy(element => element.CodeCat)
                            .Select(grp => new TimeActivities
                            {
                                Tag = grp.Select(x => x.Tag).FirstOrDefault(),
                                TimeAmount = grp.Sum(el => el.TimeAmount)
                            });

                return result;
            }

            return null;
        }

        private void ChartToday_Load(object sender, RoutedEventArgs e)
        {
            if (vm_day == null)
            {
                vm_day = new TimeActivitiesViewModel();
            }

            var res = LoadChart("day", vm_day);

            if (res != null) { (PieChartToday.Series[0] as PieSeries).ItemsSource = res; }
            else
            {
                var l = new List<TimeActivities>();
                l.Add(new TimeActivities { Tag = "null", TimeAmount = 100 });
                (PieChartToday.Series[0] as PieSeries).ItemsSource = l;
            }
            
        }

        private void ChartWeek_Load(object sender, RoutedEventArgs e)
        {
            if (vm_week == null)
            {
                vm_week = new TimeActivitiesViewModel();
            }
 
            var res = LoadChart("week", vm_week);
            if (res != null) { (PieChartWeek.Series[0] as PieSeries).ItemsSource = res; }
            else {
                var l = new List<TimeActivities>();
                l.Add(new TimeActivities { Tag = "null", TimeAmount = 100 });
                (PieChartWeek.Series[0] as PieSeries).ItemsSource = l;
            }
            
        }

        private void ChartMonth_Load(object sender, RoutedEventArgs e)
        {
            if (vm_month == null)
            {
                vm_month = new TimeActivitiesViewModel();
            }
            
            var res = LoadChart("month", vm_day);
            if (res != null) { (PieChartMonth.Series[0] as PieSeries).ItemsSource = res; }
            else{
                var l = new List<TimeActivities>();
                l.Add(new TimeActivities { Tag = "null", TimeAmount = 100 });
                (PieChartMonth.Series[0] as PieSeries).ItemsSource = l;
            }
           
        }

        private void ChartTotal_Load(object sender, RoutedEventArgs e)
        {
            var vm_total = new TimeActivitiesViewModel();
            var res = LoadChart("total", vm_total);
            if(res != null) { (PieChartTotal.Series[0] as PieSeries).ItemsSource = res; }
            else{
                 var l = new List<TimeActivities>();
                l.Add(new TimeActivities { Tag = "null", TimeAmount = 100 });
                (PieChartTotal.Series[0] as PieSeries).ItemsSource = l;
            } 
        }

        private void ReloadGraphs_Click(object sender, RoutedEventArgs e)
        {
            if (vm_day == null) { vm_day = new TimeActivitiesViewModel(); }
            if (vm_month == null) { vm_month = new TimeActivitiesViewModel(); }
            if (vm_week == null) { vm_week = new TimeActivitiesViewModel(); }
            var vm_total = new TimeActivitiesViewModel();
            (PieChartToday.Series[0] as PieSeries).ItemsSource = LoadChart("day", vm_total); ;
            (PieChartWeek.Series[0] as PieSeries).ItemsSource = LoadChart("week", vm_total); ;
            (PieChartMonth.Series[0] as PieSeries).ItemsSource = LoadChart("month", vm_total);
            (PieChartTotal.Series[0] as PieSeries).ItemsSource = LoadChart("total", vm_total);
        }

        #endregion

        #endregion

       
        #region TimerControlButtons
   
        // Start timer Click button
        private void startTimerClick(object sender, RoutedEventArgs e)
        {
            if (!_timerRunning && cmbTypeActivity.SelectedItem != null)
            {
                txtNoRunning.Visibility = Visibility.Collapsed;
                _timerRunning = true;
                ChangeButtonState();
                spinningRing.IsActive = true;
                abortTimerButton.IsEnabled = true;
                string acType = (string)(cmbTypeActivity.SelectedItem as ActivityModel).name;
                currentAc = new TimeActivities(acType, DateTime.Now);
                currentAc.CodeCat = (int)(cmbTypeActivity.SelectedItem as ActivityModel).id;

            }else if(_timerRunning) {

                txtNoRunning.Visibility = Visibility.Visible;
                spinningRing.IsActive = false;
                _timerRunning = false;
                _timerStopped = true;
                ChangeButtonState();           
                abortTimerButton.IsEnabled = false;

                if (currentAc != null)
                {
                    currentAc.End();
                    currentAc.Calc_Duration();
                    if (currentAc.TimeAmount > 0)
                    {
                        currentAc.Save();
                    }

                }
            }
            else if (cmbTypeActivity.SelectedItem == null) {
                // far qualcosa?

            }

        }

        // Stop timer Click button
        private void stopTimerClick(object sender, RoutedEventArgs e)
        {
            if (_timerRunning)
            {
                txtNoRunning.Visibility = Visibility.Visible;
                spinningRing.IsActive = false;

                _timerRunning = false;
                _timerStopped = true;

                
                //stopTimerButton.IsEnabled = false;
                abortTimerButton.IsEnabled = false;

                if (currentAc != null)
                {
                    currentAc.End();
                    currentAc.Calc_Duration();
                    if (currentAc.TimeAmount > 0)
                    {
                        currentAc.Save();
                    }
                    
                }
                
            }
        }

        // Abort activity Click button
        private void abortTimerClick(object sender, RoutedEventArgs e)
        {
            if (_timerRunning || _timerStopped)
            {
                
                _timerRunning = false;
                _timerStopped = false;
                txtNoRunning.Visibility = Visibility.Visible;
                spinningRing.IsActive = false;
                ChangeButtonState();                
                abortTimerButton.IsEnabled = false;
            }
        }

        public void ChangeButtonState()
        {
            if (!_timerRunning)
            {
                startTimerButton.Style = (Style)Application.Current.Resources["ButtonStartDarkTh"];
            }
            else
            {
                startTimerButton.Style = (Style)Application.Current.Resources["ButtonStopDarkTh"];
            }
        }

        #endregion

        #region AppBarButtonsAction

        private void MenuItem_Models(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ReadModelsList));
        }

        private void MenuItem_About(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void MenuItem_Export(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ExportData));
        }

        private void MenuItem_Settings(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }
        
        private void MenuItem_AdsBlock(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RemoveAds));
        }
        
        #endregion

        private void SearchQuery_Click(object sender, RoutedEventArgs e)
        {
            string[] parameters = new String[2];
            parameters[0] = startQueryTimeDt.Date.ToString("yyyy-MM-dd");
            parameters[1] = endQueryTimeDt.Date.ToString("yyyy-MM-dd");

            Frame.Navigate(typeof(QueryActivitiesList), parameters);
        }

        public void UpdateTile( string tile, string subtitle)
        {
            if (settings.Values.ContainsKey("liveTile") && (bool)settings.Values["liveTile"] == true)
            {
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueue(true);
                updater.Clear();

                var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);
                var tileImage = tileXml.GetElementsByTagName("image")[0] as XmlElement;
                tileImage.SetAttribute("src", "ms-appx:///Assets/SmallLogo.scale-240.png");
                var tileText = tileXml.GetElementsByTagName("text");

                (tileText[0] as XmlElement).InnerText = tile;
                (tileText[1] as XmlElement).InnerText = subtitle;

                var tileNotification = new TileNotification(tileXml);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            }

        }
    }
}
