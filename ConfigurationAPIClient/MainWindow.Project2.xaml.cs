// <copyright file="MainWindow.Project2.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Google.Protobuf.Collections;
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

namespace SystemMonitorConfigurationTest;

public partial class MainWindow
{
    private void GetActiveCANConfig_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            Mouse.OverrideCursor = Cursors.Wait;
            var stopwatch = Stopwatch.StartNew();

            for (uint slot = 1; slot <= 16; slot++)
            {
                var name = new SlotRequest
                {
                    Slot = slot
                };

                var stopwatch2 = Stopwatch.StartNew();
                var reply = this.projectClient.GetActiveCANConfig(name, this.header);
                this.results.Items.Add(
                    $"Get Active CAN Config - Slot {slot}: '{reply.Active}' \t{stopwatch2.ElapsedMilliseconds}ms");

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

    private void SetActiveCANConfig_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();

            var dialog = new SetSlot(16);
            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var slot = new SlotActiveRequest
                {
                    Slot = Convert.ToUInt32(dialog.slot.Text),
                    Active = dialog.activate.IsChecked != null && (bool)dialog.activate.IsChecked
                };

                var reply = this.projectClient.SetActiveCANConfig(slot, this.header);
                this.results.Items.Add($"Set Active CAN Config - Slot: {slot.Slot}: Activated: {slot.Active}");

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

    private void GetFIACANConfig_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            Mouse.OverrideCursor = Cursors.Wait;
            var stopwatch = Stopwatch.StartNew();

            for (uint slot = 1; slot <= 16; slot++)
            {
                var name = new SlotRequest
                {
                    Slot = slot
                };

                var stopwatch2 = Stopwatch.StartNew();
                var reply = this.projectClient.GetFIACANConfig(name, this.header);
                this.results.Items.Add(
                    $"Get FIA CAN Config - Slot {slot}: '{reply.Active}' \t{stopwatch2.ElapsedMilliseconds}ms");

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

    private void SetFIACANConfig_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();

            var dialog = new SetSlot(16);
            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var slot = new SlotActiveRequest
                {
                    Slot = Convert.ToUInt32(dialog.slot.Text),
                    Active = dialog.activate.IsChecked != null && (bool)dialog.activate.IsChecked
                };

                var reply = this.projectClient.SetFIACANConfig(slot, this.header);
                this.results.Items.Add($"Set FIA CAN Config - Slot: {slot.Slot}: Activated: {slot.Active}");

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

    private void CANBuffersExport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CAN Buffer Export File (.csv)|*.csv",
                FileName = "CAN Buffers 1"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var file = new CANRequest
                {
                    Index = 1,
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"CAN Buffer Export Export: Bus: 1 \t'{file.FilePath}'");
                var reply = this.projectClient.CANBuffersExport(file, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("CAN Buffer Export: No File Name");
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

    private void CANBuffersImport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CAN Buffer Import File (.csv)|*.csv",
                FileName = "CAN Buffers 1"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var import = new CANRequest
                {
                    Index = 1,
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"CAN Buffer Import: Bus: 1 \t'{import.FilePath}'");
                var reply = this.projectClient.CANBuffersImport(import, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("CAN Buffer Import: No File Name");
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

    private void CANMessagesExport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CAN Message Export File (.csv)|*.csv",
                FileName = "CAN Messages 1"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var file = new CANRequest
                {
                    Index = 1,
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"CAN Message Export Export: Bus: 1 \t'{file.FilePath}'");
                var reply = this.projectClient.CANMessagesExport(file, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("CAN Message Export: No File Name");
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

    private void CANMessagesImport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CAN Messages Import File (.csv)|*.csv",
                FileName = "CAN Messages 1"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var import = new CANMergeRequest
                {
                    Index = 1,
                    FilePath = dialog.FileName,
                    Merge = false
                };

                this.results.Items.Add($"CAN Messages Import: Bus: 1 \t'{import.FilePath}'");
                var reply = this.projectClient.CANMessagesImport(import, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("CAN Messages Import: No File Name");
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

    private void CANConfigUnload_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();

            var dialog = new SetSlot(16)
            {
                activate =
                {
                    Visibility = Visibility.Hidden
                }
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var slot = new SlotRequest
                {
                    Slot = Convert.ToUInt32(dialog.slot.Text)
                };

                var reply = this.projectClient.CANConfigUnload(slot, this.header);
                this.results.Items.Add($"CAN Config Unload - Slot: {slot.Slot}");

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

    private void MatlabImport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".m",
                Filter = "Matlab File (.m)|*.m"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var import = new FileRequest
                {
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"Matlab Import: '{import.FilePath}'");
                var reply = this.projectClient.MatlabImport(import, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("Matlab Import: No Matlab File Name");
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

    private void MatlabExport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new MatlabExport(this.projectClient, this.header)
            {
                dtvName =
                {
                    Visibility = Visibility.Hidden
                },
                dtvNameLable =
                {
                    Visibility = Visibility.Hidden
                }
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (!string.IsNullOrEmpty(dialog.exportName.Text))
                {
                    var export = new MatlabRequest
                    {
                        AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                        ExportPath = dialog.exportName.Text,
                        DataOnly = dialog.dataOnly.IsChecked != null && (bool)dialog.dataOnly.IsChecked
                    };

                    foreach (DataRowView item in dialog.type.SelectedItems)
                    {
                        var type = (ParameterType)item.Row.ItemArray[0]!;
                        export.DataTypes.Add(type);
                    }

                    var reply = this.projectClient.MatlabExport(export, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Matlab Export: No Export Name selected");
                    this.SetErrorCode(ErrorCode.NoError);
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

    private void MatlabExportDTV_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new MatlabExport(this.projectClient, this.header)
            {
                appList =
                {
                    Visibility = Visibility.Hidden
                },
                appsLabel =
                {
                    Visibility = Visibility.Hidden
                }
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (!string.IsNullOrEmpty(dialog.exportName.Text) && !string.IsNullOrEmpty(dialog.dtvName.Text))
                {
                    var export = new MatlabDTVRequest
                    {
                        DtvPath = dialog.dtvName.Text,
                        ExportPath = dialog.exportName.Text,
                        DataOnly = dialog.dataOnly.IsChecked != null && (bool)dialog.dataOnly.IsChecked
                    };

                    foreach (DataRowView item in dialog.type.SelectedItems)
                    {
                        var type = (ParameterType)item.Row.ItemArray[0]!;
                        export.DataTypes.Add(type);
                    }

                    var reply = this.projectClient.MatlabExportDTV(export, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Matlab Export: No Export Name selected");
                    this.SetErrorCode(ErrorCode.NoError);
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

    private void MatlabExportSelected_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new MatlabExport(this.projectClient, this.header)
            {
                dtvName =
                {
                    Visibility = Visibility.Hidden
                },
                dtvNameLable =
                {
                    Visibility = Visibility.Hidden
                },
                appList =
                {
                    SelectedIndex = this.appList.SelectedIndex,
                    IsEnabled = false
                }
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (!string.IsNullOrEmpty(dialog.exportName.Text))
                {
                    var export = new MatlabSelectedRequest
                    {
                        AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                        ExportPath = dialog.exportName.Text,
                        DataOnly = dialog.dataOnly.IsChecked != null && (bool)dialog.dataOnly.IsChecked
                    };

                    var field = export.ParameterIds;
                    this.AddParams(dialog, field);

                    var reply = this.projectClient.MatlabExportSelected(export, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Matlab Export: No Export Name selected");
                    this.SetErrorCode(ErrorCode.NoError);
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

    private void GenerateParamSet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new MatlabExport(this.projectClient, this.header)
            {
                Title = "Generate ParamSet",
                dtvName =
                {
                    Visibility = Visibility.Hidden
                },
                dtvNameLable =
                {
                    Visibility = Visibility.Hidden
                },
                dataOnly =
                {
                    Visibility = Visibility.Hidden
                },
                appList =
                {
                    SelectedIndex = this.appList.SelectedIndex,
                    IsEnabled = false
                }
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (!string.IsNullOrEmpty(dialog.exportName.Text))
                {
                    var export = new ParametersFileRequest
                    {
                        FilePath = dialog.exportName.Text
                    };

                    var field = export.ParameterIds;
                    this.AddParams(dialog, field);

                    var reply = this.projectClient.GenerateParamSet(export, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Generate ParamSet: No File Name selected");
                    this.SetErrorCode(ErrorCode.NoError);
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

    private void AddParams(MatlabExport dialog, RepeatedField<string> field)
    {
        foreach (DataRowView item in dialog.type.SelectedItems)
        {
            var type = (ParameterType)item.Row.ItemArray[0]!;
            switch (type)
            {
                case ParameterType.Scalar:
                    if (!string.IsNullOrEmpty(this.scalar.Text))
                        field.Add((string)((DataRowView)this.scalar.SelectedItem).Row
                            .ItemArray[0]);

                    break;
                case ParameterType.String:
                    if (!string.IsNullOrEmpty(this.str.Text))
                        field.Add((string)((DataRowView)this.str.SelectedItem).Row
                            .ItemArray[0]);

                    break;
                case ParameterType.Axis1:
                    if (!string.IsNullOrEmpty(this.axis1.Text))
                        field.Add((string)((DataRowView)this.axis1.SelectedItem).Row
                            .ItemArray[0]);

                    break;
                case ParameterType.Axis2:
                    if (!string.IsNullOrEmpty(this.axis2.Text))
                        field.Add((string)((DataRowView)this.axis2.SelectedItem).Row
                            .ItemArray[0]);

                    break;
                case ParameterType.Array:
                    if (!string.IsNullOrEmpty(this.array.Text))
                        field.Add((string)((DataRowView)this.array.SelectedItem).Row
                            .ItemArray[0]);

                    break;
                case ParameterType.Axis:
                    if (!string.IsNullOrEmpty(this.axis.Text))
                        field.Add((string)((DataRowView)this.axis.SelectedItem).Row
                            .ItemArray[0]);

                    break;
            }
        }
    }

    private void GeneratePULFile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new MatlabExport(this.projectClient, this.header)
            {
                Title = "Generate PUL File",
                dtvName =
                {
                    Visibility = Visibility.Hidden
                },
                dtvNameLable =
                {
                    Visibility = Visibility.Hidden
                },
                dataOnly =
                {
                    Visibility = Visibility.Hidden
                },
                appList =
                {
                    SelectedIndex = this.appList.SelectedIndex,
                    IsEnabled = false
                }
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                if (!string.IsNullOrEmpty(dialog.exportName.Text))
                {
                    var export = new AppParametersFileRequest
                    {
                        FilePath = dialog.exportName.Text
                    };

                    var field = export.ParameterIds;
                    this.AddParams(dialog, field);

                    var reply = this.projectClient.GeneratePULFile(export, this.header);
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Generate PUL File: No File Name selected");
                    this.SetErrorCode(ErrorCode.NoError);
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

    private void GeneratePULFileFromParamSet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".pms",
                Filter = "ParamSet File (.pms)|*.pms"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var import = new AppFileRequest
                {
                    AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"Generate PUL From: '{import.FilePath}'");
                var reply = this.projectClient.GeneratePULFileFromParamSet(import, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("Generate PUL From ParamSet: No File Name");
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

    private void AddParametersToUnlockList_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new MatlabExport(this.projectClient, this.header)
        {
            Title = "Add Parameters to Unlock List",
            dtvName =
            {
                Visibility = Visibility.Hidden
            },
            dtvNameLable =
            {
                Visibility = Visibility.Hidden
            },
            exportName =
            {
                Visibility = Visibility.Hidden
            },
            exportLable =
            {
                Visibility = Visibility.Hidden
            },
            dataOnly =
            {
                Visibility = Visibility.Hidden
            }
        };
        try
        {
            this.ClearResults();

            dialog.appList.SelectedIndex = this.appList.SelectedIndex;
            dialog.appList.IsEnabled = false;

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var request = new AppParametersFileRequest
                {
                    AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                    FilePath = dialog.exportName.Text
                };

                var field = request.ParameterIds;
                this.AddParams(dialog, field);

                var reply = this.projectClient.AddParametersToUnlockList(request, this.header);
                if (reply.ReturnCode == ErrorCode.NoError)
                {
                    this.results.Items.Add($"Unlock List updated: '{reply.FilePath}'");
                }
                else
                {
                    this.results.Items.Add("FAILED: Check Unlock List already exists on app.");
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

    private void RemoveParameterFromUnlockList_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new MatlabExport(this.projectClient, this.header)
            {
                Title = "Remove Parameters From Unlock List",
                dtvName =
                {
                    Visibility = Visibility.Hidden
                },
                dtvNameLable =
                {
                    Visibility = Visibility.Hidden
                },
                exportName =
                {
                    Visibility = Visibility.Hidden
                },
                exportLable =
                {
                    Visibility = Visibility.Hidden
                },
                dataOnly =
                {
                    Visibility = Visibility.Hidden
                },
                appList =
                {
                    SelectedIndex = this.appList.SelectedIndex,
                    IsEnabled = false
                }
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var request = new AppParametersFileRequest
                {
                    FilePath = dialog.exportName.Text
                };

                var field = request.ParameterIds;
                this.AddParams(dialog, field);

                var reply = this.projectClient.RemoveParametersFromUnlockList(request, this.header);
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

    private void GetAppsHoldingParam_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.measurement.SelectedItem != null)
            {
                var request = new ParameterIdRequest
                {
                    ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                };

                var reply = this.projectClient.GetAppsHoldingParam(request, this.header);

                foreach (var appId in reply.AppIds)
                    this.results.Items.Add($"Apps Holding Param: '{request.ParameterId}' - 0x{appId:X04}");

                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Apps Holding Param: NO APPS OR PARAMETERS SELECTED");
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

    private void GetAppsHoldingMeasurementParam_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.measurement.SelectedItem != null)
            {
                var request = new ParameterIdRequest
                {
                    ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]
                };

                var reply = this.projectClient.GetAppsHoldingMeasurementParam(request, this.header);

                foreach (var appId in reply.AppIds)
                    this.results.Items.Add($"Apps Holding Measurement Param: '{request.ParameterId}' - 0x{appId:X04}");

                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Apps Holding Measurement Param: NO APPS OR PARAMETERS SELECTED");
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

    private void GetAppsHoldingControlParam_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.measurement.SelectedItem != null)
            {
                var request = new ParameterIdRequest
                {
                    ParameterId = (string)((DataRowView)this.scalar.SelectedItem).Row.ItemArray[0]
                };

                var reply = this.projectClient.GetAppsHoldingControlParam(request, this.header);

                foreach (var appId in reply.AppIds)
                    this.results.Items.Add($"Apps Holding Control Param: '{request.ParameterId}' - 0x{appId:X04}");

                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Apps Holding Control Param: NO APPS OR PARAMETERS SELECTED");
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
    
    private void ParameterExists_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null && this.measurement.SelectedItem != null)
            {
                var request = new ExistsRequest
                {
                    AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = (string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]!,
                    DataType = ParameterType.Measurement
                };

                var reply = this.projectClient.ParameterExists(request, this.header);

                this.results.Items.Add($"Check '{request.ParameterId}' exists - Result: {reply.Exists}");

                var request2 = new ExistsRequest
                {
                    AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                    ParameterId = "TestParamId",
                    DataType = ParameterType.Measurement
                };

                var reply2 = this.projectClient.ParameterExists(request2, this.header);

                this.results.Items.Add($"Check '{request2.ParameterId}' exists - Result: {reply2.Exists}");

                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Register Enhanced Row Parameters: NO APPS OR PARAMETERS SELECTED");
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

    private void RegisterEnhancedRowParameters_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null && this.measurement.SelectedItem != null)
            {
                var request = new AppParametersRequest
                {
                    AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                };

                request.ParameterIds.Add((string)((DataRowView)this.measurement.SelectedItem).Row.ItemArray[0]);

                foreach (var param in request.ParameterIds)
                    this.results.Items.Add($"Register Enhanced Row Parameters: '{param}' - 0x{request.AppId:X04}");

                var reply = this.projectClient.RegisterEnhancedRowParameters(request, this.header);
                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Register Enhanced Row Parameters: NO APPS OR PARAMETERS SELECTED");
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

    private void ClearEnhancedRowParameters_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null)
            {
                var request = new AppRequest
                {
                    AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                };

                this.results.Items.Add($"Clear Enhanced Row Parameters: '0x{request.AppId:X04}");

                var reply = this.projectClient.ClearEnhancedRowParameters(request, this.header);
                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Clear Enhanced Row Parameters: NO APPS OR PARAMETERS SELECTED");
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

    private void RegisterCANEnhancedRowParameters_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null && this.can.SelectedItem != null)
            {
                var request = new ParametersRequest();
                request.ParameterIds.Add((string)((DataRowView)this.can.SelectedItem).Row.ItemArray[0]);

                foreach (var param in request.ParameterIds)
                    this.results.Items.Add($"Register CAN Enhanced Row Parameters: '{param}'");

                var reply = this.projectClient.RegisterCANEnhancedRowParameters(request, this.header);
                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Register CAN Enhanced Row Parameters: NO APPS OR PARAMETERS SELECTED");
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

    private void RegisterVirtualEnhancedRowParameters_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null && this.virt.SelectedItem != null)
            {
                var request = new ParametersRequest();
                request.ParameterIds.Add((string)((DataRowView)this.virt.SelectedItem).Row.ItemArray[0]);

                foreach (var param in request.ParameterIds)
                    this.results.Items.Add($"Register Virtual Enhanced Row Parameters: '{param}'");

                var reply = this.projectClient.RegisterVirtualEnhancedRowParameters(request, this.header);
                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Register Virtual Enhanced Row Parameters: NO APPS OR PARAMETERS SELECTED");
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

    private void ActivateEnhancedRowParameters_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            this.results.Items.Add("Activate Enhanced Row Parameters");
            var reply = this.projectClient.ActivateEnhancedRowParameters(new Empty(), this.header);
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

    private void DumpEvents_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Events Dump File (.txt)|*.txt",
                FileName = "DumpEvents"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var file = new FileRequest
                {
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"Dump Events Export: \t'{file.FilePath}'");
                var reply = this.projectClient.DumpEvents(file, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("Dump Events Export: No File Name");
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

    private void DumpErrors_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Errors Dump File (.txt)|*.txt",
                FileName = "DumpErrors"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var file = new FileRequest
                {
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"Dump Errors Export: \t'{file.FilePath}'");
                var reply = this.projectClient.DumpErrors(file, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("Dump Errors Export: No File Name");
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

    private void DumpRowData_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Row Data Dump File (.txt)|*.txt",
                FileName = "RowData"
            };

            if (dialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                var file = new FileRequest
                {
                    FilePath = dialog.FileName
                };

                this.results.Items.Add($"Dump Row Data Export: \t'{file.FilePath}'");
                var reply = this.projectClient.DumpRowData(file, this.header);

                this.SetErrorCode(reply.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            else
            {
                this.results.Items.Add("Dump Row Data Export: No File Name");
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

    private void ClearEvents_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            Mouse.OverrideCursor = Cursors.Wait;
            var stopwatch = Stopwatch.StartNew();

            this.results.Items.Add("Clear Events");
            var reply = this.projectClient.ClearEvents(new Empty(), this.header);
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

    private void GetEventDetails_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null && this.evnt.SelectedItem != null)
            {
                var request = new EventRequest
                {
                    AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                    EventId = (ushort)((DataRowView)this.evnt.SelectedItem).Row.ItemArray[0]!
                };

                this.results.Items.Add($"Event Details: App: 0x{request.AppId:X04} \t- Event: 0x{request.EventId:X04}");

                var reply = this.projectClient.GetEventDetails(request, this.header);

                this.results.Items.Add($"\t Event: 0x{reply.EventId:X04}");
                this.results.Items.Add($"\t Description: '{reply.Description}'");
                this.results.Items.Add($"\t Conv1: '{reply.ConversionId1}'");
                this.results.Items.Add($"\t Conv2: '{reply.ConversionId2}'");
                this.results.Items.Add($"\t Conv3: '{reply.ConversionId3}'");
                this.results.Items.Add($"\t Priority: {reply.Priority}");

                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Get Event Details: NO APPS OR EVENTS SELECTED");
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

    private void GetErrorDefinitions_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null && this.evnt.SelectedItem != null)
            {
                var request = new AppRequest
                {
                    AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                };

                this.results.Items.Add($"Error Details: App: 0x{request.AppId:X04}");

                var reply = this.projectClient.GetErrorDefinitions(request, this.header);

                this.results.Items.Add($"Total Definitions: {reply.ErrorDefinitions.Count}");
                foreach (var definition in reply.ErrorDefinitions)
                {
                    this.results.Items.Add($"Event: '{definition.Id}' \t- Name: '{definition.Name}'");
                    this.results.Items.Add($"\t Description: '{definition.Description}'");
                    this.results.Items.Add($"\t Group: '{definition.Group}' \t- Bit Number: {definition.BitNumber}");
                    this.results.Items.Add($"\t Current: '{definition.Current}' \t' Logged: {definition.Logged}");
                }

                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Get Error Definitions: NO APPS OR ERRORS SELECTED");
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

    private void GetErrors_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.ClearResults();
            var stopwatch = Stopwatch.StartNew();

            if (this.appList.SelectedItem != null && this.evnt.SelectedItem != null)
            {
                var reply = this.projectClient.GetErrors(new Empty(), this.header);

                this.results.Items.Add($"Total Errors: {reply.ErrorInstances.Count}");
                foreach (var instance in reply.ErrorInstances)
                {
                    this.results.Items.Add($"Event: '{instance.Name}'");
                    this.results.Items.Add($"\t Description: '{instance.Description}'");
                    this.results.Items.Add($"\t Status: '{instance.Status}'");
                }

                this.SetErrorCode(reply.ReturnCode);
            }
            else
            {
                this.results.Items.Add("Get Errors: NO APPS OR ERRORS SELECTED");
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

    private void DeleteErrors_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ClearResults();
            Mouse.OverrideCursor = Cursors.Wait;
            var stopwatch = Stopwatch.StartNew();

            this.results.Items.Add("Delete Errors");
            var reply = this.projectClient.DeleteErrors(new Empty(), this.header);
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
}