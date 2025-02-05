﻿using System;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class ProfileProvider
    {
        private readonly TokenService _tokenService;
        private readonly Lazy<Task<dynamic>> _lazyProfile;
        public Task<string> Token => _tokenService.GetToken(Code, VerifierId);
        public string Code { private get; set; }
        public string VerifierId { private get; set; }

        public ProfileProvider(IForgeOSS forgeOss, TokenService tokenService)
        {
            _lazyProfile = new Lazy<Task<dynamic>>(async () => await forgeOss.GetProfileAsync(await Token));
            _tokenService = tokenService;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Code);

        public Task<dynamic> GetProfileAsync() => _lazyProfile.Value;
    }
}
