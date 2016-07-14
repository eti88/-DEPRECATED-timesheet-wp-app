using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timesheet.Common;
using timesheet.Model;

namespace timesheet.ViewModel
{
    public class TimeActivitiesVMQueryList
    {
        DatabaseHelper Db_Helper = new DatabaseHelper();
        public ObservableCollection<TimeActivities> Activities { get; set; }

		/// <summary>
		/// Restituisce il risultao di una query, nello specifico resituisce
		/// la lista di activity presenti in un intervallo di date, associandolo
		/// alla vista.
		/// </summary>
		/// <returns>The query list.</returns>
		/// <param name="startDate">Start date.</param>
		/// <param name="endDate">End date.</param>
        public void GetQueryList(string startDate, string endDate)
        {
            var am = Db_Helper.QueryActivities(startDate, endDate);
            if (am != null && am.Count > 0)
            {
                Activities = am;
            }
             
        }
    }
}
