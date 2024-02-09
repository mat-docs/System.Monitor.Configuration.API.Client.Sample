// <copyright file="SettingsAuth.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Windows;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class SettingsAuth
    {
        private readonly Settings settings;

        public SettingsAuth( Settings settings)
        {
            this.InitializeComponent();
            this.settings = settings;

            this.key.Text = this.settings.Key;
            this.tokenUri.Text = this.settings.TokenUri;
            this.audience.Text = this.settings.Audience;
            this.clientId.Text = this.settings.ClientId;
            this.UseToken.IsChecked = this.settings.UseToken;
            this.certificate.Text = this.settings.Certificate;
            this.clientSecret.Text = this.settings.ClientSecret;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.settings.Key = this.key.Text;
            this.settings.TokenUri = this.tokenUri.Text;
            this.settings.Audience = this.audience.Text;
            this.settings.ClientId = this.clientId.Text;
            this.settings.Certificate = this.certificate.Text;
            this.settings.UseToken = (bool)this.UseToken.IsChecked;
            this.settings.ClientSecret = this.clientSecret.Text;
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
