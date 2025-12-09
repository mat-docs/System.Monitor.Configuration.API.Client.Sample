// <copyright file="SetLoggingCondition.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class SetLoggingCondition
    {
        private readonly TriggersReply triggers;

        public SetLoggingCondition(SystemMonitorLogging.SystemMonitorLoggingClient client, TriggerCondition condition, Metadata header)
        {
            this.InitializeComponent();

            this.triggers = client.GetLoggingTriggers(new Empty(), header);

            if (this.triggers.Triggers.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(uint));
                dt.Columns.Add("Name", typeof(string));

                dt.Rows.Add(TriggerType.OnData, $"{TriggerType.OnData}");
                dt.Rows.Add(TriggerType.IgnitionOn, $"{TriggerType.IgnitionOn}");
                dt.Rows.Add(TriggerType.LapTrigger, $"{TriggerType.LapTrigger}");
                dt.Rows.Add(TriggerType.NoCondition, $"{TriggerType.NoCondition}");
                dt.Rows.Add(TriggerType.ExternalTrigger, $"{TriggerType.ExternalTrigger}");

                var binding = new Binding
                {
                    Source = dt
                };

                this.type.DisplayMemberPath = "Name";
                this.type.SetBinding(ItemsControl.ItemsSourceProperty, binding);


                var dt2 = new DataTable();
                dt2.Columns.Add("Id", typeof(uint));
                dt2.Columns.Add("Name", typeof(string));

                dt2.Rows.Add(TriggerOperator.Equals, $"{TriggerOperator.Equals}");
                dt2.Rows.Add(TriggerOperator.LessThan, $"{TriggerOperator.LessThan}");
                dt2.Rows.Add(TriggerOperator.GreaterThan, $"{TriggerOperator.GreaterThan}");
                dt2.Rows.Add(TriggerOperator.NotEqualTo, $"{TriggerOperator.NotEqualTo}");
                dt2.Rows.Add(TriggerOperator.GreaterThanOrEqual, $"{TriggerOperator.GreaterThanOrEqual}");
                dt2.Rows.Add(TriggerOperator.LessThanOrEqual, $"{TriggerOperator.LessThanOrEqual}");

                var binding2 = new Binding
                {
                    Source = dt2
                };

                this.op.DisplayMemberPath = "Name";
                this.op.SetBinding(ItemsControl.ItemsSourceProperty, binding2);

                this.type.SelectedIndex = condition.Type == TriggerType.OnData ? 0 : (int)condition.Type - 1;
                this.op.SelectedIndex = (int)condition.Operator;
                this.parameter.Text = condition.ParameterId;
                this.app.Text = condition.AppId.ToString();
                this.threshold.Text = condition.Threshold.ToString();
                this.repeat.Text = condition.RepeatCount.ToString();

                this.title.Content = condition.Index > 2 ? $"Stop Trigger {condition.Index - 2}" : $"Start Trigger {condition.Index + 1}";
            }
        }

        void FillCondition(int index, TriggerCondition condition, TextBox c)
        {
            if (condition.Type != TriggerType.NoCondition)
            {
                c.Text = $"{condition.Index + 1}: - Type: {condition.Type} - Parameter: '{condition.ParameterId}' - App: 0x{condition.AppId:X4} - Operator: {condition.Operator} - Threshold: {condition.Threshold} - Repeat: {condition.RepeatCount}";
            }
            else
            {
                c.Text = "No Condition";
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

    }
}
