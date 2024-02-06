// <copyright file="Settings.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Newtonsoft.Json;

namespace SystemMonitorConfigurationTest
{
    public class Settings
    {
        [JsonProperty("use_token")]
        public bool UseToken { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("token_uri")]
        public string TokenUri { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("certifiate")]
        public string Certificate { get; set; }
        
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}