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
using ForgeControllers.Controllers;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Forms;
using WebApplication.Definitions;

namespace CatalogBuilder.UI
{
    public partial class CmdCreateLibrary : Command
    {
        FrmCreateLibrary m_frmCreateLibrary;

        private AddInServer m_addinserver;
        private DataGridView m_configurationDataGrid;

        private List<ProjectConfiguration> m_configs = new List<ProjectConfiguration>();

        public CmdCreateLibrary(AddInServer addInServer)
        {
            m_addinserver = addInServer;
        }

        static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri(ConfiguratorController.BaseURL)
        };

        public override void StartCommand()
        {
            m_frmCreateLibrary = new FrmCreateLibrary(this);

            m_configurationDataGrid = m_frmCreateLibrary.ConfigurationsDataGrid;
            m_configurationDataGrid.Columns.Add("Id", "Id");
            string json = System.IO.File.ReadAllText(Globals.ConfigurationsJsonPath);
            if (System.IO.File.Exists(Globals.ConfigurationsJsonPath))
                m_configs = JsonConvert.DeserializeObject<List<ProjectConfiguration>>(json);
            else
                m_configs = m_addinserver.ProjectConfigurations;

            if (m_configs.Count == 0)
            {
                CmdReadParameters cmdReadParameters = new CmdReadParameters(m_addinserver);
                cmdReadParameters.ReadParameters();
                cmdReadParameters = null;
            }
            foreach (string key in m_configs[1].Config.Keys)
            {
                int column = m_configurationDataGrid.Columns.Add(key, key);
                m_configurationDataGrid.Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            int i = 1;
            foreach (ProjectConfiguration c in m_configs)
            {

                string[] values = new string[c.Config.Count + 1];
                values[0] = i.ToString();
                i++;
                int j = 0;
                foreach (KeyValuePair<string, InventorParameter> entry in c.Config)
                {
                    j++;
                    values[j] = entry.Value.Value;
                }
      

                int row = m_configurationDataGrid.Rows.Add(values);
            }

            //var bindingList = new BindingList<List<string, string>>(allConfigs);
            //var source = new BindingSource(bindingList, null);
            

            if (!(m_frmCreateLibrary == null))
            {
                m_frmCreateLibrary.Activate();
                //m_frmCreateLibrary.TopMost = true;
                m_frmCreateLibrary.ShowInTaskbar = true;
                System.Windows.Forms.DialogResult dialogResult = m_frmCreateLibrary.ShowDialog();
            }
        }

        public override void ExecuteCommand()
        {
            System.Windows.MessageBox.Show("A each configuration is being built in an asynchronous process.");
            var t = Task.Run(async () =>
            {

                try
                {

                    string endPoint = "projects/adopt?url=";

                    List<string> configurationsURIs = new List<String>();

                    if (System.IO.File.Exists(Globals.ConfigurationsURIsJsonPath))
                    {
                        string json = System.IO.File.ReadAllText(Globals.ConfigurationsURIsJsonPath);
                        configurationsURIs = JsonConvert.DeserializeObject<List<string>>(json);
                    }
                    else
                        configurationsURIs = m_addinserver.ProjectConfigurationsURLs;

                    List<string> configURLs = configurationsURIs;

                    var configurations = await CreateConfigurations(configURLs, endPoint);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                }
            });

            var p = Task.Run(async () =>
            {

                // Create a new ProgressBar object.
                var progress = await CreateProgressBar(m_configs);
            });

            StopCommand();
        }

        public override void StopCommand()
        {
            base.StopCommand();

            if (m_frmCreateLibrary != null)
            {
                // Destroy the command dialog
                m_frmCreateLibrary.Hide();
                m_frmCreateLibrary.Dispose();
                m_frmCreateLibrary = null;
            }
        }
        public static async Task<ProjectWithParametersDTO[]> CreateConfigurations(List<string> configURIs, 
                                                                            string endPoint)
        {
            var taskList = new List<Task<ProjectWithParametersDTO>>();
            for (int i = 0; i <= configURIs.Count - 1; i++)
            {
                string configURI = configURIs[i];
                string encodedUri = HttpUtility.UrlEncode(configURI);
                string uri = endPoint + encodedUri;
                taskList.Add(CreateConfiguration(uri));
            }


            try
            {
                // asynchronously wait until all tasks are complete
                return await Task.WhenAll(taskList);
            }
            catch (Exception)
            {

            }

            return null;

        }

        public static async Task<Inventor.ProgressBar> CreateProgressBar(List<ProjectConfiguration> configs)
        {
            Inventor.ProgressBar progressBar = 
                inventorApplication.CreateProgressBar(true, configs.Count, "Create Library");

            // Set the message for the progress bar
            progressBar.Message = "Creating Configurations";
            for (int i = 0; i <= configs.Count - 1; i++)
            {

                int file = i + 1;
                progressBar.Message = "Creating Configuration - " + file + " of " + configs.Count;
                progressBar.UpdateProgress();
                await Task.Delay(1000);
            }

            // Terminate the progress bar.
            progressBar.Close();
            return (Inventor.ProgressBar)progressBar;
       
        }


    //{

    //}
    public static async Task<ProjectWithParametersDTO> CreateConfiguration(string uri)
        {
            var response = await client.PostAsync(uri, null);
            var output = await response.Content.ReadAsStringAsync();
            ProjectWithParametersDTO user = JsonConvert.DeserializeObject<ProjectWithParametersDTO>(output);
            
            await Task.Delay(10000);

            //string imageUrl = client.BaseAddress + user.Project.Image.Substring(1);
            //string modelUrl = client.BaseAddress + user.Project.ModelDownloadUrl.Substring(1); ;
            //using (WebClient webClient = new WebClient())
            //{
            //    webClient.DownloadFileAsync(new Uri(imageUrl), @"c:\temp\" + user.Project.Label + ".png");
            //}
            //using (WebClient webClient2 = new WebClient())
            //{
            //    webClient2.DownloadFileAsync(new Uri(modelUrl), @"c:\temp\" + user.Project.Label + "." + user.Parameters.Hash + ".zip");
            //}

            return user;

        }
    }
}
