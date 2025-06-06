using Autodesk.Forge.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Collections.Concurrent;
using WebApplication.Utilities;
using System.Linq;
using System.Collections;
using Autodesk.Authentication;
using Autodesk.SDKManager;
using Autodesk.Authentication.Model;
using System.Text;
using Autodesk.Authentication.Client;
using WebApplication.Definitions;

namespace WebApplication.Services
{
    public class TokenService
    {
        private readonly ForgeConfiguration _forgeConfig;
        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // store PKCE code to token pair
        private ConcurrentDictionary<string, KeyValuePair<DateTime, Lazy<Task<ThreeLeggedToken>>>> codeToToken = new ConcurrentDictionary<string, KeyValuePair<DateTime, Lazy<Task<ThreeLeggedToken>>>>();

        // store loginID to code_verifier, to be able to use correct stored 'code_verifier' per login
        public ConcurrentDictionary<string, string> id2Verifier = new ConcurrentDictionary<string, string>();

        public ForgeConfiguration Configuration { get; }

        private AuthenticationClient authenticationClient;

        public TokenService(IOptions<ForgeConfiguration> forgeConfiguration, IOptions<ForgeConfiguration> optionsAccessor,
            IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = optionsAccessor.Value.Validate();
            _forgeConfig = forgeConfiguration.Value;

            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;

            var sdkManager = SdkManagerBuilder.Create().Build();

            // Set environment from AuthenticationAddress
            sdkManager.SetEnvFromAuthAddress(Configuration.AuthenticationAddress.AbsolutePath);

            authenticationClient = new AuthenticationClient(sdkManager);
        }

        public async Task<string> GetToken(string code, string verifierId)
        {
            if (string.IsNullOrEmpty(code))
                return "";

            if (!codeToToken.ContainsKey(code))
                codeToToken.TryAdd(code, KeyValuePair.Create(DateTime.Now, new Lazy<Task<ThreeLeggedToken>>(async () => await GetTokenFromCode(code, verifierId))));

            Lazy<Task<ThreeLeggedToken>> lazy = codeToToken[code].Value;
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

        private async Task<ThreeLeggedToken> GetTokenFromCode(string code, string verifierId)
        {
            string codeVerifier = id2Verifier[verifierId];
            var callbackUrl = GetCallbackUrl();

            CleanupExpiredTokens();

            var encodedHost = HttpUtility.UrlEncode(callbackUrl);
            string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(Configuration.ClientId + ":" + Configuration.ClientSecret));

            var response = await authenticationClient.tokenApi.FetchTokenAsync(authorization, GrantType.AuthorizationCode, code, callbackUrl, codeVerifier, throwOnError: false);
            if (response.IsSuccessStatusCode)
            {
                return await LocalMarshalling.DeserializeAsync<ThreeLeggedToken>(response.Content);
            }

            return null;

            // This function is currently unusable because it ignores codeVerifier
            // return await authenticationClient.GetThreeLeggedTokenAsync(Configuration.ClientId, code, callbackUrl, Configuration.ClientSecret, codeVerifier);
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

                var tokenExpTime = tokenTime.AddSeconds((double)ExpiresIn);
                if (DateTime.Now > tokenExpTime)
                    codesForDelete.Add(item);
            }

            codesForDelete.ForEach(k => codeToToken.TryRemove(k));
        }
    }
}
