// <copyright file="BufferType.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class BufferType
    {
        public BufferType()
        {
            this.InitializeComponent();

            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(FileType));
            dt.Columns.Add("Type", typeof(SystemMonitorProtobuf.BufferType));

            dt.Rows.Add(SystemMonitorProtobuf.BufferType.UnitBuffer, SystemMonitorProtobuf.BufferType.UnitBuffer);
            dt.Rows.Add(SystemMonitorProtobuf.BufferType.EditBuffer, SystemMonitorProtobuf.BufferType.EditBuffer);
            dt.Rows.Add(SystemMonitorProtobuf.BufferType.UnitAndEditBuffer, SystemMonitorProtobuf.BufferType.UnitAndEditBuffer);

            var binding = new Binding
            {
                Source = dt
            };

            this.list.DisplayMemberPath = "Type";
            this.list.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.list.SelectedIndex = 0;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
