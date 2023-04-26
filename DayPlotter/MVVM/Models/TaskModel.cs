using System;

namespace DayPlotter.MVVM.Models
{
    public class TaskModel
    {
        public TaskModel(int id, string task, DateTime addedon, string repeat, bool completed)
        {
            ID = id;
            TaskName = task;
            AddedOn = addedon;
            RepeatDays = repeat;
            Completed = completed;
        }
        public int ID { get; set; }
        public string TaskName { get; set; }
        public DateTime AddedOn { get; set; }
        public bool Completed { get; set; }
        public string RepeatDays { get; set; }
    }
}
