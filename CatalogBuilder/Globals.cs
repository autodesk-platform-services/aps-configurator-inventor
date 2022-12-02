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
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogBuilder
{
    static class Globals
    {
        private static Inventor.Application m_inventorApplication;

        public static Inventor.Application InventorApplication
        {
            set { m_inventorApplication = value; }
            get { return m_inventorApplication; }
        }

        static string jsonDirectory;
        public static string JsonDirectory
        {
            set { jsonDirectory = value; }
            get { return jsonDirectory; }
        }

        static string projectName;
        public static string ProjectName
        {
            set { projectName = value; }
            get { return projectName; }
        }

        static string topLevelAssembly;
        public static string TopLevelAssembly
        {
            set { topLevelAssembly = value; }
            get { return topLevelAssembly; }
        }

        static string projectUrl;
        public static string ProjectUrl
        {
            set { projectUrl = value; }
            get { return projectUrl; }
        }

        static string configurationsURIsJsonPath;
        public static string ConfigurationsURIsJsonPath
        {
            set { configurationsURIsJsonPath = value; }
            get { return configurationsURIsJsonPath; }
        }

        static string configurationsJsonPath;
        public static string ConfigurationsJsonPath
        {
            set { configurationsJsonPath = value; }
            get { return configurationsJsonPath; }
        }

    }
}
