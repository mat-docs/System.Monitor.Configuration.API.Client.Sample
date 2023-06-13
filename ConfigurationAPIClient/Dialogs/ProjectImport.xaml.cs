// <copyright file="ImportProject.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Microsoft.Win32;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class ProjectImport
    {
        public ProjectImport()
        {
            this.InitializeComponent();

            try
            {
                var dt = new DataTable();
                this.baseList.ItemsSource = null;
                this.baseList.DisplayMemberPath = "Name";
                dt.Columns.Add("Id", typeof(string));
                dt.Columns.Add("Name", typeof(string));


                using var key = Registry.CurrentUser.OpenSubKey("Software\\McLaren Electronic Systems\\System Monitor\\8.0\\Multi Asap Bases");
                if (key != null)
                {
                    foreach (var keyname in key.GetSubKeyNames())
                    {
                        dt.Rows.Add(keyname, keyname);
                    }
                }

                var binding = new Binding
                {
                    Source = dt
                };

                this.baseList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void SelectProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".smx",
                Filter = "System Monitor Export (.smx)|*.smx"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.projectName.Text = dialog.FileName;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.projectName.Text = string.Empty;
            this.DialogResult = false;
        }

    }
}
