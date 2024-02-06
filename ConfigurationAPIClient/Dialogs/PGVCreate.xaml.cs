// <copyright file="PGVCreate.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Windows;
using System.Windows.Forms;


namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class PGVCreate
    {
        public PGVCreate()
        {
            this.InitializeComponent();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.InitialDirectory = this.location.Text;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK )
            {
                this.location.Text = dialog.SelectedPath;
            }
        }

        private void SelectASAP_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".a2l",
                Filter = "Base ASAP File (.a2l)|*.a2l"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.asap.Text = dialog.FileName;
            }
        }

        private void SelectHex_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".hex",
                Filter = "HEX File (.hex)|*.hex"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.hex.Text = dialog.FileName;
            }
        }

        private void SelectCont_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".ini",
                Filter = "Controllers File (.ini)|*.ini"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.controllers.Text = dialog.FileName;
            }
        }

        private void SelectErrors_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".ini",
                Filter = "Errors File (.ini)|*.ini"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.errors.Text = dialog.FileName;
            }
        }

        private void SelectEvents_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".ini",
                Filter = "Events File (.ini)|*.ini"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.events.Text = dialog.FileName;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.location.Text = string.Empty;
            this.DialogResult = false;
        }

    }
}
