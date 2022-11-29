/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Autodesk Design Automation team for Inventor
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
using System.Linq;

namespace Shared
{
    public class ParameterNormalizer
    {
        public ParameterNormalizer()
        {

        }

        public Dictionary<string, string> NormailzeParameters(InventorParameters inventorParameters)
        {
            Dictionary<string, string> parametersDictionary = new Dictionary<string, string>();
            char quotationMark = '"';

            foreach (KeyValuePair<string, InventorParameter> entry in inventorParameters)
            {
                InventorParameter ip = entry.Value;
                string value = ip.Value;
                if (value.Contains(quotationMark))
                    value = value.Trim(quotationMark);
                //if (!string.IsNullOrEmpty(ip.Unit) && value.EndsWith(ip.Unit))
                //    value = value.Remove(ip.Unit.Length);
                value = value.Trim();
                parametersDictionary.Add(entry.Key, value);
            }
            return parametersDictionary;
        }

    }
}