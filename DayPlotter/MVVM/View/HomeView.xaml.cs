using System.Windows;
using System.Windows.Controls;

namespace DayPolotter.MVVM.View
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void FlowDocumentScrollViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            FlowDocumentScrollViewer v = sender as FlowDocumentScrollViewer;
            v.Document = null;
        }
    }
}
