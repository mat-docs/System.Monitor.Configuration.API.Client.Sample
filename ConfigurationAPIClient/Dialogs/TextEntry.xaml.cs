// <copyright file="TextEntry.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Windows;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class TextEntry
    {
        public TextEntry()
        {
            this.InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.text.Text = string.Empty;
            this.DialogResult = false;
        }
    }
}
