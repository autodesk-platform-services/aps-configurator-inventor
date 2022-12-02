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
using System.Diagnostics;
using PluginUtilities;
using System.Security.RightsManagement;
using Autodesk.Forge.Controllers;
using RestSharp;
using WebApplication.Utilities;

namespace CatalogBuilder.UI
{
    public partial class CmdReadParameters : Command
    {
        private FrmReadParameters m_frmReadParameters;
        private AssemblyDocument m_assemblyDocument;
        private ParametersExtractor m_ParametersExtractor;
        private InventorParameters m_inventorParameters;


        private ParameterList m_parameterList;
        private DataGridView m_parameterDataGrid;
        private DataGridViewColumn m_nameColumn;
        private DataGridViewColumn m_unitTypeColumn;
        private DataGridViewColumn m_equationColumn;
        private DataGridViewColumn m_valueListColumn;

        private AddInServer m_addinserver;

        public CmdReadParameters(AddInServer addInServer)
        {
            m_addinserver = addInServer;
        }


        public override void ButtonDefinition_OnExecute(NameValueMap context)
        {
            base.ButtonDefinition_OnExecute(context);
            
        }

        public override void StartCommand()
        {
            base.StartCommand();

            ReadParameters();

            if (!(m_frmReadParameters == null))
            {
                m_frmReadParameters.Activate();
                //m_frmReadParameters.TopMost = true;
                m_frmReadParameters.ShowInTaskbar = true;
                System.Windows.Forms.DialogResult dialogResult = m_frmReadParameters.ShowDialog();
            }




        }

        public void ReadParameters()
        {
            m_assemblyDocument = (AssemblyDocument)Globals.InventorApplication.ActiveDocument;
            Parameters parameters = ((AssemblyDocument)m_assemblyDocument).ComponentDefinition.Parameters;

            m_ParametersExtractor = new ParametersExtractor();
            m_ParametersExtractor.Extract((Document)m_assemblyDocument, parameters, null, false);
            string documentParamsJson = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "documentParams.json");

            string jsonFile = System.IO.File.ReadAllText(documentParamsJson);
            m_inventorParameters = JsonConvert.DeserializeObject<InventorParameters>(jsonFile);

            m_parameterList = new ParameterList();
            List<ParameterRow> parameterRows = new List<ParameterRow>();
            m_parameterList.parameterRows = parameterRows;
            List<ParameterRow> lists = new List<ParameterRow>();
            foreach (KeyValuePair<string, InventorParameter> entry in m_inventorParameters)
            {
                InventorParameter ip = entry.Value;
                ParameterRow pr = new ParameterRow(ip);
                if (ip.Label is null)
                {
                    pr.Name = entry.Key;
                }
                else
                {
                    pr.Name = ip.Label;
                }
                pr.inventorParameterName = entry.Key;
                string[] values = ip.Values;
                if (pr.inventorParameter.Unit == "Boolean")
                {
                    pr.ValueList = "True,False";
                }
                else if (values.Length == 0)
                {
                    pr.ValueList = ip.Value;
                }
                else
                {
                    pr.ValueList = string.Join(",", values);
                }

                m_parameterList.parameterRows.Add(pr);
            }

            m_frmReadParameters = new FrmReadParameters(this);

            var bindingList = new BindingList<ParameterRow>(m_parameterList.parameterRows);
            var source = new BindingSource(bindingList, null);
            m_parameterDataGrid = m_frmReadParameters.ParameterDataGrid;
            m_parameterDataGrid.DataSource = source;
            SetColumns();

            UpdateConfigurations();
        }

