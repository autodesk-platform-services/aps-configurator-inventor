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

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Shared;

namespace CreateStpPlugin
{
    [ComVisible(true)]
    public class CreateStpAutomation : AutomationBase
    {
        public CreateStpAutomation(InventorServer inventorApp) : base(inventorApp)
        {
        }

        public override void ExecWithArguments(Document doc, NameValueMap map)
        {
            LogTrace("Run called with {0}", doc.DisplayName);
            try
            {
                using (new HeartBeat())
                {
                    ExportSTP(doc);
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

        #region ExportSTP file 

        public void ExportSTP(Document doc) 
        { 
            LogTrace("Export STP file.");

            var startDir = System.IO.Directory.GetCurrentDirectory();

            // output file name
            var fileName = System.IO.Path.Combine(startDir, "Output.stp");

            LogTrace($"Exporting to {fileName}");

            DateTime t = DateTime.Now;
            DateTime t2;
            using (new HeartBeat())
            {
                try
                {
                    doc.SaveAs(fileName, true);
                    LogTrace("Export finished");
                    t2 = DateTime.Now;
                }
                catch (Exception e)
                {
                    t2 = DateTime.Now;
                    LogTrace($"ERROR: Exporting FAILED : {e.Message}");
                }
            }

            if (System.IO.File.Exists(fileName))
            {
                LogTrace($"EXPORTED to : {fileName}");
                LogTrace($"EXPORT took : {(t2 - t).TotalSeconds} seconds");
            }
            else
            {
                LogTrace($"ERROR: EXPORT does not exist !!!!!!!");
            }

        }
        #endregion
    }
}