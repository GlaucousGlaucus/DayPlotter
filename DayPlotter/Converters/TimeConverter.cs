using System;
using System.Globalization;
using System.Windows.Data;
using System.Linq;

namespace DayPolotter.Converters
{
    class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(double)) { 
                int seconds = (int) Math.Round((double)value, 0);
                TimeSpan time = TimeSpan.FromSeconds(seconds);
                int hours = time.Hours;
                int minutes = time.Minutes;
                seconds = time.Seconds;
                return (hours<10 ? "0"+ hours.ToString() : hours.ToString()) + ":" +
                    (minutes < 10 ? "0" + minutes.ToString() : minutes.ToString()) + ":" +
                    (seconds < 10 ? "0" + seconds.ToString() : seconds.ToString());
            }
            return "00:00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int currentTime = 0;
            string[] times = value.ToString().Split(":");
            for (int i = 0; i < times.Length; i++)
            {
                currentTime += (int) (int.Parse(times[i]) * Math.Pow(60, 2-i));
            }
            return currentTime;
        }
    }
}
