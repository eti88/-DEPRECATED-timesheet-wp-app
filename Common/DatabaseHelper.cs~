using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timesheet.Model;

namespace timesheet.Common
{
	/// <summary>
	/// Database helper, con operazioni permesse incorporate.
	/// Per il suo corretto funzionamento deve essere stato installato "sqlite-net" da nuget.
	/// </summary>
    public  class DatabaseHelper
    {
        SQLiteConnection dbConn;

		/// <summary>
		/// Crea le tabelle predefinite per il corretto funzionamento del database
		/// </summary>
		/// <returns>true se operazione eseguita con successo.</returns>
		/// <param name="DB_PATH">Percors locaz. database</param>
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<TimeActivities>();
                        dbConn.CreateTable<ActivityModel>();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

		/// <summary>
		/// Controlla se il file esiste nella memoria (fisica).
		/// </summary>
		/// <returns>True se viene trovato il file</returns>
		/// <param name="filename">Filename path.</param>
        private async Task<bool> CheckFileExists(string filename)
        {
            try {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                return true;
            }
            catch {
                return false;
            }
        }

		/// <summary>
		/// Controlla se esiste l'activity specificata dall'id
		/// </summary>
		/// <returns>True se la query ritorna un risultato.</returns>
		/// <param name="id">id associato</param>
        public bool ActivityExist(int id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingActivity = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE Id =" + id).FirstOrDefault();
                if (existingActivity.Id == id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
			
		/// <summary>
		/// Restitusice l'oggetto "TimeActivities" dal database
		/// </summary>
		/// <returns>La timeActivities specifica.</returns>
		/// <param name="activityId">id associato</param>
        public TimeActivities ReadActivity(int activityId)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingActivity = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE Id =" + activityId).FirstOrDefault();
                return existingActivity;
            }
        }

