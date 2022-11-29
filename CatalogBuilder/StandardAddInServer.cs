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
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using Inventor;
using Microsoft.Win32;
using CatalogBuilder.UI;
using CatalogBuilder.Properties;
using System.Collections.Generic;

namespace CatalogBuilder
{
	/// <summary>
	/// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
	/// that all Inventor AddIns are required to implement. The communication between Inventor and
	/// the AddIn is via the methods on this interface.
	/// </summary>

	[GuidAttribute("9B140960-B56E-42AF-9787-555256C95544")]
	public class AddInServer : Inventor.ApplicationAddInServer
    {
        #region Data Members

        //Inventor application object
        private Inventor.Application m_inventorApplication;
        private Inventor.ApplicationEvents m_appEvents;

        //buttons	
        private CmdUploadModel m_cmdUploadModel;
        private CmdCreateLibrary m_cmdCreateLibrary;
        private CmdReadParameters m_cmdReadParameters;

        //user interface event
        private UserInterfaceEvents m_userInterfaceEvents;

        //Ribbon tsb
        RibbonTab m_assemblyCatalogBuilderRibbonTab;

        // ribbon panel
        RibbonPanel m_assemblyCatalogBuilderRibbonPanel;

		//event handler delegates
		//private Inventor.ComboBoxDefinitionSink_OnSelectEventHandler SlotWidthComboBox_OnSelectEventDelegate;
		//private Inventor.ComboBoxDefinitionSink_OnSelectEventHandler SlotHeightComboBox_OnSelectEventDelegate;

        private Inventor.UserInterfaceEventsSink_OnResetCommandBarsEventHandler UserInterfaceEventsSink_OnResetCommandBarsEventDelegate;
        private Inventor.UserInterfaceEventsSink_OnResetEnvironmentsEventHandler UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate;
        private Inventor.UserInterfaceEventsSink_OnResetRibbonInterfaceEventHandler UserInterfaceEventsSink_OnResetRibbonInterfaceEventDelegate;

        private List<ProjectConfiguration> m_projectConfigurations;
        public List<ProjectConfiguration> ProjectConfigurations
        {
            get { return m_projectConfigurations; }
            set { m_projectConfigurations = value; }
        }

        private List<string> m_projectConfigurationsURLs;
        public List<string> ProjectConfigurationsURLs
        {
            get { return m_projectConfigurationsURLs;}
            set { m_projectConfigurationsURLs = value; }
        }
        #endregion

        public AddInServer()
		{
		}

