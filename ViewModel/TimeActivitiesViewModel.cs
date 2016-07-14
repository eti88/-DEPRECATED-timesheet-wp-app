using System.Collections.ObjectModel;
using System.ComponentModel;
using timesheet.Common;
using timesheet.Model;

namespace timesheet.ViewModel
{
    public class TimeActivitiesViewModel : INotifyPropertyChanged
    {
        
        DatabaseHelper Db_Helper = new DatabaseHelper();
        public ObservableCollection<TimeActivities> Activities { get; set; }

        public void GetTodayList()
        {
            if (Activities != null)
            {
                Activities.Clear();
            }
            
            var at = Db_Helper.ReadTodayActivities();
            if (at.Count > 0)
            {
                Activities = at;
            }
        }

        public void GetWeekList()
        {
            if(Activities != null)
            {
                Activities.Clear();
            }

            var aw = Db_Helper.ReadWeekActivities();
            if (aw.Count > 0)
            {
                Activities = aw;
            }
        }

        public void GetMonthList()
        {
            if (Activities != null)
            {
                Activities.Clear();
            }

            var am = Db_Helper.ReadMonthActivities();
            if (am.Count > 0) 
            {
                Activities = am;
            }
        }
        
        public void GetAllElements()
        {
            if (Activities != null)
            {
                Activities.Clear();
            }

            var tot = Db_Helper.ReadActivities();
            if (tot.Count > 0)
            {
                Activities = tot;
            }
        }
        
        public void Save_List()
        {
            foreach (TimeActivities a in Activities)
            {
                if (!Db_Helper.ActivityExist(a.Id))
                {
                    a.Save();
                }
            }
        }

        public void Clear()
        {
            if (Activities != null)
            {
                Activities.Clear();
            }
        }



        #region EventHandler

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChangeed(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
