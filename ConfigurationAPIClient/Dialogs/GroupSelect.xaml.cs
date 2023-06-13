// <copyright file="GroupSelect.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Grpc.Core;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class GroupSelect
    {
        public GroupSelect(SystemMonitorVirtual.SystemMonitorVirtualClient virtualClient, Metadata header, bool add)
        {
            this.InitializeComponent();

            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(string));
            dt.Columns.Add("Type", typeof(string));

            var reply = virtualClient.GetVirtualParameterGroups(new Empty(), header);
            if (add)
            {
                dt.Rows.Add("\\", "ROOT");
            }

            foreach (var group in reply.Ids)
            {
                dt.Rows.Add("\\" + group, "   " + group);
            }

            var binding = new Binding
            {
                Source = dt
            };

            this.list.DisplayMemberPath = "Type";
            this.list.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.list.SelectedIndex = 0;

            if (!add)
            {
                this.text.Visibility = Visibility.Hidden;
                this.name.Visibility = Visibility.Hidden;
                this.dtext.Visibility = Visibility.Hidden;
                this.description.Visibility = Visibility.Hidden;
            }
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
