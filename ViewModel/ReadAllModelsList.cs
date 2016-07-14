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
    public class ReadAllModelsList
    {
        DatabaseHelper Db_Helper = new DatabaseHelper();

		/// <summary>
		/// Quando richiamata restituisce tramite il DatabaseHelper tutti i modelli presenti nel
		/// database.
		/// </summary>
		/// <returns>Tutti i modelli presenti nel db</returns>
        public ObservableCollection<ActivityModel> GetAllModels()
        {
            return Db_Helper.ReadModels();
        }
    }
}
