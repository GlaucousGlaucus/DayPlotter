using DayPolotter.Core;
using System;
using System.Windows.Threading;

namespace DayPolotter.MVVM.ViewModel
{

    class PomoViewModel : ObservableObject
    {

        public RelayCommand StartTimer { get; set; }
        public RelayCommand ResetTimer { get; set; }
        public RelayCommand BreakTimeTimer { get; set; }

        private TimeSpan _startedFrom;

        public TimeSpan StartedFrom
        {
            get { return _startedFrom; }
            set { _startedFrom = value; OnPropertyChanged(); }
        }


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


        private DispatcherTimer _timer;
        public DispatcherTimer Timer
        {
            get { return _timer; }
            set { _timer = value; OnPropertyChanged(); }
        }

        private void timer_ticks(object sender, EventArgs e)
        {
            CurrentTime -= new TimeSpan(0, 0, 1);
            if (CurrentTime.TotalSeconds <= 0)
            {
                _timer.Stop();
                IsFinished = true;
                StartStopText = StartStopBtnText();
                
            }
        }

        private string _startStopText;

        public string StartStopText
        {
            get { return _startStopText; }
            set { _startStopText = value; OnPropertyChanged(); }
        }

        private TimeSpan _countdownTime;

        public TimeSpan CountDownTime
        {
            get { return _countdownTime; }
            set { _countdownTime = value; OnPropertyChanged(); }
        }

        private TimeSpan _breakTime;

        public TimeSpan BreakTime
        {
            get { return _breakTime; }
            set { _breakTime = value; OnPropertyChanged(); }
        }



        //TODO the timers configurable
        //private TimeSpan COUNTDOWN_TIME = new TimeSpan(0, 25, 0);
        //private TimeSpan BREAK_TIME = new TimeSpan(0, 5, 0);
        private string _btn_start_text = "Pl";
        private string _btn_stop_text = "Pa";


        public string StartStopBtnText()
        {
            return _timer.IsEnabled ? _btn_stop_text : _btn_start_text;
        }

        public PomoViewModel()
        {
            StartStopText = _btn_start_text;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(timer_ticks);
            CountDownTime = new TimeSpan(0, 25, 0);
            BreakTime = new TimeSpan(0, 5, 0);
            CurrentTime = CountDownTime;
            IsFinished = false;
            StartTimer = new RelayCommand(o =>
            {
                if (_timer.IsEnabled) { _timer.Stop(); }
                else
                {
                    if (CurrentTime.TotalSeconds <= 0) CurrentTime = StartedFrom;
                    _timer.Start();
                    StartedFrom = CurrentTime;
                    IsFinished = false;
                }
                StartStopText = StartStopBtnText();
            });
            ResetTimer = new RelayCommand(o =>
            {
                if (_timer.IsEnabled) _timer.Stop();
                CurrentTime = StartedFrom;
                StartStopText = StartStopBtnText();
            });
            BreakTimeTimer = new RelayCommand(_ => {
                if (_timer.IsEnabled) _timer.Stop();
                StartedFrom = CurrentTime = BreakTime;
                _timer.Start();
                IsFinished = false;
                StartStopText = StartStopBtnText();
            });
        }
    }
}
