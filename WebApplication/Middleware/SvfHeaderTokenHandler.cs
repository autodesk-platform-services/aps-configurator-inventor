/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Autodesk Inventor Automation team
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
    /// Middleware to extract access token from HTTP headers.
    /// this one is special one for SFV to read 'Bearer', because LMV is sending it to us this way
    /// it is not real Bearer, because we are sending PKCE CODE from javascript, but LMV resend it as Bearer
    /// </summary>
    public class SvfHeaderTokenHandler
    {
        private const string BearerPrefix = "Bearer ";

        private readonly RequestDelegate _next;

        public SvfHeaderTokenHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ProfileProvider profileProvider)
        {
            while (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var values))
            {
                var headerValue = values[0];
                if (headerValue.Length <= BearerPrefix.Length) break;
                if (!headerValue.StartsWith(BearerPrefix)) break;

                string code = headerValue.Substring(BearerPrefix.Length);
                if (string.IsNullOrEmpty(code)) break;

                profileProvider.Code = code;
                break;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}