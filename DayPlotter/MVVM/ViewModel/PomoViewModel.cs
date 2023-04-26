using DayPlotter.MVVM.Models;
using DayPolotter.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace DayPolotter.MVVM.ViewModel
{

    class PomoViewModel : ObservableObject
    {

        public RelayCommand StartTimer { get; set; }
        public RelayCommand ResetTimer { get; set; }
        public RelayCommand BreakTimeTimer { get; set; }
        public RelayCommand SaveCurrentPreset { get; set; }
        public RelayCommand DeletePreset { get; set; }

        private TimeSpan _startedFrom;

        private readonly ObservableCollection<TimerPresetModel> _timePresets;

        public ObservableCollection<TimerPresetModel> TimePresets
        {
            get { return _timePresets; }
        }

        private int _selectedPresetIndex;

        public int SelectedPresetIndex
        {
            get { return _selectedPresetIndex; }
            set { _selectedPresetIndex = value; OnPropertyChanged(); _setTimers(); }
        }



        public TimeSpan StartedFrom
        {
            get { return _startedFrom; }
            set { _startedFrom = value; OnPropertyChanged(); }
        }

        private bool _isNormalTime;

        public bool IsNormalTime
        {
            get { return _isNormalTime; }
            set { _isNormalTime = value; OnPropertyChanged(); }
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


        private void _timer_ticks(object sender, EventArgs e)
        {
            CurrentTime -= new TimeSpan(0, 0, 1);
            if (CurrentTime.TotalSeconds <= 0)
            {
                _timer.Stop();
                IsFinished = true;
                IsNormalTime = true;
                StartStopText = StartStopBtnText();

            }
        }

        private string _btn_start_text = "Pl";
        private string _btn_stop_text = "Pa";

        public string StartStopBtnText()
        {
            return _timer.IsEnabled ? _btn_stop_text : _btn_start_text;
        }

        private void _setTimers()
        {
            TimerPresetModel presetModel = _timePresets.ElementAtOrDefault(SelectedPresetIndex);
            if (presetModel != null)
            {
                CountDownTime = presetModel.NormalTime;
                BreakTime = presetModel.BreakTime;
            } else
            {
                CountDownTime = new TimeSpan(0, 25, 0);
                BreakTime = new TimeSpan(0, 5, 0);
            }
            if (!_timer.IsEnabled) CurrentTime = CountDownTime;
        }

        private void readSqlData(MySqlConnection conn)
        {
            try
            {
                conn.Open();
                string sql = "SELECT * FROM TIME_PRESETS ORDER BY ID";
                MySqlDataReader reader = new MySqlCommand(sql, conn).ExecuteReader();
                while (reader.Read())
                {
                    if (int.TryParse(reader[0].ToString(), out int ind) &
                        TimeSpan.TryParse(reader[1].ToString(), out TimeSpan NormTime) &
                        TimeSpan.TryParse(reader[2].ToString(), out TimeSpan BrkTime))
                    {
                        _timePresets.Add(new TimerPresetModel(ind, NormTime, BrkTime));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR IN READING SQL");
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public PomoViewModel()
        {
            _startStopText = _btn_start_text;
            _isNormalTime = true;
            _selectedPresetIndex = 0;
            _timePresets = new ObservableCollection<TimerPresetModel>();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(_timer_ticks);

            string connStr = "server=localhost;user=root;database=dayplotter;port=3306;password=1234";
            MySqlConnection conn = new MySqlConnection(connStr);
            readSqlData(conn);
            _setTimers();
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
            BreakTimeTimer = new RelayCommand(_ =>
            {
                if (_timer.IsEnabled) _timer.Stop();
                StartedFrom = CurrentTime = BreakTime;
                _timer.Start();
                IsFinished = false;
                IsNormalTime = false;
                StartStopText = StartStopBtnText();
            });
            SaveCurrentPreset = new RelayCommand(o => 
            {
                TimeSpan normTime = _timer.IsEnabled ? StartedFrom : CurrentTime;
                string sql = string.Format("INSERT INTO TIME_PRESETS (NORM, BREAK) VALUES (\"{0}\", \"{1}\")",
                    normTime.ToString(), BreakTime.ToString());
                new MySqlCommand(sql, conn).ExecuteNonQuery();
                sql = "SELECT MAX(ID) FROM TIME_PRESETS";
                if (int.TryParse(new MySqlCommand(sql, conn).ExecuteScalar().ToString(), out int ind))
                _timePresets.Add(new TimerPresetModel(ind, normTime, BreakTime));
            });
            DeletePreset = new RelayCommand(o => 
            {
                TimerPresetModel? selItem = _timePresets.ElementAtOrDefault(SelectedPresetIndex);
                if (selItem == null)
                {
                    MessageBox.Show("Select a task first to update!"); return;
                }
                string sql = string.Format("DELETE FROM TIME_PRESETS WHERE ID={0}", selItem.ID);
                new MySqlCommand(sql, conn).ExecuteNonQuery();
                _timePresets.RemoveAt(SelectedPresetIndex);
            });
        }
    }
}
