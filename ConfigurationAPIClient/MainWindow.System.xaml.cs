// <copyright file="MainWindow.System.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using SystemMonitorConfigurationTest.Dialogs;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest;

public partial class MainWindow
{
    private void GetStatus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var state = this.systemClient.GetStatus(new Empty(), this.header);

            this.LinkState = state.LinkStatus;
            this.OnlineState = state.Online;
            this.LiveState = state.LiveUpdate;
            if (state.ReturnCode == ErrorCode.NoError)
            {
                this.results.Items.Add($"Link State: {this.LinkState.ToString()}");
                this.results.Items.Add($"Online State: {this.OnlineState}");
                this.results.Items.Add($"Live Update State: {this.LiveState}");
                this.SetOnline.Content = $"SetOnline: {!this.OnlineState}";
                this.SetLiveUpdate.Content = $"SetLiveUpdate: {!this.LiveState}";
                this.SetOnline.IsEnabled = true;
                this.SetLiveUpdate.IsEnabled = true;
            }

            this.SetErrorCode(state.ReturnCode);
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

    private void SetOnline_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            this.OnlineState = !this.OnlineState;
            this.results.Items.Add($"Online State set: {this.OnlineState}");

            var state = new OnlineRequest()
            {
                State = this.OnlineState
            };

            var error = this.systemClient.SetOnline(state, this.header);

            if (error.ReturnCode == 0)
            {
                this.SetOnline.Content = $"SetOnline: {!this.OnlineState}";
            }
            else
            {
                this.OnlineState = !this.OnlineState;
                this.results.Items.Add("Set Online set: FAILED");
            }

            this.SetErrorCode(error.ReturnCode);
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

    private void SetLiveUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            this.LiveState = !this.LiveState;
            this.results.Items.Add($"Live Update State set: {this.LiveState}");

            var state = new LiveUpdateRequest()
            {
                State = this.LiveState,
                Action = 0
            };

            var error = this.systemClient.SetLiveUpdate(state, this.header);

            if (error.ReturnCode == 0)
            {
                this.SetLiveUpdate.Content = $"SetLiveUpdate: {!this.LiveState}";
            }
            else
            {
                this.LiveState = !this.LiveState;
                this.results.Items.Add("Live Update set: FAILED");
            }

            this.SetErrorCode(error.ReturnCode);
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

    private void GetUnitName_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var unit = this.systemClient.GetUnitName(new Empty(), this.header);
            this.results.Items.Add($"Unit: '{unit.Name}'");

            this.SetErrorCode(unit.ReturnCode);
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

    private void GetUnitList_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var units = this.systemClient.GetUnitList(new Empty(), this.header);

