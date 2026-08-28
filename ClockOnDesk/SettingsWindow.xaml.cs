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
            if (mainWindow == null)
                return;

            if (FontComboBox.SelectedItem == null)
                return;

            if (FontComboBox.SelectedItem is ComboBoxItem item)
            {
                string? font = item.Content.ToString();



                mainWindow.ChangeFont(font);
            }
        }
        private void FontComboBox_SelectionChangeSize(object sender, SelectionChangedEventArgs e)
        {
            if (mainWindow == null)
                return;

           

            if (FontComboBoxSize.SelectedItem is ComboBoxItem item)
            {
                if (double.TryParse(item.Content.ToString(), out double size))
                {


                    mainWindow.ChangeFontSize(size);
                }
            }
        }
        private void FontComboBox_SelectionChangedColor(object sender, SelectionChangedEventArgs e)
        {

            if (mainWindow == null)
                return;

            if (FontComboBoxColor.SelectedItem is ComboBoxItem item)
            {
                string? color = item.Content.ToString();

                mainWindow.ChangeFontColor(color);
            }
        }

        private void CheckVox_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.Date.Visibility = Visibility.Collapsed;
            CheckDate.IsChecked = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainWindow.Date.Visibility = Visibility.Visible;
            CheckDate.IsChecked = false;
        }

        private MainWindow mainWindow;

        public SettingWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
           
        }

    }
}
