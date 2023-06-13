// <copyright file="FileNew.xaml.cs" company="McLaren Applied Ltd.">
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
    public partial class FileNew
    {
        public FileNew(SystemMonitorProject.SystemMonitorProjectClient projectClient, Metadata header)
        {
            this.InitializeComponent();

            var apps = projectClient.GetAppDetails(new Empty(), header);
            if (apps.Apps.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Value", typeof(FileType));
                dt.Columns.Add("Type", typeof(string));

                dt.Rows.Add(FileType.Can, "CAN Config");
                dt.Rows.Add(FileType.Virtuals, "Virtuals");
                dt.Rows.Add(FileType.LoggingCofig, "Logging Config");

                var binding = new Binding
                {
                    Source = dt
                };

                this.typeList.DisplayMemberPath = "Type";
                this.typeList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                this.typeList.SelectedIndex = 0;
            }
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var ext = "";
            var filter = "";
            var type = (FileType)((DataRowView)this.typeList.SelectedItem).Row.ItemArray[0];
            switch (type)
            {
                case FileType.LoggingCofig:
                    ext = ".rlc";
                    filter = "Logging Config Files (.rlc)|*.rlc";
                    break;

                case FileType.Virtuals:
                    ext = ".vpx";
                    filter = "Virtual Parameter Files (.vpx)|*.vpx";
                    break;

                case FileType.Can:
                    ext = ".clc";
                    filter = "CAN Config Files (.clc)|*.clc";
                    break;
            }

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ext,
                Filter = filter
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.filePath.Text = dialog.FileName;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.filePath.Text = string.Empty;
            this.DialogResult = false;
        }

    }
}
