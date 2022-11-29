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
using Newtonsoft.Json;
using Shared;

namespace CatalogBuilder
{
    public partial class ProjectConfiguration
    {
        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("TopLevelAssembly")]
        public string TopLevelAssembly { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Config")]
        public InventorParameters Config { get; set; }
    }


}
