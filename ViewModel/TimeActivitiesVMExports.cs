using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timesheet.Model;
using Windows.Storage;
using System.IO;
using timesheet.Common;

namespace timesheet.ViewModel
{
    public class TimeActivitiesVMExports
    {
        DatabaseHelper Db_Helper = new DatabaseHelper();
        public ObservableCollection<TimeActivities> Activities { get; set; }

		/// <returns>The all elements.</returns>
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

        public async Task WriteToFile(ObservableCollection<TimeActivities> collection, StorageFolder folder, string fileName, bool subFolder)
        {
            StorageFile file;

            if (subFolder)
            {
                // Crea una nuova cartella in cui scrivere i dati
                var data_dir = await folder.CreateFolderAsync("exported", CreationCollisionOption.OpenIfExists);
                // Crea un nuovo file
                file = await data_dir.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            }
            

            byte[] header = Encoding.UTF8.GetBytes("Id;StartTime;Hours;EndTime;Hours;TimeAmount;Tag\n");
            byte[] encoded;

            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(header, 0, header.Length);

                foreach (var obj in collection)
                {
                    var row = obj.Id + ";" + obj.StartTime.ToString("dd/MM/yyyy") + ";" + obj.StartTime.ToString("HH:mm") + ";" + obj.EndTime.ToString("dd/MM/yyyy") + ";" + obj.EndTime.ToString("HH:mm") + ";" + obj.TimeAmount + ";" + obj.Tag + "\n";
                    encoded = Encoding.UTF8.GetBytes(row);
                    s.Write(encoded, 0, encoded.Length);
                }


            }

        }
    }
}
