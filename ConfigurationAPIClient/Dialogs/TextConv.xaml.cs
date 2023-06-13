// <copyright file="TextConv.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class TextConv
    {
        public List<TextConversion> values;

        public TextConv()
        {
            this.InitializeComponent();
            this.values = new List<TextConversion>();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.raw.Text) == false && string.IsNullOrEmpty(this.map.Text) == false)
            {
                this.list.Items.Add($"Raw: {this.raw.Text} -> Mapped: {this.map.Text}");

                var value = new TextConversion
                {
                    Raw = Convert.ToDouble(this.raw.Text),
                    Mapped = this.map.Text
                };
                this.values.Add(value);
            }
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
