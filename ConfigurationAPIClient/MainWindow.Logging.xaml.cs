// <copyright file="MainWindow.Logging.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SystemMonitorConfigurationTest.Dialogs;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest
{
    public partial class MainWindow
    {
        private void GetLoggingChannelProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingChannelProperties(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Get Logging Channel Properties: Total: {reply.Channels.Count}");
                foreach (var channel in reply.Channels)
                {
                    this.results.Items.Add(
                        $"Channel {channel.Index}: '{channel.Name}' - \t Logging: {channel.LogLogging} - Rate: {channel.LoggingRate:F2} - \t Telemetry: {channel.LogTelemetry} - Rate: {channel.TelemetryRate:F2} - \t Rearm: {channel.TriggerRearm} - \t Slot: {channel.Slot}");
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void SetLoggingChannelProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new SetLoggingChannel(this.loggingClient, this.header);
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var channel = new ChannelRequest
                    {
                        Index = (uint)((DataRowView)dialog.list.SelectedItem).Row.ItemArray[0]! - 1,
                        Name = dialog.name.Text,
                        LogToUnit = dialog.log.IsChecked != null && (bool)dialog.log.IsChecked,
                        LogTelemetry = dialog.tel.IsChecked != null && (bool)dialog.tel.IsChecked,
                        TriggerRearm = dialog.rearm.IsChecked != null && (bool)dialog.rearm.IsChecked
                    };

                    var reply = this.loggingClient.SetLoggingChannelProperties(channel, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.results.Items.Add(
                        $"Set Channel {channel.Index}: '{channel.Name}' - \t Logging: {channel.LogToUnit} - \t Telemetry: {channel.LogTelemetry} - \t Rearm: {channel.TriggerRearm}");

                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingTriggers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingTriggers(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Get Logging Triggers: Total: {reply.Triggers.Count}");
                foreach (var trigger in reply.Triggers)
                {
                    this.results.Items.Add(
                        $"Trigger {trigger.Index}:  - \t Start Post Trigger: {trigger.StartPostTrigger} ms - \t Stop Post Trigger: {trigger.StopPostTrigger} ms - \t Slot: {trigger.Slot}");
                    foreach (var condition in trigger.StartConditions)
                    {
                        if (condition.Type != TriggerType.NoCondition)
                        {
                            this.results.Items.Add(
                                $"   Start Condition {condition.Index + 1}: - \t Type: {condition.Type} - \t Parameter: '{condition.ParameterId}' - \t App: 0x{condition.AppId:X4} - \t Operator: {condition.Operator} - \t Threshold: {condition.Threshold} - \t Repeat: {condition.RepeatCount}");
                        }
                    }

                    foreach (var condition in trigger.StopConditions)
                    {
                        if (condition.Type != TriggerType.NoCondition)
                        {
                            this.results.Items.Add(
                                $"   Stop  Condition {condition.Index + 1}: - \t Type: {condition.Type} - \t Parameter: '{condition.ParameterId}' - \t App: 0x{condition.AppId:X4} - \t Operator: {condition.Operator} - \t Threshold: {condition.Threshold} - \t Repeat: {condition.RepeatCount}");
                        }
                    }
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }


        private void SetLoggingTrigger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new SetLoggingTrigger(this.loggingClient, this.header);
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var index = (int)(uint)((DataRowView)dialog.list.SelectedItem).Row.ItemArray[0]! - 1;
                    var trigger = dialog.triggers.Triggers[index];

                    var request = new TriggerRequest
                    {
                        Index = trigger.Index,
                        StartPostTrigger = Convert.ToInt32(dialog.start.Text),
                        StopPostTrigger = Convert.ToInt32(dialog.stop.Text)
                    };

                    foreach (var condition in trigger.StartConditions)
                    {
                        request.StartConditions.Add(condition);
                    }

                    foreach (var condition in trigger.StopConditions)
                    {
                        request.StopConditions.Add(condition);
                    }

                    var reply = this.loggingClient.SetLoggingTrigger(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.results.Items.Add($"Edited Logging Trigger: {trigger.Index} - {reply.ReturnCode}");

                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingDuration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingDuration(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Logging: Duration: {reply.EstimatedTime.ToTimeSpan()} \t- Laps {reply.EstimatedLaps:F2}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingWrap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingWrap(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Logging Wrap: {reply.Wrap}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void SetLoggingWrap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingWrap(new Empty(), this.header);
                this.results.Items.Add($"Logging Wrap: {reply.Wrap}");

                var stopwatch = Stopwatch.StartNew();

                var request = new WrapRequest
                {
                    Wrap = !reply.Wrap
                };

                var wrap = this.loggingClient.SetLoggingWrap(request, this.header);
                this.SetErrorCode(wrap.ReturnCode);
                this.results.Items.Add($"Logging Wrap Toggled to : {request.Wrap}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingOffset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingOffset(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add(reply.ReturnCode != ErrorCode.InvalidCommand
                    ? $"Logging Offset: {reply.Offset}"
                    : $"Logging Offset: Cannot be changed on this unit type: {reply.ReturnCode}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void SetLoggingOffset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingOffset(new Empty(), this.header);
                var stopwatch = Stopwatch.StartNew();

                this.results.Items.Add($"Logging Offset: {reply.Offset}");


                var request = new LoggingOffsetRequest
                {
                    Offset = reply.Offset + 1
                };

                var wrap = this.loggingClient.SetLoggingOffset(request, this.header);
                this.SetErrorCode(wrap.ReturnCode);

                this.results.Items.Add(wrap.ReturnCode != ErrorCode.InvalidCommand
                    ? $"Logging Offset increased Set to : {request.Offset}"
                    : $"Logging Offset: Cannot be changed on this unit type: {wrap.ReturnCode}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingSessionDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new TextEntry
                {
                    text2 =
                    {
                        Visibility = Visibility.Hidden
                    },
                    label2 =
                    {
                        Visibility = Visibility.Hidden
                    },
                    label =
                    {
                        Content = "Detail:"
                    }
                };

                if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.text.Text))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new GetSessionDetailRequest
                    {
                        Name = dialog.text.Text
                    };

                    var reply = this.loggingClient.GetLoggingSessionDetails(request, this.header);
                    this.results.Items.Add($"Get Session Details: '{request.Name}': \tValue: '{reply.Value}'");
                    this.SetErrorCode(reply.ReturnCode);

                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void SetLoggingSessionDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new TextEntry
                {
                    label =
                    {
                        Content = "Detail:"
                    },
                    label2 =
                    {
                        Content = "Value:"
                    }
                };

                if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.text.Text))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new SetSessionDetailRequest
                    {
                        Name = dialog.text.Text,
                        Value = dialog.text2.Text
                    };

                    var reply = this.loggingClient.SetLoggingSessionDetails(request, this.header);
                    this.results.Items.Add($"Set Session Details: '{request.Name}': \tValue: '{request.Value}'");
                    this.SetErrorCode(reply.ReturnCode);

                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingParameterDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var reply = this.loggingClient.GetLoggingParameterDetails(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);

                if (reply.ReturnCode == (int)ErrorCode.NoError)
                {
                    this.results.Items.Add($"Logging Config: Parameters: {reply.Parameters.Count}");
                    this.results.Items.Add($"Logging Config: Channels: {reply.ChannelNames.Count} - Parameters: {reply.ChannelNames}");
                    this.results.Items.Add(string.Empty);

                    foreach (var parameter in reply.Parameters)
                    {
                        this.results.Items.Add($"Parameter: 0x{parameter.AppId:X4} - '{parameter.ParameterName}' \t- '{parameter.ParameterId}' \t- '{parameter.ParameterDescription}'  \t- {parameter.DataSize} bytes \t- {parameter.DataSize} bytes \t- slot: {parameter.Slot}");
                        foreach (var value in parameter.Values)
                        {
                            this.results.Items.Add($"   {value}");

                        }
                    }
                }
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void LoggingConfigDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var request = new DownloadRequest();
                this.results.Items.Add("Logging Config Download Requested.. Please Wait");
                var reply = this.loggingClient.LoggingConfigDownload(request, this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Logging Config Download Completed: {reply.OptionalValue}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void LoggingConfigUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                this.results.Items.Add("Logging Config Download Upload.. Please Wait");
                var reply = this.loggingClient.LoggingConfigUpload(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add("Logging Config Upload Completed");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void LoggingConfigDownloadInProgress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.LoggingConfigDownloadInProgress(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Logging Config Download In Progress: {reply.InProgress}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void RemoveLoggingParameter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                if (this.measurement.SelectedItem != null)
                {
                    var request = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.loggingClient.RemoveLoggingParameter(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.results.Items.Add($"Remove Parameter From Logging Config: 0x{request.AppId} - '{request.ParameterId}' - {reply.ReturnCode}");
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ClearAllLoggingParameters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var request = new ClearRequest
                {
                    RemoveTriggers = true
                };

                var reply = this.loggingClient.ClearAllLoggingParameters(request, this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Clear All Logging Config Parameters: {reply.ReturnCode}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingSlotsUsed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetLoggingSlotsUsed(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Logging Config Slots Used : {reply.SlotCount}");

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetLoggingSlotPercentage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                if (this.measurement.SelectedItem != null)
                {
                    var request = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.loggingClient.GetLoggingSlotPercentage(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.results.Items.Add($"Logging Config Parameter: 0x{reply.AppId} - '{reply.ParameterId}' - {reply.ReturnCode}");
                    this.results.Items.Add($"Logging Slot Percentage: {reply.SlotPercentage}%");
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void GetECULoggingConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.loggingClient.GetECULoggingConfig(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"ECU Logging Config Parameter: '{reply.ConfigName}' - {reply.ReturnCode}");
                if (reply.ReturnCode == ErrorCode.InvalidFile)
                {
                    this.results.Items.Add("  No config downloaded");
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void AddLoggingParameter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                if (this.measurement.SelectedItem != null)
                {
                    var request = new AddParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var one = new LoggingChannelValue()
                    {
                        ChannelId = 1,
                        Type = LoggingType.Frequency,
                        Value = 200
                    };

                    var two = new LoggingChannelValue()
                    {
                        ChannelId = 2,
                        Type = LoggingType.Frequency,
                        Value = 10
                    };

                    request.LoggingRate.Add(one);
                    request.LoggingRate.Add(two);

                    var reply = this.loggingClient.AddLoggingParameter(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.results.Items.Add($"Added Parameter to Logging Config: 0x{request.AppId:X4} - '{request.ParameterId}' - Error: {reply.ReturnCode}");
                    if (reply.ReturnCode == ErrorCode.NoError)
                    {
                        this.results.Items.Add("   Channel 1 @ 200 hz");
                        this.results.Items.Add("   Channel 2 @ 10 hz");
                    }
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void AddVirtualLoggingParameter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                if (this.virt.SelectedItem != null)
                {
                    var request = new AddVirtualParameterRequest
                    {
                        ParameterId = (string)((DataRowView)this.virt.SelectedItem).Row.ItemArray[0]
                    };

                    var one = new LoggingChannelValue()
                    {
                        ChannelId = 1,
                        Type = LoggingType.Frequency,
                        Value = 100
                    };

                    var two = new LoggingChannelValue()
                    {
                        ChannelId = 2,
                        Type = LoggingType.Frequency,
                        Value = 5
                    };

                    request.LoggingRate.Add(one);
                    request.LoggingRate.Add(two);

                    var reply = this.loggingClient.AddVirtualLoggingParameter(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.results.Items.Add($"Added Virtual Parameter to Logging Config: '{request.ParameterId}' - Error: {reply.ReturnCode}");
                    if (reply.ReturnCode == ErrorCode.NoError)
                    {
                        this.results.Items.Add("   Channel 1 @ 100 hz");
                        this.results.Items.Add("   Channel 2 @ 5 hz");
                    }
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            catch (RpcException)
            {
                this.results.Items.Add("Server connection FAILED");
            }
            catch
            {
                this.results.Items.Add("Unknown Client ERROR");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
