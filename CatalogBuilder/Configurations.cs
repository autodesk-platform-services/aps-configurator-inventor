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
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogBuilder.UI
{
    static class Configurations
    {
        public static Dictionary<string, HashSet<string>> GetParameterValues(List<ParameterRow> parameterRows)
        {
            Dictionary<string, HashSet<string>> results = new Dictionary<string, HashSet<string>>();
            foreach (ParameterRow pr in parameterRows)
            {
                var hashSet = new HashSet<string>(pr.Values);
                results.Add(pr.Name, hashSet);
            }
            return results;
        }

        public static IEnumerable<string> GetAllPossibleValues(Dictionary<string, HashSet<string>> Data)
        {
            return Data.Select(x => x.Value)
                .CartesianProduct()
                .Select(x => x.Aggregate((a, b) => a + "," + b)).ToList();
        }


        static IEnumerable<IEnumerable<T>> CartesianProduct<T>
        (this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct =
              new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
              emptyProduct,
              (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }

        public static List<ProjectConfiguration> GetConfigurations(InventorParameters inventorParameters, List<ParameterRow> parameterRows, List<string> allPossibleValues)
        {
            if (parameterRows.Count != allPossibleValues[1].Split(',').Length)
            {
                throw new System.InvalidOperationException("parameterRows count does not match allPossibleValues length ");
            }

            List<string> parameterNames = new List<string>();
            foreach (ParameterRow pr in parameterRows)
            {
                parameterNames.Add(pr.inventorParameterName);
            }

            List<ProjectConfiguration> result = new List<ProjectConfiguration>();
            foreach (string pv in allPossibleValues)
            {
                ProjectConfiguration projectConfiguration = GetConfiguration(inventorParameters, parameterNames, pv);
                result.Add(projectConfiguration);
            }
                

            return result;
        }

        public static ProjectConfiguration GetConfiguration(InventorParameters inventorParameters,List<string> parameterNames,  string possibleValue)
        {
            ProjectConfiguration configuration = new ProjectConfiguration();
            configuration.Name = Globals.ProjectName;
            configuration.TopLevelAssembly = Globals.TopLevelAssembly;
            configuration.Url = Globals.ProjectUrl;

            configuration.Config = new InventorParameters();
            string[] values = possibleValue.Split(',');

            for (int i = 0; i < parameterNames.Count; i++)
            {
                string name = parameterNames[i];
                string value = values[i];
                InventorParameter ip = CopyInventorParameter(inventorParameters[name], value);
                configuration.Config.Add(name, ip);
            }
            return configuration;
        }

        public static ProjectConfiguration GetConfiguration(InventorParameters inventorParameters)
        {
            ProjectConfiguration configuration = new ProjectConfiguration();
            configuration.Name = Globals.ProjectName;
            configuration.TopLevelAssembly = Globals.TopLevelAssembly;
            configuration.Url = Globals.ProjectUrl;

            configuration.Config = CopyInventorParameters(inventorParameters);

            return configuration;
        }

        private static InventorParameters CopyInventorParameters(InventorParameters inventorParameters)
        {
            InventorParameters ips = new InventorParameters();
            foreach (KeyValuePair<string, InventorParameter> entry in inventorParameters)
            {
                ips.Add(entry.Key, CopyInventorParameter(entry.Value));
            }

            return ips;
        }

        private static InventorParameter CopyInventorParameter(InventorParameter inventorParameter, string value = null)
        {
            InventorParameter ip = new InventorParameter();
            ip.Label = inventorParameter.Label;
            ip.ReadOnly = inventorParameter.ReadOnly;
            ip.Unit = inventorParameter.Unit;
            
            if (value == null)
                value = inventorParameter.Value;

            ip.Value = value;
                
            ip.Values = inventorParameter.Values;
            return ip;
        }
    }
}

