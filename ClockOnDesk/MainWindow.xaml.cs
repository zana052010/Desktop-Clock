using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClockOnDesk
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _clockTimer;
        private DispatcherTimer _saveDebounceTimer;

        //################
        //##############################################################################

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        //##############################################################################
        //################

        public MainWindow()
        {
            InitializeComponent();

            SetSettings();

            LocationChanged += (_, _) => ScheduleSavePosition();
            SizeChanged += (_, _) => ScheduleSavePosition();
            Closing += (_, _) => SavePositionImmediate();
            Closing += MainWindow_Closing;

            StartClock();
            SetPositionWindowInBounds();

            //################
            //###################################################
            Loaded += (_, _) =>
            {
                var hwnd = new WindowInteropHelper(this).Handle;

                IntPtr style = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
                long styleValue = style.ToInt64();

                styleValue &= ~WS_EX_APPWINDOW;
                styleValue |= WS_EX_TOOLWINDOW;

                SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(styleValue));
            };
            //###################################################
            //################
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }


        public void SetSettings()
        {
            try
            {
                Time.FontSize = Properties.Settings.Default.FontSize;
                Time.Foreground = ParseBrushSafe(Properties.Settings.Default.FontColor);
                Time.FontFamily = new FontFamily(Properties.Settings.Default.Font);

                Date.FontSize = Properties.Settings.Default.FontSize / 3;
                Date.Foreground = ParseBrushSafe(Properties.Settings.Default.FontColor);
                Date.FontFamily = new FontFamily(Properties.Settings.Default.Font);

                this.Left = Properties.SettingsPosition.Default.Left;
                this.Top = Properties.SettingsPosition.Default.Top;
                

            }
            catch (Exception ex)
            {
                // Настройки повреждены/некорректны — используем значения по умолчанию,
                // но не роняем приложение
                System.Diagnostics.Debug.WriteLine($"SetSettings error: {ex.Message}");
            }
        }

        /// <summary>
        /// Безопасное преобразование строки в цвет. При ошибке возвращает белый цвет.
        /// </summary>
        private Brush ParseBrushSafe(string color)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(color))
                    return Brushes.White;

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
            catch
            {
                return Brushes.White;
            }
        }

        /// <summary>
        /// Проверка размеров/позиции окна перед первым отображением
        /// (защита от отрицательных/мусорных значений после сворачивания и т.п.)
        /// </summary>
        private void SetPositionWindowInBounds()
        {
            if (Width <= 0) Width = 300;
            if (Height <= 0) Height = 100;

            SavePositionImmediate();
        }

        /// <summary>
        /// Откладывает сохранение позиции окна, чтобы не писать в файл настроек
        /// на каждый пиксель при перетаскивании/ресайзе.
        /// </summary>
        private void ScheduleSavePosition()
        {
            // Игнорируем "мусорные" значения при сворачивании окна
            if (WindowState == WindowState.Minimized)
                return;

            if (Width <= 0 || Height <= 0)
                return;

            if (_saveDebounceTimer == null)
            {
                _saveDebounceTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(500)
                };
                _saveDebounceTimer.Tick += (_, _) =>
                {
                    _saveDebounceTimer.Stop();
                    SavePositionImmediate();
                };
            }

            _saveDebounceTimer.Stop();
            _saveDebounceTimer.Start();
        }

        private void SavePositionImmediate()
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

            Brush brush;
            try
            {
                brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
            catch
            {
                // Некорректная строка цвета — просто игнорируем изменение
                return;
            }

            Time.Foreground = brush;
            Date.Foreground = brush;

            Properties.Settings.Default.FontColor = color;
            Properties.Settings.Default.Save();
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
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += Timer_Tick_UpdateClock;
            _clockTimer.Start();

            // Обновляем сразу при запуске, не дожидаясь первого тика
            Timer_Tick_UpdateClock(null, EventArgs.Empty);
        }

        private void Timer_Tick_UpdateClock(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            if (Time != null)
            {
                Time.Text = now.ToString("HH:mm");
            }

            if (Date != null)
            {
                Date.Text = now.ToString("dd.MM.yy");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingWindow
            {
                Owner = this
            };
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