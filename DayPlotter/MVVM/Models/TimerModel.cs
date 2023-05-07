using DayPlotter.Core;
using System;
using System.Windows.Threading;

namespace DayPlotter.MVVM.Models
{
    public class TimerModel : ObservableObject
    {
        private static TimerModel _instance;
        private DispatcherTimer _timer;

        private string _btn_start_text = "Play";
        private string _btn_stop_text = "Pause";

        public event EventHandler TimerStarted;
        public event EventHandler TimerStopped;
        public event EventHandler TimeElapsed;

        private TimeSpan _startedFrom;

        public TimeSpan StartedFrom
        {
            get { return _startedFrom; }
            set { _startedFrom = value; OnPropertyChanged(); }
        }

        private TimeSpan _currentTime;

        public TimeSpan CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; OnPropertyChanged(); }
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

        public bool IsEnabled
        {
            get { return _timer.IsEnabled; }
        }


        private bool _isNormalTime;

        public bool IsNormalTime
        {
            get { return _isNormalTime; }
            set { _isNormalTime = value; OnPropertyChanged(); }
        }

        private TimerModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_ticks;
            IsNormalTime = true;
        }

        public static TimerModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TimerModel();
                }
                return _instance;
            }
        }


        public void Toggle()
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                TimerStopped?.Invoke(this, EventArgs.Empty);
                return;
            }
            StartedFrom = CurrentTime;
            _timer.Start();
            TimerStarted?.Invoke(this, EventArgs.Empty);
            IsNormalTime = true;
        }

        public void Break()
        {
            if (_timer.IsEnabled)
            {
                TimerStopped?.Invoke(this, EventArgs.Empty);
                _timer.Stop();
            }
            StartedFrom = CurrentTime = BreakTime;
            _timer.Start();
            IsNormalTime = false;
            TimerStarted?.Invoke(this, EventArgs.Empty);
        }

        public void Reset()
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                CurrentTime = StartedFrom;
            }
        }

        private void Timer_ticks(object sender, EventArgs e)
        {
            CurrentTime -= new TimeSpan(0, 0, 1);
            if (CurrentTime.TotalSeconds <= 0)
            {
                _timer.Stop();
                IsNormalTime = true;
            }
        }

        public string StartStopBtnText()
        {
            return _timer.IsEnabled ? _btn_stop_text : _btn_start_text;
        }

    }
}
