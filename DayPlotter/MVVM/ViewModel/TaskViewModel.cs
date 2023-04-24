using DayPolotter.Core;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace DayPolotter.MVVM.ViewModel
{

    public class Item
    {
        public Item(int id, string task, DateTime addedon, string repeat, bool completed)
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

    class TaskViewModel : ObservableObject
    {
        Regex regex = new("([a-z]|[A-Z]|[0-9])+");
        public RelayCommand AddTaskBtnCmd { get; set; }
        public RelayCommand CompleteTaskBtnCmd { get; set; }
        private readonly ObservableCollection<Item> _taskItems;

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set {
                _selectedIndex = value; 
                OnPropertyChanged();
                if (value >=0)
                {
                    TaskEntry = _taskItems.ElementAtOrDefault(value).TaskName;
                }
            }
        }


        public ObservableCollection<Item> TaskItems
        {
            get { return _taskItems; }
        }

        private string _textBoxTaskEntry;

        public string TaskEntry
        {
            get { return _textBoxTaskEntry is null ? "" : _textBoxTaskEntry; }
            set { _textBoxTaskEntry = value; OnPropertyChanged(); }
        }

        private void AddTaskItem(MySqlConnection conn)
        {
            if (regex.IsMatch(TaskEntry))
            {
                string add_date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                string sql = String.Format("INSERT INTO Test_table (task, AddedOn, Completed, Repeat_Task) VALUES" +
                    "('{0}', '{1}', {2}, '{3}')", TaskEntry, add_date, false, "Never");
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteReader().Close();
                _taskItems.Add(new Item(_taskItems.ElementAtOrDefault(_taskItems.Count-1).ID, TaskEntry, DateTime.Now, "Never", false));
            } else
            {
                MessageBox.Show("Yuck! What kind of task is that ??");
            }
        }

        public void OnListItemSelect()
        {
            MessageBox.Show("YAY");
        }

        public TaskViewModel()
        {
            _taskItems = new ObservableCollection<Item>();

            string connStr = "server=localhost;user=root;database=dayplotter;port=3306;password=1234";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                string sql = "SELECT * FROM Test_table ORDER BY ID";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (int.TryParse(rdr[0].ToString(), out int taskid) & 
                        DateTime.TryParse(rdr[2].ToString(), out DateTime date_result)
                        & bool.TryParse(rdr[4].ToString(), out bool comp_result))
                    {
                        if (!comp_result)
                        {
                            _taskItems?.Add(new Item(taskid,
                                rdr[1].ToString(), 
                                date_result,
                                rdr[3].ToString(), 
                                comp_result));
                        }
                    }
                }
                rdr.Close();

                SelectedIndex = 0;

                AddTaskBtnCmd = new RelayCommand(o =>
                {
                    AddTaskItem(conn);
                });

                CompleteTaskBtnCmd = new RelayCommand(o =>
                {
                    Item selItem = _taskItems.ElementAtOrDefault(SelectedIndex);
                    if (selItem != null)
                    {
                        int selID = selItem.ID;
                        string sql = String.Format("DELETE FROM TEST_TABLE WHERE ID={0}", selItem.ID);
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteReader().Close();
                        _taskItems.RemoveAt(SelectedIndex);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
                SelectedIndex = 0;
                AddTaskBtnCmd = new RelayCommand(o =>{});
                CompleteTaskBtnCmd = new RelayCommand(o =>{});
            }
        }

    }
}