		#region ApplicationAddInServer Members
		public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
			try
            {

                //initialize AddIn members
                m_inventorApplication = addInSiteObject.Application;
                Globals.InventorApplication = m_inventorApplication;
                Button.InventorApplication = m_inventorApplication;

                //the Activate method is called by Inventor when it loads the addin
                //the AddInSiteObject provides access to the Inventor Application object
                //the FirstTime flag indicates if the addin is loaded for the first time
                m_appEvents = Globals.InventorApplication .ApplicationEvents;

                m_appEvents.OnActivateDocument += new ApplicationEventsSink_OnActivateDocumentEventHandler(ApplicationEvents_OnActivateDocument);


                //initialize event delegates
                m_userInterfaceEvents = Globals.InventorApplication .UserInterfaceManager.UserInterfaceEvents;

                UserInterfaceEventsSink_OnResetCommandBarsEventDelegate = new UserInterfaceEventsSink_OnResetCommandBarsEventHandler(UserInterfaceEvents_OnResetCommandBars);
                m_userInterfaceEvents.OnResetCommandBars += UserInterfaceEventsSink_OnResetCommandBarsEventDelegate;

                UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate = new UserInterfaceEventsSink_OnResetEnvironmentsEventHandler(UserInterfaceEvents_OnResetEnvironments);
                m_userInterfaceEvents.OnResetEnvironments += UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate;

                UserInterfaceEventsSink_OnResetRibbonInterfaceEventDelegate = new UserInterfaceEventsSink_OnResetRibbonInterfaceEventHandler(UserInterfaceEvents_OnResetRibbonInterface);
                m_userInterfaceEvents.OnResetRibbonInterface += UserInterfaceEventsSink_OnResetRibbonInterfaceEventDelegate;

                //load image icons for UI items
                Icon uploadModelIcon = Properties.Resources.UploadModel;
                Icon readParametersIcon = Properties.Resources.ReadParameters;
                Icon createLibraryIcon = Properties.Resources.CreateLibrary;

                //retrieve the GUID for this class
                GuidAttribute addInCLSID;
                addInCLSID = (GuidAttribute)GuidAttribute.GetCustomAttribute(typeof(AddInServer), typeof(GuidAttribute));
                string addInCLSIDString;
                addInCLSIDString = "{" + addInCLSID.Value + "}";

                //create buttons
                m_cmdUploadModel = new CmdUploadModel();
                m_cmdUploadModel.CreateButton(m_inventorApplication,
                    "Upload Model", "Autodesk:CatalogBuilder:uploadModelCmdBtn", CommandTypesEnum.kShapeEditCmdType,
                    addInCLSIDString, "uploades the Active Assembly and generates a project in the configurator",
                    "Upload Model", uploadModelIcon, uploadModelIcon, ButtonDisplayEnum.kAlwaysDisplayText);

                //create buttons
                m_cmdReadParameters = new CmdReadParameters(this);
                m_cmdReadParameters.CreateButton(m_inventorApplication,
                    "Parameters", "Autodesk:CatalogBuilder:readParametersCmdBtn", CommandTypesEnum.kShapeEditCmdType,
                    addInCLSIDString, "Reads the parameters of the Active Assembly and generates a json for each configuration",
                    "Read Parameters", readParametersIcon, readParametersIcon, ButtonDisplayEnum.kAlwaysDisplayText);

                //create buttons

                m_cmdCreateLibrary = new CmdCreateLibrary(this);
                m_cmdCreateLibrary.CreateButton(m_inventorApplication,
                    "Create Library", "Autodesk:CatalogBuilder:CreateLibraryCmdBtn", CommandTypesEnum.kShapeEditCmdType,
                    addInCLSIDString, "Creates a library of configurations",
                    "Creates a library", createLibraryIcon, createLibraryIcon, ButtonDisplayEnum.kAlwaysDisplayText);

   
                if (firstTime == true)
                {
                    //access user interface manager
                    UserInterfaceManager userInterfaceManager;
                    userInterfaceManager = Globals.InventorApplication .UserInterfaceManager;

                    InterfaceStyleEnum interfaceStyle;
                    interfaceStyle = userInterfaceManager.InterfaceStyle;
                                        
                    //get the ribbon associated with part document
                    Inventor.Ribbons ribbons;
                    ribbons = userInterfaceManager.Ribbons;

                    Inventor.Ribbon assemblyRibbon;
                    assemblyRibbon = ribbons["Assembly"];

                    //get the tabs associated with assembly ribbon
                    RibbonTabs ribbonTabs;
                    ribbonTabs = assemblyRibbon.RibbonTabs;

                    //create a new Tab
                    m_assemblyCatalogBuilderRibbonTab = ribbonTabs.Add("Catalog Builder", "Autodesk:CatalogBuilder:CatalogBuilderTab", "{DB59D9A7-EE4C-434A-BB5A-F93E8866E872}", "", false) ;

                    //create a new panel with the tab
                    RibbonPanels ribbonPanels;
                    ribbonPanels = m_assemblyCatalogBuilderRibbonTab.RibbonPanels;

                    m_assemblyCatalogBuilderRibbonPanel = ribbonPanels.Add("Catalog Builder", "Autodesk:CatalogBuilder:CatalogBuilderPanel", "{DB59D9A7-EE4C-434A-BB5A-F93E8866E872}", "", false);

                    //add controls to the slot panel
                    CommandControls assemblyCatalogBuilderRibbonPanelCtrls;
                    assemblyCatalogBuilderRibbonPanelCtrls = m_assemblyCatalogBuilderRibbonPanel.CommandControls;

                    //add the buttons to the ribbon panel
                    CommandControl uploadModelCmdBtnCmdCtrl;
                    uploadModelCmdBtnCmdCtrl = assemblyCatalogBuilderRibbonPanelCtrls.AddButton(m_cmdUploadModel.buttonDefinition, true, true, "", false);

                    CommandControl readParametersCmdBtnCmdCtrl;
                    readParametersCmdBtnCmdCtrl = assemblyCatalogBuilderRibbonPanelCtrls.AddButton(m_cmdReadParameters.buttonDefinition, true, true, "", false);

                    CommandControl createLibraryCmdBtnCmdCtrl;
                    createLibraryCmdBtnCmdCtrl = assemblyCatalogBuilderRibbonPanelCtrls.AddButton(m_cmdCreateLibrary.buttonDefinition, true, true, "", false);


                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void Deactivate()
        {
            //the Deactivate method is called by Inventor when the AddIn is unloaded
            //the AddIn will be unloaded either manually by the user or
            //when the Inventor session is terminated

            try
            {
                m_userInterfaceEvents.OnResetCommandBars -= UserInterfaceEventsSink_OnResetCommandBarsEventDelegate;
                m_userInterfaceEvents.OnResetEnvironments -= UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate;

                UserInterfaceEventsSink_OnResetCommandBarsEventDelegate = null;
                UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate = null;
                m_userInterfaceEvents = null;

                if (m_assemblyCatalogBuilderRibbonPanel != null)
                {
                    m_assemblyCatalogBuilderRibbonPanel.Delete();
                }

                if (m_assemblyCatalogBuilderRibbonTab != null)
                {
                    m_assemblyCatalogBuilderRibbonTab.Delete();
                }


                //release inventor Application object
                Marshal.ReleaseComObject(Globals.InventorApplication );
                Globals.InventorApplication  = null;
                m_appEvents.OnActivateDocument -= new ApplicationEventsSink_OnActivateDocumentEventHandler(ApplicationEvents_OnActivateDocument);



                m_appEvents = null;

                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void ApplicationEvents_OnActivateDocument(_Document DocumentObject,
                                                            EventTimingEnum BeforeOrAfter,
                                                            NameValueMap Context,
                                                            out HandlingCodeEnum HandlingCode)

        {
            HandlingCode = Inventor.HandlingCodeEnum.kEventNotHandled;

            if (BeforeOrAfter != Inventor.EventTimingEnum.kAfter)
            {
                if(DocumentObject.DocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                {
                    SetGlobalJsonDirectory((AssemblyDocument)DocumentObject);
                    Globals.ProjectName = System.IO.Path.GetFileNameWithoutExtension(DocumentObject.DisplayName);

                    string fileLocationsDirectory = System.IO.Path.GetDirectoryName(Globals.InventorApplication.FileLocations.FileLocationsFile);
                    string workSpace = Globals.InventorApplication.FileLocations.Workspace;
                    string workspacePath = System.IO.Path.Combine(fileLocationsDirectory, workSpace);
                    string topLevelAssembly= DocumentObject.FullFileName.Replace(
                        workspacePath, "");
                    if (topLevelAssembly.StartsWith("\\"))
                    {
                        //Character
                        char backslash = @"\"[0];
                        topLevelAssembly = topLevelAssembly.TrimStart(backslash);
                    }
                    Globals.TopLevelAssembly = topLevelAssembly;

                    string configsJsonFileName = Globals.ProjectName + ".Configurations.json";
                    Globals.ConfigurationsJsonPath = System.IO.Path.Combine(Globals.JsonDirectory, configsJsonFileName);


                    string uRIsJsonFileName = Globals.ProjectName + ".ConfigURLs.json";
                    Globals.ConfigurationsURIsJsonPath = System.IO.Path.Combine(Globals.JsonDirectory, uRIsJsonFileName);
                }
                
                return;
            }
            ProjectConfigurations = new List<ProjectConfiguration>();
            ProjectConfigurationsURLs = new List<string>();

            HandlingCode = Inventor.HandlingCodeEnum.kEventHandled;



            //MessageBox.Show(DocumentObject.DisplayName, "C# - OnActivateDocument",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

  


        private void ApplicationEvents_OnCloseDocument(_Document DocumentObject,
                                                    EventTimingEnum BeforeOrAfter,
                                                    NameValueMap Context,
                                                    out HandlingCodeEnum HandlingCode)

        {
            HandlingCode = Inventor.HandlingCodeEnum.kEventNotHandled;
            ProjectConfigurations = null;
        }

            public void SetGlobalJsonDirectory(AssemblyDocument assembly)
        {
            try
            {
                if (string.IsNullOrEmpty(Globals.JsonDirectory))
                {
                    // Define the path to the JSON
                    string jSONDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                                            System.IO.Path.GetFileNameWithoutExtension(assembly.FullFileName) + ".Project");

                    if (!System.IO.Directory.Exists(jSONDirectory))
                        System.IO.Directory.CreateDirectory(jSONDirectory);
                    Globals.JsonDirectory = jSONDirectory;
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public void ExecuteCommand(int CommandID)
		{
			//this method was used to notify when an AddIn command was executed
			//the CommandID parameter identifies the command that was executed
    
			//Note:this method is now obsolete, you should use the new
			//ControlDefinition objects to implement commands, they have
			//their own event sinks to notify when the command is executed
		}

		public object Automation
		{
			//if you want to return an interface to another client of this addin,
			//implement that interface in a class and return that class object 
			//through this property

			get
			{
				return null;
			}
		}

        private void UserInterfaceEvents_OnResetCommandBars(ObjectsEnumerator commandBars, NameValueMap context)
        {
            try
            {
                CommandBar commandBar;
                for (int commandBarCt = 1; commandBarCt <= commandBars.Count; commandBarCt++)
                {
                    commandBar = (Inventor.CommandBar)commandBars[commandBarCt];
                }               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UserInterfaceEvents_OnResetEnvironments(ObjectsEnumerator environments, NameValueMap context)
        {
            try
            {                
                Inventor.Environment environment;
                for (int environmentCt = 1; environmentCt <= environments.Count; environmentCt++)
                {
                    environment = (Inventor.Environment)environments[environmentCt];
                    if (environment.InternalName == "PMxPartSketchEnvironment")
                    {
                        //make this command bar accessible in the panel menu for the 2d sketch environment.
                        environment.PanelBar.CommandBarList.Add(Globals.InventorApplication .UserInterfaceManager.CommandBars["Autodesk:CatalogBuilder:SlotToolbar"]);

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UserInterfaceEvents_OnResetRibbonInterface(NameValueMap context)
        {
            try
            {

                UserInterfaceManager userInterfaceManager;
                userInterfaceManager = Globals.InventorApplication .UserInterfaceManager;

                //get the ribbon associated with part document
                Inventor.Ribbons ribbons;
                ribbons = userInterfaceManager.Ribbons;

                Inventor.Ribbon partRibbon;
                partRibbon = ribbons["Part"];

                //get the tabls associated with part ribbon
                RibbonTabs ribbonTabs;
                ribbonTabs = partRibbon.RibbonTabs;

                RibbonTab partSketchRibbonTab;
                partSketchRibbonTab = ribbonTabs["id_TabSketch"];

                //create a new panel with the tab
                RibbonPanels ribbonPanels;
                ribbonPanels = partSketchRibbonTab.RibbonPanels;

                m_assemblyCatalogBuilderRibbonPanel = ribbonPanels.Add("Slot", "Autodesk:CatalogBuilder:SlotRibbonPanel", 
                                                             "{DB59D9A7-EE4C-434A-BB5A-F93E8866E872}", "", false);

                //add controls to the slot panel
                CommandControls partSketchSlotRibbonPanelCtrls;
                partSketchSlotRibbonPanelCtrls = m_assemblyCatalogBuilderRibbonPanel.CommandControls;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }



		#endregion
	}
}
