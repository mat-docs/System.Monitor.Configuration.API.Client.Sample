// <copyright file="SelectApps.xaml.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace SystemMonitorConfigurationTest.Dialogs
{
    /// <summary>
    /// Interaction logic for SelectParameters.xaml
    /// </summary>
    public partial class SelectParameters : Window
    {
        public SelectParameters(Dictionary<string, string> parameters)
        {
            InitializeComponent();
            this.Total.Content = $"Total: {parameters.Count}";
            
            if (parameters.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(string));
                dt.Columns.Add("Name", typeof(string));

                foreach (var parameter in parameters)
                {
                    dt.Rows.Add(parameter.Key, $"{parameter.Value}");
                }
                
                var binding = new Binding
                {
                    Source = dt
                };

                this.Parameters.DisplayMemberPath = "Name";
                this.Parameters.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            }
            else
            {
                this.Parameters.IsEnabled = false;
                this.SelectAll.IsChecked = true;
            }
        }
        
        private void Select_Checked(object sender, RoutedEventArgs e)
        {
            this.Parameters.IsEnabled = false;
        }

        private void Select_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Parameters.IsEnabled = true;
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
