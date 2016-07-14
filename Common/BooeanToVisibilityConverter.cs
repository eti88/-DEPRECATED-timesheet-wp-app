using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI;


namespace timesheet.Common
{
	/// <summary>
	/// In caso di pubblicità rimossa si occupa di non farle vedere
	/// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {

        private bool triggerValue = false;
        public bool TriggerValue
        {
            get { return triggerValue; }
            set
            {
                triggerValue = value;
            }
        }

        private bool isHidden;
        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; }
        }

        private object GetVisibility(object value)
        {
            if (!(value is bool))
                return DependencyProperty.UnsetValue;

            bool objValue = (bool)value;
            if((objValue && TriggerValue && IsHidden) || (!objValue && !TriggerValue && isHidden)) {
                return Visibility.Collapsed;
            }

            if ((objValue && TriggerValue && !IsHidden) || (!objValue && !TriggerValue && !IsHidden))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //string formatString = parameter as string;
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}
