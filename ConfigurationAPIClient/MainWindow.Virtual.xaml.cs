// <copyright file="MainWindow.Virtual.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Win32;
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
        private void SetVirtualParameter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new AddVirtual(this.virtualClient, this.header);
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var param = new VirtualParameterRequest
                    {
                        Id = dialog.id.Text,
                        Name = dialog.name.Text,
                        Description = dialog.description.Text,
                        MinDisplay = Convert.ToDouble(dialog.min.Text),
                        MaxDisplay = Convert.ToDouble(dialog.max.Text),
                        MinLoggingRate = Convert.ToInt32(dialog.rate.Text),
                        ScalingFactor = Convert.ToInt32(dialog.scaling.Text),
                        IsMinNotDef = dialog.minnotdefined.IsChecked != null && (bool)dialog.minnotdefined.IsChecked,
                        Expression = dialog.expression.Text,
                        ConversionId = dialog.conversion.Text,
                        Overwrite = dialog.overwrite.IsChecked != null && (bool)dialog.overwrite.IsChecked,
                        Units = dialog.units.Text,
                        FormatOverride = dialog.format.Text,
                        LowerWarning = Convert.ToDouble(dialog.lower.Text),
                        UpperWarning = Convert.ToDouble(dialog.upper.Text),
                        Group = (string)((DataRowView)dialog.group.SelectedItem).Row.ItemArray[0]!,
                        DataType = (DataType)((DataRowView)dialog.type.SelectedItem).Row.ItemArray[0]!
                    };

                    var reply = this.virtualClient.SetVirtualParameter(param, this.header);

                    if (reply.ReturnCode == ErrorCode.NoError)
                    {
                        this.results.Items.Add($"Added Virtual: '{param.Id}' - Name: '{param.Name}'");
                        this.results.Items.Add($"\t Description: '{param.Description}' ");
                        this.results.Items.Add($"\t Display: Min: {param.MinDisplay} \t Max: {param.MaxDisplay} ");
                        this.results.Items.Add($"\t MinLoggingRate: {param.MinLoggingRate} \t Scaling Factor: {param.ScalingFactor} ");
                        this.results.Items.Add($"\t MinNotDef: {param.IsMinNotDef} \t Conversion: {param.ConversionId} ");
                        this.results.Items.Add($"\t Expression: '{param.Expression}' ");
                        this.results.Items.Add($"\t Overwrite: '{param.Overwrite}' ");
                        this.results.Items.Add($"\t Units: {param.Units} \t FormatOverride: {param.FormatOverride} ");
                        this.results.Items.Add($"\t Warning: Lower: {param.LowerWarning} \t Upper: {param.UpperWarning} ");
                        this.results.Items.Add($"\t Group {param.Group} \t DataType: {param.DataType} ");
                    }

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

        private void GetVirtualParameterProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var request = new ParametersRequest();
                foreach (var item in this.virt.Items)
                {
                    request.ParameterIds.Add(((DataRowView)item).Row.ItemArray[0]?.ToString());
                }

                var reply = this.virtualClient.GetVirtualParameterProperties(request, this.header);
                this.SetErrorCode(reply.ReturnCode);

                this.results.Items.Add($"Count: {reply.Parameters.Count}");
                foreach (var param in reply.Parameters)
                {
                    var data = $"'{param.Id}' - '{param.Name}':\tDesc: '{param.Description}' - Lower: '{param.LowerDisplayLimit}' - Upper: '{param.UpperDisplayLimit}'";
                    data += $" - MinRate: '{param.MinLoggingRate}' - Conv: '{param.ConversionId}' - DataType: {param.DataType}";
                    data += $" - Scaling: {param.ScalingFactor} - MinNotDef: {param.MinNotDefined} - Units: '{param.Units}'";
                    data += $" - Format: {param.Format} - Group: {param.Group} - Expression: '{param.Expression}'";
                    data += $" - Error: '{param.ReturnCode}'";

                    this.results.Items.Add(data);
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


        private void RemoveVirtualParameters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.virt.SelectedItem != null)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var param = new VirtualsRequest();
                    param.Ids.Add((string)((DataRowView)this.virt.SelectedItem).Row.ItemArray[0]);

                    var reply = this.virtualClient.RemoveVirtualParameters(param, this.header);

                    this.results.Items.Add($"Remove Virtual Parameters Requested: '{reply.Ids[0].Id}' \t- Error: {reply.Ids[0].ReturnCode}");

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Remove Virtual Parameters: NO APP OR VIRTUAL PARAMETERS SELECTED");
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

        private void RemoveAllVirtualParameters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.virt.SelectedItem != null)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var reply = this.virtualClient.RemoveAllVirtualParameters(new Empty(), this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                    this.results.Items.Add($"Remove All Virtual Parameters Requested: Error: {reply.ReturnCode}");
                }
                else
                {
                    this.results.Items.Add("Remove All Virtual Parameters: NO APP OR VIRTUAL PARAMETERS SELECTED");
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

        private void RemoveVirtualConversions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.conv.SelectedItem != null)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var param = new VirtualsRequest();
                    param.Ids.Add((string)((DataRowView)this.conv.SelectedItem).Row.ItemArray[0]);

                    var reply = this.virtualClient.RemoveVirtualConversions(param, this.header);

                    this.results.Items.Add($"Remove Virtual Conversions Requested: '{reply.Ids[0].Id}' \t- Error: {reply.Ids[0].ReturnCode}");

                    if (reply.ReturnCode == ErrorCode.NonSpecific)
                    {
                        this.results.Items.Add("Unable to Remove Virtual Conversions that are in use");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Remove Virtual Conversions: NO APP OR VIRTUAL CONVERSIONS SELECTED");
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

        private void RemoveAllVirtualConversions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.conv.SelectedItem != null)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var reply = this.virtualClient.RemoveAllVirtualConvertions(new Empty(), this.header);
                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                    this.results.Items.Add($"Remove All Virtual Conversions Requested: Error: {reply.ReturnCode}");

                    if (reply.ReturnCode == ErrorCode.NonSpecific)
                    {
                        this.results.Items.Add("Unable to Remove Virtual Conversions that are in use");
                    }
                }
                else
                {
                    this.results.Items.Add("Remove All Virtual Conversions: NO APP OR VIRTUAL PARAMETERS SELECTED");
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

        private void GetVirtualParameterGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.virtualClient.GetVirtualParameterGroups(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Get All Virtual Parameter Groups: Total: {reply.Ids.Count}");
                foreach (var group in reply.Ids)
                {
                    this.results.Items.Add($"    Group: '{group}'");
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

        private void GetVirtualParameterGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.virtualClient.GetVirtualParameterGroups(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                this.results.Items.Add($"Get Virtual Parameter Group Details: Total: {reply.Ids.Count}");
                foreach (var group in reply.Ids)
                {
                    var split = group.Split("\\");
                    var name = split.Length == 0 ? group : split[^1];

                    var vgroup = new VirtualGroupRequest
                    {
                        Group = name
                    };

                    var reply2 = this.virtualClient.GetVirtualParameterGroup(vgroup, this.header);
                    this.SetErrorCode(reply2.ReturnCode);
                    this.results.Items.Add($"    Name: '{reply2.Name}' \tDescription: '{reply2.Description}' \tRO: {reply2.ReadOnly} \tError: {reply2.ReturnCode}");
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

        private void GetVirtualParametersInGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var reply = this.virtualClient.GetVirtualParameterGroups(new Empty(), this.header);
                this.SetErrorCode(reply.ReturnCode);
                foreach (var group in reply.Ids)
                {
                    this.results.Items.Add($"  Group: {group}");
                    var vgroup = new VirtualGroupRequest
                    {
                        Group = "\\" + group
                    };

                    var reply2 = this.virtualClient.GetVirtualParametersInGroup(vgroup, this.header);
                    this.SetErrorCode(reply2.ReturnCode);

                    foreach (var id in reply2.Ids)
                    {
                        this.results.Items.Add($"    Name: {id} \tError: {reply2.ReturnCode}");
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
        private void VirtualParametersExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".vpx",
                    Filter = "Virtual Parameters Export File (.vpx)|*.vpx",
                    FileName = "Virtuals"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var file = new VirtualExportRequest
                    {
                        FilePath = dialog.FileName,
                        Group = ""
                    };

                    this.results.Items.Add($"Virtual Parameters Export: \t'{file.FilePath}'");
                    var reply = this.virtualClient.VirtualParametersExport(file, this.header);

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Virtual Parameters Export: No File Name");
                    this.SetErrorCode(ErrorCode.NoError);
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

        private void VirtualParametersImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".vpx",
                    Filter = "Virtual Parameters Export File (.vpx)|*.vpx",
                    FileName = "Virtuals"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var file = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    this.results.Items.Add($"Virtual Parameters Import: \t'{file.FilePath}'");
                    var reply = this.virtualClient.VirtualParametersImport(file, this.header);

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Virtual Parameters Import: No File Name");
                    this.SetErrorCode(ErrorCode.NoError);
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

        private void AddVirtualParameterGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new GroupSelect(this.virtualClient, this.header, true);

                if (dialog.ShowDialog() == true &&
                    !string.IsNullOrEmpty(dialog.name.Text) &&
                    !string.IsNullOrEmpty(dialog.description.Text))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var group = new AddGroupRequest
                    {
                        GroupPath = (string)((DataRowView)dialog.list.SelectedItem).Row.ItemArray[0]!,
                        Name = dialog.name.Text,
                        Description = dialog.description.Text,
                        ReadOnly = false
                    };

                    this.results.Items.Add($"Add Virtual Parameter Group: \t'{group.Name}' to '{group.GroupPath}'");
                    var reply = this.virtualClient.AddVirtualParameterGroup(group, this.header);

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Virtual Parameter Group Add: No Group Name or Description");
                    this.SetErrorCode(ErrorCode.NoError);
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

        private void RemoveVirtualParameterGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new GroupSelect(this.virtualClient, this.header, false);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var group = new VirtualGroupRequest
                    {
                        Group = (string)((DataRowView)dialog.list.SelectedItem).Row.ItemArray[0]!
                    };

                    this.results.Items.Add($"Remove Virtual Parameter Group: \t'{group.Group}'");
                    var reply = this.virtualClient.RemoveVirtualParameterGroup(group, this.header);

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Remove Parameter Group Add: No Group Name");
                    this.SetErrorCode(ErrorCode.NoError);
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
        private void RemoveAllVirtualParametersFromGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new GroupSelect(this.virtualClient, this.header, false);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var group = new VirtualGroupRequest
                    {
                        Group = (string)((DataRowView)dialog.list.SelectedItem).Row.ItemArray[0]!
                    };

                    this.results.Items.Add($"Remove All Virtual Parameters from Group: \t'{group.Group}'");
                    var reply = this.virtualClient.RemoveAllVirtualParametersFromGroup(group, this.header);

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Remove All Virtual Parameters from Group: No Group Name");
                    this.SetErrorCode(ErrorCode.NoError);
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

        private void SetVirtualParameterDataType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new ChangeDataType(this);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new VirtualParameterDataTypeRequest
                    {
                        Id = (string)((DataRowView)dialog.vList.SelectedItem).Row.ItemArray[0]!,
                        DataType = (DataType)((DataRowView)dialog.typeList.SelectedItem).Row.ItemArray[0]!
                    };

                    var reply = this.virtualClient.SetVirtualParameterDataType(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"Set Virtual Parameter: '{request.Id}'");
                    this.results.Items.Add($"\t To Type: '{request.DataType}'");

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
}
