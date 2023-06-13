// <copyright file="RationalConv.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Windows;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class RationalConv
    {
        public RationalConv()
        {
            this.InitializeComponent();
            this.co1.Text = "0";
            this.co2.Text = "1";
            this.co3.Text = "0";
            this.co4.Text = "0";
            this.co5.Text = "0";
            this.co6.Text = "1";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.name.Text = string.Empty;
            this.DialogResult = false;
        }
    }
}
