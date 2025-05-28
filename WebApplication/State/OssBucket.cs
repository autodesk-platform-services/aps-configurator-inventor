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
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Autodesk.Oss;
using Autodesk.Oss.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApplication.Services;
using WebApplication.Utilities;

namespace WebApplication.State
{
    /// <summary>
    /// Wrapper to work with OSS bucket.
    /// </summary>

    public class OssBucketFactory
    {
        private readonly IForgeOSS _forgeOSS;
        private readonly ILogger<OssBucketFactory> _logger;
        public OssBucketFactory(IForgeOSS forgeOSS, ILogger<OssBucketFactory> logger)
        {
            _forgeOSS = forgeOSS;
            _logger = logger;
        }
        public OssBucket CreateBucket(string bucketKey)
        {
            return new OssBucket(_forgeOSS, bucketKey, _logger);
        }

    }
    public class OssBucket
    {
        public string BucketKey { get; }
        private readonly IForgeOSS _forgeOSS;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="forgeOSS">Forge OSS service.</param>
        /// <param name="bucketKey">The bucket key.</param>
        /// <param name="logger">Logger to use.</param>
        public OssBucket(IForgeOSS forgeOSS, string bucketKey, ILogger logger)
        {
            BucketKey = bucketKey;
            _forgeOSS = forgeOSS;
            _logger = logger;
        }

        /// <summary>
        /// Create bucket.
        /// </summary>
        public async Task CreateAsync()
        {
            await _forgeOSS.CreateBucketAsync(BucketKey);
        }

        /// <summary>
        /// Delete the bucket.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            await _forgeOSS.DeleteBucketAsync(BucketKey);
        }

        /// <summary>
        /// Deletes the bucket.
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        public async Task DeleteBucketAsync(string bucketName)
        {
            await _forgeOSS.DeleteBucketAsync(bucketName);
        }

        /// <summary>
        /// Get bucket objects.
        /// </summary>
        /// <param name="beginsWith">Search filter ("begin with")</param>
        public async Task<List<ObjectDetails>> GetObjectsAsync(string beginsWith = null)
        {
            return await _forgeOSS.GetBucketObjectsAsync(BucketKey, beginsWith);
        }

        /// <summary>
        /// List all buckets.
        /// </summary>
        /// <returns>List of buckets</returns>
        public async Task<List<string>> GetBucketsAsync()
        {
            return await _forgeOSS.GetBucketsAsync();
        }

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        public async Task<string> CreateSignedUrlAsync(string objectName, Access access = Access.Read, int minutesExpiration = 30)
        {
            return await _forgeOSS.CreateSignedUrlAsync(BucketKey, objectName, access, minutesExpiration);
        }

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        public async Task CopyAsync(string fromName, string toName)
        {
            await _forgeOSS.CopyAsync(BucketKey, fromName, toName);
        }

        /// <summary>
        /// Download OSS file.
        /// </summary>
        public async Task DownloadFileAsync(string objectName, string localFullName)
        {
            await _forgeOSS.DownloadFileAsync(BucketKey, objectName, localFullName);
        }

        /// <summary>
        /// Ensure local copy of OSS file.
        /// NOTE: it only checks presence of local file.
        /// </summary>
        public async Task EnsureFileAsync(string objectName, string localFullName)
        {
            if (!File.Exists(localFullName))
            {
                await _forgeOSS.DownloadFileAsync(BucketKey, objectName, localFullName);
            }
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public async Task RenameObjectAsync(string oldName, string newName, bool ignoreNotExisting = true)
        {
            try
            {
                await _forgeOSS.RenameObjectAsync(BucketKey, oldName, newName);
            }
            catch (OssApiException e) when (e.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                if(ignoreNotExisting)
                {
                    _logger.LogInformation($"Cannot rename object: {oldName} to ${newName} because object doesn't exists which was expected by using ignoreNotExisting = true");
                } else
                {
                    throw;
                }
                
            }
        }

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        public async Task DeleteObjectAsync(string objectName)
        {
            await _forgeOSS.DeleteAsync(BucketKey, objectName);
        }

        public async Task UploadObjectAsync(string objectName, Stream stream)
        {
            await _forgeOSS.UploadObjectAsync(BucketKey, objectName, stream);
        }

        /// <summary>
        /// Upload JSON representation of object to OSS.
        /// </summary>
        public async Task UploadAsJsonAsync<T>(string objectName, T obj, bool writeIndented = false)
        {
            await using var stream = Json.ToStream(obj, writeIndented);
            await _forgeOSS.UploadObjectAsync(BucketKey, objectName, stream);
        }

        public async Task<Stream> GetObjectAsync(string objectName)
        {
            return await _forgeOSS.GetObjectAsync(BucketKey, objectName);
        }

        /// <summary>
        /// Load JSON from OSS and deserialize it to <see cref="T"/> instance.
        /// </summary>
        public async Task<T> DeserializeAsync<T>(string objectName)
        {
            var objectStream = await _forgeOSS.GetObjectAsync(BucketKey, objectName);
            if (objectStream == null) return default;

            return await JsonSerializer.DeserializeAsync<T>(objectStream);
        }

        /// <summary>
        /// Check if bucket contains the object.
        /// </summary>
        public async Task<bool> ObjectExistsAsync(string objectName)
        {
            try
            {
                await _forgeOSS.GetObjectDetailsAsync(BucketKey, objectName); // don't care about result
                return true;
            }
            catch (OssApiException e) when (e.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // the file is not found. Just swallow the exception
            }

            return false;
        }

        public async Task<string> TryToCreateSignedUrlForReadAsync(string objectName)
        {
            string url = null;
            try
            {
                url = await CreateSignedUrlAsync(objectName);
            }
            catch (OssApiException e) when (e.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // the file does not exist
            }

            return url;
        }

        /// <summary>
        /// Upload file to OSS.
        /// Uses upload in chunks if necessary.
        /// </summary>
        public async Task SmartUploadAsync(string fileName, string objectName, int chunkMbSize = 5)
        {
            await using var fileReadStream = File.OpenRead(fileName);

            // The underlying APS .NET SDK already does chunking  
            await UploadObjectAsync(objectName, fileReadStream);
        }
    }
}
