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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using WebApplication.Services;

namespace WebApplication.Middleware
{
    /// <summary>
    /// Middleware to extract access code from route parameters.
    /// </summary>
    public class RouteTokenHandler
    {
        private readonly RequestDelegate _next;

        public RouteTokenHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ProfileProvider profileProvider, ILogger<RouteTokenHandler> logger)
        {
            string code = context.GetRouteValue("code") as string; // IMPORTANT: parameter name must be in sync with route definition
            if (!string.IsNullOrEmpty(code))
            {
                logger.LogInformation("Extracted code from route");
                profileProvider.Code = code; // assign Code, Token will be received with code
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    /// <summary>
    /// Special pipeline to extract code from route values
    /// </summary>
    public class RouteTokenPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<RouteTokenHandler>();
        }
    }
}
