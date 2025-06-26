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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Autodesk.Authentication;
using Autodesk.Authentication.Model;
using Autodesk.Forge.Core;
using Autodesk.Oss;
using Autodesk.Oss.Model;
using Autodesk.SDKManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Polly;
using Polly.Wrap;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    /// <summary>
    /// Class to work with Forge APIs.
    /// </summary>
    public class ForgeOSS : IForgeOSS
    {
        private SDKManager sdkManager;
        private OssClient ossClient;
        private AuthenticationClient authenticationClient;

        /// <summary>
        /// Page size for "Get Bucket Objects" operation.
        /// </summary>
        private const int PageSize = 50;

        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ForgeOSS> _logger;
        private static readonly List<Scopes> _scope = new List<Scopes>() { Scopes.DataRead, Scopes.DataWrite, Scopes.BucketCreate, Scopes.BucketDelete, Scopes.BucketRead };

        private readonly AsyncPolicyWrap _ossResiliencyPolicy;

        public Task<string> TwoLeggedAccessToken => _twoLeggedAccessToken.Value;
        private Lazy<Task<string>> _twoLeggedAccessToken;

        /// <summary>
        /// Forge configuration.
        /// </summary>
        public ForgeConfiguration Configuration { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ForgeOSS(IHttpClientFactory clientFactory, IOptions<ForgeConfiguration> optionsAccessor, ILogger<ForgeOSS> logger)
        {
            sdkManager = SdkManagerBuilder.Create().Build();
            ossClient = new OssClient(sdkManager);
            authenticationClient = new AuthenticationClient(sdkManager);

            _clientFactory = clientFactory;
            _logger = logger;
            Configuration = optionsAccessor.Value.Validate();

            RefreshApiToken();

            // create policy to refresh API token on expiration (401 error code)
            var refreshTokenPolicy = Policy
                                    .Handle<OssApiException>(e => e.HttpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                                    .RetryAsync(5, (_, __) => RefreshApiToken());

            var bulkHeadPolicy = Policy.BulkheadAsync(10, int.MaxValue);
            var rateLimitRetryPolicy = Policy
                .Handle<OssApiException>(e => e.HttpResponseMessage.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(40)
                });
            var waitForObjectPolicy = Policy
                .Handle<OssApiException>(e => e.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                    retryCount: 4,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan) => _logger.LogWarning("Cannot get fresh OSS object. Repeating")
                );
            _ossResiliencyPolicy = refreshTokenPolicy
                .WrapAsync(rateLimitRetryPolicy)
                .WrapAsync(bulkHeadPolicy)
                .WrapAsync(waitForObjectPolicy);
        }

        public async Task<List<ObjectDetails>> GetBucketObjectsAsync(string bucketKey, string beginsWith = null)
        {
            BucketObjects objects = new BucketObjects();
            objects.Items = new List<ObjectDetails>();

            string startAt = null; // next page pointer

            do {
                var tempObjects = await WithOssClientAsync(async ossClient =>
                {
                    var tempObjects = await ossClient.GetObjectsAsync(bucketKey, PageSize, beginsWith, startAt);
                    return tempObjects;
                });

                objects.Items.AddRange(tempObjects.Items);

                startAt = tempObjects.Next;

            } while (startAt != null);

            return objects.Items;
        }

        /// <summary>
        /// List all buckets
        /// </summary>
        /// <returns>List of all buckets for the account</returns>
        public async Task<List<string>> GetBucketsAsync()
        {
            var buckets = new List<string>();
            string startAt = null;

            do
            {
                var bucketList = await WithOssClientAsync(async ossClient =>
                {
                    return await ossClient.GetBucketsAsync(/* use default (US region) */ null, PageSize, startAt);
                });

                foreach(var bucket in bucketList.Items)
                {
                    buckets.Add(bucket.BucketKey);
                }

                startAt = bucketList.Next;

            } while (startAt != null);

            return buckets;
        }

      /// <summary>
      /// Create bucket with given name
      /// </summary>
      /// <param name="bucketKey">The bucket name.</param>
      public async Task CreateBucketAsync(string bucketKey)
        {
            await WithOssClientAsync(async ossClient =>
            {
                var payload = new CreateBucketsPayload
                {
                    BucketKey = bucketKey,
                    Allow = null,
                    PolicyKey = PolicyKey.Persistent
                };

                await ossClient.CreateBucketAsync(Region.US, payload);
            });
        }

        public async Task DeleteBucketAsync(string bucketKey)
        {
            await WithOssClientAsync(async ossClient =>
                await ossClient.DeleteBucketAsync(bucketKey)
            );
        }

        /// <summary>
        /// Generate a signed URL to OSS object.
        /// NOTE: An empty object created if not exists.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="objectName">Object name.</param>
        /// <param name="access">Requested access to the object.</param>
        /// <param name="minutesExpiration">Minutes while the URL is valid. Default is 30 minutes.</param>
        /// <returns>Signed URL</returns>
        public async Task<string> CreateSignedUrlAsync(string bucketKey, string objectName, Access access = Access.Read, int minutesExpiration = 30)
        {
            return await GetSignedUrl(bucketKey, objectName, access, minutesExpiration);
        }

        public async Task<ObjectFullDetails> GetObjectDetailsAsync(string bucketKey, string objectName)
        {
            return await ossClient.GetObjectDetailsAsync(bucketKey, EncodedObjectName(objectName), accessToken: await TwoLeggedAccessToken);
        }

        public async Task UploadObjectAsync(string bucketKey, string objectName, Stream stream)
        {
            await WithOssClientAsync(async ossClient => await ossClient.UploadObjectAsync(bucketKey, EncodedObjectName(objectName), stream));
        }

        /// <summary>
        /// Rename object.
        /// </summary>
        /// <param name="bucketKey">Bucket key.</param>
        /// <param name="oldName">Old object name.</param>
        /// <param name="newName">New object name.</param>
        public async Task RenameObjectAsync(string bucketKey, string oldName, string newName)
        {
            // OSS does not support renaming, so emulate it with more ineffective operations
            await WithOssClientAsync(async ossClient => await ossClient.CopyToAsync(bucketKey, EncodedObjectName(oldName), EncodedObjectName(newName)));
            await WithOssClientAsync(async ossClient => await ossClient.DeleteObjectAsync(bucketKey, EncodedObjectName(oldName)));
        }

        public async Task<Stream> GetObjectAsync(string bucketKey, string objectName)
        {
            Stream stream = await WithOssClientAsync(async ossClient => await ossClient.DownloadObjectAsync(bucketKey, EncodedObjectName(objectName)));
            return stream;
        }

        /// <summary>
        /// Copy OSS object.
        /// </summary>
        public async Task CopyAsync(string bucketKey, string fromName, string toName)
        {
            await WithOssClientAsync(async ossClient => await ossClient.CopyToAsync(bucketKey, EncodedObjectName(fromName), EncodedObjectName(toName)));
        }

        /// <summary>
        /// Delete OSS object.
        /// </summary>
        public async Task DeleteAsync(string bucketKey, string objectName)
        {
            await WithOssClientAsync(async ossClient => await ossClient.DeleteObjectAsync(bucketKey, EncodedObjectName(objectName)));
        }

        /// <summary>
        /// Download OSS file.
        /// </summary>
        public async Task DownloadFileAsync(string bucketKey, string objectName, string localFullName)
        {
            var url = await CreateSignedUrlAsync(bucketKey, objectName);

            var client = _clientFactory.CreateClient();
            await client.DownloadAsync(url, localFullName);
        }

        /// <summary>
        /// Get profile for the user with the access token.
        /// </summary>
        /// <param name="token">Oxygen access token.</param>
        /// <returns>Object with User Info</returns>
        /// <remarks>
        /// User Profile fields: https://aps.autodesk.com/en/docs/oauth/v2/reference/http/users-@me-GET/#body-structure-200
        /// </remarks>
        public async Task<UserInfo> GetProfileAsync(string token)
        {
            return await authenticationClient.GetUserInfoAsync(token); // TODO: use Polly cache policy!
        }

        /// <summary>
        /// Run action against OSS Client.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private async Task WithOssClientAsync(Func<OssClient, Task> action)
        {
            await _ossResiliencyPolicy.ExecuteAsync(async () =>
            {
                ossClient.AuthenticationProvider = new StaticAuthenticationProvider(await TwoLeggedAccessToken);
                await action(ossClient);
            });
        }

        /// <summary>
        /// Run action against OSS Client.
        /// </summary>
        /// <remarks>The action runs with retry policy to handle API token expiration.</remarks>
        private async Task<T> WithOssClientAsync<T>(Func<OssClient, Task<T>> action)
        {
            return await _ossResiliencyPolicy.ExecuteAsync(async () =>
            {
                ossClient.AuthenticationProvider = new StaticAuthenticationProvider(await TwoLeggedAccessToken);
                return await action(ossClient);
            });
        }

        private void RefreshApiToken()
        {
            _twoLeggedAccessToken = new Lazy<Task<string>>(async () => await _2leggedAsync());
        }

        private async Task<string> _2leggedAsync()
        {
            _logger.LogInformation("Refreshing Forge token");
            TwoLeggedToken twoLeggedToken = await authenticationClient.GetTwoLeggedTokenAsync(Configuration.ClientId, Configuration.ClientSecret, _scope);

            return twoLeggedToken.AccessToken;
        }

        /// <summary>
        /// Generate signed URL for the OSS object.
        /// </summary>
        private async Task<string> GetSignedUrl(string bucketKey, string objectName,
                                                        Access access = Access.Read, int minutesExpiration = 30)
        {
            CreateSignedResource createSignedResource = new CreateSignedResource
            {
                MinutesExpiration = minutesExpiration,
                SingleUse = false
            };

            CreateObjectSigned result = await WithOssClientAsync(async ossClient => await ossClient.CreateSignedResourceAsync(bucketKey, EncodedObjectName(objectName), createSignedResource, access));
            return result.SignedUrl;
        }

        /// <summary>
        /// Encode object name.
        /// New SDK expects encoded names.
        /// </summary>
        private string EncodedObjectName(string objectName)
        {
            return HttpUtility.UrlEncode(objectName);
        }
    }
}
