// <copyright file="MainWindow.Project.xaml.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using SystemMonitorConfigurationTest.Dialogs;
using SystemMonitorProtobuf;

namespace SystemMonitorConfigurationTest
{
    public partial class MainWindow
    {
        private void ProjectOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".prj",
                    Filter = "System Monitor Projects (.prj)|*.prj"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var open = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    this.results.Items.Add($"Project Open: {open.FilePath}");
                    var error = this.projectClient.ProjectOpen(open, this.header);

                    this.SetErrorCode(error.ReturnCode);
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

        private void ProjectClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                var close = new ProjectCloseRequest
                {
                    Action = 2
                };

                this.results.Items.Add("Project Close: NO SAVE");
                var error = this.projectClient.ProjectClose(close, this.header);

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

        private void ProjectSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                var save = new ProjectSaveRequest
                {
                    SaveAll = true
                };

                this.results.Items.Add("Project Save: SAVE ALL");
                var error = this.projectClient.ProjectSave(save, this.header);

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

        private void ProjectSaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new ProjectSaveAs();
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    if (!string.IsNullOrEmpty(dialog.projectName.Text))
                    {
                        var save = new ProjectSaveAsRequest
                        {
                            ProjectName = dialog.projectName.Text,
                            SaveAll = dialog.saveAll.IsChecked != null && (bool)dialog.saveAll.IsChecked,
                            Comments = dialog.comments.Text,
                            Notes = dialog.notes.Text,
                        };

                        this.results.Items.Add($"Project SaveAs: '{save.ProjectName}' ");
                        this.results.Items.Add($"Save All: {save.SaveAll.ToString()}");
                        this.results.Items.Add($"Comments: '{save.Comments}'");
                        this.results.Items.Add($"Notes: '{save.Notes}'");
                        var error = this.projectClient.ProjectSaveAs(save, this.header);

                        this.SetErrorCode(error.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("Project SaveAs: No Project Name");
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

        private void ProjectImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new ProjectImport();
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    if (!string.IsNullOrEmpty(dialog.projectName.Text) && !string.IsNullOrEmpty(dialog.baseList.Text))
                    {
                        var import = new ProjectImportRequest
                        {
                            ProjectPath = dialog.projectName.Text,
                            Base = dialog.baseList.Text
                        };

                        this.results.Items.Add($"Project Import: '{import.ProjectPath}'");
                        this.results.Items.Add($"Base: '{import.Base}'");
                        var error = this.projectClient.ProjectImport(import, this.header);
                        this.SetErrorCode(error.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("Project Import: No Project Name");
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

        private void ProjectExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                var export = new ProjectExportRequest
                {
                    SaveModified = true
                };

                this.results.Items.Add("Project Export: SAVE MODIFIED");
                var error = this.projectClient.ProjectExport(export, this.header);

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

        private void ProjectCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new ProjectCreate();
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    if (!string.IsNullOrEmpty(dialog.projectName.Text) && dialog.apps.Items.Count > 0)
                    {
                        var create = new ProjectCreateRequest
                        {
                            ProjectPath = dialog.projectName.Text,
                            DesktopPath = dialog.desktopName.Text,
                            CanPath = dialog.canName.Text,
                            LoggingConfigPath = dialog.loggingName.Text,
                            VirtualsPath = dialog.virtualName.Text,
                        };

                        foreach (var app in dialog.apps.Items)
                        {
                            create.AppPaths.Add(app.ToString());
                        }

                        var error = this.projectClient.ProjectCreate(create, this.header);
                        this.SetErrorCode(error.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("Project Create: No Project Name");
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

        private void Reprogram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new SelectApps(this.projectClient, this.header);
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var apps = new ReprogramRequest
                    {
                        Force = dialog.force.IsChecked != null && (bool)dialog.force.IsChecked
                    };

                    foreach (DataRowView item in dialog.appList.SelectedItems)
                    {
                        var appId = (ushort)item.Row.ItemArray[0]!;
                        this.results.Items.Add($"Reprogram App: 0x{appId:X4}");
                        apps.AppIds.Add(appId);
                    }

                    var error = this.projectClient.Reprogram(apps, this.header);
                    this.SetErrorCode(error.ReturnCode);
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

        private void DownloadDataChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var build = this.projectClient.DownloadDataChanges(version, this.header);
                    this.results.Items.Add($"Download Data Changes: '{this.appList.Text}'");
                    this.SetErrorCode(build.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Download Data Changes: NO APP SELECTED");
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

        private void EditBufferSynced_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var sync = this.projectClient.EditBufferSynced(version, this.header);
                    this.results.Items.Add($"EditBufferSynced: '{this.appList.Text}' \t Synced: {sync.Synced}");
                    this.SetErrorCode(sync.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("EditBufferSynced: NO APP SELECTED");
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

        private void UploadDataVersion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var sync = this.projectClient.UploadDataVersion(version, this.header);
                    this.results.Items.Add($"Upload Data Version: '{this.appList.Text}'");
                    this.SetErrorCode(sync.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Upload Data Version: NO APP SELECTED");
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

        private void GetBuildNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                var build = this.projectClient.GetBuildNumber(new Empty(), this.header);
                this.results.Items.Add($"Get Build Number: '{build.BuildNumber}'");

                this.SetErrorCode(build.ReturnCode);
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

        private void GetDTVVersion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var build = this.projectClient.GetDTVVersion(version, this.header);
                    this.results.Items.Add($"DTV Version: '{this.appList.Text}': \tValue: {build.Text}");
                    this.SetErrorCode(build.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get DTV Version: NO APP SELECTED");
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

        private void GetEcuDTVVersion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var build = this.projectClient.GetEcuDTVVersion(version, this.header);
                    this.results.Items.Add($"Ecu DTV Version: '{this.appList.Text}': \tValue: {build.Text}");
                    this.SetErrorCode(build.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get Ecu DTV Version: NO APP SELECTED");
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

        private void GetNextDTVVersion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var build = this.projectClient.GetNextDTVVersion(version, this.header);
                    this.results.Items.Add($"Get Next DTV Version: '{this.appList.Text}': \tValue: {build.Text}");
                    this.SetErrorCode(build.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get Next DTV Version: NO APP SELECTED");
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

        private void GetPGVVersion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var build = this.projectClient.GetPGVVersion(version, this.header);
                    this.results.Items.Add($"PGV Version: '{this.appList.Text}': \tValue: {build.Text}");
                    this.SetErrorCode(build.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get PGV Version: NO APP SELECTED");
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


        private void GetVersionNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                var build = this.projectClient.GetVersionNumber(new Empty(), this.header);
                this.results.Items.Add(
                    $"System Monitor Version: 'v{build.MajorVersion}.{build.MinorVersion}.{build.BuildVersion}'");

                this.SetErrorCode(build.ReturnCode);
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

        private void GetDTVModified_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var modified = this.projectClient.GetDTVModified(version, this.header);
                    this.results.Items.Add($"Get DTV Modified: '{this.appList.Text}': \tValue: {modified.Modified.ToString()}");
                    this.SetErrorCode(modified.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get DTV Modified: NO APP SELECTED");
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

        private void GetDTVSavedOn_Click(object sender, RoutedEventArgs e)
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

                    var saved = this.projectClient.GetDTVSavedOn(app, this.header);
                    this.results.Items.Add($"Get DTV Saved On: '{this.appList.Text}': \tValue: {saved.SavedOn}");
                    this.SetErrorCode(saved.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get DTV Saved On: NO APP SELECTED");
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
        private void GetDTVNotes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var notes = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var reply = this.projectClient.GetDTVNotes(notes, this.header);
                    this.results.Items.Add($"Get DTV Notes: '{this.appList.Text}': \tValue: '{reply.Text}'");
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get DTV Notes: NO APP SELECTED");
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

        private void SetDTVNotes_Click(object sender, RoutedEventArgs e)
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
                    }
                };

                if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.text.Text))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var notes = new DetailsRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        Text = dialog.text.Text
                    };

                    var reply = this.projectClient.SetDTVNotes(notes, this.header);
                    this.results.Items.Add($"Set DTV Notes: '{this.appList.Text}': \tValue: '{notes.Text}'");
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

        private void ClearDTVNotes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var notes = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var reply = this.projectClient.ClearDTVNotes(notes, this.header);
                    this.results.Items.Add($"Clear DTV Notes: '{this.appList.Text}'");
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Clear DTV Notes: NO APP SELECTED");
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

        private void GetDTVComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var comment = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var reply = this.projectClient.GetDTVComment(comment, this.header);
                    this.results.Items.Add($"Get DTV Comment: '{this.appList.Text}': \tValue: '{reply.Text}'");
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get DTV Comment: NO APP SELECTED");
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

        private void SetDTVComment_Click(object sender, RoutedEventArgs e)
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
                    }
                };

                if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.text.Text))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var comment = new DetailsRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!,
                        Text = dialog.text.Text
                    };

                    var reply = this.projectClient.SetDTVComment(comment, this.header);
                    this.results.Items.Add($"Set DTV Comment: '{this.appList.Text}': \tValue: '{comment.Text}'");
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

        private void EnableDTVBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();

                var stopwatch = Stopwatch.StartNew();
                var comment = new EnableRequest
                {
                    Enable = !this.EnableBackup
                };

                var reply = this.projectClient.EnableDTVBackup(comment, this.header);
                this.results.Items.Add($"Set Enable DTV Backup: {comment.Enable.ToString()}");

                this.EnableBackup = !this.EnableBackup;
                var value = this.EnableBackup ? "N" : "Y";
                this.EnableDTVBackup.Content = $"EnableDTVBackup: {value}";

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

        private void GetPGVID_Click(object sender, RoutedEventArgs e)
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

                    var reply = this.projectClient.GetPGVID(app, this.header);
                    this.results.Items.Add($"Get PGV Id: '{this.appList.Text}': \tValue: 0x{reply.PgvId:X4}");
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get PGV Id: NO APP SELECTED");
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

        private void DTVOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".dtv",
                    Filter = "DTV Files (.dtv)|*.dtv"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var open = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    this.results.Items.Add($"DTV Open: {open.FilePath}");
                    var error = this.projectClient.DTVOpen(open, this.header);

                    this.SetErrorCode(error.ReturnCode);
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
        private void DTVSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new DTVSaveAs(this.projectClient, this.header)
                {
                    consortium =
                    {
                        Visibility = Visibility.Hidden
                    },
                    consortiumLabel =
                    {
                        Visibility = Visibility.Hidden
                    }
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    if (!string.IsNullOrEmpty(dialog.dtvName.Text))
                    {
                        var path = Path.GetDirectoryName(dialog.dtvName.Text);
                        if (!Directory.Exists(path))
                        {
                            this.results.Items.Add($"DTV must point to a valid path: '{dialog.dtvName.Text}' ");
                            return;
                        }

                        if (File.Exists(path))
                        {
                            this.results.Items.Add($"DTV name must not already exist: '{dialog.dtvName.Text}' ");
                            return;
                        }

                        if (!dialog.dtvName.Text.EndsWith(".dtv", true, CultureInfo.CurrentCulture))
                        {
                            dialog.dtvName.Text += ".dtv";
                        }

                        var save = new DTVSaveRequest
                        {
                            AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                            SavePath = dialog.dtvName.Text,
                            Comment = dialog.comments.Text,
                            Notes = dialog.notes.Text
                        };

                        this.results.Items.Add($"DTV Save: '{save.SavePath}' ");
                        this.results.Items.Add($"Get PGV Id: '{dialog.appList.Text}'");
                        this.results.Items.Add($"Comments: '{save.Comment}'");
                        this.results.Items.Add($"Notes: '{save.Notes}'");
                        var error = this.projectClient.DTVSave(save, this.header);

                        this.SetErrorCode(error.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("DTV SaveAs: No Project Name");
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

        private void DTVSaveCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new DTVSaveAs(this.projectClient, this.header);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    if (!string.IsNullOrEmpty(dialog.dtvName.Text))
                    {
                        var path = Path.GetDirectoryName(dialog.dtvName.Text);
                        if (!Directory.Exists(path))
                        {
                            this.results.Items.Add($"DTV must point to a valid path: '{dialog.dtvName.Text}' ");
                            return;
                        }

                        if (File.Exists(path))
                        {
                            this.results.Items.Add($"DTV name must not already exist: '{dialog.dtvName.Text}' ");
                            return;
                        }

                        if (!dialog.dtvName.Text.EndsWith(".dtv", true, CultureInfo.CurrentCulture))
                        {
                            dialog.dtvName.Text += ".dtv";
                        }

                        var save = new DTVSaveCopyRequest
                        {
                            AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                            Consortium = dialog.consortium.Text,
                            SavePath = dialog.dtvName.Text,
                            Comment = dialog.comments.Text,
                            Notes = dialog.notes.Text
                        };

                        this.results.Items.Add($"DTV Save: '{save.SavePath}' ");
                        this.results.Items.Add($"Get PGV Id: '{dialog.appList.Text}'");
                        this.results.Items.Add($"Comments: '{save.Comment}'");
                        this.results.Items.Add($"Notes: '{save.Notes}'");
                        this.results.Items.Add($"Consortium: '{save.Consortium}'");
                        var error = this.projectClient.DTVSaveCopy(save, this.header);

                        this.SetErrorCode(error.ReturnCode);
                    }
                    else
                    {
                        this.results.Items.Add("DTV Save Copy: No Project Name");
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

        private void DTVSaveIncrement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new DTVSaveAs(this.projectClient, this.header)
                {
                    dtvName =
                    {
                        Visibility = Visibility.Hidden
                    },
                    nameLabel =
                    {
                        Visibility = Visibility.Hidden
                    },
                    consortium =
                    {
                        Visibility = Visibility.Hidden
                    },
                    consortiumLabel =
                    {
                        Visibility = Visibility.Hidden
                    }
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var save = new DTVSaveIncrementRequest
                    {
                        AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                        Comment = dialog.comments.Text,
                        Notes = dialog.notes.Text
                    };

                    this.results.Items.Add($"PGV Id: '{dialog.appList.Text}'");
                    this.results.Items.Add($"Comments: '{save.Comment}'");
                    this.results.Items.Add($"Notes: '{save.Notes}'");
                    var error = this.projectClient.DTVSaveIncrement(save, this.header);

                    this.SetErrorCode(error.ReturnCode);
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

        private void GetActiveApps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                var reply = this.projectClient.GetActiveApps(new Empty(), this.header);
                foreach (var app in reply.AppIds)
                {
                    this.results.Items.Add($"Active PGV: 0x{app:X4}");
                }

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

        private void SetActiveApps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new SelectApps(this.projectClient, this.header)
                {
                    force =
                    {
                        Visibility = Visibility.Hidden
                    }
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var apps = new MultiAppRequest();
                    foreach (DataRowView item in dialog.appList.SelectedItems)
                    {
                        var appId = (ushort)item.Row.ItemArray[0]!;
                        this.results.Items.Add($"Set App Id Active: 0x{appId:X4}");
                        apps.AppIds.Add(appId);
                    }

                    var error = this.projectClient.SetActiveApps(apps, this.header);
                    this.SetErrorCode(error.ReturnCode);
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

        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".dtv",
                    Filter = "DTV file (.dtv)|*.dtv"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var open = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    this.results.Items.Add($"Add App: {open.FilePath}");
                    var error = this.projectClient.AddApp(open, this.header);

                    this.SetErrorCode(error.ReturnCode);
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

        private void RemoveApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                if (this.appList.SelectedItem != null)
                {
                    var version = new AppRequest
                    {
                        AppId = (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]!
                    };

                    var reply = this.projectClient.RemoveApp(version, this.header);
                    this.results.Items.Add($"Remove App: '{this.appList.Text}'");
                    this.SetErrorCode(reply.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get DTV Version: NO APP SELECTED");
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

        private void CompareApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new CompareApp(this.projectClient, this.header);
                if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.dtv1.Text) && !string.IsNullOrEmpty(dialog.dtv2.Text))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var compare = new CompareAppRequest
                    {
                        AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                        Dtv1Path = dialog.dtv1.Text,
                        Dtv2Path = dialog.dtv2.Text
                    };

                    this.results.Items.Add($"Compare App: '{dialog.appList.Text}'");
                    this.results.Items.Add($"DTV1: {compare.Dtv1Path}");
                    this.results.Items.Add($"DTV2: {compare.Dtv2Path}");
                    this.results.Items.Add(string.Empty);

                    var reply = this.projectClient.CompareApp(compare, this.header);

                    foreach (var param in reply.Parameters)
                    {
                        this.results.Items.Add($"Id: {param.ParameterId} \tType: {param.Type.ToString()} \tReason1: {param.Reason1} \tReason1: {param.Reason2}");
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

        private void GetAppPULFile_Click(object sender, RoutedEventArgs e)
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

                    var file = this.projectClient.GetAppPULFile(app, this.header);
                    this.results.Items.Add($"Get App PUL File: '{this.appList.Text}': \tValue: '{file.FilePath}'");
                    this.SetErrorCode(file.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Get App PUL File: NO APP SELECTED");
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

        private void SetAppPULFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                var dialog = new PULFileSave(this.projectClient, this.header);
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var file = new AppFileRequest
                    {
                        AppId = (ushort)((DataRowView)dialog.appList.SelectedItem).Row.ItemArray[0]!,
                        FilePath = dialog.fileName.Text
                    };

                    this.results.Items.Add($"Set App PUL File: '{this.appList.Text}': \tValue: '{file.FilePath}'");
                    var error = this.projectClient.SetAppPULFile(file, this.header);
                    this.SetErrorCode(error.ReturnCode);
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

        private void CreateFFCFromPGV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".pgv",
                    Filter = "PVV Files (.pgv)|*.pgv"
                };

                if (dialog.ShowDialog() == true)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var file = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    var reply = this.projectClient.CreateFFCFromPGV(file, this.header);
                    this.results.Items.Add($"Create FFC From PGV: '{file.FilePath}'");
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

        private void ExportToHexFile_Click(object sender, RoutedEventArgs e)
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

                    var build = this.projectClient.ExportToHexFile(app, this.header);
                    this.results.Items.Add($"Export To Hex File: '{this.appList.Text}'");
                    this.SetErrorCode(build.ReturnCode);
                }
                else
                {
                    this.results.Items.Add("Export To Hex File: NO APP SELECTED");
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

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new FileOpen(this.projectClient, this.header);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var open = new FileOpenRequest
                    {
                        FilePath = dialog.filePath.Text,
                        Activate = dialog.activate.IsChecked != null && (bool)dialog.activate.IsChecked,
                        FileType = (FileType)((DataRowView)dialog.typeList.SelectedItem).Row.ItemArray[0]!,
                        Slot = Convert.ToUInt32(dialog.slot.Text)
                    };

                    this.results.Items.Add($"File Open: {open.FilePath}");
                    var error = this.projectClient.FileOpen(open, this.header);

                    this.SetErrorCode(error.ReturnCode);
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

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new FileSave(this.projectClient, this.header);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var save = new FileSaveRequest
                    {
                        FilePath = dialog.fileName.Text,
                        FileType = (FileType)((DataRowView)dialog.typeList.SelectedItem).Row.ItemArray[0]!,
                        Comment = dialog.comments.Text,
                        Notes = dialog.notes.Text,
                        SaveCopyAs = dialog.saveAs.IsChecked != null && (bool)dialog.saveAs.IsChecked,
                        Consortium = dialog.consortium.Text
                    };

                    this.results.Items.Add($"File Save: {save.FilePath}");
                    var error = this.projectClient.FileSave(save, this.header);

                    this.SetErrorCode(error.ReturnCode);
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

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new FileNew(this.projectClient, this.header);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var file = new FileNewRequest
                    {
                        FilePath = dialog.filePath.Text,
                        FileType = (FileType)((DataRowView)dialog.typeList.SelectedItem).Row.ItemArray[0]!,
                        SaveExisting = dialog.save.IsChecked != null && (bool)dialog.save.IsChecked,
                        Overwrite = dialog.overwrite.IsChecked != null && (bool)dialog.overwrite.IsChecked,
                    };

                    this.results.Items.Add($"File New: {file.FilePath}");
                    var error = this.projectClient.FileNew(file, this.header);

                    this.SetErrorCode(error.ReturnCode);
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

        private void GetFileName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new FileName(this.projectClient, this.header);

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var name = new FileNameRequest
                    {
                        FileType = (FileType)((DataRowView)dialog.typeList.SelectedItem).Row.ItemArray[0]!,
                        Slot = Convert.ToUInt32(dialog.slot.Text) - 1
                    };

                    this.results.Items.Add($"File Type: {name.FileType.ToString()}");
                    var reply = this.projectClient.GetFileName(name, this.header);
                    this.results.Items.Add($"File Name: '{reply.FilePath}'");

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

        private void GetFileDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".*",
                    Filter = "File (.*)|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var name = new FileRequest
                    {
                        FilePath = dialog.FileName
                    };

                    this.results.Items.Add($"File: {name.FilePath}");
                    var reply = this.projectClient.GetFileDetails(name, this.header);

                    if (reply.ReturnCode == ErrorCode.NoError)
                    {
                        this.results.Items.Add($"Saved By: '{reply.SavedBy}'");
                        this.results.Items.Add($"Saved On: '{reply.SavedOn.ToDateTime()}'");
                        this.results.Items.Add($"Notes: '{reply.Notes}'");
                        this.results.Items.Add($"Comment: '{reply.Comment}'");
                        this.results.Items.Add($"Consortium: '{reply.Consortium}'");
                        this.results.Items.Add($"Owner: '{reply.Owner}'");
                        this.results.Items.Add($"RDA: '{reply.Rda}'");
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

        private void GetActiveLoggingConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();
                Mouse.OverrideCursor = Cursors.Wait;
                var stopwatch = Stopwatch.StartNew();

                for (uint slot = 1; slot <= 8; slot++)
                {
                    var name = new SlotRequest
                    {
                        Slot = slot
                    };

                    var stopwatch2 = Stopwatch.StartNew();
                    var reply = this.projectClient.GetActiveLoggingConfig(name, this.header);
                    this.results.Items.Add(
                        $"Get Active Logging Config - Slot {slot}: '{reply.Active}' \t{stopwatch2.ElapsedMilliseconds}ms");

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

        private void SetActiveLoggingConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new SetSlot(8);
                if (dialog.ShowDialog() == true)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var stopwatch = Stopwatch.StartNew();

                    var slot = new SlotActiveRequest
                    {
                        Slot = Convert.ToUInt32(dialog.slot.Text),
                        Active = dialog.activate.IsChecked != null && (bool)dialog.activate.IsChecked
                    };

                    var reply = this.projectClient.SetActiveLoggingConfig(slot, this.header);
                    this.results.Items.Add($"Set Active Logging Config - Slot: {slot.Slot}: Activated: {slot.Active}");

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

        private void LoggingConfigUnload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearResults();

                var dialog = new SetSlot(8)
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

                    var reply = this.projectClient.LoggingConfigUnload(slot, this.header);
                    this.results.Items.Add($"Logging Config Unload - Slot: {slot.Slot}");

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
    }
}
