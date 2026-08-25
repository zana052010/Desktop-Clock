using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClockOnDesk
{

    public partial class SettingWindow : Window
    {
        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontComboBox.SelectedItem is ComboBoxItem item)
            {
                string font = item.Content.ToString();

                MainWindow mainWindow = (MainWindow)Owner;

                mainWindow.ChangeFont(font);
            }
        }
        private void FontComboBox_SelectionChangeSize(object sender, SelectionChangedEventArgs e)
        {
            if (FontComboBoxSize.SelectedItem is ComboBoxItem item)
            {
                if (double.TryParse(item.Content.ToString(), out double size))
                {
                    MainWindow mainWindow = (MainWindow)Owner;

                    mainWindow.ChangeFontSize(size + 10);
                }
            }
        }
        private void FontComboBox_SelectionChangedColor(object sender, SelectionChangedEventArgs e)
        {
            if (FontComboBoxColor.SelectedItem is ComboBoxItem item)
            {
                string color = item.Content.ToString();

                MainWindow mainWindow = (MainWindow)Owner;

                mainWindow.ChangeFontColor(color);
            }
        }

        public SettingWindow()
        {
            InitializeComponent();
        }


    }
}
