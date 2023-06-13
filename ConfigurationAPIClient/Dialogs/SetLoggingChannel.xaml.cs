// <copyright file="SetLoggingChannel.xaml.cs" company="McLaren Applied Ltd.">
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
    public partial class SetLoggingChannel
    {
        private readonly ChannelPropertiesReply properties;
        public SetLoggingChannel(SystemMonitorLogging.SystemMonitorLoggingClient client, Metadata header)
        {
            this.InitializeComponent();

            this.properties = client.GetLoggingChannelProperties(new Empty(), header);

            if (this.properties.Channels.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(uint));
                dt.Columns.Add("Name", typeof(string));

                foreach (var channel in this.properties.Channels)
                {
                    dt.Rows.Add(channel.Index, $"{channel.Index}");
                }

                var binding = new Binding
                {
                    Source = dt
                };

                this.list.DisplayMemberPath = "Name";
                this.list.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                this.list.SelectedIndex = 0;
            }
        }

        private void Channel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = (int)(uint)((DataRowView)this.list.SelectedItem).Row.ItemArray[0]! - 1;
            var channel = this.properties.Channels[index];
            this.name.Text = channel.Name;
            this.log.IsChecked = channel.LogLogging;
            this.tel.IsChecked = channel.LogTelemetry;
            this.rearm.IsChecked = channel.TriggerRearm;
        }


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.name.Text = string.Empty;
            this.DialogResult = false;
        }
    }
}
