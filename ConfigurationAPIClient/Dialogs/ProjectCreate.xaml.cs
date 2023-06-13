// <copyright file="ProjectCreate.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Windows;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class ProjectCreate
    {
        public ProjectCreate()
        {
            this.InitializeComponent();
        }

        private void SelectProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".prj",
                Filter = "System Monitor Project (.prj)|*.prj"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.projectName.Text = dialog.FileName;
            }
        }
        private void SelectDesktop_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".dtp",
                Filter = "System Monitor Desktop (.dtp)|*.dtp"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.desktopName.Text = dialog.FileName;
            }
        }

        private void SelectVirtual_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".vpx",
                Filter = "System Monitor Virtuals (.vpx)|*.vpx"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.virtualName.Text = dialog.FileName;
            }
        }

        private void SelectCan_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".clc",
                Filter = "System Monitor CAN (.clc)|*.clc"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.canName.Text = dialog.FileName;
            }
        }

        private void SelectLogging_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".clc",
                Filter = "System Monitor Logging Config (.rlc)|*.rlc"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.loggingName.Text = dialog.FileName;
            }
        }

        private void SelectApp_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".pgv",
                Filter = "System Monitor PGV (.pgv)|*.pgv"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.apps.Items.Add(dialog.FileName);
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
