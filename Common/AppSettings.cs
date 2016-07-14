using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timesheet.Common
{
	/// <summary>
	/// Questa classe serve per controllare se "l'in-app purchase" associato all'eliminazione
	/// della pullblicità è attivo o pure no.
	/// </summary>

    public class AppSettings
    {
        private static bool _displayAds = true;
        private static bool _isAdBlockerActive = false;
        public static bool IsAdBlockerActive
        {
            get { return _isAdBlockerActive; }
            set
            {
                _isAdBlockerActive = value;
            }
        }

        public static bool DisplayAds
        {
            get { return _displayAds; }
            set
            {
                _displayAds = value;
            }
        }
    }
}
