// <copyright file="FileSave.xaml.cs" company="McLaren Applied Ltd.">
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
    public partial class FileSave
    {
        public FileSave(SystemMonitorProject.SystemMonitorProjectClient projectClient, Metadata header)
        {
            this.InitializeComponent();

            var apps = projectClient.GetAppDetails(new Empty(), header);
            if (apps.Apps.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Value", typeof(FileType));
                dt.Columns.Add("Type", typeof(string));

                dt.Rows.Add(FileType.Desktop, "Desktop");
                dt.Rows.Add(FileType.Can, "CAN Config");
                dt.Rows.Add(FileType.Virtuals, "Virtuals");
                dt.Rows.Add(FileType.LoggingConfig, "Logging Config");

                var binding = new Binding
                {
                    Source = dt
                };

                this.typeList.DisplayMemberPath = "Type";
                this.typeList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                this.typeList.SelectedIndex = 0;
            }
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var type = (FileType)((DataRowView)this.typeList.SelectedItem).Row.ItemArray[0];
            if (type == FileType.LoggingConfig || type == FileType.Desktop)
            {
                this.consortium.Visibility = Visibility.Visible;
                this.consortiumLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.consortium.Visibility = Visibility.Hidden;
                this.consortiumLabel.Visibility = Visibility.Hidden;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.fileName.Text = string.Empty;
            this.DialogResult = false;
        }
    }
}
