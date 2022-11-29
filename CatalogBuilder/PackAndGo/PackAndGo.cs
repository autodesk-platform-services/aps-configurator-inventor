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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace PackAndGoApp
{
    public static class PackAndGo
    {
        static void Main(string[] args)
        {
            //CreatePackAndGo(
            //    System.IO.Path.GetFullPath(
            //        "C:\\Users\\ricej\\OneDrive - autodesk\\Samples\\iLogic\\Wheel_2021\\WheelAssembly.iam"),
            //    System.IO.Path.GetFullPath(
            //        "C:\\Users\\ricej\\OneDrive - autodesk\\Samples\\iLogic\\Wheel_2021\\WheelAssembly\\PackNGo"));
        }

        public static string CreatePackAndGo(Inventor.Application application, 
                                                string zipDirectory)
        {
            //ApprenticeServerComponent apprenticeServer = new ApprenticeServerComponent();

            //PackAndGoLib.PackAndGoComponent oPacknGoComp = new PackAndGoLib.PackAndGoComponent();

            //if (!System.IO.Directory.Exists(outputDirectory))
            //{
            //    System.IO.Directory.CreateDirectory(outputDirectory);
            //}

            //PackAndGoLib.PackAndGo oPacknGo;
            //oPacknGo = oPacknGoComp.CreatePackAndGo(@assemblyPath, @outputDirectory);


            //// Set the design project. This defaults to the current active project.
            //oPacknGo.ProjectFile = apprenticeServer.DesignProjectManager.ActiveDesignProject.FullFileName;
            ////Globals.g_InventorApplication.DesignProjectManager.ActiveDesignProject.FullFileName;

            //var sRefFiles = new string[] { };
            //var sMissFiles = new object();

            //// Set the options
            //oPacknGo.SkipLibraries = false;
            //oPacknGo.SkipStyles = true;
            //oPacknGo.SkipTemplates = true;
            //oPacknGo.CollectWorkgroups = false;
            //oPacknGo.KeepFolderHierarchy = true;
            //oPacknGo.IncludeLinkedFiles = true;

            //// Get the referenced files
            //oPacknGo.SearchForReferencedFiles(out sRefFiles, out sMissFiles);

            //// Add the referenced files for package
            //oPacknGo.AddFilesToPackage(sRefFiles);

            //// Start the pack and go to create the package
            //oPacknGo.CreatePackage();

            //apprenticeServer = null;


            string fileLocationsDirectory = System.IO.Path.GetDirectoryName(application.FileLocations.FileLocationsFile);
            string workSpace = application.FileLocations.Workspace;
            string workspacePath = System.IO.Path.Combine(fileLocationsDirectory, workSpace);
            string[] directories = System.IO.Directory.GetDirectories(workspacePath, "*",
                System.IO.SearchOption.AllDirectories);
            
            System.IO.Directory.CreateDirectory(zipDirectory);

            //Now Create all of the directories
            foreach (string directory in directories)
            {
                if (directory.Contains("OldVersions"))
                {
                    continue;
                }
                string outpuDirectory = directory.Replace(workspacePath, zipDirectory);
                System.IO.Directory.CreateDirectory(outpuDirectory);
            }


            //Copy all the files & Replaces any files with the same name
            string[] files = System.IO.Directory.GetFiles(workspacePath, "*.*",
                System.IO.SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string newPath = file.Replace(workspacePath, zipDirectory);
                string directory = System.IO.Path.GetDirectoryName(newPath);
                if (System.IO.Directory.Exists(directory))
                    System.IO.File.Copy(file, newPath, true);
            }
                
           
            string zipPath = zipDirectory + ".zip";
            
            if (System.IO.File.Exists(zipPath))
                System.IO.File.Delete(zipPath);

            ZipFile.CreateFromDirectory(zipDirectory, zipPath);
            return zipPath;
        }
    }
}
