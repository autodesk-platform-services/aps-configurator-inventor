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

        static string m_baseURL = "https://localhost:5001/";
        public static string BaseURL
        {
            set { m_baseURL = value; }
            get { return m_baseURL; }
        }

        static string m_bucketId = "projects-rlw-47d7d336825a8837618f3bed";
        public static string BucketId
        {

            set { m_bucketId = value; }
            get { return m_bucketId; }
        }

        static string m_JsonDirectory;
        public static string JsonDirectory
        {
            set { m_JsonDirectory = value; }
            get { return m_JsonDirectory; }
        }

        static string m_projectName;
        public static string ProjectName
        {
            set { m_projectName = value; }
            get { return m_projectName; }
        }

        static string m_topLevelAssembly;
        public static string TopLevelAssembly
        {
            set { m_topLevelAssembly = value; }
            get { return m_topLevelAssembly; }
        }

        static string m_projectUrl = "https://sdra-default-projects.s3-us-west-2.amazonaws.com/Wheel_2021.zip";
        public static string ProjectUrl
        {
            set { m_projectUrl = value; }
            get { return m_projectUrl; }
        }

        static string m_configurationsURIsJsonPath;
        public static string ConfigurationsURIsJsonPath
        {
            set { m_configurationsURIsJsonPath = value; }
            get { return m_configurationsURIsJsonPath; }
        }

        static string m_configurationsJsonPath;
        public static string ConfigurationsJsonPath
        {
            set { m_configurationsJsonPath = value; }
            get { return m_configurationsJsonPath; }
        }

        static InventorParameters m_inventorParameters;
        public static InventorParameters InventorParameters
        {
            set { m_inventorParameters = value; }
            get { return m_inventorParameters; }
        }


    }
}
