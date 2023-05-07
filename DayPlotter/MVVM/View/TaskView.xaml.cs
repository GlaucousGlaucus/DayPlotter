using System.Windows;
using System.Windows.Controls;

namespace DayPlotter.MVVM.View
{
    public partial class TaskView : UserControl
    {
        public TaskView()
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
