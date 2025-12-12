// <copyright file="MainWindow.Parameter.xaml.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SystemMonitorConfigurationTest.Dialogs;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest
{
    public partial class MainWindow
    {
        private void GetParametersAndGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var app = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var groups = this.paramClient.GetParameterAndGroups(app, this.header);

                    foreach (var parameter in groups.Parameters)
                    {
                        this.results.Items.Add($"Parameter: '{parameter.Id}' \t Group: '{parameter.Group}'");
                    }

                    this.SetErrorCode(groups.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get Parameter And Groups: NO APP SELECTED");
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

        private void GetParameterProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new AppType(this.projectClient, this.header);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new AppTypeRequest
                    {
                        AppId = (uint)(int)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                        DataType = (ParameterType)((DataRowView)dialog.typeList.SelectedItem).Row.ItemArray[0]!
                    };

                    var reply = this.paramClient.GetParameterProperties(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"Count: {reply.Parameters.Count}");
                    foreach (var param in reply.Parameters)
                    {
                        var data = $"'{param.Id}' - '{param.Name}':\tDesc: '{param.Description}' - Type: '{param.Type}' - Units: '{param.Units}'";
                        data += $" - Format: '{param.Format}' - Conv: '{param.ConversionId}' - DataType: {param.DataType}";
                        data += $" - DataSize: {param.DataSize} - Lower: {param.LowerEngineeringLimit} - Upper: {param.UpperEngineeringLimit}";
                        data += $" - Max Rate: {param.MaxLoggingRate} - Prime: {param.Prime} - Read Only: {param.ReadOnly} - Tuneable: {param.Tuneable}";
                        if (param.Groups.Count > 0)
                        {
                            data += " - Groups:  ";
                            data = param.Groups.Aggregate(data, (current, group) => current + $"'{group}'  ");
                        }

                        if (param.MultiplexedIds.Count > 0)
                        {
                            data += " - Mux:  ";
                            data = param.MultiplexedIds.Aggregate(data, (current, mux) => current + $"'{mux}'  ");
                        }
                        this.results.Items.Add(data);
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

        private void GetMapProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.axis2.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.axis2.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetMapProperties(param, this.header);

                    this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\t XAxis: '{reply.XAxisId}' - Pts: {reply.XPoints}");
                    this.results.Items.Add($"\t YAxis: '{reply.YAxisId}' - Pts: {reply.YPoints}");

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("GetMapProperties: NO MAP SELECTED");
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

        private void GetRowDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetRowDetails(param, this.header);

                    this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\t Row Id: {reply.RowId} - Offset: 0x{reply.IdentOffset:X8}");

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("GetRowDetails: NO PARAMETER SELECTED");
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

        private void GetCANParameterProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var request = new ParametersRequest();
                var parameters = new Dictionary<string, string>();
                foreach (var item in this.can.Items)
                {
                    parameters.Add(((DataRowView)item).Row.ItemArray[0]?.ToString()!,((DataRowView)item).Row.ItemArray[1]?.ToString());
                }
                
                var dialog = new SelectParameters(parameters);
                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();
                    Mouse.OverrideCursor = Cursors.Wait;

                    if (dialog.SelectAll.IsChecked == false)
                    {
                        foreach (DataRowView item in dialog.Parameters.SelectedItems)
                        {
                            request.ParameterIds.Add((string)item.Row.ItemArray[0]);
                        }
                    }
                    else
                    {
                        // Send Empty list if SelectAll checked
                    }

                    var reply = this.paramClient.GetCANParameterProperties(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"Count: {reply.Parameters.Count}");
                    foreach (var param in reply.Parameters)
                    {
                        var data =
                            $"'{param.Id}' - '{param.Name}':\tDesc: '{param.Description}' - Lower: '{param.LowerDisplayLimit}' - Upper: '{param.UpperDisplayLimit}'";
                        data +=
                            $" - MinRate: '{param.MinLoggingRate}' - Conv: '{param.ConversionId}' - DataType: {param.DataType}";
                        data +=
                            $" - Scaling: {param.ScalingFactor} - MinNotDef: {param.MinNotDefined} - Rx: {param.Rx}";
                        data +=
                            $" - Bus: {param.CanBus} - Message: {param.CanMessage} - Start: {param.CanStartBit} - Length: {param.CanBitLength}";
                        data +=
                            $" - Gain: {param.CanGain} - Offset: {param.CanOffset} - Mux: {param.CanMuxId} - Order: {param.CanByteOrder}";
                        data += $" - Error: '{param.ReturnCode}'";

                        this.results.Items.Add(data);
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

        private void GetParameterAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new AppType(this.projectClient, this.header);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new ParameterTypeRequest
                    {
                        AppId = (uint)(int)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                        DataType = (ParameterType)((DataRowView)dialog.typeList.SelectedItem).Row.ItemArray[0]!
                    };

                    switch (request.DataType)
                    {
                        case ParameterType.Scalar:
                            request.ParameterId = (string)((DataRowView)this.scalar.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.Axis1:
                            request.ParameterId = (string)((DataRowView)this.axis1.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.Axis2:
                            request.ParameterId = (string)((DataRowView)this.axis2.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.Array:
                            request.ParameterId = (string)((DataRowView)this.array.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.String:
                            request.ParameterId = (string)((DataRowView)this.str.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.Can:
                            request.ParameterId = (string)((DataRowView)this.can.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.Virtual:
                            request.ParameterId = (string)((DataRowView)this.virt.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.Axis:
                            request.ParameterId = (string)((DataRowView)this.axis.SelectedItem).Row.ItemArray[0];
                            break;
                        case ParameterType.Input:
                            //request.ParameterId = (string)((DataRowView)input.SelectedItem).Row.ItemArray[0]
                            break;
                        case ParameterType.Ecu:
                        case ParameterType.Measurement:
                            request.ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0];
                            break;
                    }

                    var reply = this.paramClient.GetParameterAddress(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"App: '{request.AppId}' - Parameter: '{request.ParameterId}' - Type {request.DataType}");
                    this.results.Items.Add($"\t Address: 0x{reply.Address:X8} - Ident: {reply.Ident}");

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

        private void GetParameterBitMask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetParameterBitMask(param, this.header);

                    this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\t Bit Mask: 0x{reply.Mask:X8}");

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("GetParameterBitMask: NO PARAMETER SELECTED");
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

        private void GetParameterBitShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetParameterBitShift(param, this.header);

                    this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\t Bit Shift: {reply.Shift}");

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("GetParameterBitShift: NO PARAMETER SELECTED");
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

        private void GetParameterByteOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetParameterByteOrder(param, this.header);

                    this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\t Byte Order: {reply.ByteOrder}");

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("GetParameterByteOrder: NO PARAMETER SELECTED");
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

        private void ParameterLoggable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.ParameterLoggable(param, this.header);

                    this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\t Loggable: {reply.Loggable}");

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("ParameterLoggable: NO PARAMETER SELECTED");
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

        private void GetModifiedParameters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var app = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var groups = this.paramClient.GetModifiedParameters(app, this.header);

                    this.results.Items.Add($"Modified Parameters in: 0x{app.AppId:X4}");
                    foreach (var parameter in groups.Parameters)
                    {
                        this.results.Items.Add($"Parameter: '{parameter.Id}' \t '{parameter.Name}'");
                    }

                    this.SetErrorCode(groups.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get Modified Parameters: NO APP SELECTED");
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

        private void GetParameterWarningLimits_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (this.measurement.SelectedItem != null)
                {

                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetParameterWarningLimits(param, this.header);

                    this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\t Low: {reply.Low} \t High: {reply.High} ");

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

        private void SetParameterWarningLimits_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new TextEntry
                {
                    label =
                    {
                        Content = "Low"
                    },
                    label2 =
                    {
                        Content = "High"
                    }
                };

                if (dialog.ShowDialog() == true)
                {
                    if (this.measurement.SelectedItem != null)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        var stopwatch = Stopwatch.StartNew();

                        var param = new WarningLimitsRequest
                        {
                            AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                            ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0],
                            Low = Convert.ToDouble(dialog.text.Text),
                            High = Convert.ToDouble(dialog.text2.Text)
                        };

                        var reply = this.paramClient.SetParameterWarningLimits(param, this.header);

                        this.results.Items.Add($"App: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                        this.results.Items.Add($"\t Low: {param.Low} \t High: {param.High} ");

                        this.SetErrorCode(reply.ReturnCode);
                        this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                    }
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

        private void DeleteMinMax_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();
                var reply = this.paramClient.DeleteMinMax(new Empty(), this.header);

                this.results.Items.Add("Delete Min Max");

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

        private void ExportInputSignals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".xml",
                    Filter = "Input Signal File (.xml)|*.xml",
                    FileName = "Input Signals"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var file = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    this.results.Items.Add($"Input Signals Export: \t'{file.FilePath}'");
                    var error = this.paramClient.ExportInputSignals(file, this.header);

                    this.SetErrorCode(error.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Input Signals Export: No File Name");
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

        private void ImportInputSignals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".xml",
                    Filter = "Input Signal File (.xml)|*.xml",
                    FileName = "Input Signals"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var file = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    this.results.Items.Add($"Input Signals Import: \t'{file.FilePath}'");
                    var error = this.paramClient.ImportInputSignals(file, this.header);

                    this.SetErrorCode(error.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Input Signals Import: No File Name");
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

        private void RegenerateInputSignalParameters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();
                var reply = this.paramClient.RegenerateInputSignalParameters(new Empty(), this.header);

                this.results.Items.Add("Regenerate Input Signal Parameters");

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

        private void UndoDataChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new Dialogs.BufferType();

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var buffer = new UndoRequest
                    {
                        BufferType = (SystemMonitorProtobuf.BufferType)(int)((DataRowView)dialog.list.SelectedItem).Row.ItemArray[0]!,
                    };

                    var reply = this.paramClient.UndoDataChanges(buffer, this.header);

                    this.results.Items.Add("Undo Data Changes");

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

        private void RestoreValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.scalar.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.RestoreValue(param, this.header);

                    this.results.Items.Add($"Restore Value: \tApp: '{param.AppId}' - Parameter: '{param.ParameterId}'");

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

        private void GetAxisParameterFromMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (this.axis2.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.axis2.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetAxisParameterFromMap(param, this.header);

                    this.results.Items.Add($"Get Axis Parameter From Map: \tApp: '{param.AppId}' - Parameter: '{param.ParameterId}'");
                    this.results.Items.Add($"\tX Axis: '{reply.ParameterIds[0]}'");
                    this.results.Items.Add($"\tY Axis: '{reply.ParameterIds[1]}'");

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


        private void GetConversionUse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (this.conv.SelectedItem != null)
                {
                    var conversion = new ConversionRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ConversionId = (string)((DataRowView)this.conv.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetConversionUse(conversion, this.header);

                    this.results.Items.Add($"Get Conversion Use: \tApp: '{conversion.AppId}' - Conversion: '{conversion.ConversionId}'");
                    foreach (var parameter in reply.ParameterIds)
                    {
                        this.results.Items.Add($"\tParameter: '{parameter}'");
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

        private void GetConversionType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (this.conv.SelectedItem != null)
                {
                    var conversion = new ConversionNoAppRequest
                    {
                        ConversionId = (string)((DataRowView)this.conv.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetConversionType(conversion, this.header);

                    this.results.Items.Add($"Get Conversion Type: '{conversion.ConversionId}'");
                    this.results.Items.Add($"\tType: '{reply.Type}'");

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

        private void GetConversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (this.conv.SelectedItem != null)
                {
                    var type = ConversionType.Rational;
                    var text = (string)((DataRowView)this.conv.SelectedItem).Row.ItemArray[1];
                    if (text != null && ( text.EndsWith($"{ConversionType.Text}") || text.EndsWith($"{ConversionType.Text} - VIRTUAL OR CAN") ) )
                    {
                        type = ConversionType.Text;
                    }
                    else if (text != null && ( text.EndsWith($"{ConversionType.Table}")|| text.EndsWith($"{ConversionType.Table} - VIRTUAL OR CAN") ) )
                    {
                        type = ConversionType.Table;
                    }
                    else if (text != null && ( text.EndsWith($"{ConversionType.Formula}") || text.EndsWith($"{ConversionType.Formula} - VIRTUAL OR CAN") ) )
                    {
                        type = ConversionType.Formula;
                    }

                    var conversion = new ConversionNoAppRequest
                    {
                        ConversionId = (string)((DataRowView)this.conv.SelectedItem).Row.ItemArray[0]
                    };

                    switch (type)
                    {
                        case ConversionType.Rational:
                            {
                                var reply = this.paramClient.GetRationalConversion(conversion, this.header);
                                this.results.Items.Add($"Get Conversion : Conversion: '{conversion.ConversionId}'");
                                this.results.Items.Add($"\t1: {reply.Coefficient1} \t2: {reply.Coefficient2} \t3: {reply.Coefficient3} \t4: {reply.Coefficient4} \t5: {reply.Coefficient5} \t6: {reply.Coefficient6}");
                                this.results.Items.Add($"\tComment: '{reply.Comment}'");
                                this.results.Items.Add($"\tFormat: '{reply.Format}'");
                                this.results.Items.Add($"\tUnits: '{reply.Units}'");
                                this.results.Items.Add($"\tDefault: '{reply.Default}'");
                                this.SetErrorCode(reply.ReturnCode);
                            }
                            break;

                        case ConversionType.Table:
                            {
                                var reply = this.paramClient.GetTableConversion(conversion, this.header);
                                this.results.Items.Add($"Get Conversion : Conversion: '{conversion.ConversionId}'");
                                this.results.Items.Add($"\tComment: '{reply.Comment}'");
                                this.results.Items.Add($"\tInterpolate: '{reply.Interpolate}'");
                                this.results.Items.Add($"\tFormat: '{reply.Format}'");
                                this.results.Items.Add($"\tUnits: '{reply.Units}'");
                                this.results.Items.Add($"\tDefault: '{reply.Default}'");

                                foreach (var value in reply.Values)
                                {
                                    this.results.Items.Add($"\tRaw: '{value.Raw}'   \tMapped: '{value.Mapped}'");
                                }

                                this.SetErrorCode(reply.ReturnCode);
                            }
                            break;

                        case ConversionType.Text:
                            {
                                var reply = this.paramClient.GetTextConversion(conversion, this.header);
                                this.results.Items.Add($"Get Conversion : Conversion: '{conversion.ConversionId}'");
                                this.results.Items.Add($"\tFormat: '{reply.Format}'");
                                this.results.Items.Add($"\tUnits: '{reply.Units}'");
                                this.results.Items.Add($"\tDefault: '{reply.Default}'");

                                foreach (var value in reply.Values)
                                {
                                    this.results.Items.Add($"\tRaw: '{value.Raw}'   \tMapped: '{value.Mapped}'");
                                }

                                this.SetErrorCode(reply.ReturnCode);
                            }
                            break;

                        case ConversionType.Formula:
                            {
                                var reply = this.paramClient.GetFormulaConversion(conversion, this.header);
                                this.results.Items.Add($"Get Conversion : Conversion: '{conversion.ConversionId}'");
                                this.results.Items.Add($"\tComment: '{reply.Comment}'");
                                this.results.Items.Add($"\tFormat: '{reply.Format}'");
                                this.results.Items.Add($"\tUnits: '{reply.Units}'");
                                this.results.Items.Add($"\tFormula: '{reply.Formula}'");
                                this.results.Items.Add($"\tInverse: '{reply.Inverse}'");
                                this.SetErrorCode(reply.ReturnCode);
                            }
                            break;
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

        private void GetAppConversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (this.conv.SelectedItem != null)
                {
                    var type = ConversionType.Rational;
                    var text = (string)((DataRowView)this.conv.SelectedItem).Row.ItemArray[1];
                    if (text != null && ( text.EndsWith($"{ConversionType.Text}") || text.EndsWith($"{ConversionType.Text} - VIRTUAL OR CAN") ) )
                    {
                        type = ConversionType.Text;
                    }
                    else if (text != null && ( text.EndsWith($"{ConversionType.Table}")|| text.EndsWith($"{ConversionType.Table} - VIRTUAL OR CAN") ) )
                    {
                        type = ConversionType.Table;
                    }
                    else if (text != null && ( text.EndsWith($"{ConversionType.Formula}") || text.EndsWith($"{ConversionType.Formula} - VIRTUAL OR CAN") ) )
                    {
                        type = ConversionType.Formula;
                    }

                    var conversion = new ConversionRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ConversionId = (string)((DataRowView)this.conv.SelectedItem).Row.ItemArray[0]
                    };

                    switch (type)
                    {
                        case ConversionType.Rational:
                            {
                                var reply = this.paramClient.GetAppRationalConversion(conversion, this.header);
                                this.results.Items.Add($"Get Conversion : Conversion: '{conversion.ConversionId}'");
                                this.results.Items.Add($"\t1: {reply.Coefficient1} \t2: {reply.Coefficient2} \t3: {reply.Coefficient3} \t4: {reply.Coefficient4} \t5: {reply.Coefficient5} \t6: {reply.Coefficient6}");
                                this.results.Items.Add($"\tComment: '{reply.Comment}'");
                                this.results.Items.Add($"\tFormat: '{reply.Format}'");
                                this.results.Items.Add($"\tUnits: '{reply.Units}'");
                                this.results.Items.Add($"\tDefault: '{reply.Default}'");
                                this.SetErrorCode(reply.ReturnCode);
                            }
                            break;

                        case ConversionType.Table:
                            {
                                var reply = this.paramClient.GetAppTableConversion(conversion, this.header);
                                this.results.Items.Add($"Get Conversion : Conversion: '{conversion.ConversionId}'");
                                this.results.Items.Add($"\tComment: '{reply.Comment}'");
                                this.results.Items.Add($"\tInterpolate: '{reply.Interpolate}'");
                                this.results.Items.Add($"\tFormat: '{reply.Format}'");
                                this.results.Items.Add($"\tUnits: '{reply.Units}'");
                                this.results.Items.Add($"\tDefault: '{reply.Default}'");

                                foreach (var value in reply.Values)
                                {
                                    this.results.Items.Add($"\tRaw: '{value.Raw}'   \tMapped: '{value.Mapped}'");
                                }

                                this.SetErrorCode(reply.ReturnCode);
                            }
                            break;

                        case ConversionType.Text:
                            this.results.Items.Add("Not Supported for Text Conversions");
                            break;

                        case ConversionType.Formula:
                            this.results.Items.Add("Not Supported for Formula Conversions");
                            break;
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

        private void SetRationalConversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new RationalConv();

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new RationalConversionRequest
                    {
                        ConversionId = dialog.name.Text,
                        Coefficient1 = Convert.ToDouble(dialog.co1.Text),
                        Coefficient2 = Convert.ToDouble(dialog.co2.Text),
                        Coefficient3 = Convert.ToDouble(dialog.co3.Text),
                        Coefficient4 = Convert.ToDouble(dialog.co4.Text),
                        Coefficient5 = Convert.ToDouble(dialog.co5.Text),
                        Coefficient6 = Convert.ToDouble(dialog.co6.Text),
                        Comment = dialog.comment.Text,
                        Format = dialog.format.Text,
                        Units = dialog.units.Text,
                        Default = dialog.def.Text,
                        Overwrite = dialog.overwrite.IsChecked != null && (bool)dialog.overwrite.IsChecked
                    };

                    var reply = this.paramClient.SetRationalConversion(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"Conversion: '{request.ConversionId}''");
                    this.results.Items.Add($"\t - 1: {request.Coefficient1} - 2: {request.Coefficient2} - 3: {request.Coefficient3}");
                    this.results.Items.Add($"\t - 4: {request.Coefficient4} - 5: {request.Coefficient5} - 6: {request.Coefficient6}");
                    this.results.Items.Add($"\t - Comment: '{request.Comment}'");
                    this.results.Items.Add($"\t - Format: '{request.Format}'");
                    this.results.Items.Add($"\t - Units: '{request.Units}'");
                    this.results.Items.Add($"\t - Default: '{request.Default}'");
                    this.results.Items.Add($"\t - Overwrite: {request.Overwrite}");
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

        private void SetTableConversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new TableConv();

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new TableConversionRequest
                    {
                        ConversionId = dialog.name.Text,
                        Comment = dialog.comment.Text,
                        Format = dialog.format.Text,
                        Units = dialog.units.Text,
                        Default = dialog.def.Text,
                        Interpolate = dialog.interpolate.IsChecked != null && (bool)dialog.interpolate.IsChecked,
                        Overwrite = dialog.overwrite.IsChecked != null && (bool)dialog.overwrite.IsChecked
                    };

                    this.results.Items.Add($"Conversion: '{request.ConversionId}''");
                    foreach (var value in dialog.values)
                    {
                        request.Values.Add(value);
                        this.results.Items.Add($"\t Map: {value.Raw} =>: {value.Mapped}");
                    }

                    var reply = this.paramClient.SetTableConversion(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"\t - Comment: '{request.Comment}'");
                    this.results.Items.Add($"\t - Format: '{request.Format}'");
                    this.results.Items.Add($"\t - Units: '{request.Units}'");
                    this.results.Items.Add($"\t - Default: '{request.Default}'");
                    this.results.Items.Add($"\t - Interpolate: {request.Interpolate}");
                    this.results.Items.Add($"\t - Overwrite: {request.Overwrite}");
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

        private void SetTextConversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new TextConv();

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new TextConversionRequest
                    {
                        ConversionId = dialog.name.Text,
                        Format = dialog.format.Text,
                        Units = dialog.units.Text,
                        Default = dialog.def.Text,
                        Overwrite = dialog.overwrite.IsChecked != null && (bool)dialog.overwrite.IsChecked
                    };

                    this.results.Items.Add($"Conversion: '{request.ConversionId}''");
                    foreach (var value in dialog.values)
                    {
                        request.Values.Add(value);
                        this.results.Items.Add($"\t Map: {value.Raw} =>: {value.Mapped}");
                    }

                    var reply = this.paramClient.SetTextConversion(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"\t - Format: '{request.Format}'");
                    this.results.Items.Add($"\t - Units: '{request.Units}'");
                    this.results.Items.Add($"\t - Default: '{request.Default}'");
                    this.results.Items.Add($"\t - Overwrite: {request.Overwrite}");
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

        private void SetFormulaConversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new FormulaConv();

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var request = new FormulaConversionRequest
                    {
                        ConversionId = dialog.name.Text,
                        Format = dialog.format.Text,
                        Units = dialog.units.Text,
                        Formula = dialog.formula.Text,
                        Inverse = dialog.inverse.Text,
                        Comment = dialog.comment.Text,
                        Overwrite = dialog.overwrite.IsChecked != null && (bool)dialog.overwrite.IsChecked
                    };

                    this.results.Items.Add($"Conversion: '{request.ConversionId}''");
                    this.results.Items.Add($"\t Formula: '{request.Formula}'");
                    this.results.Items.Add($"\t Inverse: '{request.Inverse}'");

                    var reply = this.paramClient.SetFormulaConversion(request, this.header);
                    this.SetErrorCode(reply.ReturnCode);

                    this.results.Items.Add($"\t - Comment: '{request.Comment}'");
                    this.results.Items.Add($"\t - Format: '{request.Format}'");
                    this.results.Items.Add($"\t - Units: '{request.Units}'");
                    this.results.Items.Add($"\t - Overwrite: {request.Overwrite}");
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
