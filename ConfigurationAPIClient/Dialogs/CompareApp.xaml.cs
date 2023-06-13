// <copyright file="CompareApp.xaml.cs" company="McLaren Applied Ltd.">
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
    public partial class CompareApp
    {
        public CompareApp(SystemMonitorProject.SystemMonitorProjectClient projectClient, Metadata header)
        {
            this.InitializeComponent();

            var apps = projectClient.GetAppDetails(new Empty(), header);
            if (apps.Apps.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(ushort));
                dt.Columns.Add("Name", typeof(string));

                foreach (var app in apps.Apps)
                {
                    dt.Rows.Add(app.AppId, $"0x{app.AppId,4:X4} - {app.AppName}");
                }

                var binding = new Binding
                {
                    Source = dt
                };

                this.appList.DisplayMemberPath = "Name";
                this.appList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                this.appList.SelectedIndex = 0;
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

        private void SelectDTV1_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".dtv",
                Filter = "DTV file (.dtv)|*.dtv"
            };

            if (dialog.ShowDialog() == true)
            {
                this.dtv1.Text = dialog.FileName;
            }
        }

        private void SelectDTV2_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".dtv",
                Filter = "DTV file (.dtv)|*.dtv"
            };

            if (dialog.ShowDialog() == true)
            {
                this.dtv2.Text = dialog.FileName;
            }
        }
    }
}
