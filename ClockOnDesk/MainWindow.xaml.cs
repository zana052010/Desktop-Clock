
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Xps.Serialization;

namespace ClockOnDesk
{
    public partial class MainWindow : Window
    {

        private DispatcherTimer _timer;
        public string SelectedFont { get; set; }
        public int SelectedFontSize { get; set; }
        public string SelectedFontColor { get; set; }
                                    //################
        //##############################################################################

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        //##############################################################################
                                    //################
        public MainWindow()
        {
            InitializeComponent();
            StartClock();
            LocationChanged += (_, _) => SetPositionWindow();
            SizeChanged += (_, _) => SetPositionWindow();
            SetSettings();
            SetPositionWindow();
                            //################
            //###################################################
            Loaded += (_, _) =>
            {
                var hwnd = new WindowInteropHelper(this).Handle;

                int style = GetWindowLong(hwnd, GWL_EXSTYLE);

                style &= ~WS_EX_APPWINDOW;
                style |= WS_EX_TOOLWINDOW;

                SetWindowLong(hwnd, GWL_EXSTYLE, style);
            };
            //###################################################
                            //################
        }

        public void SetSettings()
        {
            
            Time.FontSize = Properties.Settings.Default.FontSize;
            Date.FontSize = Properties.Settings.Default.FontSize / 3;

            
            string colorString = Properties.Settings.Default.FontColor;

            Color color;

            try
            {
                color = (Color)ColorConverter.ConvertFromString(colorString);
            }
            catch
            {
                color = Colors.White;

                Properties.Settings.Default.FontColor = color.ToString();
                Properties.Settings.Default.Save();
            }

            Brush brush = new SolidColorBrush(color);

            Time.Foreground = brush;
            Date.Foreground = brush;
            string font = Properties.Settings.Default.Font;

            if (string.IsNullOrWhiteSpace(font))
                font = "Arial";

            Time.FontFamily = new FontFamily(font);
            Date.FontFamily = new FontFamily(font);
            try
            {
                this.Left = Properties.SettingsPosition.Default.Left;
                this.Top = Properties.SettingsPosition.Default.Top;
                this.Width = Properties.SettingsPosition.Default.Width;
                this.Height = Properties.SettingsPosition.Default.Height;
            }
            catch
            {
                this.Left = 100;
                this.Top = 100;
                this.Width = 300;
                this.Height = 150;
            }
        }



        public void SetPositionWindow()
        {
            Properties.SettingsPosition.Default.Left = Left;
            Properties.SettingsPosition.Default.Top = Top;
            Properties.SettingsPosition.Default.Width = Width;
            Properties.SettingsPosition.Default.Height = Height;
            Properties.SettingsPosition.Default.Save();
            
        }

        public void ChangeFontSize(double size)
        {
            if (size <= 0)
                return;

            Time.FontSize = size;
            ReworkChangeFontSize(size, Date);
            Properties.Settings.Default.FontSize = size;
            Properties.Settings.Default.Save();
        }

        public void ReworkChangeFontSize(double size, TextBlock text)
        {
            text.FontSize = size / 3;
        }

        public void ChangeFontColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return;

            try
            {
                Color parsedColor =
                    (Color)ColorConverter.ConvertFromString(color);

                Brush brush = new SolidColorBrush(parsedColor);

                Time.Foreground = brush;
                Date.Foreground = brush;

                Properties.Settings.Default.FontColor =
                    parsedColor.ToString();

                Properties.Settings.Default.Save();
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    $"{color}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        public void ChangeFont(string font)
        {
            if (string.IsNullOrWhiteSpace(font))
                return;

            Time.FontFamily = new FontFamily(font);
            Date.FontFamily = new FontFamily(font);
            Properties.Settings.Default.Font = font;
            Properties.Settings.Default.Save();
        }



        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick_For_Date;
            _timer.Tick += Timer_Tick_For_Days;
            _timer.Start();
        }

        private void Timer_Tick_For_Date(object sender, EventArgs e)
        {
       
                //ChangeFont(Date, SelectedFont);
            if (Time != null)
            {
                Time.Text = DateTime.Now.ToString("HH:mm");
            }
        }
        private void Timer_Tick_For_Days(object sender, EventArgs e)
        {
     
            if (Time != null)
            {
                Date.Text = DateTime.Now.ToString("dd.MM.yy");
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingsWindow = new SettingWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Запускаем перетаскивание только если левая кнопка действительно нажата
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }


    }
}