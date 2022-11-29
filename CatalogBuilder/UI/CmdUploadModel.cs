/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
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
using Inventor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Shared;
using Newtonsoft.Json;
using System.Drawing;
using System.Net.Http;
using System.Net;
using PackAndGoApp;
using System.IO.Compression;
using Autodesk.Forge.Controllers;
using RestSharp;
using PluginUtilities;
using Color = Inventor.Color;
using WebApplication.Utilities;
using WebApplication.Definitions;
using System.Web;

namespace CatalogBuilder.UI
{
    public partial class CmdUploadModel : Command
    {
        private FrmUploadModel m_frmUploadModel;
        private AssemblyDocument m_assemblyDocument;
        private InventorParameters m_inventorParameters;
        private string m_hash;


        public override void ButtonDefinition_OnExecute(NameValueMap context)
        {
            base.ButtonDefinition_OnExecute(context);

        }

        public override void StartCommand()
        {
            base.StartCommand();
            m_assemblyDocument = (AssemblyDocument)Globals.InventorApplication .ActiveDocument;

            string imageFileName = System.IO.Path.Combine(System.IO.Path.GetFileNameWithoutExtension(m_assemblyDocument.FullFileName));
            string imagePath = System.IO.Path.Combine(Globals.JsonDirectory, imageFileName + ".png");
            CreateThumbnail((Document)m_assemblyDocument, imagePath);
           
            m_frmUploadModel = new FrmUploadModel(this);

            if (!(m_frmUploadModel == null))
            {
                m_frmUploadModel.Activate();
                //m_frmUploadModel.TopMost = true;
                m_frmUploadModel.ShowInTaskbar = true;
                m_frmUploadModel.uploadPicture.Image = Image.FromFile(imagePath);
                System.Windows.Forms.DialogResult dialogResult = m_frmUploadModel.ShowDialog();
            }
           
        }


        public override void StopCommand()
        {
            base.StopCommand();

            if (m_frmUploadModel != null)
            {
                // Destroy the command dialog
                m_frmUploadModel.Hide();
                m_frmUploadModel.Dispose();
                m_frmUploadModel = null;
            }
        }



