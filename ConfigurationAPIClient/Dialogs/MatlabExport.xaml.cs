// <copyright file="MatlabExport.xaml.cs" company="McLaren Applied Ltd.">
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
    public partial class MatlabExport
    {
        public MatlabExport(SystemMonitorProject.SystemMonitorProjectClient projectClient, Metadata header)
        {
            this.InitializeComponent();

            var apps = projectClient.GetAppDetails(new Empty(), header);
            if (apps.Apps.Count > 0)
            {
                var dt = new DataTable();
                var dt2 = new DataTable();
                dt.Columns.Add("Id", typeof(ushort));
                dt.Columns.Add("Name", typeof(string));
                dt2.Columns.Add("Value", typeof(ParameterType));
                dt2.Columns.Add("Type", typeof(string));

                foreach (var app in apps.Apps)
                {
                    dt.Rows.Add(app.AppId, $"0x{app.AppId,4:X4} - {app.AppName}");
                }

                dt2.Rows.Add(ParameterType.Scalar, "Scalar");
                dt2.Rows.Add(ParameterType.String, "String");
                dt2.Rows.Add(ParameterType.Axis1, "1 Axis");
                dt2.Rows.Add(ParameterType.Axis2, "2 Axis");
                dt2.Rows.Add(ParameterType.Array, "Array");
                dt2.Rows.Add(ParameterType.Axis, "Axis");

                var binding = new Binding
                {
                    Source = dt
                };

                var binding2 = new Binding
                {
                    Source = dt2
                };

                this.appList.DisplayMemberPath = "Name";
                this.appList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                this.appList.SelectedIndex = 0;

                this.type.DisplayMemberPath = "Type";
                this.type.SetBinding(ItemsControl.ItemsSourceProperty, binding2);

                for (var i = 0; i < this.type.Items.Count; i++)
                {
                    this.type.SelectedItems.Add(this.type.Items.GetItemAt(i));
                }
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.dtvName.Text = string.Empty;
            this.exportName.Text = string.Empty;
            this.DialogResult = false;
        }
    }
}
