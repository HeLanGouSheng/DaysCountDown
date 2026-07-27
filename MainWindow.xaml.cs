using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MediaColor = System.Windows.Media.Color;
using MediaColorConverter = System.Windows.Media.ColorConverter;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace CountdownDay
{
    public partial class MainWindow : Window
    {
        private AppDataContainer _appData = new();
        private System.Windows.Forms.NotifyIcon? _notifyIcon;

        public double CurrentBgOpacity => _appData.BackgroundOpacity;
        public double CurrentTextOpacity => _appData.TextOpacity;

        public MainWindow()
        {
            InitializeComponent();
            InitNotifyIcon();
            Loaded += MainWindow_Loaded;
            LoadData();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTaskbarMenuItemStatus(_appData.ShowInTaskbar);
            UpdateLanguageMenuCheck(_appData.Language);
        }

        private void InitNotifyIcon()
        {
            try
            {
                var trayMenu = new System.Windows.Forms.ContextMenuStrip();
                var exitItem = new System.Windows.Forms.ToolStripMenuItem(App.GetString("MenuExit"));
                exitItem.Click += (s, e) => MenuExit_Click(s, new RoutedEventArgs());
                trayMenu.Items.Add(exitItem);

                _notifyIcon = new System.Windows.Forms.NotifyIcon
                {
                    Icon = new System.Drawing.Icon("app.ico"),
                    Visible = true,
                    Text = "CountdownDay",
                    ContextMenuStrip = trayMenu
                };

                _notifyIcon.DoubleClick += (s, e) =>
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"托盘图标加载失败: {ex.Message}");
            }
        }

        private void LoadData()
        {
            _appData = StorageManager.LoadData();

            // 还原保存的语言并应用
            App.SwitchLanguage(_appData.Language);

            ShowInTaskbar = _appData.ShowInTaskbar;

            if (_appData.Events.Count == 0)
            {
                _appData.Events.Add(new EventItem { Title = App.GetString("DefaultEventTitle"), TargetDate = DateTime.Today.AddDays(7) });
            }

            ApplyBgOpacity(_appData.BackgroundOpacity);
            ApplyTextOpacity(_appData.TextOpacity);
            UpdateUI();
        }

        // 切换语言：中文
        private void MenuLangZh_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("zh-CN");
        }

        // 切换语言：英文
        private void MenuLangEn_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("en-US");
        }

        private void SetLanguage(string langCode)
        {
            _appData.Language = langCode;
            App.SwitchLanguage(langCode);
            StorageManager.SaveData(_appData);

            UpdateLanguageMenuCheck(langCode);
            UpdateTaskbarMenuItemStatus(_appData.ShowInTaskbar);
            UpdateUI();
        }

        // 保证语言菜单单选打勾状态正确
        private void UpdateLanguageMenuCheck(string langCode)
        {
            if (MenuLangZh != null) MenuLangZh.IsChecked = (langCode == "zh-CN");
            if (MenuLangEn != null) MenuLangEn.IsChecked = (langCode == "en-US");
        }

        private void UpdateTaskbarMenuItemStatus(bool isShow)
        {
            if (MenuShowTaskbar != null)
            {
                MenuShowTaskbar.IsChecked = isShow;
                MenuShowTaskbar.Header = isShow ? App.GetString("MenuTaskbarOn") : App.GetString("MenuTaskbarOff");
            }
        }

        private void MenuShowTaskbar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                ShowInTaskbar = menuItem.IsChecked;
                _appData.ShowInTaskbar = menuItem.IsChecked;
                UpdateTaskbarMenuItemStatus(menuItem.IsChecked);
                StorageManager.SaveData(_appData);
            }
        }

        public void ApplyBgOpacity(double opacity)
        {
            _appData.BackgroundOpacity = opacity;
            byte alpha = (byte)(opacity * 255);
            MainBackgroundBorder.Background = new SolidColorBrush(MediaColor.FromArgb(alpha, 0x2D, 0x2D, 0x30));
        }

        public void ApplyTextOpacity(double opacity)
        {
            _appData.TextOpacity = opacity;
            MainContentGrid.Opacity = opacity;
        }

        private void UpdateUI()
        {
            EventListView.ItemsSource = null;
            EventListView.ItemsSource = _appData.Events;
        }

        private void BtnPin_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            BtnPin.Foreground = Topmost ? new SolidColorBrush((MediaColor)MediaColorConverter.ConvertFromString("#007ACC")) 
                                        : new SolidColorBrush((MediaColor)MediaColorConverter.ConvertFromString("#888888"));
            BtnPin.ToolTip = Topmost ? App.GetString("UnpinToolTip") : App.GetString("PinToolTip");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void EventCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement element && element.DataContext is EventItem item)
            {
                OpenEditDialog(item);
            }
        }

        private void MenuEditItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is EventItem item)
            {
                OpenEditDialog(item);
            }
        }

        private void MenuDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is EventItem item)
            {
                _appData.Events.Remove(item);
                StorageManager.SaveData(_appData);
                UpdateUI();
            }
        }

        private void OpenEditDialog(EventItem item)
        {
            var editWin = new EditWindow(item) { Owner = this };
            if (editWin.ShowDialog() == true)
            {
                StorageManager.SaveData(_appData);
                UpdateUI();
            }
        }

        private void MenuAdd_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new EventItem { Title = App.GetString("NewEventTitle"), TargetDate = DateTime.Today.AddDays(7) };
            var editWin = new EditWindow(newItem) { Owner = this };

            if (editWin.ShowDialog() == true)
            {
                _appData.Events.Add(newItem);
                StorageManager.SaveData(_appData);
                UpdateUI();
            }
        }

        private void MenuOpacity_Click(object sender, RoutedEventArgs e)
        {
            double oldBg = _appData.BackgroundOpacity;
            double oldText = _appData.TextOpacity;

            var opacityWin = new OpacityWindow(this) { Owner = this };

            if (opacityWin.ShowDialog() == true)
            {
                StorageManager.SaveData(_appData);
            }
            else
            {
                ApplyBgOpacity(oldBg);
                ApplyTextOpacity(oldText);
            }
        }

        private void MenuClearOld_Click(object sender, RoutedEventArgs e)
        {
            _appData.Events.RemoveAll(x => x.IsExpired);
            StorageManager.SaveData(_appData);
            UpdateUI();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            System.Windows.Application.Current.Shutdown();
        }
    }
}