        public override void ExecuteCommand()
        {


            MessageBox.Show("The current model is being uploaded and a project on the configurator is being created.  This will take a few minutes.");
            m_frmUploadModel.Cursor = Cursors.WaitCursor;
            string zipDirectory = System.IO.Path.Combine(Globals.JsonDirectory, Globals.ProjectName);
            string zipPath = PackAndGo.CreatePackAndGo(inventorApplication,
                                zipDirectory);

            CreateInventorParameters();

            ParameterNormalizer parameterNormalizer = new ParameterNormalizer();
            Dictionary<string, string> parametersDictionary = parameterNormalizer.NormailzeParameters(m_inventorParameters);

            m_hash = Crypto.GenerateObjectHashString(parametersDictionary);

            var t = Task.Run(async () =>
            {
 
                try
                {

                    DataManagementController dMController = new DataManagementController();
                    IRestResponse zipPathResponse = await dMController.UploadObjectASync(zipPath, oAuthController.Token);
                    string zipPathSignedurl =
                        await dMController.GetSignedURLAsync(zipPathResponse, oAuthController.Token);
                    Globals.ProjectUrl = zipPathSignedurl;

                    ProjectConfiguration config = Configurations.GetConfiguration(m_inventorParameters);
                    string jsonPath = CreateJsonFile(config);

                    IRestResponse jsonPathResponse = 
                        await dMController.UploadObjectASync(jsonPath, oAuthController.Token);

                    string jsonPathSignedUrl = 
                        await dMController.GetSignedURLAsync(jsonPathResponse, oAuthController.Token);

                    string endPoint = "?url=";// "projects/adopt?url=";
                    var configuration = 
                        await CreateConfiguration(jsonPathSignedUrl, endPoint);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            });

            var p = Task.Run(async () =>
            {
                try
                {
                    double zipSize = new System.IO.FileInfo(zipPath).Length;
                    while (zipSize >= 1024)
                    {
                        zipSize = zipSize / 1024;
                    }
                    //rate in Mbps
                    double uploadRate = 20;
                    // In ms
                    double uploadTime = zipSize / uploadRate * 2000;
                    // Sleep 0.2 sec to simulate some process
                    int timeDelay = 2000;
                    int progressSteps = (int)Math.Ceiling(uploadTime / timeDelay * 10);

                    // Create a new ProgressBar object.
                    Inventor.ProgressBar oProgressBar = inventorApplication.CreateProgressBar(true, progressSteps, "Upload Model");

                    // Set the message for the progress bar
                    oProgressBar.Message = "Uploading Model";

                    long i;
                    for (i = 1; i <= progressSteps; i++)
                    {
                        // Sleep 0.2 sec to simulate some process
                        await Task.Delay(timeDelay);
                        oProgressBar.UpdateProgress();
                    }

                    // Terminate the progress bar.
                    oProgressBar.Close();
                    object p = System.Diagnostics.Process.Start(client.BaseAddress.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }


            });


            m_frmUploadModel.Cursor = Cursors.Default;

            StopCommand();
        }

        static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri(Globals.BaseURL)
        };



        public static async Task<ProjectWithParametersDTO> CreateConfiguration(string configURI, string endPoint)
        {

            string encodedUri = HttpUtility.UrlEncode(configURI);
            string uri = endPoint + encodedUri;
            var response = await client.PostAsync(uri, null);
            var output = await response.Content.ReadAsStringAsync();
            ProjectWithParametersDTO user = JsonConvert.DeserializeObject<ProjectWithParametersDTO>(output);

            return user;

        }

        private void CreateInventorParameters()
        {
            Parameters parameters = (m_assemblyDocument).ComponentDefinition.Parameters;

            ParametersExtractor parametersExtractor = new ParametersExtractor();
            parametersExtractor.Extract((Document)m_assemblyDocument, parameters, null, false);
            string documentParams = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "documentParams.json");

            string json = System.IO.File.ReadAllText(documentParams);
            m_inventorParameters = JsonConvert.DeserializeObject<InventorParameters>(json);
        }

        public string CreateJsonFile(ProjectConfiguration projectConfiguration)
        {

            string configJson = JsonConvert.SerializeObject(projectConfiguration, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None });
            string jsonFileName = projectConfiguration.Name + "." + m_hash + ".json";
            string jsonPath = System.IO.Path.Combine(Globals.JsonDirectory, jsonFileName);
            if (System.IO.File.Exists(jsonPath))
                System.IO.File.Delete(jsonPath);

            System.IO.File.WriteAllText(jsonPath, configJson);
            return jsonPath;

        }

        public void CreateThumbnail(Document doc, string imagePath)
        {
            try
            {
                if (System.IO.File.Exists(imagePath))
                    return;

                AssemblyDocument invDoc = (AssemblyDocument)doc;

                // TODO: only IAM and IPT are supported now, but it's not validated
                invDoc.ObjectVisibility.AllWorkFeatures = false;
                invDoc.ObjectVisibility.Sketches = false;
                invDoc.ObjectVisibility.Sketches3D = false;

                if (doc.DocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                {
                    invDoc.ObjectVisibility.WeldmentSymbols = false;
                }


                inventorApplication.DisplayOptions.Show3DIndicator = false;
                Camera cam = inventorApplication.TransientObjects.CreateCamera();
                cam.SceneObject = invDoc.ComponentDefinition;
                cam.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
                cam.Fit();
                cam.ApplyWithoutTransition();

                Color backgroundColor = inventorApplication.TransientObjects.CreateColor(0xEC, 0xEC, 0xEC, 0.0); // hardcoded. Make as a parameter

                int ThumbnailSize = 500;

                // generate image twice as large, and then downsample it (antialiasing)
                cam.SaveAsBitmap(imagePath, ThumbnailSize , ThumbnailSize , backgroundColor, backgroundColor);

            }
            catch (Exception e)
            {
                MessageBox.Show("Processing failed. " + e.ToString());
            }
        }



    }
}
