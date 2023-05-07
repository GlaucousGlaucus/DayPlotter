using DayPlotter.Core;
using System;

namespace DayPlotter.MVVM.Models
{
    public class TaskModel : ObservableObject
    {
        public TaskModel(int id, string task, DateTime addedon, string repeat, bool completed)
        {
            _id = id;
            _taskName = task;
            _addedon = addedon;
            _repeat = repeat;
            _completed = completed;
        }
        private int _id;
        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); }}
        private string _taskName;
        public string TaskName { get { return _taskName; } set { _taskName = value; OnPropertyChanged(); } }
        private DateTime _addedon;
        public DateTime AddedOn { get { return _addedon; } set { _addedon = value; OnPropertyChanged(); } }
        private bool _completed;
        public bool Completed { get { return _completed; } set { _completed = value; OnPropertyChanged(); } }
        private string _repeat;
        public string RepeatDays { get { return _repeat; } set { _repeat = value; OnPropertyChanged(); } }

    }
}
