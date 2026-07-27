using System;
using System.Windows;
using System.Windows.Input;

namespace CountdownDay
{
    public partial class EditWindow : Window
    {
        public EventItem EventData { get; private set; }

        public EditWindow(EventItem item)
        {
            InitializeComponent();
            EventData = item;

            TxtTitle.Text = item.Title;
            DpTargetDate.SelectedDate = item.TargetDate;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                System.Windows.MessageBox.Show(
                    App.GetString("MsgEmptyTitle"), 
                    App.GetString("MsgPrompt"), 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning
                );
                return;
            }

            if (!DpTargetDate.SelectedDate.HasValue)
            {
                System.Windows.MessageBox.Show(
                    App.GetString("MsgEmptyDate"), 
                    App.GetString("MsgPrompt"), 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning
                );
                return;
            }

            EventData.Title = TxtTitle.Text.Trim();
            EventData.TargetDate = DpTargetDate.SelectedDate.Value;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}