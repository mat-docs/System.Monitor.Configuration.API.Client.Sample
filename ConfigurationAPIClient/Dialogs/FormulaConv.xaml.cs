// <copyright file="FormulaConv.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Collections.Generic;
using System.Windows;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class FormulaConv
    {
        public List<TableConversion> values;

        public FormulaConv()
        {
            this.InitializeComponent();
            this.values = new List<TableConversion>();
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
