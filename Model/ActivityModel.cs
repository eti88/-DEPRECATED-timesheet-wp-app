using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timesheet.Model
{
	/// <summary>
	/// Activity model, Permette la definizione delle attività da monitorare
	/// nel tempo. L'attributo "id" viente utilizzato come primary key autoinc.
	/// all'interno del database.
	/// </summary>
    public class ActivityModel
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public ActivityModel() { }

        public ActivityModel(string n, string d)
        {
            this.name = n;
            this.description = d;
        }
    }
}
