using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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

        public MainWindow()
        {
            InitializeComponent();
            StartClock();
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
    }
}