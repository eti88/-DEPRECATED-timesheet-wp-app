using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using timesheet.Common;

namespace timesheet.Model
{
	/// <summary>
	/// Time activities, modello costituito da una data di inizio, una di fine,
	/// codice relativo alla categoria, descrizione se presente e l'ultima voce
	/// riguarda il fatto se la "timeactivity" era già in esecuzione sul dispositivo prima
	/// della sospensione dell'applicazione.
	/// </summary>
    public class TimeActivities : INotifyPropertyChanged
    {

        DatabaseHelper Db_Helper = new DatabaseHelper();

        public TimeActivities() { }

        public TimeActivities(string ta, DateTime start)
        {
            Tag = ta;
            StartTime = start;
        }

        public TimeActivities(string desc, DateTime start, DateTime end, int code, bool iss) {
            StartTime = start;
            EndTime = end;
            CodeCat = code;
            Tag = desc;
            IsStarted = iss;
        }

        #region Attributes

        // id utilizzato per il l'indicizzazione del database
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        // indica quando comincia una determinata attività
        private DateTime startTime;
        public DateTime StartTime 
        {
            get { return startTime; }
            set 
            {
                startTime = value;
                OnPropertyChangeed("StartTime");
            }
        }

        public string ShortStartTimeDate
        {
            get
            {
                if (startTime != null)
                {
                    return startTime.ToString("dd/MM/yyyy");
                }
                return null;
            }
        }

        public string ShortStartTime
        {
            get
            {
                if (startTime != null)
                {
                    return startTime.ToString("HH:mm");
                }
                return null;
            }
        }

        // Indica quando finisce una determinata attività
        private DateTime endTime;
        public DateTime EndTime {
            get { return endTime; }
            set
            {
                endTime = value;
                OnPropertyChangeed("EndTime");
            }
        }

        public string ShortEndTimeDate
        {
            get
            {
                if (endTime != null)
                {
                    return endTime.ToString("dd/MM/yyyy");
                }
                return null;
            }
        }

        public string ShortEndTime
        {
            get
            {
                if (endTime != null)
                {
                    return endTime.ToString("HH:mm");
                }
                return null;
            }
        }


        private DateTime suspendedTime;
        [SQLite.Ignore]
        public DateTime SuspendedTime 
        {
            get { return suspendedTime; }
            set
            {
                suspendedTime = value;
                OnPropertyChangeed("SuspendedTime");
            }
        }

        // Indica i minuti totali dell'attività svolta espressa in minuti
        private int timeAmount;
        public int TimeAmount 
        {
            get { return timeAmount; }
            set
            {
                timeAmount = value;
                OnPropertyChangeed("TimeAmount");
            }
        }

        // Contiene l'eticetta dell'attività svolta come ad esempio "programmare"
        private string tag;
        public string Tag 
        {
            get { return tag; }
            set
            {
                tag = value;
                OnPropertyChangeed("Tag");
            }
        }

        // Contine l'id del modello dell'attività svolta per poterle raggruppare successivamente sotto forma di grafico
        private int codeCat;
        public int CodeCat 
        {
            get { return codeCat; }
            set
            {
                codeCat = value;
                OnPropertyChangeed("CodeCat");
            }
        }

        // Valore booleano per sapere se l'attività è attualmente in esecuzione
        private Boolean isStarted;
        public Boolean IsStarted 
        {
            get { return isStarted; }
            set
            {
                isStarted = value;
                OnPropertyChangeed("IsStarted");
            }
        }

        #endregion


        #region Methods

        public void Start() {
            StartTime = DateTime.Now;
            IsStarted = true;
        }

        public void Abort()
        {
            IsStarted = false;
        }

        public void End()
        {
            EndTime = DateTime.Now;
            IsStarted = false;

        }

        public void Save()
        {
            Db_Helper.Insert(this);
        }

        public void Calc_Duration()
        {
            if (StartTime != DateTime.MinValue && EndTime != DateTime.MinValue)
            {
                var tA = StartTime.Ticks;
                var tB = EndTime.Ticks;
                var totTicks = tB - tA;
                TimeSpan res = new TimeSpan(totTicks);
                int total = (int)res.TotalMinutes;

                if (res.Seconds < 30)
                {
                    TimeAmount = total;
                }
                else
                {
                    TimeAmount = total + 1;
                }
            }
        }

        public TimeSpan Calc_OnResume()
        {
            if (SuspendedTime != null)
            {
                var time = SuspendedTime.Subtract(StartTime);
                return time;
            }
            return TimeSpan.Zero;
        }

        public string SerializeToString() {
            string serializedData = string.Empty;

            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using(StringWriter sw = new StringWriter()) {
                serializer.Serialize(sw, this);
                serializedData = sw.ToString();
            }
            return serializedData;
        }

        public TimeActivities DeserializeFromString(string serializedData)
        {
            TimeActivities deserializedAc;

            XmlSerializer deserializer = new XmlSerializer(typeof(TimeActivities));
            using (TextReader tr = new StringReader(serializedData))
            {
                deserializedAc = (TimeActivities)deserializer.Deserialize(tr);
            }
            return deserializedAc;
        }

        #endregion

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
