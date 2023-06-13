// <copyright file="MainWindow.Parameter2.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Grpc.Core;
using Microsoft.Win32;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest
{
    public partial class MainWindow
    {
        private void GetValueOffset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.GetValueOffset(param, this.header);
                    this.results.Items.Add($"Get Value Offset: '{this.appList.Text}' - '{this.measurement.Text}' :- {reply.Offset}");
                    this.SetErrorCode(reply.ReturnCode);

                    this.ValueOffset = reply.Offset + 1;
                    this.setOffset.Content = $"SetValueOffset: {this.ValueOffset}";
                }
                else
                {
                    this.results.Items.Add("Get Value Offset: NO APP OR MEASUREMENT PARAMETER SELECTED");
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

        private void SetValueOffset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.measurement.SelectedItem != null)
                {
                    var param = new OffsetRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0],
                        Offset = ValueOffset

                    };

                    var reply = this.paramClient.SetValueOffset(param, this.header);
                    this.results.Items.Add($"Set Value Offset: '{this.appList.Text}' - '{this.measurement.Text}' :- {this.ValueOffset}");
                    this.SetErrorCode(reply.ReturnCode);

                    this.ValueOffset = this.ValueOffset + 1;
                    this.setOffset.Content = $"SetValueOffset: {this.ValueOffset}";
                }
                else
                {
                    this.results.Items.Add("Set Value Offset: NO APP OR MEASUREMENT PARAMETER SELECTED");
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

        private void ZeroLiveValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.measurement.SelectedItem != null)
                {
                    var param = new ParameterRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                    };

                    var reply = this.paramClient.ZeroLiveValue(param, this.header);
                    this.results.Items.Add($"Zero Live Value: '{this.appList.Text}' - '{this.measurement.Text}'");
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Zero Live Value: NO APP OR MEASUREMENT PARAMETER SELECTED");
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

        private void GetValueMeasurement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.measurement.SelectedItem != null)
                {
                    var param = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.measurement.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var stopwatch = Stopwatch.StartNew();
                    var reply = this.paramClient.GetValueMeasurement(param, this.header);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";

                    this.results.Items.Add($"Total Measurement Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"Measurement Value: '{value.ParameterId}' \t= {value.Value} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Measurement Value: NO APP OR MEASUREMENT PARAMETERS SELECTED");
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

        private void GetValueScalar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.scalar.SelectedItem != null)
                {
                    var param = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.scalar.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValueScalar(param, this.header);

                    this.results.Items.Add($"Total Scalar Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"Scalar Value: '{value.ParameterId}' \t= {value.Value} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Scalar Value: NO APP OR SCALAR PARAMETERS SELECTED");
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

        private void GetValue1AxisMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.axis1.SelectedItem != null)
                {
                    var param = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.axis1.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValue1AxisMap(param, this.header);

                    this.results.Items.Add($"Total 1 Axis Map Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"1 Axis Map Values: '{value.ParameterId}' \t= {value.Values} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("1 Axis Map Values: NO APP OR 1 AXIS MAP PARAMETERS SELECTED");
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

        private void GetValue2AxisMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.axis2.SelectedItem != null)
                {
                    var param = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.axis2.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValue2AxisMap(param, this.header);

                    this.results.Items.Add($"Total 2 Axis Map Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"2 Axis Map Values: '{value.ParameterId}' \t- Error: {value.ReturnCode}");
                        foreach (var row in value.Rows)
                        {
                            this.results.Items.Add($"\t{row.Values}");
                        }
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("2 Axis Map Values: NO APP OR 2 AXIS MAP PARAMETERS SELECTED");
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


        private void GetValueAxis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.axis.SelectedItem != null)
                {
                    var param = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.axis.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValueAxis(param, this.header);

                    this.results.Items.Add($"Total Axis Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"Axis Values: '{value.ParameterId}' \t= {value.Values} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Axis Values: NO APP OR AXIS PARAMETERS SELECTED");
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

        private void GetValueArray_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.array.SelectedItem != null)
                {
                    var param = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.array.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValueArray(param, this.header);

                    this.results.Items.Add($"Total Array Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"Array Values: '{value.ParameterId}' \t= {value.Values} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Array Values: NO APP OR ARRAY PARAMETERS SELECTED");
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


        private void GetValueString_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.str.SelectedItem != null)
                {
                    var param = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.str.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValueString(param, this.header);

                    this.results.Items.Add($"Total String Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"String Value: '{value.ParameterId}' \t= '{value.Value}' \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("String Value: NO APP OR STRING PARAMETERS SELECTED");
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

        private void GetValueCAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.can.SelectedItem != null)
                {
                    var param = new ParametersRequest();
                    foreach (var parameter in this.can.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValueCAN(param, this.header);

                    this.results.Items.Add($"Total CAN Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"CAN Value: '{value.ParameterId}' \t= {value.Value} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("CAN Value: NO APP OR CAN PARAMETERS SELECTED");
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

        private void GetValueVirtual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null && this.virt.SelectedItem != null)
                {
                    var param = new ParametersRequest();
                    foreach (var parameter in this.virt.Items)
                    {
                        param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var reply = this.paramClient.GetValueVirtual(param, this.header);

                    this.results.Items.Add($"Total Virtual Values Requested: {reply.Values.Count}");
                    foreach (var value in reply.Values)
                    {
                        this.results.Items.Add(
                            $"Virtual Value: '{value.ParameterId}' \t= {value.Value} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Virtual Value: NO APP OR VIRTUAL PARAMETERS SELECTED");
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

        private void GetDTVValueScalar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".DTV",
                    Filter = "DTV (.DTV)|*.DTV"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();

                    if (this.appList.SelectedItem != null && this.scalar.SelectedItem != null)
                    {
                        var param = new ParametersFileRequest
                        {
                            FilePath = dialog.FileName
                        };

                        var count = 0;
                        foreach (var parameter in this.scalar.Items)
                        {
                            // Just first 10 parameters
                            if (count++ < 10)
                            {
                                param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                            }
                        }

                        var reply = this.paramClient.GetDTVValueScalar(param, this.header);

                        this.results.Items.Add($"DTV: '{param.FilePath}'");
                        this.results.Items.Add($"Total Scalar Values Requested: {reply.Values.Count}");
                        foreach (var value in reply.Values)
                        {
                            this.results.Items.Add($"Scalar Value: '{value.ParameterId}' \t= {value.Value}");
                        }

                        this.SetErrorCode(reply.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("Scalar Value: NO APP OR SCALAR PARAMETERS SELECTED");
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

        private void GetDTVValue1AxisMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".DTV",
                    Filter = "DTV (.DTV)|*.DTV"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();

                    if (this.appList.SelectedItem != null && this.axis1.SelectedItem != null)
                    {
                        var param = new ParametersFileRequest
                        {
                            FilePath = dialog.FileName
                        };

                        foreach (var parameter in this.axis1.Items)
                        {
                            param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                        }

                        var reply = this.paramClient.GetDTVValue1AxisMap(param, this.header);

                        this.results.Items.Add($"DTV: '{param.FilePath}'");
                        this.results.Items.Add($"Total 1 Axis Map Values Requested: {reply.Values.Count}");
                        foreach (var value in reply.Values)
                        {
                            this.results.Items.Add(
                                $"1 Axis Map Values: '{value.ParameterId}' \t= {value.Values}");
                        }

                        this.SetErrorCode(reply.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("1 Axis Value: NO APP OR 1 AXIS PARAMETERS SELECTED");
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

        private void GetDTVValue2AxisMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".DTV",
                    Filter = "DTV (.DTV)|*.DTV"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();

                    if (this.appList.SelectedItem != null && this.axis2.SelectedItem != null)
                    {
                        var param = new ParametersFileRequest
                        {
                            FilePath = dialog.FileName
                        };

                        foreach (var parameter in this.axis2.Items)
                        {
                            param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                        }

                        var reply = this.paramClient.GetDTVValue2AxisMap(param, this.header);

                        this.results.Items.Add($"DTV: '{param.FilePath}'");
                        this.results.Items.Add($"Total 2 Axis Map Values Requested: {reply.Values.Count}");
                        foreach (var value in reply.Values)
                        {
                            this.results.Items.Add(
                                $"2 Axis Map Values: '{value.ParameterId}'");
                            foreach (var row in value.Rows)
                            {
                                this.results.Items.Add($"\t{row.Values}");
                            }
                        }

                        this.SetErrorCode(reply.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("2 Axis Value: NO APP OR 2 AXIS PARAMETERS SELECTED");
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

        private void GetDTVValueAxis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".DTV",
                    Filter = "DTV (.DTV)|*.DTV"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();

                    if (this.appList.SelectedItem != null && this.axis.SelectedItem != null)
                    {
                        var param = new ParametersFileRequest
                        {
                            FilePath = dialog.FileName
                        };

                        foreach (var parameter in this.axis.Items)
                        {
                            param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                        }

                        var reply = this.paramClient.GetDTVValueAxis(param, this.header);

                        this.results.Items.Add($"DTV: '{param.FilePath}'");
                        this.results.Items.Add($"Total Axis Values Requested: {reply.Values.Count}");
                        foreach (var value in reply.Values)
                        {
                            this.results.Items.Add(
                                $"Axis Values: '{value.ParameterId}' \t= {value.Values}");
                        }

                        this.SetErrorCode(reply.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("Axis Value: NO APP OR AXIS PARAMETERS SELECTED");
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

        private void GetDTVValueArray_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".DTV",
                    Filter = "DTV (.DTV)|*.DTV"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();

                    if (this.appList.SelectedItem != null && this.array.SelectedItem != null)
                    {
                        var param = new ParametersFileRequest
                        {
                            FilePath = dialog.FileName
                        };

                        foreach (var parameter in this.array.Items)
                        {
                            param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                        }

                        var reply = this.paramClient.GetDTVValueArray(param, this.header);

                        this.results.Items.Add($"DTV: '{param.FilePath}'");
                        this.results.Items.Add($"Total Array Values Requested: {reply.Values.Count}");
                        foreach (var value in reply.Values)
                        {
                            this.results.Items.Add(
                                $"Array Values: '{value.ParameterId}' \t= {value.Values}");
                        }

                        this.SetErrorCode(reply.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("Array Value: NO APP OR ARRAY PARAMETERS SELECTED");
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

        private void GetDTVValueString_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var dialog = new OpenFileDialog
                {
                    DefaultExt = ".DTV",
                    Filter = "DTV (.DTV)|*.DTV"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();

                    if (this.appList.SelectedItem != null && this.str.SelectedItem != null)
                    {
                        var param = new ParametersFileRequest
                        {
                            FilePath = dialog.FileName
                        };

                        foreach (var parameter in this.str.Items)
                        {
                            param.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                        }

                        var reply = this.paramClient.GetDTVValueString(param, this.header);

                        this.results.Items.Add($"DTV: '{param.FilePath}'");
                        this.results.Items.Add($"Total String Values Requested: {reply.Values.Count}");
                        foreach (var value in reply.Values)
                        {
                            this.results.Items.Add(
                                $"String Values: '{value.ParameterId}' \t= {value.Value}");
                        }

                        this.SetErrorCode(reply.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("String Value: NO STRING OR ARRAY PARAMETERS SELECTED");
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

        private void SetValueScalar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.scalar.SelectedItem != null)
                {
                    var stopwatchGet = Stopwatch.StartNew();

                    // First Get all Scalar Parameter Values for the App
                    var paramGet = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.scalar.Items)
                    {
                        paramGet.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var replyGet = this.paramClient.GetValueScalar(paramGet, this.header);

                    this.results.Items.Add($"Total Scalar Values Requested: {replyGet.Values.Count} - {stopwatchGet.ElapsedMilliseconds}ms");
                    this.results.Items.Add("\tUpdate each Scalar value by 1");


                    // Set Values + 1
                    var stopwatch = Stopwatch.StartNew();
                    this.results.Items.Add($"Total Scalar Values Set: {replyGet.Values.Count}");

                    var param = new AppParameterValuesRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in replyGet.Values)
                    {
                        var set = new ParameterSetValue
                        {
                            ParameterId = parameter.ParameterId,
                            Value = parameter.Value + 1
                        };

                        param.Parameters.Add(set);
                    }

                    var reply = this.paramClient.SetValueScalar(param, this.header);

                    this.results.Items.Add($"Total Scalar Values Set: {reply.Parameters.Count}");


                    foreach (var value in reply.Parameters)
                    {
                        this.results.Items.Add(
                            $"Scalar Value: '{value.ParameterId}' \t= {value.Value} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Scalar Value: NO APP OR SCALAR PARAMETERS SELECTED");
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

        private void SetValue1AxisMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.axis1.SelectedItem != null)
                {
                    var stopwatchGet = Stopwatch.StartNew();

                    // First Get all Parameter Values for the App
                    var paramGet = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.axis1.Items)
                    {
                        paramGet.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var replyGet = this.paramClient.GetValue1AxisMap(paramGet, this.header);

                    this.results.Items.Add($"Total 1 Axis Values Requested: {replyGet.Values.Count} - {stopwatchGet.ElapsedMilliseconds}ms");
                    this.results.Items.Add("\tUpdate each 1 Axis value by 1");


                    // Set Values + 1
                    var stopwatch = Stopwatch.StartNew();
                    var param = new AppArray1dParameterValuesRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in replyGet.Values)
                    {
                        var set = new Array1dParameterSetValue
                        {
                            ParameterId = parameter.ParameterId,
                        };

                        foreach (var value in parameter.Values)
                        {
                            set.Values.Add(value + 1);
                        }

                        param.Parameters.Add(set);
                    }

                    var reply = this.paramClient.SetValue1AxisMap(param, this.header);

                    this.results.Items.Add($"Total 1 Axis Values Set: {reply.Parameters.Count}");

                    foreach (var value in reply.Parameters)
                    {
                        this.results.Items.Add(
                            $"1 Axis Value: '{value.ParameterId}' \t= {value.Values} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("1 Axis Value: NO APP OR 1 AXIS PARAMETERS SELECTED");
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

        private void SetValue2AxisMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.axis2.SelectedItem != null)
                {
                    var stopwatchGet = Stopwatch.StartNew();

                    // First Get all Parameter Values for the App
                    var paramGet = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.axis2.Items)
                    {
                        paramGet.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var replyGet = this.paramClient.GetValue2AxisMap(paramGet, this.header);

                    this.results.Items.Add($"Total 2 Axis Values Requested: {replyGet.Values.Count} - {stopwatchGet.ElapsedMilliseconds}ms");
                    this.results.Items.Add("\tUpdate each 2 Axis value");


                    // Set Values + 1
                    var stopwatch = Stopwatch.StartNew();
                    var param = new AppArray2dParameterValuesRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in replyGet.Values)
                    {
                        var set = new Array2dParameterSetValue
                        {
                            ParameterId = parameter.ParameterId,
                        };

                        var count1 = 0;
                        foreach (var row in parameter.Rows)
                        {
                            var count2 = 1;
                            var rowValues = new RowValues();
                            foreach (var unused in row.Values)
                            {
                                rowValues.Values.Add((count1 * 10) + count2++);
                            }

                            count1++;
                            set.Rows.Add(rowValues);
                        }

                        param.Parameters.Add(set);
                    }

                    var reply = this.paramClient.SetValue2AxisMap(param, this.header);

                    this.results.Items.Add($"Total 2 Axis Values Set: {reply.Parameters.Count}");

                    foreach (var value in reply.Parameters)
                    {
                        this.results.Items.Add(
                            $"2 Axis Value: '{value.ParameterId}' \t- Error: {value.ReturnCode}");
                        foreach (var row in value.Rows)
                        {
                            this.results.Items.Add($"\t \t= {row.Values}");
                        }
                    }

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("2 Axis Value: NO APP OR 2 AXIS PARAMETERS SELECTED");
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

        private void SetValueAxis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.axis.SelectedItem != null)
                {
                    var stopwatchGet = Stopwatch.StartNew();

                    // First Get all Parameter Values for the App
                    var paramGet = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.axis.Items)
                    {
                        paramGet.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var replyGet = this.paramClient.GetValueAxis(paramGet, this.header);

                    this.results.Items.Add($"Total Axis Values Requested: {replyGet.Values.Count} - {stopwatchGet.ElapsedMilliseconds}ms");
                    this.results.Items.Add("\tUpdate each Axis value by 1");


                    // Set Values + 1
                    var stopwatch = Stopwatch.StartNew();
                    var param = new AppArray1dParameterValuesRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in replyGet.Values)
                    {
                        var set = new Array1dParameterSetValue
                        {
                            ParameterId = parameter.ParameterId,
                        };

                        foreach (var value in parameter.Values)
                        {
                            set.Values.Add(value + 1);
                        }

                        param.Parameters.Add(set);
                    }

                    var reply = this.paramClient.SetValueAxis(param, this.header);

                    this.results.Items.Add($"Total Axis Values Set: {reply.Parameters.Count}");

                    foreach (var value in reply.Parameters)
                    {
                        this.results.Items.Add(
                            $"Axis Value: '{value.ParameterId}' \t= {value.Values} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Axis Value: NO APP OR AXIS PARAMETERS SELECTED");
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

        private void SetValueArray_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.array.SelectedItem != null)
                {
                    var stopwatchGet = Stopwatch.StartNew();

                    // First Get all Parameter Values for the App
                    var paramGet = new AppParametersRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.array.Items)
                    {
                        paramGet.ParameterIds.Add((string)((DataRowView)parameter).Row.ItemArray[0]);
                    }

                    var replyGet = this.paramClient.GetValueArray(paramGet, this.header);

                    this.results.Items.Add($"Total Array Values Requested: {replyGet.Values.Count} - {stopwatchGet.ElapsedMilliseconds}ms");
                    this.results.Items.Add("\tUpdate each Array value by 1");


                    // Set Values + 1
                    var stopwatch = Stopwatch.StartNew();
                    var param = new AppArray1dParameterValuesRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in replyGet.Values)
                    {
                        var set = new Array1dParameterSetValue
                        {
                            ParameterId = parameter.ParameterId,
                        };

                        foreach (var value in parameter.Values)
                        {
                            set.Values.Add(value + 1);
                        }

                        param.Parameters.Add(set);
                    }

                    var reply = this.paramClient.SetValueArray(param, this.header);

                    this.results.Items.Add($"Total Array Values Set: {reply.Parameters.Count}");

                    foreach (var value in reply.Parameters)
                    {
                        this.results.Items.Add(
                            $"Array Value: '{value.ParameterId}' \t= {value.Values} \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("Array Value: NO APP OR ARRAY PARAMETERS SELECTED");
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

        private void SetValueString_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                if (this.appList.SelectedItem != null && this.scalar.SelectedItem != null)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var param = new AppStringParameterValuesRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    foreach (var parameter in this.str.Items)
                    {
                        var set = new StringParameterSetValue
                        {
                            ParameterId = (string)((DataRowView)parameter).Row.ItemArray[0],
                            Value = "Test"
                        };

                        param.Parameters.Add(set);
                    }

                    this.results.Items.Add($"Total String Values Set: {param.Parameters.Count}");
                    var reply = this.paramClient.SetValueString(param, this.header);

                    foreach (var value in reply.Parameters)
                    {
                        this.results.Items.Add(
                            $"String Value: '{value.ParameterId}' \t= '{value.Value}' \t- Error: {value.ReturnCode}");
                    }

                    this.SetErrorCode(reply.ReturnCode);
                    this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                }
                else
                {
                    this.results.Items.Add("String Value: NO APP OR STRING PARAMETERS SELECTED");
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
