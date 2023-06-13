// <copyright file="RequestToken.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitorConfigurationTest
{
    internal class RequestToken
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
    }
}
