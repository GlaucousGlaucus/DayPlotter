using System;

namespace DayPlotter.MVVM.Models
{
    public class TimerPresetModel
    {
        public TimerPresetModel(int id, TimeSpan normalTime, TimeSpan breakTime) 
        {
            ID = id;
            NormalTime = normalTime;
            BreakTime = breakTime;
        }

        public int ID { get; set; }
        public TimeSpan NormalTime { get; set; }
        public TimeSpan BreakTime { get; set; }
    }

}