		/// <summary>
		/// Restituisce le Activity relative a oggi
		/// </summary>
		/// <returns>Una ObservableCollection di timeActivities</returns>
        public ObservableCollection<TimeActivities> ReadTodayActivities()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<TimeActivities> myCollection = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE startTime BETWEEN datetime('now', 'start of day') AND datetime('now', 'localtime') ORDER BY startTime DESC;");
                ObservableCollection<TimeActivities> activitiesTodayList = new ObservableCollection<TimeActivities>(myCollection);
                return activitiesTodayList;
            }
        }
        
		/// <summary>
		/// Restituisce le Activity relative alla settimana.
		/// </summary>
		/// <returns>Una ObservableCollection di timeActivities</returns>
        public ObservableCollection<TimeActivities> ReadWeekActivities()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {   
                List<TimeActivities> myCollection = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE startTime BETWEEN datetime('now', '-6 days') AND datetime('now', 'localtime') ORDER BY startTime DESC;");
                ObservableCollection<TimeActivities> activitiesWeekList = new ObservableCollection<TimeActivities>(myCollection);
                return activitiesWeekList;
            }
        }

        /// <summary>
        /// Restituisce le Activity relative al mese.
        /// </summary>
		/// <returns>Una ObservableCollection di timeActivities</returns>
        public ObservableCollection<TimeActivities> ReadMonthActivities()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<TimeActivities> myCollection = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE startTime BETWEEN datetime('now', 'start of month') AND datetime('now', 'localtime') ORDER BY startTime DESC;");
                ObservableCollection<TimeActivities> activitiesMonthList = new ObservableCollection<TimeActivities>(myCollection);
                return activitiesMonthList;
            }
        }

        /// <summary>
        /// Restituisce tutte le Activity in un range di date. Le date sono passate sotto forma di stringhe
		/// nel formato del database.
        /// </summary>
		/// <returns>Una ObservableCollection di timeActivities</returns>
        /// <param name="startDate">Data di partenza</param>
        /// <param name="endDate">Data di fine</param>
        public ObservableCollection<TimeActivities> QueryActivities(string startDate, string endDate)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<TimeActivities> myCollection = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE startTime BETWEEN '" + startDate + "' AND '" + endDate +"' ORDER BY startTime DESC;");
                ObservableCollection<TimeActivities> activities = new ObservableCollection<TimeActivities>(myCollection);
                return activities;
            }
        }

        /// <summary>
        /// Recupera dal database tutte le Activity presenti nel database.
        /// </summary>
		/// <returns>Una ObservableCollection di timeActivities</returns>
        public ObservableCollection<TimeActivities> ReadActivities()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<TimeActivities> myCollection = dbConn.Table<TimeActivities>().ToList<TimeActivities>();
                ObservableCollection<TimeActivities> activitiesList = new ObservableCollection<TimeActivities>(myCollection);
                return activitiesList;
            }
        }

        /// <summary>
        /// Modifica una Activity già presente nel database.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="activity">Activity specificata</param>
        public void UpdateActivity(TimeActivities activity)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingActivity = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE Id =" + activity.Id).FirstOrDefault();
                if (existingActivity != null)
                {
                    existingActivity.StartTime = activity.StartTime;
                    existingActivity.EndTime = activity.EndTime;
                    existingActivity.TimeAmount = activity.TimeAmount;
                    existingActivity.Tag = activity.Tag;
                    existingActivity.IsStarted = activity.IsStarted;
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingActivity);
                    });
                }
            }
        }

        /// <summary>
        /// Inserisce una Activity nel database
        /// </summary>
        /// <param name="newActivity">Activity da inserire.</param>
        public void Insert(TimeActivities newActivity)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newActivity);
                });
            }
        }

        /// <summary>
        /// Elimina dal database una specifica Activity passando il suo id.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="Id">Id Activity</param>
        public void DeleteActivity(int Id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingActivity = dbConn.Query<TimeActivities>("SELECT * FROM TimeActivities WHERE Id =" + Id).FirstOrDefault();
                if (existingActivity != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingActivity);
                    });
                }
            }
        }

        /// <summary>
        /// Elimina tutte le Activity dal database
        /// </summary>
        /// <returns>void</returns>
        public void DeleteAllActivities()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.DropTable<TimeActivities>();
                dbConn.CreateTable<TimeActivities>();
                dbConn.Dispose();
                dbConn.Close();
            }
        }

        /// <summary>
        /// Restituisce i modelli dal database
        /// </summary>
        /// <returns>Il modello specifico</returns>
        /// <param name="modelId">id del modello</param>
        public ActivityModel ReadModel(int modelId)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingModel = dbConn.Query<ActivityModel>("SELECT * FROM ActivityModel WHERE id =" + modelId).FirstOrDefault();
                return existingModel;
            }
        }

        /// <summary>
        /// Preleva dal database tutti i modelli presenti.
        /// </summary>
		/// <returns>Una ObservableCollection di ActivityModel.</returns>
        public ObservableCollection<ActivityModel> ReadModels()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<ActivityModel> myCollection = dbConn.Table<ActivityModel>().ToList<ActivityModel>();
                ObservableCollection<ActivityModel> modelsList = new ObservableCollection<ActivityModel>(myCollection);
                return modelsList;
            }
        }

        /// <summary>
        /// Modifica le informazioni di un Model già presente nel database
        /// </summary>
        /// <returns>void</returns>
        /// <param name="modelz">ActivityModel da modificare</param>
        public void UpdateModel(ActivityModel modelz)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingModel = dbConn.Query<ActivityModel>("SELECT * FROM ActivityModel WHERE id =" + modelz.id).FirstOrDefault();
                if (existingModel != null)
                {
                    existingModel.name = modelz.name;
                    existingModel.description = modelz.description;
                    
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingModel);
                    });
                }
            }
        }

        /// <summary>
        /// Inserisce ndel database una nuova ActivityModel
        /// </summary>
        /// <returns>void</returns>
        /// <param name="newModel">ActivityModel da salvare</param>
        public void InsertModel(ActivityModel newModel)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newModel);
                });
            }
        }

        /// <summary>
        /// Elimina dal database un specifico modello.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="Id">id ActivityModel</param>
        public void DeleteModel(int Id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingModel = dbConn.Query<ActivityModel>("SELECT * FROM ActivityModel WHERE id =" + Id).FirstOrDefault();
                if (existingModel != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingModel);
                    });
                }
            }
        }

      	/// <summary>
      	/// Elimina tutti gli ActivityModel presenti nel database.
      	/// </summary>
      	/// <returns>void</returns>
        public void DeleteAllModels()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.DropTable<ActivityModel>();
                dbConn.CreateTable<ActivityModel>();
                dbConn.Dispose();
                dbConn.Close();
            }
        }

    }
}
