// <copyright file="SetSlot.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Windows;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class SetSlot
    {
        public SetSlot(int slotCount)
        {
            this.InitializeComponent();

            for (int i = 1; i <= slotCount; i++)
            {
                this.slot.Items.Add(i.ToString());
            }

            this.slot.SelectedIndex = 0;
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