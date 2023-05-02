using DayPlotter.MVVM.Models;
using DayPolotter.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace DayPolotter.MVVM.ViewModel
{

    class PomoViewModel : ObservableObject
    {

        public RelayCommand StartTimer { get; set; }
        public RelayCommand ResetTimer { get; set; }
        public RelayCommand BreakTimeTimer { get; set; }
        public RelayCommand SaveCurrentPreset { get; set; }
        public RelayCommand DeletePreset { get; set; }

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

        public TimerModel PomoTimer = TimerModel.Instance;

        public TimeSpan CurrTime
        {
            get { return PomoTimer.CurrentTime; }
            set { PomoTimer.CurrentTime = value; OnPropertyChanged(); }
        }

        public bool IsItNormalTime
        {
            get { return PomoTimer.IsNormalTime; }
            set { PomoTimer.IsNormalTime = value; OnPropertyChanged(); }
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

        private void _setTimers()
        {
            TimerPresetModel? presetModel = _timePresets.ElementAtOrDefault(SelectedPresetIndex);
            if (presetModel == null)
            {
                CountDownTime = new TimeSpan(0, 25, 0);
                BreakTime = new TimeSpan(0, 5, 0);
                return;
            }
            CountDownTime = presetModel.NormalTime;
            PomoTimer.BreakTime = BreakTime = presetModel.BreakTime;
            if (!PomoTimer.IsEnabled) PomoTimer.CurrentTime = CountDownTime;
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

        private void OnMySingletonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PomoTimer.CurrentTime))
            {
                OnPropertyChanged(nameof(CurrTime));
            }
            if (e.PropertyName == nameof(PomoTimer.IsNormalTime))
            {
                OnPropertyChanged(nameof(IsItNormalTime));
            }
        }

        public PomoViewModel()
        {
            _selectedPresetIndex = 0;
            _timePresets = new ObservableCollection<TimerPresetModel>();

            PomoTimer.PropertyChanged += OnMySingletonPropertyChanged;

            string connStr = "server=localhost;user=root;database=dayplotter;port=3306;password=1234";
            MySqlConnection conn = new MySqlConnection(connStr);
            readSqlData(conn);
            _setTimers();

            StartTimer = new RelayCommand(o =>
            {
                PomoTimer.Toggle();
                StartStopText = PomoTimer.StartStopBtnText();
            });

            ResetTimer = new RelayCommand(o =>
            {
                PomoTimer.Reset();
                StartStopText = PomoTimer.StartStopBtnText();
            });

            BreakTimeTimer = new RelayCommand(o =>
            {
                PomoTimer.Break();
                StartStopText = PomoTimer.StartStopBtnText();
            });

            SaveCurrentPreset = new RelayCommand(o =>
            {
                TimeSpan normTime = PomoTimer.IsEnabled ? PomoTimer.StartedFrom : PomoTimer.CurrentTime;
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