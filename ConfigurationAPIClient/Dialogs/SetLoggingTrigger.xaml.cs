// <copyright file="SetLoggingChannel.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Grpc.Core;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class SetLoggingTrigger
    {
        public readonly TriggersReply triggers;
        private readonly SystemMonitorLogging.SystemMonitorLoggingClient client;

        public SetLoggingTrigger(SystemMonitorLogging.SystemMonitorLoggingClient client, Metadata header)
        {
            this.InitializeComponent();

            this.client = client;
            this.triggers = client.GetLoggingTriggers(new Empty(), header);

            if (this.triggers.Triggers.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(uint));
                dt.Columns.Add("Name", typeof(string));

                foreach (var trigger in this.triggers.Triggers)
                {
                    dt.Rows.Add(trigger.Index, $"{trigger.Index}");
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
            var trigger = this.triggers.Triggers[index];
            this.start.Text = trigger.StartPostTrigger.ToString();
            this.stop.Text = trigger.StopPostTrigger.ToString();

            this.FillCondition(index, trigger.StartConditions[0], this.c1);
            this.FillCondition(index, trigger.StartConditions[1], this.c2);
            this.FillCondition(index, trigger.StartConditions[2], this.c3);
            this.FillCondition(index, trigger.StopConditions[0], this.c4);
            this.FillCondition(index, trigger.StopConditions[1], this.c5);
            this.FillCondition(index, trigger.StopConditions[2], this.c6);
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

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)(uint)((DataRowView)this.list.SelectedItem).Row.ItemArray[0]! - 1;
            var trigger = this.triggers.Triggers[index];
            var dialog = new SetLoggingCondition(this.client, trigger.StartConditions[0]);
            if (dialog.ShowDialog() == true)
            {
                var condition = new TriggerCondition
                {
                    Index = trigger.StartConditions[0].Index,
                    Type = (TriggerType)(uint)((DataRowView)dialog.type.SelectedItem).Row.ItemArray[0]!,
                    Operator = (TriggerOperator)(uint)((DataRowView)dialog.op.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = dialog.parameter.Text,
                    AppId = Convert.ToUInt32(dialog.app.Text),
                    Threshold = Convert.ToDouble(dialog.threshold.Text),
                    RepeatCount = Convert.ToUInt32(dialog.repeat.Text)
                };

                trigger.StartConditions[0] = condition;
                this.FillCondition(index, trigger.StartConditions[0], this.c1);
            }
        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)(uint)((DataRowView)this.list.SelectedItem).Row.ItemArray[0]! - 1;
            var trigger = this.triggers.Triggers[index];
            var dialog = new SetLoggingCondition(this.client, trigger.StartConditions[1]);
            if (dialog.ShowDialog() == true)
            {
                var condition = new TriggerCondition
                {
                    Index = trigger.StartConditions[1].Index,
                    Type = (TriggerType)(uint)((DataRowView)dialog.type.SelectedItem).Row.ItemArray[0]!,
                    Operator = (TriggerOperator)(uint)((DataRowView)dialog.op.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = dialog.parameter.Text,
                    AppId = Convert.ToUInt32(dialog.app.Text),
                    Threshold = Convert.ToDouble(dialog.threshold.Text),
                    RepeatCount = Convert.ToUInt32(dialog.repeat.Text)
                };

                trigger.StartConditions[1] = condition;
                this.FillCondition(index, trigger.StartConditions[1], this.c2);
            }
        }

        private void B3_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)(uint)((DataRowView)this.list.SelectedItem).Row.ItemArray[0]! - 1;
            var trigger = this.triggers.Triggers[index];
            var dialog = new SetLoggingCondition(this.client, trigger.StartConditions[2]);
            if (dialog.ShowDialog() == true)
            {
                var condition = new TriggerCondition
                {
                    Index = trigger.StartConditions[2].Index,
                    Type = (TriggerType)(uint)((DataRowView)dialog.type.SelectedItem).Row.ItemArray[0]!,
                    Operator = (TriggerOperator)(uint)((DataRowView)dialog.op.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = dialog.parameter.Text,
                    AppId = Convert.ToUInt32(dialog.app.Text),
                    Threshold = Convert.ToDouble(dialog.threshold.Text),
                    RepeatCount = Convert.ToUInt32(dialog.repeat.Text)
                };

                trigger.StartConditions[2] = condition;
                this.FillCondition(index, trigger.StartConditions[2], this.c3);
            }
        }

        private void E1_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)(uint)((DataRowView)this.list.SelectedItem).Row.ItemArray[0]! - 1;
            var trigger = this.triggers.Triggers[index];
            var dialog = new SetLoggingCondition(this.client, trigger.StopConditions[0]);
            if (dialog.ShowDialog() == true)
            {
                var condition = new TriggerCondition
                {
                    Index = trigger.StopConditions[0].Index,
                    Type = (TriggerType)(uint)((DataRowView)dialog.type.SelectedItem).Row.ItemArray[0]!,
                    Operator = (TriggerOperator)(uint)((DataRowView)dialog.op.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = dialog.parameter.Text,
                    AppId = Convert.ToUInt32(dialog.app.Text),
                    Threshold = Convert.ToDouble(dialog.threshold.Text),
                    RepeatCount = Convert.ToUInt16(dialog.repeat.Text)
                };

                trigger.StopConditions[0] = condition;
                this.FillCondition(index, trigger.StopConditions[0], this.c4);
            }
        }

        private void E2_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)(uint)((DataRowView)this.list.SelectedItem).Row.ItemArray[0]! - 1;
            var trigger = this.triggers.Triggers[index];
            var dialog = new SetLoggingCondition(this.client, trigger.StopConditions[1]);
            if (dialog.ShowDialog() == true)
            {
                var condition = new TriggerCondition
                {
                    Index = trigger.StopConditions[1].Index,
                    Type = (TriggerType)(uint)((DataRowView)dialog.type.SelectedItem).Row.ItemArray[0]!,
                    Operator = (TriggerOperator)(uint)((DataRowView)dialog.op.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = dialog.parameter.Text,
                    AppId = Convert.ToUInt32(dialog.app.Text),
                    Threshold = Convert.ToDouble(dialog.threshold.Text),
                    RepeatCount = Convert.ToUInt32(dialog.repeat.Text)
                };

                trigger.StopConditions[1] = condition;
                this.FillCondition(index, trigger.StopConditions[1], this.c5);
            }
        }

        private void E3_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)(uint)((DataRowView)this.list.SelectedItem).Row.ItemArray[0]! - 1;
            var trigger = this.triggers.Triggers[index];
            var dialog = new SetLoggingCondition(this.client, trigger.StopConditions[2]);
            if (dialog.ShowDialog() == true)
            {
                var condition = new TriggerCondition
                {
                    Index = trigger.StopConditions[2].Index,
                    Type = (TriggerType)(uint)((DataRowView)dialog.type.SelectedItem).Row.ItemArray[0]!,
                    Operator = (TriggerOperator)(uint)((DataRowView)dialog.op.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = dialog.parameter.Text,
                    AppId = Convert.ToUInt32(dialog.app.Text),
                    Threshold = Convert.ToDouble(dialog.threshold.Text),
                    RepeatCount = Convert.ToUInt32(dialog.repeat.Text)
                };

                trigger.StopConditions[2] = condition;
                this.FillCondition(index, trigger.StopConditions[2], this.c6);
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
