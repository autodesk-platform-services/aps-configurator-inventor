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

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Autodesk.Oss;
using Microsoft.AspNetCore.Mvc;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowParametersChangedController : ControllerBase
    {
        private readonly UserResolver _userResolver;

        public ShowParametersChangedController(UserResolver userResolver)
        {
            _userResolver = userResolver;
        }

        [HttpGet]
        public async Task<bool> Get()
        {
            bool result = true;

            Stream objectStream = null;

            try
            {
                var bucket = await _userResolver.GetBucketAsync();
                objectStream = await bucket.GetObjectAsync(ONC.ShowParametersChanged);
            } 
            catch (OssApiException ex) when (ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // the file is not found. Just swallow the exception
            }

            if(objectStream != null)
            {
                result = await JsonSerializer.DeserializeAsync<bool>(objectStream);
            }

            return result;
        }

        [HttpPost]
        public async Task<bool> Set([FromBody]bool show)
        {
            var bucket = await _userResolver.GetBucketAsync();
            await bucket.UploadAsJsonAsync(ONC.ShowParametersChanged, show);
            return show;
        }
    }
}
