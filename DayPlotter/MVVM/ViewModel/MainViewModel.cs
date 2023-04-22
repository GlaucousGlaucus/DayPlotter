using DayPolotter.Core;
using System;

namespace DayPolotter.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand PomoViewCommand { get; set; }
        public RelayCommand TaskViewCommand { get; set; }

        public HomeViewModel HomeViewModel { get; set; }
        public PomoViewModel PomoViewModel { get; set; }
        public TaskViewModel TaskViewModel { get; set; }

        private Object _currentView;

        public Object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }


        public MainViewModel()
        {
            HomeViewModel = new HomeViewModel();
            PomoViewModel = new PomoViewModel();
            TaskViewModel = new TaskViewModel();
            CurrentView = HomeViewModel;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeViewModel;
            });

            PomoViewCommand = new RelayCommand(o =>
            {
                CurrentView = PomoViewModel;
            });

            TaskViewCommand = new RelayCommand(o =>
            {
                CurrentView = TaskViewModel;
            });
        }
    }
}
