using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace DayPolotter.MVVM.View
{
    public partial class PomoView : UserControl
    {
        public PomoView()
        {
            InitializeComponent();
        }

        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
