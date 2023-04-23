using System.Windows;
using System.Windows.Input;

namespace DayPolotter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxNormWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal) 
            {
                WindowState = WindowState.Maximized; 
            } else
            {
                WindowState = WindowState.Normal;
            }
            
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
