// <copyright file="AddVirtual.xaml.cs" company="McLaren Applied Ltd.">
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
    public partial class AddVirtual
    {
        public AddVirtual(SystemMonitorVirtual.SystemMonitorVirtualClient virtualClient, Metadata header)
        {
            this.InitializeComponent();
            this.min.Text = "0";
            this.rate.Text = "0";
            this.max.Text = "100";
            this.lower.Text = "0";
            this.upper.Text = "100";
            this.scaling.Text = "0";
            this.conversion.Text = "C_None";

            var dt = new DataTable();
            dt.Columns.Add("Value", typeof(DataType));
            dt.Columns.Add("Type", typeof(string));

            dt.Rows.Add(DataType.Ubyte, DataType.Ubyte.ToString());
            dt.Rows.Add(DataType.Byte, DataType.Byte.ToString());
            dt.Rows.Add(DataType.Uword, DataType.Uword.ToString());
            dt.Rows.Add(DataType.Word, DataType.Word.ToString());
            dt.Rows.Add(DataType.Ulong, DataType.Ulong.ToString());
            dt.Rows.Add(DataType.Long, DataType.Long.ToString());
            dt.Rows.Add(DataType.Float, DataType.Float.ToString());

            var binding2 = new Binding
            {
                Source = dt
            };

            this.type.DisplayMemberPath = "Type";
            this.type.SetBinding(ItemsControl.ItemsSourceProperty, binding2);
            this.type.SelectedIndex = 5;

            var dt2 = new DataTable();
            dt2.Columns.Add("Value", typeof(string));
            dt2.Columns.Add("Type", typeof(string));

            var reply = virtualClient.GetVirtualParameterGroups(new Empty(), header);
            dt2.Rows.Add("\\", "ROOT");

            foreach (var gr in reply.Ids)
            {
                dt2.Rows.Add("\\" + gr, "   " + gr);
            }

            var binding = new Binding
            {
                Source = dt2
            };

            this.group.DisplayMemberPath = "Type";
            this.group.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.group.SelectedIndex = 0;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.id.Text = string.Empty;
            this.DialogResult = false;
        }
    }
}
