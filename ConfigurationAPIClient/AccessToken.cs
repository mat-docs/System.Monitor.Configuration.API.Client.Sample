// <copyright file="AccessToken.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Newtonsoft.Json;

namespace SystemMonitorConfigurationTest
{
    internal class AccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("expires_in")]
        public string Expires { get; set; }

        [JsonProperty("token_type")]
        public string Type { get; set; }
    }
}
