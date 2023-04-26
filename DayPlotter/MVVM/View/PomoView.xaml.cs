using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Windows;

namespace DayPolotter.MVVM.View
{
    public partial class PomoView : UserControl
    {
        public PomoView()
        {
            InitializeComponent();
        }

        public void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            TextBox box = ((TextBox)e.Source);
            char[] chars = box.Text.ToCharArray();
            box.CaretIndex += chars.ElementAtOrDefault(box.CaretIndex).ToString() == ":" ? 1 : 0;
            bool flag = regex.IsMatch(e.Text) || (box.CaretIndex >= 8) || (box.CaretIndex < 0);
            if (!flag)
            {
                box.SelectionStart = box.CaretIndex;
                box.SelectionLength = 1;
            }
            e.Handled = flag;
        }

        public void TimeBox_KeyPrev(object sender, KeyEventArgs e)
        {
            int del_or_bak = e.Key == Key.Back ? -1 : e.Key == Key.Delete ? 1 : 0;
            bool keyPresses = e.Key == Key.Space;
            if (del_or_bak != 0)
            {
                TextBox box = ((TextBox)e.Source);
                char[] chars = box.Text.ToCharArray();
                box.CaretIndex += chars.ElementAtOrDefault(box.CaretIndex + (del_or_bak == -1 ? -1 : 0)).ToString() == ":" ? del_or_bak : 0;
                e.Handled = box.CaretIndex < 0 || box.CaretIndex > 8;
            }
            e.Handled = e.Handled || keyPresses;
        }

    }
}
