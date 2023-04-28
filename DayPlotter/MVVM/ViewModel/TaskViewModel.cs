using DayPolotter.Core;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Linq;
using DayPlotter.MVVM.Models;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Controls;

namespace DayPolotter.MVVM.ViewModel
{

    class TaskViewModel : ObservableObject
    {
        Regex taskInRegex = new("([a-z]|[A-Z]|[0-9])+");
        char[] day_abbv_chars = new char[7] { 'S', 'M', 'T', 'W', 'A', 'F', 'B' };
        public RelayCommand AddTaskBtnCmd { get; set; }
        public RelayCommand CompleteTaskBtnCmd { get; set; }
        public RelayCommand SaveChangesBtnCmd { get; set; }
        public RelayCommand RepDaySaveCmd { get; set; }
        public RelayCommand DelTaskCmd { get; set; }

        private readonly ObservableCollection<TaskModel> _taskItems;

        private bool[] _repDays;

        public bool[] RepDays
        {
            get { return _repDays; }
            set { _repDays = value; OnPropertyChanged(); }
        }

        private bool _completeTaskBtnToggle;

        public bool CompleteTaskBtnToggle
        {
            get { return _completeTaskBtnToggle; }
            set { _completeTaskBtnToggle = value; OnPropertyChanged(); }
        }

        private bool _editTaskBtnToggle;
        
        public bool EditTaskBtnToggle
        {
            get { return _editTaskBtnToggle; }
            set { _editTaskBtnToggle = value; OnPropertyChanged(); }
        }

        private bool _deleteTaskBtnToggle;

        public bool DeleteTaskBtnToggle
        {
            get { return _deleteTaskBtnToggle; }
            set { _deleteTaskBtnToggle = value; OnPropertyChanged(); }
        }



        private bool _repSunday;

        public bool RepSunday
        {
            get { return _repSunday; }
            set { _repSunday = value; OnPropertyChanged(); }
        }

        private bool _repMonday;

        public bool RepMonday
        {
            get { return _repMonday; }
            set { _repMonday = value; OnPropertyChanged(); }
        }

        private bool _repTuesday;

        public bool RepTuesday
        {
            get { return _repTuesday; }
            set { _repTuesday = value; OnPropertyChanged(); }
        }

        private bool _repWednesday;

        public bool RepWednesday
        {
            get { return _repWednesday; }
            set { _repWednesday = value; OnPropertyChanged(); }
        }

        private bool _repThursday;

        public bool RepThursday
        {
            get { return _repThursday; }
            set { _repThursday = value; OnPropertyChanged(); }
        }

        private bool _repFriday;

        public bool RepFriday
        {
            get { return _repFriday; }
            set { _repFriday = value; OnPropertyChanged(); }
        }

        private bool _repSaturday;

        public bool RepSaturday
        {
            get { return _repSaturday; }
            set { _repSaturday = value; OnPropertyChanged(); }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                if (value >= 0)
                {
                    TaskModel? item = _taskItems.ElementAtOrDefault(value);
                    if (item != null)
                    {
                        TaskEntry = item.TaskName;
                        RepeatFreq = item.RepeatDays;
                        DateAdded = item.AddedOn;
                        bool[] reps = RepeatDayDecrypt(RepeatFreq);
                        RepSunday = reps[0];
                        RepMonday = reps[1];
                        RepTuesday = reps[2];
                        RepWednesday = reps[3];
                        RepThursday = reps[4];
                        RepFriday = reps[5];
                        RepSaturday = reps[6];
                        CompleteTaskBtnToggle = true;
                        EditTaskBtnToggle = true;
                        DeleteTaskBtnToggle = true;
                    }
                } else
                {
                    CompleteTaskBtnToggle = false;
                    EditTaskBtnToggle = false;
                    DeleteTaskBtnToggle = false;
                }
            }
        }

        private string _repFreq;

        public string RepeatFreq
        {
            get { return _repFreq; }
            set { _repFreq = value; OnPropertyChanged(); }
        }

