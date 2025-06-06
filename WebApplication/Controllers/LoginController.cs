/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Autodesk Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Autodesk.Forge.Core;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Middleware;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;
using Autodesk.Authentication.Model;
using Autodesk.Authentication;
using Autodesk.SDKManager;
using System.Collections.Generic;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private static readonly ProfileDTO AnonymousProfile = new ProfileDTO { Name = "Anonymous", AvatarUrl = "logo-xs-white-BG.svg" };

        private readonly ILogger<LoginController> _logger;
        private readonly ProfileProvider _profileProvider;
        private readonly InviteOnlyModeConfiguration _inviteOnlyModeConfig;
        private readonly TokenService _tokenService;

        private AuthenticationClient authenticationClient;

        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }

        public LoginController(ILogger<LoginController> logger, IOptions<ForgeConfiguration> optionsAccessor, ProfileProvider profileProvider, IOptions<InviteOnlyModeConfiguration> inviteOnlyModeOptionsAccessor, TokenService  tokenService)
        {
            _logger = logger;
            _profileProvider = profileProvider;
            Configuration = optionsAccessor.Value.Validate();
            _inviteOnlyModeConfig = inviteOnlyModeOptionsAccessor.Value;
            _tokenService = tokenService;

            var sdkManager = SdkManagerBuilder.Create().Build();

            // Set environment from AuthenticationAddress
            sdkManager.SetEnvFromAuthAddress(Configuration.AuthenticationAddress.AbsolutePath);

            authenticationClient = new AuthenticationClient(sdkManager);
        }

        [HttpGet]
        public RedirectResult Get()
        {
            _logger.LogInformation("Authorize against the Oxygen");


            var callbackUrl = _tokenService.GetCallbackUrl();

            // prepare scope
            List<Scopes> scopes = new List<Scopes> { Scopes.UserProfileRead };

            string verifierID = CryptoRandom.CreateUniqueId(10);
            string codeVerifier = CryptoRandom.CreateUniqueId(32);
            _tokenService.id2Verifier.TryAdd(verifierID, codeVerifier);

            string code_challenge;
            using (var sha256 = SHA256.Create())
            {
                // Here we create a hash of the code verifier
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

                // and produce the "Code Challenge" from it by base64Url encoding it.
                code_challenge = Base64Url.Encode(challengeBytes);
            }
            string code_challenge_method = "S256";

            var authUrl = authenticationClient.Authorize(Configuration.ClientId, ResponseType.Code, callbackUrl, scopes, codeChallenge: code_challenge, codeChallengeMethod: code_challenge_method, state: verifierID);

            return Redirect(authUrl);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ProfileDTO>> Profile()
        {
            _logger.LogInformation("Get profile");
            if (_profileProvider.IsAuthenticated)
            {
                UserInfo profile = await _profileProvider.GetProfileAsync();
                if (_inviteOnlyModeConfig.Enabled)
                {
                    var inviteOnlyChecker = new InviteOnlyChecker(_inviteOnlyModeConfig);
                    if (!profile.EmailVerified == true || !inviteOnlyChecker.IsInvited(profile.Email))
                    {
                        return StatusCode(403);
                    }
                }
                return new ProfileDTO { Name = profile.Name, AvatarUrl = profile.Thumbnails["sizeX40"] };
            }
            else
            {
                return AnonymousProfile;
            }
        }
    }
}
