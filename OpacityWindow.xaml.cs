using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CountdownDay
{
    public partial class OpacityWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private bool _isLoaded = false;

        public OpacityWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            int bgVal = (int)Math.Round(_mainWindow.CurrentBgOpacity * 100);
            int textVal = (int)Math.Round(_mainWindow.CurrentTextOpacity * 100);

            SldBgOpacity.Value = bgVal;
            TxtBgOpacity.Text = bgVal.ToString();

            SldTextOpacity.Value = textVal;
            TxtTextOpacity.Text = textVal.ToString();

            _isLoaded = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        // 背景透明度事件
        private void SldBgOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isLoaded) return;
            int val = (int)e.NewValue;
            if (TxtBgOpacity != null && TxtBgOpacity.Text != val.ToString())
            {
                TxtBgOpacity.Text = val.ToString();
            }
            _mainWindow.ApplyBgOpacity(val / 100.0);
        }

        private void TxtBgOpacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;
            if (int.TryParse(TxtBgOpacity.Text, out int val))
            {
                val = Math.Clamp(val, 0, 100);
                if (SldBgOpacity != null && Math.Abs(SldBgOpacity.Value - val) > 0.1)
                {
                    SldBgOpacity.Value = val;
                }
                _mainWindow.ApplyBgOpacity(val / 100.0);
            }
        }

        // 文字透明度事件
        private void SldTextOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isLoaded) return;
            int val = (int)e.NewValue;
            if (TxtTextOpacity != null && TxtTextOpacity.Text != val.ToString())
            {
                TxtTextOpacity.Text = val.ToString();
            }
            _mainWindow.ApplyTextOpacity(val / 100.0);
        }

        private void TxtTextOpacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;
            if (int.TryParse(TxtTextOpacity.Text, out int val))
            {
                val = Math.Clamp(val, 20, 100);
                if (SldTextOpacity != null && Math.Abs(SldTextOpacity.Value - val) > 0.1)
                {
                    SldTextOpacity.Value = val;
                }
                _mainWindow.ApplyTextOpacity(val / 100.0);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}