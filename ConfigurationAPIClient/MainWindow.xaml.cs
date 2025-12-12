// <copyright file="MainWindow.xaml.cs" company="Motion Applied Ltd.">
// Copyright (c) Motion Applied Ltd.</copyright>

using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Grpc.Core;
using SystemMonitorConfigurationTest.Dialogs;
using SystemMonitorProtobuf;
using ParameterType = SystemMonitorProtobuf.ParameterType;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SystemMonitorConfigurationTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private SystemMonitorSystem.SystemMonitorSystemClient systemClient;
        private SystemMonitorProject.SystemMonitorProjectClient projectClient;
        private SystemMonitorVirtual.SystemMonitorVirtualClient virtualClient;
        private SystemMonitorLogging.SystemMonitorLoggingClient loggingClient;
        private SystemMonitorParameter.SystemMonitorParameterClient paramClient;

        private Settings settings;
        private AccessToken token;
        private Metadata header;

        private bool BatchMode { get; set; }
        private bool LiveState { get; set; }
        private bool OnlineState { get; set; }
        private bool EnableBackup { get; set; }
        private LinkStatus LinkState { get; set; }
        private bool LiveLoggingState { get; set; }
        private double ValueOffset { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            this.settings = new Settings();
            this.LoadConfig();

            this.LinkState = 0;
            this.ValueOffset = 0;
            this.BatchMode = false;
            this.LiveState = false;
            this.OnlineState = false;
            this.EnableBackup = false;
            this.LiveLoggingState = false;
        }

        public int PopulateParams(ParameterType dataType, ref ComboBox combo)
        {
            combo.ItemsSource = null;
            var stopwatch = Stopwatch.StartNew();

            var request = new AppTypeRequest
            {
                AppId = this.appList.SelectedItem != null ? (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]! : (ushort)0,
                DataType = dataType
            };

            var reply = this.paramClient.GetParameters(request, this.header);
            if (reply.Parameters.Count > 0)
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(string));
                dt.Columns.Add("Name", typeof(string));

                foreach (var param in reply.Parameters)
                {
                    dt.Rows.Add(param.Id, param.Name);
                }

                var binding = new Binding
                {
                    Source = dt
                };

                combo.DisplayMemberPath = "Name";
                combo.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                combo.SelectedIndex = 0;
            }

            this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            this.results.Items.Add($"   {dataType.ToString()} Params: {reply.Parameters.Count} \t- {stopwatch.ElapsedMilliseconds}ms");
            return reply.Parameters.Count;
        }

        private void Setup()
        {
            if (this.settings.UseToken)
            {
                if (this.token == null)
                {
                    var result = this.GetAccessToken();
                    if (result != HttpStatusCode.OK)
                    {
                        this.results.Items.Add("Access Token Request FAILED!");
                    }
                }
            }

            var cert = new X509Certificate2(this.settings.Certificate, this.settings.Key);
            var httpHandler = new HttpClientHandler();
            httpHandler.ClientCertificates.Add(cert);
            
            // Add server certificate validation
            httpHandler.ServerCertificateCustomValidationCallback = (message, serverCert, chain, errors) =>
            {
                // For production, implement proper certificate pinning or validation logic
                // This example allows self-signed or untrusted certificates (development only)
                if (errors == System.Net.Security.SslPolicyErrors.None)
                {
                    return true;
                }
                
                // Return true to allow the connection despite validation errors
                // WARNING: Only for development/testing with self-signed certificates
                return true;
            };
            
            var httpClient = new HttpClient(httpHandler);
            var channel = GrpcChannel.ForAddress(this.serverAddress.Text, new GrpcChannelOptions
            {
                HttpClient = httpClient,
            });

            this.systemClient = new SystemMonitorSystem.SystemMonitorSystemClient(channel);
            this.projectClient = new SystemMonitorProject.SystemMonitorProjectClient(channel);
            this.virtualClient = new SystemMonitorVirtual.SystemMonitorVirtualClient(channel);
            this.loggingClient = new SystemMonitorLogging.SystemMonitorLoggingClient(channel);
            this.paramClient = new SystemMonitorParameter.SystemMonitorParameterClient(channel);
        }

        private void ClearResults()
        {
            if (this.serverAddress.IsEnabled)
            {
                this.serverAddress.IsEnabled = false;

                this.SaveConfig();

                this.Setup();
            }

            this.results.Items.Clear();
            this.resultList.ItemsSource = null;
            this.AuthSettings.IsEnabled = false;
            this.GetUnitByIndex.IsEnabled = false;
            this.SetUnitByIndex.IsEnabled = false;
            this.SetLiveLogging.IsEnabled = false;
            this.SetMultiApplicationBase.IsEnabled = false;
        }

        private HttpStatusCode GetAccessToken()
        {
            this.token = null;
            this.header = null;

            var requestToken = new RequestToken
            {
                ClientId = this.settings.ClientId,
                ClientSecret = this.settings.ClientSecret,
                Audience = this.settings.Audience,
                GrantType = "client_credentials"
            };
            var requestString = JsonConvert.SerializeObject(requestToken);

            var client = new RestClient(this.settings.TokenUri);
            var request = new RestRequest();
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json",
                requestString,
                RestSharp.ParameterType.RequestBody);
            var response = client.ExecutePost(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                this.token = JsonConvert.DeserializeObject<AccessToken>(response.Content);

                this.header = new Metadata
                {
                    { "Authorization", $"{this.token.Type} {this.token.Token}" }
                };
            }
            else
            {
                this.results.Items.Add($"Authorization FAILED: '{response.StatusCode}' - '{response.Content}'");
            }

            return response.StatusCode;
        }

        private void SaveConfig()
        {
            this.settings.Address = this.serverAddress.Text;

            var baseDir = Environment.CurrentDirectory;
            using var file = File.CreateText(@$"{baseDir}\settings.json");
            var serializer = new JsonSerializer();
            serializer.Serialize(file, this.settings );
        }

        private void LoadConfig()
        {
            var baseDir = Environment.CurrentDirectory;
            var filename = @$"{baseDir}\settings.json";
            if (File.Exists(filename))
            {
                using var file = File.OpenText(filename);
                var serializer = new JsonSerializer();
                this.settings = (Settings)serializer.Deserialize(file, typeof(Settings));
            }

            if (this.settings != null)
            {
                this.settings.Address ??= "https://localhost:7000";
                this.serverAddress.Text = this.settings.Address;

                this.GetToken.IsEnabled = this.settings.UseToken;
                this.NoToken.IsEnabled = this.settings.UseToken;
                this.InvalidToken.IsEnabled = this.settings.UseToken;
            }
        }

        private void SetErrorCode(ErrorCode error)
        {
            this.resultCode.Content = $"Result: {error}";
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsAuth(this.settings);
            if (dialog.ShowDialog() == true)
            {
                this.SaveConfig();
            }
            this.GetToken.IsEnabled = this.settings.UseToken;
            this.NoToken.IsEnabled = this.settings.UseToken;
            this.InvalidToken.IsEnabled = this.settings.UseToken;
        }

        private void GetToken_Click(object sender, RoutedEventArgs e)
        {
            if (this.settings.UseToken)
            {
                var result = this.GetAccessToken();

                if (result == HttpStatusCode.OK)
                {
                    var expires = DateTime.Now + TimeSpan.FromSeconds(Convert.ToDouble(this.token.Expires));

                    this.results.Items.Add($"Get Access Token: {result}");
                    this.results.Items.Add($"   Token:   '{this.token.Token}'");
                    this.results.Items.Add($"   Type:    {this.token.Type}");
                    this.results.Items.Add($"   Expires: {this.token.Expires}s => {expires}");
                }
            }
        }

        private void NoToken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                this.systemClient.GetStatus(new Empty());
                this.results.Items.Add("No Token Test FAILED: API Call Succeeded without Authentication!");
            }
            catch (RpcException ex)
            {
                this.results.Items.Add("No Token Test SUCCEEDED!");
                this.results.Items.Add($"{ex.Message}");
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

        private void InvalidToken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.ClearResults();

                var invalid = new Metadata
                {
                    { "Authorization", "Invalid Token" }
                };

                this.systemClient.GetStatus(new Empty(), invalid);
                this.results.Items.Add("Invalid Token Test FAILED: API Call Succeeded with INVALID Authentication!");
            }
            catch (RpcException ex)
            {
                this.results.Items.Add("Invalid Token Test SUCCEEDED!");
                this.results.Items.Add($"{ex.Message}");
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

        private void GetApps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                this.appList.ItemsSource = null;
                var stopwatch = Stopwatch.StartNew();

                var apps = this.projectClient.GetAppDetails(new Empty(), this.header);
                if (apps.Apps.Count > 0)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Id", typeof(ushort));
                    dt.Columns.Add("Name", typeof(string));

                    foreach (var app in apps.Apps)
                    {
                        dt.Rows.Add(app.AppId, $"0x{app.AppId,4:X4} - {app.AppName}");
                    }

                    var binding = new Binding
                    {
                        Source = dt
                    };

                    this.appList.DisplayMemberPath = "Name";
                    this.appList.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                    this.appList.SelectedIndex = 0;
                }

                this.SetErrorCode(apps.ReturnCode);
                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void PopulateConversions(DataTable dt, uint appId)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = new AppRequest
            {
                AppId = appId
            };

            var reply = this.paramClient.GetConversions(request, this.header);
            if (reply.Conversions.Count > 0)
            {
                foreach (var conversion in reply.Conversions)
                {
                    dt.Rows.Add(conversion.Id,
                        appId == 0
                            ? $"{conversion.Id} \t- {conversion.Type} - VIRTUAL OR CAN"
                            : $"{conversion.Id} \t- {conversion.Type}");
                }
            }

            this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            this.results.Items.Add(appId == 0
                ? $"   Other Conversions: {reply.Conversions.Count} \t- {stopwatch.ElapsedMilliseconds}ms"
                : $"   Conversions: {reply.Conversions.Count} \t- {stopwatch.ElapsedMilliseconds}ms");
        }

        private void PopulateConversions()
        {
            this.conv.ItemsSource = null;
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            var binding = new Binding
            {
                Source = dt
            };

            this.conv.DisplayMemberPath = "Name";
            this.conv.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.conv.SelectedIndex = 0;

            var appId = this.appList.SelectedItem != null ? (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]! : (ushort)0;
            if (appId != 0)
            {
                this.PopulateConversions(dt, appId);
            }
            this.PopulateConversions(dt, 0);
        }

        private void PopulateEvents()
        {
            var stopwatch = Stopwatch.StartNew();

            this.evnt.ItemsSource = null;
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(ushort));
            dt.Columns.Add("Name", typeof(string));
            var binding = new Binding
            {
                Source = dt
            };

            this.evnt.DisplayMemberPath = "Name";
            this.evnt.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.evnt.SelectedIndex = 0;

            var appId = this.appList.SelectedItem != null ? (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]! : (ushort)0;
            if (appId != 0)
            {
                var request = new AppRequest
                {
                    AppId = appId
                };

                var reply = this.projectClient.GetEvents(request, this.header);
                if (reply.Events.Count > 0)
                {
                    foreach (var item in reply.Events)
                    {
                        dt.Rows.Add((ushort)item.Id, $"0x{item.Id:X04}, \t- '{item.Name}' \t- {item.Priority}");
                    }
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                this.results.Items.Add($"   Events: {reply.Events.Count} \t- {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private void PopulateErrors()
        {
            var stopwatch = Stopwatch.StartNew();

            this.err.ItemsSource = null;
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            var binding = new Binding
            {
                Source = dt
            };

            this.err.DisplayMemberPath = "Name";
            this.err.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            this.err.SelectedIndex = 0;

            var appId = this.appList.SelectedItem != null ? (ushort)((DataRowView)this.appList.SelectedItem).Row.ItemArray[0]! : (ushort)0;
            if (appId != 0)
            {
                var request = new AppRequest
                {
                    AppId = appId
                };

                var reply = this.projectClient.GetErrorDefinitions(request, this.header);
                if (reply.ErrorDefinitions.Count > 0)
                {
                    foreach (var item in reply.ErrorDefinitions)
                    {
                        dt.Rows.Add(item.Id, $"'{item.Id}', \t- '{item.Name}'");
                    }
                }

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
                this.results.Items.Add($"   Errors: {reply.ErrorDefinitions.Count} \t- {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private void GetParams_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                this.ClearResults();
                var stopwatch = Stopwatch.StartNew();

                var count = 0;
                this.results.Items.Add($"App: '{this.appList.Text}'");
                if (this.appList.SelectedItem != null)
                {
                    count += this.PopulateParams(ParameterType.Measurement, ref this.measurement);
                    count += this.PopulateParams(ParameterType.Scalar, ref this.scalar);
                    count += this.PopulateParams(ParameterType.Axis1, ref this.axis1);
                    count += this.PopulateParams(ParameterType.Axis2, ref this.axis2);
                    count += this.PopulateParams(ParameterType.Axis, ref this.axis);
                    count += this.PopulateParams(ParameterType.Array, ref this.array);
                    count += this.PopulateParams(ParameterType.String, ref this.str);
                }

                count += this.PopulateParams(ParameterType.Can, ref this.can);
                count += this.PopulateParams(ParameterType.Virtual, ref this.virt);
                this.results.Items.Add($"   Total Params: {count} \t- {stopwatch.ElapsedMilliseconds}ms");

                this.PopulateConversions();
                this.PopulateEvents();
                this.PopulateErrors();

                this.executeTime.Content = $"{stopwatch.ElapsedMilliseconds}ms";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
