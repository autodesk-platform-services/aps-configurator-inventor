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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;
using ForgeControllers;
using Newtonsoft.Json;
using RestSharp;

namespace Autodesk.Forge.Controllers
{
    public class DataManagementController
    {
        private static string baseUrl = "https://developer.api.autodesk.com/oss/v2/";
        private static string bucketId = "<name of bucket created by the configurator web app>";
        static RestClient client = new RestClient();


        public void UploadObject(string filePath, string token)
        {

            string objectName = System.IO.Path.GetFileName(filePath);
            string endpoint = "/buckets/" + bucketId + "/objects/inputs" + objectName;
            string objectExt = System.IO.Path.GetExtension(objectName);

            client.BaseUrl = new Uri(baseUrl + endpoint);
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-Type", "application/png");
            byte[] binaryFile = GetBinaryFile(filePath);
            request.AddParameter("application/"+ objectExt, "<file contents here>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

        }

        public async Task<IRestResponse> UploadObjectASync(string filePath,
                                                           string token)
        {
            string objectName = System.IO.Path.GetFileName(filePath);
            string objectExt = System.IO.Path.GetExtension(objectName);
            string endpoint = "/buckets/" + bucketId + "/objects/" + objectName;

            client.BaseUrl = new Uri(baseUrl + endpoint);
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-Type", "application/" + objectExt);
            byte[] binaryFile = GetBinaryFile(filePath);
            request.AddParameter("application/" + objectExt, binaryFile, ParameterType.RequestBody);

            var cancellationTokenSource = new CancellationToken();
            IRestResponse response =
                await client.ExecuteTaskAsync(request, cancellationTokenSource);
            Console.WriteLine(response.Content);
            return response;
        }



        public string GetSignedURL(IRestResponse objectUri, string token)
        {
            string responsePath = objectUri.ResponseUri.ToString();
            string objectName = System.IO.Path.GetFileName(responsePath);
            string objectExt = System.IO.Path.GetExtension(objectName);

            client.BaseUrl = new Uri(responsePath + "/signed");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/" + objectExt);
            request.AddHeader("Authorization", token);
            request.AddParameter("application/" + objectExt, "{}\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            SignedDto signedDto = JsonConvert.DeserializeObject<SignedDto>(response.Content);
            Console.WriteLine(response.Content);
            return signedDto.signedUrl;
        }

        public async Task<string> GetSignedURLAsync(IRestResponse objectUri, string token)
        {
            string responsePath = objectUri.ResponseUri.ToString();
            string objectName = System.IO.Path.GetFileName(responsePath);

            client.BaseUrl = new Uri(responsePath + "/signed?access=readwrite");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", token);
            request.AddParameter("application / json", "{}", ParameterType.RequestBody);
            var cancellationTokenSource = new CancellationToken();
            IRestResponse response =
                await client.ExecuteTaskAsync(request, cancellationTokenSource);
            SignedDto signedDto = JsonConvert.DeserializeObject<SignedDto>(response.Content);
            Console.WriteLine(response.Content);
            return signedDto.signedUrl;
        }

        private byte[] GetBinaryFile(string filePath)
        {
            byte[] bytes;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
            }
            return bytes;
        }

    }
}