        public void UpdateConfigurations()
        {
            Dictionary<string, HashSet<string>> configValues = Configurations.GetParameterValues(m_parameterList.parameterRows);

            List<string> allPossibleValues = (List<string>)Configurations.GetAllPossibleValues(configValues);
            
            List<ProjectConfiguration> projectConfigurations = Configurations.GetConfigurations(m_inventorParameters, m_parameterList.parameterRows, allPossibleValues);
            m_addinserver.ProjectConfigurations = projectConfigurations;
            string configurationsJson = JsonConvert.SerializeObject(m_addinserver.ProjectConfigurations);
            System.IO.File.WriteAllText(Globals.ConfigurationsJsonPath, configurationsJson);

            m_frmReadParameters.txtTotalConfigurations.Text = allPossibleValues.Count + " Configurations";

        }
        public void SetColumns()
        {
            m_nameColumn = m_parameterDataGrid.Columns[0];
            m_unitTypeColumn = m_parameterDataGrid.Columns[1];
            m_equationColumn = m_parameterDataGrid.Columns[2];
            m_valueListColumn = m_parameterDataGrid.Columns[3];


            m_nameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            m_nameColumn.ReadOnly = true;
            m_nameColumn.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;

            m_unitTypeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            m_unitTypeColumn.ReadOnly = true;
            m_unitTypeColumn.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;

            m_equationColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            m_equationColumn.ReadOnly = true;
            m_equationColumn.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;

            m_valueListColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public ParameterRow GetParameterRow(int RowIndex)
        {
            return m_parameterList.parameterRows[RowIndex];
        }





        public override void StopCommand()
        {
            base.StopCommand();

            if (m_frmReadParameters != null)
            {
                // Destroy the command dialog
                m_frmReadParameters.Hide();
                m_frmReadParameters.Dispose();
                m_frmReadParameters = null;
            }
        }


        public override void ExecuteCommand()
        {
            MessageBox.Show("A json for each configuration will be uploaded to OSS in an asynchronous process.");
 
            var t = Task.Run(async () =>
            {

                try
                {
                    List<string> jsonFiles = await CreateJsonFiles(m_addinserver.ProjectConfigurations);
                    DataManagementController dMController = new DataManagementController();
                    
                    // Create a new ProgressBar object.
                    Inventor.ProgressBar oProgressBar = inventorApplication.CreateProgressBar(true, jsonFiles.Count, "Read Parameters");

                    // Set the message for the progress bar
                    oProgressBar.Message = "Uploading json files";
                    for (int i = 0; i <= jsonFiles.Count - 1; i++ )
                    {
                        string jsonFile = jsonFiles[i];
                        IRestResponse response = await dMController.UploadObjectASync(jsonFile, oAuthController.Token);
                        string jsonFileSignedurl =
                            await dMController.GetSignedURLAsync(response, oAuthController.Token);
                        m_addinserver.ProjectConfigurationsURLs.Add(jsonFileSignedurl);

                        int file = i + 1;
                        oProgressBar.Message = "Uploading json file - " + file + " of " + jsonFiles.Count;
                        oProgressBar.UpdateProgress();
                    }

                    string urlsJson = JsonConvert.SerializeObject(m_addinserver.ProjectConfigurationsURLs, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None });
                    System.IO.File.WriteAllText(Globals.ConfigurationsURIsJsonPath, urlsJson);
                    _ = Process.Start(@Globals.JsonDirectory);

                    // Terminate the progress bar.
                    oProgressBar.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            });

            StopCommand();
        }

        public static async Task<List<string>> CreateJsonFiles(List<ProjectConfiguration> projectConfigurations)
        {

            List<string> result = new List<string>();
            foreach (ProjectConfiguration projectConfig in projectConfigurations)
            {
                //ParameterNormalizer parameterNormalizer = new ParameterNormalizer();
                //Dictionary<string, string> parametersDictionary = parameterNormalizer.NormailzeParameters(projectConfig.Config);

                string hash = Crypto.GenerateParametersHashString(projectConfig.Config);
                string configJson = JsonConvert.SerializeObject(projectConfig, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None });
                string jsonFileName = projectConfig.Name + "." + hash + ".json";
                string jsonPath = System.IO.Path.Combine(Globals.JsonDirectory, jsonFileName);
                result.Add(jsonPath);
                await WriteFileAsync(jsonPath, configJson);
            }
            return result;

        }

        static async Task WriteFileAsync(string path, string content)
        {
            using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(
                    path))
            {
                await outputFile.WriteAsync(content);
            }
        }
    }
}
