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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Inventor;
using Microsoft.VisualBasic.Compatibility.VB6;
using System.Windows;
using System.Drawing;

namespace CatalogBuilder.UI
{
    public abstract class Command
    {
        //private data members:
        private static Inventor.Application m_inventorApplication;
        protected ButtonDefinition m_buttonDefinition;
        protected bool m_commandIsRunning;
        private ButtonDefinitionSink_OnExecuteEventHandler ButtonDefinition_OnExecuteEventDelegate;
        //public System.Windows.Forms.ProgressBar progressBar = new System.Windows.Forms.ProgressBar( );

  

        #region "Properties"

        public static Inventor.Application inventorApplication
        {
            set
            {
                m_inventorApplication = value;
            }
            get
            {
                return m_inventorApplication;
            }
        }

        public Inventor.ButtonDefinition buttonDefinition => m_buttonDefinition;

        #endregion

        public Command()
        {
            inventorApplication = null;
            m_buttonDefinition = null;

            m_commandIsRunning = false;
        }

        public void CreateButton(Inventor.Application application,
                                    string displayName, 
                                    string internalName, 
                                    CommandTypesEnum commandType, 
                                    string clientId, 
                                    string description, 
                                    string tooltip, 
                                    Icon standardIcon, 
                                    Icon largeIcon, 
                                    ButtonDisplayEnum buttonDisplayType)
        {
            inventorApplication = application;
            try
            {
                //get IPictureDisp for icons
                stdole.IPictureDisp standardIconIPictureDisp;
                standardIconIPictureDisp = (stdole.IPictureDisp)Support.IconToIPicture(standardIcon);

                stdole.IPictureDisp largeIconIPictureDisp;
                largeIconIPictureDisp = (stdole.IPictureDisp)Support.IconToIPicture(largeIcon);

                //create button definition
                m_buttonDefinition = inventorApplication.CommandManager.ControlDefinitions.AddButtonDefinition(displayName, internalName, commandType, clientId, description, tooltip, standardIconIPictureDisp, largeIconIPictureDisp, buttonDisplayType);

                //enable the button
                m_buttonDefinition.Enabled = true;

                //connect the button event sink
                ButtonDefinition_OnExecuteEventDelegate = new ButtonDefinitionSink_OnExecuteEventHandler(ButtonDefinition_OnExecute);
                m_buttonDefinition.OnExecute += ButtonDefinition_OnExecuteEventDelegate;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void CreateButton(Inventor.Application application,
                                    string displayName, 
                                    string internalName, 
                                    CommandTypesEnum commandType, 
                                    string clientId, 
                                    string description, 
                                    string tooltip, 
                                    ButtonDisplayEnum buttonDisplayType)
        {
            inventorApplication = application;
            try
            {
                //create button definition
                m_buttonDefinition = m_inventorApplication.CommandManager.ControlDefinitions.AddButtonDefinition(displayName, internalName, commandType, clientId, description, tooltip, Type.Missing, Type.Missing, buttonDisplayType);

                //enable the button
                m_buttonDefinition.Enabled = true;

                //connect the button event sink
                ButtonDefinition_OnExecuteEventDelegate = new ButtonDefinitionSink_OnExecuteEventHandler(ButtonDefinition_OnExecute);
                m_buttonDefinition.OnExecute += ButtonDefinition_OnExecuteEventDelegate;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public virtual void ButtonDefinition_OnExecute(NameValueMap context)
        {
            // If command was already started,stop it first
            if ((m_commandIsRunning))
                StopCommand();
            StartCommand();
        }

        public void RemoveButton()
        {
            // Disconnect button events sink
            if (!(m_buttonDefinition == null))
            {
                m_buttonDefinition.OnExecute -= this.ButtonDefinition_OnExecute;
                m_buttonDefinition = null;
            }
        }

        public abstract void ExecuteCommand();

        public virtual void StartCommand()
        {
            // Start interaction events
            // StartInteraction()

            // Press the button
            m_buttonDefinition.Pressed = true;

            // Set the command status to running
            m_commandIsRunning = true;
        }



        public virtual void StopCommand()
        {
            try
            {
                // 'Unsubscribe from the events
                // UnsubscribeFromEvents()

                // 'Stop interaction events
                // StopInteraction()

                // Un-press the button
                m_buttonDefinition.Pressed = false;

                // Set the command status to not-running
                m_commandIsRunning = false;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public async Task<string> TestDialogProgressBar()
        {
            int iStepCount = 50;

            // Create a new ProgressBar object.
            ProgressBar oProgressBar = m_inventorApplication.CreateProgressBar(false, iStepCount, "Test Progress");

            // Set the message for the progress bar
            oProgressBar.Message = "Executing some process";

            long i;
            for (i = 1; i <= iStepCount; i++)
            {
                // Sleep 0.2 sec to simulate some process
                await Task.Delay(2000);
                oProgressBar.Message = "Executing some process - " + i;
                oProgressBar.UpdateProgress();
            }

            // Terminate the progress bar.
            oProgressBar.Close();

            return "";
        }

        public async Task<string> StatusBarProgressBar(int iStepCount)
        {


            // Create a new ProgressBar object.
            ProgressBar oProgressBar = m_inventorApplication.CreateProgressBar(true, iStepCount, "Test Progress");

            // Set the message for the progress bar
            oProgressBar.Message = "Executing some process";

            long i;
            for (i = 1; i <= iStepCount; i++)
            {
                // Sleep 0.2 sec to simulate some process
                await Task.Delay(2000);
                oProgressBar.Message = "Executing some process - " + i;
                oProgressBar.UpdateProgress();
            }

            // Terminate the progress bar.
            oProgressBar.Close();


            return "";
        }

        //async Task Progress(ProgressBar fileProgressBar)
        //{
        //    await Task.Run(() =>
        //    {
        //        //A random threadpool thread executes the following:
        //        while (!ProgressComplete)
        //        {
        //            if (FileProgress != 0 && TotalProgress != 0)
        //            {
        //                //Here you signal the UI thread to execute the action:
        //                fileProgressBar.Invoke(new Action(() =>
        //                {
        //                    //This is done by the UI thread:
        //                    fileProgressBar.Value = (int)(fileProgress / totalProgress) * 100
        //                }));
        //            }
        //        }
        //    });
        //}

    }
}
