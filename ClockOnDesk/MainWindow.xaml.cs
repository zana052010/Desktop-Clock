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




        public void ChangeFontSize(double size)
        {
            if (size <= 0)
                return;

            Time.FontSize = size;
            Date.FontSize = size;
        }

        public void ChangeFontColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return;

            try
            {
                Brush brush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(color));

                Time.Foreground = brush;
                Date.Foreground = brush;
            }
            catch
            {
               
            }
        }

        public void ChangeFont(string font)
        {
            if (string.IsNullOrWhiteSpace(font))
                return;

            Time.FontFamily = new FontFamily(font);
            Date.FontFamily = new FontFamily(font);
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
                Date.Text = DateTime.Now.ToString("dd.MM.yyyy");
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