            var dt = new DataTable();
            this.resultList.ItemsSource = null;
            this.resultList.DisplayMemberPath = "Name";
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));

            foreach (var unit in units.Info)
            {
                this.results.Items.Add($"Unit: '{unit.Name}' \tType: '{unit.Type}' \tAddress: {unit.IpAddress}");
                dt.Rows.Add(unit.Name, unit.Name);
            }

            var binding = new Binding
            {
                Source = dt
            };

            this.resultList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.resultList.SelectedIndex = 0;
            this.GetUnitByIndex.IsEnabled = true;
            this.SetUnitByIndex.IsEnabled = true;

            this.SetErrorCode(units.ReturnCode);
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

    private void GetUnitByIndex_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.results.Items.Clear();
            var stopwatch = Stopwatch.StartNew();

            var selection = new UnitByIndexRequest()
            {
                Index = (uint)this.resultList.SelectedIndex
            };

            var unit = this.systemClient.GetUnitByIndex(selection, this.header);

            this.results.Items.Add($"Unit: '{unit.Name}' \tType: '{unit.Type}' \tAddress: {unit.IpAddress}");

            this.SetErrorCode(unit.ReturnCode);
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

    private void SetUnitByIndex_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.results.Items.Clear();
            var stopwatch = Stopwatch.StartNew();

            var selection = new UnitByIndexTypeRequest()
            {
                Index = (uint)this.resultList.SelectedIndex,
                Primary = true
            };

            var error = this.systemClient.SetUnitByIndex(selection, this.header);

            this.results.Items.Add($"Setting Unit To: '{this.resultList.Text}'");

            this.SetErrorCode(error.ReturnCode);
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

    private void GetMultiApplicationBases_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var bases = this.systemClient.GetMultiApplicationBases(new Empty(), this.header);

            var dt = new DataTable();
            this.resultList.ItemsSource = null;
            this.resultList.DisplayMemberPath = "Name";
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));

            if (bases.Info.Count > 0)
            {
                foreach (var info in bases.Info)
                {
                    this.results.Items.Add($"MultiApplicationBase: '{info.Name}' \tPath: {info.Path}");
                    dt.Rows.Add(info.Name, info.Name);
                }

                this.SetMultiApplicationBase.IsEnabled = true;
            }
            else
            {
                this.results.Items.Add("No MultiApplicationBases returned: Project may be open");
            }

            var binding = new Binding
            {
                Source = dt
            };

            this.resultList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.resultList.SelectedIndex = 0;
            this.GetUnitByIndex.IsEnabled = true;
            this.SetUnitByIndex.IsEnabled = true;

            this.SetErrorCode(bases.ReturnCode);
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

    private void GetMultiApplicationBase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var info = this.systemClient.GetMultiApplicationBase(new Empty(), this.header);
            this.results.Items.Add($"MultiApplicationBase: '{info.Name}' \tPath: {info.Path}");

            this.SetErrorCode(info.ReturnCode);
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

    private void SetMultiApplicationBase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.results.Items.Clear();
            this.SetMultiApplicationBase.IsEnabled = true;
            var stopwatch = Stopwatch.StartNew();

            var selection = new MultiApplicationBasesRequest()
            {
                BaseName = this.resultList.Text
            };

            var error = this.systemClient.SetMultiApplicationBase(selection, this.header);

            this.results.Items.Add($"Setting Multi Application Base To: '{this.resultList.Text}'");

            this.SetErrorCode(error.ReturnCode);
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

    private void GetLicenceDetails_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var info = this.systemClient.GetLicenceDetails(new Empty(), this.header);
            this.results.Items.Add($"Licence: Consortium: '{info.Consortium}' \tOwner: '{info.Owner}'");

            this.SetErrorCode(info.ReturnCode);
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

    private void GetDeviceProperties_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var units = this.systemClient.GetDeviceProperties(new Empty(), this.header);

            foreach (var device in units.Devices)
                this.results.Items.Add(
                    $"Name: '{device.DeviceName}' \tComms Path: '{device.CommsPath}' \tSerial Number: {device.SerialNumber} \tAddress: {device.IpAddress}");

            this.SetErrorCode(units.ReturnCode);
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

    private void GetLiveLogging_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var state = this.systemClient.GetLiveLogging(new Empty(), this.header);

            this.LiveLoggingState = state.LiveLoggingState;
            this.results.Items.Add($"Live Logging State: {this.LiveLoggingState}");
            this.SetLiveLogging.Content = $"SetLiveLogging: {!this.LiveLoggingState}";
            this.SetLiveLogging.IsEnabled = true;

            this.SetErrorCode(state.ReturnCode);
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

    private void SetLiveLogging_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            this.SetLiveLogging.IsEnabled = true;
            this.results.Items.Add($"Live Logging State Set: {!this.LiveLoggingState}");

            var state = new LiveLoggingRequest()
            {
                State = !this.LiveLoggingState
            };

            var error = this.systemClient.SetLiveLogging(state, this.header);
            if (error.ReturnCode == ErrorCode.NoError)
            {
                this.LiveLoggingState = !this.LiveLoggingState;
                this.SetLiveLogging.Content = $"SetLiveLogging: {!this.LiveLoggingState}";
            }
            else
            {
                this.results.Items.Add("Live Logging State Set: FAILED");
            }

            this.SetErrorCode(error.ReturnCode);
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

    private void SetBatchMode_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var mode = !this.BatchMode ? "ON" : "OFF";
            this.results.Items.Add($"Batch Mode Set: {mode}");

            var state = new BatchModeRequest()
            {
                Mode = !this.BatchMode
            };

            var error = this.systemClient.SetBatchMode(state, this.header);
            if (error.ReturnCode == ErrorCode.NoError)
            {
                this.BatchMode = !this.BatchMode;
                mode = !this.BatchMode ? "ON" : "OFF";
                this.SetBatchMode.Content = $"SetBatchMode: {mode}";
            }
            else
            {
                this.results.Items.Add("Batch State Set: FAILED");
            }

            this.SetErrorCode(error.ReturnCode);
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

    private void SendMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new MessageEntry();
            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var data = dialog.data.Text;
                var matches = data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var message = new SendMessageRequest()
                {
                    AppId = 0x3200,
                    Timeout = 500,
                    Retries = 3
                };

                try
                {
                    foreach (var input in matches)
                    {
                        var intValue = Convert.ToInt32(input, 16);
                        message.Messages.Add(intValue);
                    }
                }
                catch
                {
                    this.results.Items.Add($"Send Message FORMAT INCORRECT: '{data}'");
                    this.SetErrorCode(ErrorCode.MessageArgumentError);
                    return;
                }

                this.results.Items.Add($"Send Message Sent: '{data}'");
                this.results.Items.Add(
                    $"App Id: {message.AppId:X4} \tTimeouts: {message.Timeout} \tRetries: {message.Retries}");

                var sendMessageReply = this.systemClient.SendMessage(message, this.header);
                if (sendMessageReply.ReturnCode == ErrorCode.NoError)
                {
                    var reply = sendMessageReply.Messages.Aggregate("", (current, replyValues) =>
                        current + replyValues.ToString("X4") + " ");

                    this.results.Items.Add($"Send Message Reply: '{reply}'");
                }
                else
                {
                    this.results.Items.Add($"Send Message Request returned: {sendMessageReply.ReturnCode}");
                }

                this.SetErrorCode(sendMessageReply.ReturnCode);
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

    private void GetLogFolder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var reply = this.systemClient.GetLogFolder(new Empty(), this.header);
            this.results.Items.Add($"Log Folder: '{reply.FilePath}'");
            this.SetErrorCode(reply.ReturnCode);

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

    private void GetPPOFileName_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            var reply = this.systemClient.GetPPOFileName(new Empty(), this.header);
            this.results.Items.Add($"PPO FileName: '{reply.FilePath}'");
            this.SetErrorCode(reply.ReturnCode);

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

    private void CreatePGV_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new PGVCreate();
            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (Directory.Exists(dialog.location.Text))
                {
                    var request = new CreatePGVRequest
                    {
                        Location = dialog.location.Text,
                        Asap2FilePath = dialog.asap.Text,
                        HexFilePath = dialog.hex.Text,
                        ControllersFilePath = dialog.controllers.Text,
                        ErrorsFilePath = dialog.errors.Text,
                        EventsFilePath = dialog.events.Text,
                        Comments = dialog.comments.Text,
                        Notes = dialog.notes.Text
                    };

                    var reply = this.systemClient.CreatePGV(request, this.header);
                    if (reply.ReturnCode == 0)
                    {
                        this.results.Items.Add($"Files Created: '{request.Location}'");
                        this.results.Items.Add($"PGV: '{reply.PgvFilePath}'");
                        this.results.Items.Add($"DTV: '{reply.DtvFilePath}'");
                    }
                    else
                    {
                        this.results.Items.Add($"PGV Creation FAILED");

                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add($"Location '{dialog.location.Text}' INVALID");
                }

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
}
