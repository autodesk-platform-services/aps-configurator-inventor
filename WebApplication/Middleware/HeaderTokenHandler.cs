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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using WebApplication.Services;
using WebApplication.State;

namespace WebApplication.Middleware
{
    /// <summary>
    /// Middleware to extract access code from HTTP headers.
    /// </summary>
    public class HeaderTokenHandler
    {
        private const string CodePrefix = "Code ";
        private const string StatePrefix = ", State ";

        private readonly RequestDelegate _next;
        private readonly TokenService _tokenService;

        public HeaderTokenHandler(RequestDelegate next, TokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context, ProfileProvider profileProvider)
        {
            while (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var values))
            {
                var headerValue = values[0];
                if (headerValue.Length <= CodePrefix.Length) break;
                if (! headerValue.StartsWith(CodePrefix)) break;

                // read 'code' and 'state' value from header
                // code is for PKCE and state is id of code_verifier
                var stateIndex = headerValue.IndexOf(StatePrefix);
                string code = headerValue.Substring(CodePrefix.Length, stateIndex - CodePrefix.Length);
                string id = headerValue.Substring(stateIndex + StatePrefix.Length);
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(id)) break;

                profileProvider.VerifierId = id;
                profileProvider.Code = code;
                break;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}