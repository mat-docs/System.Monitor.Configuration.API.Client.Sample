// <copyright file="AppType.xaml.cs" company="McLaren Applied Ltd.">
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
    public partial class AppType
    {
        public AppType(SystemMonitorProject.SystemMonitorProjectClient projectClient, Metadata header)
        {
            this.InitializeComponent();

            var apps = projectClient.GetAppDetails(new Empty(), header);
            if (apps.Apps.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Value", typeof(FileType));
                dt.Columns.Add("Type", typeof(string));

                foreach (var app in apps.Apps)
                {
                    dt.Rows.Add(app.AppId, $"0x{app.AppId,4:X4} - {app.AppName}");
                }

                var binding = new Binding
                {
                    Source = dt
                };

                this.appList.DisplayMemberPath = "Type";
                this.appList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                this.appList.SelectedIndex = 0;
            }

            var dt2 = new DataTable();
            dt2.Columns.Add("Value", typeof(ParameterType));
            dt2.Columns.Add("Type", typeof(string));

            dt2.Rows.Add(ParameterType.Undefined, "None");
            dt2.Rows.Add(ParameterType.Measurement, ParameterType.Measurement.ToString());
            dt2.Rows.Add(ParameterType.Scalar, ParameterType.Scalar.ToString());
            dt2.Rows.Add(ParameterType.Axis1, ParameterType.Axis1.ToString());
            dt2.Rows.Add(ParameterType.Axis2, ParameterType.Axis2.ToString());
            dt2.Rows.Add(ParameterType.Array, ParameterType.Array.ToString());
            dt2.Rows.Add(ParameterType.String, ParameterType.String.ToString());
            dt2.Rows.Add(ParameterType.Ecu, ParameterType.Ecu.ToString());
            //            dt2.Rows.Add(ParameterType.Can, ParameterType.Can.ToString());
            //            dt2.Rows.Add(ParameterType.Virtual, ParameterType.Virtual.ToString());
            dt2.Rows.Add(ParameterType.Axis, ParameterType.Axis.ToString());
            //            dt2.Rows.Add(ParameterType.Input, ParameterType.Input.ToString());

            var binding2 = new Binding
            {
                Source = dt2
            };

            this.typeList.DisplayMemberPath = "Type";
            this.typeList.SetBinding(ItemsControl.ItemsSourceProperty, binding2);
            this.typeList.SelectedIndex = 0;

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
