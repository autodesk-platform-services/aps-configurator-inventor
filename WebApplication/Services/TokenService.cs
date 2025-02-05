using Autodesk.Forge.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static WebApplication.Services.ProfileProvider;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Text.Json.Serialization;
using System.Net.Http;
using WebApplication.Middleware;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Text.Json;
using System.Collections.Concurrent;
using WebApplication.Utilities;
using System.Linq;
using System.Collections;

namespace WebApplication.Services
{
    public class TokenService
    {
        private readonly ForgeConfiguration _forgeConfig;
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // store PKCE code to token pair
        private ConcurrentDictionary<string, KeyValuePair<DateTime, Lazy<Task<TokenJson>>>> codeToToken = new ConcurrentDictionary<string, KeyValuePair<DateTime, Lazy<Task<TokenJson>>>>();
        
        // store loginID to code_verifier, to be able to use correct stored 'code_verifier' per login
        public ConcurrentDictionary<string, string> id2Verifier = new ConcurrentDictionary<string, string>();

        public class TokenJson
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("expires_in")]
            public long ExpiresIn { get; set; }

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }
        }

        public ForgeConfiguration Configuration { get; }

        public TokenService(IOptions<ForgeConfiguration> forgeConfiguration, IOptions<ForgeConfiguration> optionsAccessor,
            IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = optionsAccessor.Value.Validate();
            _forgeConfig = forgeConfiguration.Value;

            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetToken(string code, string verifierId)
        {
            if (string.IsNullOrEmpty(code))
                return "";

            if (!codeToToken.ContainsKey(code))
                codeToToken.TryAdd(code, KeyValuePair.Create(DateTime.Now, new Lazy<Task<TokenJson>>(async () => await GetTokenFromCode(code, verifierId))));

            Lazy<Task<TokenJson>> lazy = codeToToken[code].Value;
            var jo = await lazy.Value;

            return jo.AccessToken;
        }

        public string GetCallbackUrl()
        {
            HttpContext context = _httpContextAccessor.HttpContext;
            // prepare redirect URL for Oxygen
            // NOTE: This MUST match the pattern of the callback URL field of the app's registration
            // TODO: workaround which may be removed once application will start to use https
            var scheme = context.Request.Scheme;
            if (context.Request.Host.Host == "inventor-config-demo.autodesk.io" ||
                context.Request.Host.Host == "inventor-config-demo-dev.autodesk.io")
            {
                scheme = "https";
            }
            return $"{scheme}{Uri.SchemeDelimiter}{context.Request.Host}";
        }

        private async Task<TokenJson> GetTokenFromCode(string code, string verifierId)
        {
            HttpContext context = _httpContextAccessor.HttpContext;

            var client = _clientFactory.CreateClient();
            string baseUrl = Configuration.AuthenticationAddress.GetLeftPart(System.UriPartial.Authority);
            var url = $"{baseUrl}/authentication/v2/token";

            string codeVerifier = id2Verifier[verifierId];
            var callbackUrl = GetCallbackUrl();
            var body = new System.Net.Http.FormUrlEncodedContent(
                 new Dictionary<string, string>
                 {
                     { "grant_type", "authorization_code" },
                     { "code", code},
                     { "client_id", Configuration.ClientId},
                     { "client_secret", Configuration.ClientSecret},
                     { "code_verifier", codeVerifier },
                     { "redirect_uri", $"{callbackUrl}" }
                 }
             );

            CleanupExpiredTokens();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(url, body);
            if (response.IsSuccessStatusCode)
            {
                string responseStr = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<TokenJson>(responseStr);
                return json;
            }

            return null;
        }

        private void CleanupExpiredTokens()
        {
            List<dynamic> codesForDelete = new List<dynamic>();
            foreach (var item in codeToToken)
            {
                var tokenTime = item.Value.Key;
                if (!item.Value.Value.IsValueCreated) // skip not ready yet items
                    continue;
                var tokenJson = item.Value.Value.Value.Result;
                var ExpiresIn = tokenJson.ExpiresIn;

                var tokenExpTime = tokenTime.AddSeconds(ExpiresIn);
                if (DateTime.Now > tokenExpTime)
                    codesForDelete.Add(item);
            }

            codesForDelete.ForEach(k => codeToToken.TryRemove(k));
        }
    }
}
