using DayPolotter.Core;
using System;
using System.Reflection;
using System.Windows.Threading;

namespace DayPolotter.MVVM.ViewModel
{

    class PomoViewModel : ObservableObject
    {

        public RelayCommand StartTimer { get; set; }
        private DispatcherTimer _timer;

        private bool _isFinished;

        public bool IsFinished
        {
            get { return _isFinished; }
            set { _isFinished = value; OnPropertyChanged(); }
        }


        private double _currentTimeText;

        public double CurrentTimeText
        {
            get { return _currentTimeText; }
            set { _currentTimeText = value; OnPropertyChanged(); }
        }


        private TimeSpan _currentTime;

        public TimeSpan CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; OnPropertyChanged(); }
        }

        public DispatcherTimer Timer
        {
            get { return _timer; }
            set { _timer = value; OnPropertyChanged(); }
        }

        private void timer_ticks(object sender, EventArgs e)
        {
            CurrentTime -= new TimeSpan(0,0,1);
            if (CurrentTime.TotalSeconds == 0)
            {
                _timer.Stop();
                IsFinished = true;
                StartStopText = "Start";
            }
        }

        private string _startStopText;

        public string StartStopText
        {
            get { return _startStopText; }
            set { _startStopText = value; OnPropertyChanged(); Console.WriteLine(value);}
        }

        private TimeSpan CONTDOWN_TIME_IN_SEC = new TimeSpan(0,25,0);


        public string StartStopBtnText()
        {
            if (_timer.IsEnabled)
            {
                return "Stop";
            }
            return "Start";
        }

        public PomoViewModel()
        {
            StartStopText = "Start";
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(timer_ticks);
            CurrentTime = CONTDOWN_TIME_IN_SEC;
            IsFinished = false;
            StartTimer = new RelayCommand(o =>
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                    StartStopText = "Start";
                } else
                {
                    if (CurrentTime.TotalSeconds <= 0)
                    {
                        CurrentTime = CONTDOWN_TIME_IN_SEC;
                    }
                    _timer.Start();
                    StartStopText = "Stop";
                    IsFinished = false;
                }
            });
        }
    }
}