        private DateTime _dateAdded;

        public DateTime DateAdded
        {
            get { return _dateAdded; }
            set { _dateAdded = value; OnPropertyChanged(); }
        }


        private string _textBoxTaskEntry;

        public string TaskEntry
        {
            get { return _textBoxTaskEntry is null ? "" : _textBoxTaskEntry; }
            set
            {
                _textBoxTaskEntry = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TaskModel> TaskItems
        {
            get { return _taskItems; }
        }


        private bool[] RepeatDayDecrypt(string repeatFreq)
        {
            bool[] rep_bool = new bool[7] { false, false, false, false, false, false, false };
            if (repeatFreq == "Never" || repeatFreq.Length > 7)
                return rep_bool;

            char[] chars = new char[7] { 'S', 'M', 'T', 'W', 'A', 'F', 'B' };

            for (int i = 0; i < rep_bool.Length; i++)
            {
                rep_bool[i] = repeatFreq.Contains(chars[i]);
            }

            return rep_bool;

        }

        private string RepeatDayEncrypt(bool[] repeatFreqBool)
        {
            if (repeatFreqBool.Length != 7 || repeatFreqBool.All(b => b == false)) return "Never";

            string ret = "";
            for (int i = 0; i < day_abbv_chars.Length; i++)
            {
                if (repeatFreqBool[i]) ret += day_abbv_chars[i].ToString();
            }
            return ret;

        }

        private void AddTaskItem(MySqlConnection conn)
        {
            if (!taskInRegex.IsMatch(TaskEntry))
            {
                MessageBox.Show("Invalid Task Entry!"); return;
            }
            bool[] repDays = new bool[7] { RepSunday, RepMonday,
                            RepTuesday, RepWednesday, RepThursday, RepFriday, RepSaturday};
            RepeatFreq = RepeatDayEncrypt(repDays);
            string add_date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            string sql = String.Format("INSERT INTO Test_table (task, AddedOn, Completed, Repeat_Task) VALUES" +
                "('{0}', '{1}', {2}, '{3}')", TaskEntry, add_date, false, RepeatFreq);
            new MySqlCommand(sql, conn).ExecuteNonQuery();
            sql = String.Format("Select MAX(ID) from Test_table");
            MySqlDataReader rdr = new MySqlCommand(sql, conn).ExecuteReader();
            while (rdr.Read())
            {
                if (int.TryParse(rdr[0].ToString(), out int sql_id))
                _taskItems.Add(new TaskModel(sql_id, TaskEntry, DateTime.Now, RepeatFreq, false));
            }
            rdr.Close();
        }

        private void EditTaskItem(MySqlConnection conn)
        {
            if (!taskInRegex.IsMatch(TaskEntry))
            {
                MessageBox.Show("Invalid Task Entry!"); return;
            }
            TaskModel? selItem = _taskItems.ElementAtOrDefault(SelectedIndex);
            if (selItem == null)
            {
                MessageBox.Show("Select a task first to update!"); return;
            }
            if (MessageBox.Show("Edit Task", "Do you want to update the selected task ?",
                MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No).Equals(MessageBoxResult.No)) return;
            int selID = selItem.ID;
            bool[] repDays = new bool[7] { RepSunday, RepMonday,
                            RepTuesday, RepWednesday, RepThursday, RepFriday, RepSaturday};
            RepeatFreq = RepeatDayEncrypt(repDays);
            string sql = String.Format("UPDATE TEST_TABLE SET TASK = \"{0}\", REPEAT_TASK=\"{1}\" WHERE ID={2}",
                TaskEntry, RepeatFreq, selID);
            new MySqlCommand(sql, conn).ExecuteNonQuery();
            _taskItems[SelectedIndex] = new TaskModel(selItem.ID, TaskEntry, selItem.AddedOn, RepeatFreq, selItem.Completed);
        }

        public TaskViewModel()
        {
            _taskItems = new ObservableCollection<TaskModel>();

            string connStr = "server=localhost;user=root;database=dayplotter;port=3306;password=1234";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                string sql = String.Format(
                    "UPDATE TEST_TABLE SET COMPLETED=FALSE WHERE REPEAT_TASK LIKE \"%{0}%\" and completed_on != \"{1}\""
                    , day_abbv_chars[(int)(DateTime.Now.DayOfWeek)], DateTime.Today.Date.ToString("yyyy-MM-dd"));
                new MySqlCommand(sql, conn).ExecuteNonQuery();
                sql = "SELECT * FROM Test_table ORDER BY ID";
                MySqlDataReader rdr = new MySqlCommand(sql, conn).ExecuteReader();
                while (rdr.Read())
                {
                    if (int.TryParse(rdr[0].ToString(), out int taskid) &
                        DateTime.TryParse(rdr[2].ToString(), out DateTime date_result)
                        & bool.TryParse(rdr[4].ToString(), out bool comp_result))
                    {
                        if (!comp_result)
                        {
                            _taskItems?.Add(new TaskModel(taskid,
                                rdr[1].ToString(), date_result, rdr[3].ToString(), comp_result));
                        }
                    }
                }
                rdr.Close();

                SelectedIndex = 0;

                AddTaskBtnCmd = new RelayCommand(o =>
                {
                    AddTaskItem(conn);
                    TaskEntry = "";
                });

                CompleteTaskBtnCmd = new RelayCommand(o =>
                {
                    TaskModel? selItem = _taskItems?.ElementAtOrDefault(SelectedIndex);
                    if (selItem != null)
                    {
                        int selID = selItem.ID;
                        string sql = String.Format(
                            "UPDATE TEST_TABLE SET COMPLETED = true , Completed_on=\"{0}\" WHERE ID={1}", DateTime.Today.Date.ToString("yyyy-MM-dd"), selID);
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        _taskItems?.RemoveAt(SelectedIndex);
                    }
                });

                SaveChangesBtnCmd = new RelayCommand(o =>
                {
                    EditTaskItem(conn);
                });

                RepDaySaveCmd = new RelayCommand(o =>
                {
                    TaskModel? selItem = _taskItems?.ElementAtOrDefault(SelectedIndex);
                    if (_taskItems != null & selItem != null)
                    {
                        int selID = selItem.ID;
                        bool[] repDays = new bool[7] { RepSunday, RepMonday,
                            RepTuesday, RepWednesday, RepThursday, RepFriday, RepSaturday};
                        RepeatFreq = RepeatDayEncrypt(repDays);
                        string sql = String.Format(
                            "UPDATE TEST_TABLE SET Repeat_task = \"{0}\" WHERE ID={1}",
                            RepeatFreq, selID);
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        int SelectedIndexPrev = SelectedIndex;
                        _taskItems[SelectedIndex] = new TaskModel
                        (selItem.ID, TaskEntry, selItem.AddedOn, RepeatFreq, selItem.Completed);
                        SelectedIndex = SelectedIndexPrev;
                    }
                });

                DelTaskCmd = new RelayCommand(o =>
                {
                    if (!taskInRegex.IsMatch(TaskEntry))
                    {
                        MessageBox.Show("Invalid Task Entry!"); return;
                    }
                    TaskModel? selItem = _taskItems.ElementAtOrDefault(SelectedIndex);
                    if (selItem == null)
                    {
                        MessageBox.Show("Select a task first to update!"); return;
                    }
                    string sql = String.Format("DELETE FROM TEST_TABLE WHERE ID={0}", selItem.ID);
                    new MySqlCommand(sql, conn).ExecuteNonQuery();
                    _taskItems.RemoveAt(SelectedIndex);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
                SelectedIndex = 0;
                AddTaskBtnCmd = new RelayCommand(o => { });
                CompleteTaskBtnCmd = new RelayCommand(o => { });
                SaveChangesBtnCmd = new RelayCommand(o => { });
            }
        }

    }
}
