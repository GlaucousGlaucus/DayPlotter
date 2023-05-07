using System;
using System.Globalization;
using System.Windows.Data;

namespace DayPlotter.Converters
{
    class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Input: hhmmss
            if (value.GetType() == typeof(TimeSpan))
            {
                TimeSpan ts = (TimeSpan)value;
                Console.WriteLine(ts.ToString());
                return ts;
            }
            return value.GetType().ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TimeSpan.TryParse(value.ToString(), out TimeSpan time))
            {
                return time;
            }
            return value.GetType().ToString();
                //string[] times = value.ToString().Split(":");
            //int times_int = 0;
            //for (int i = 0; i < times.Length; i++)
            //{
            //    if (int.TryParse(times[i], out int time_int))
            //    {
            //        int j = times.Length - i - 1;
            //        times_int += (int) (time_int * Math.Pow(100, j));
            //    }
            //}
            //return times_int;
        }
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value.GetType() == typeof(double)) {
        //        int timeIn = (int)Math.Round((double)value, 0);
        //        int hours = timeIn/(100*100);
        //        int minutes = timeIn / (100);
        //        seconds = time.Seconds;
        //        return (hours<10 ? "0"+ hours.ToString() : hours.ToString()) + ":" +
        //            (minutes < 10 ? "0" + minutes.ToString() : minutes.ToString()) + ":" +
        //            (seconds < 10 ? "0" + seconds.ToString() : seconds.ToString());
        //    }
        //    return "00:00:00";
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    int currentTime = 0;
        //    string[] times = value.ToString().Split(":");
        //    for (int i = 0; i < times.Length; i++)
        //    {
        //        currentTime += (int) (int.Parse(times[i]) * Math.Pow(60, 2-i));
        //    }
        //    return currentTime;
        //}
    }
}
