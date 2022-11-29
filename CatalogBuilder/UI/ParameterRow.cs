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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogBuilder.UI
{
    public class ParameterRow
    {
        [Browsable(false)]
        public InventorParameter inventorParameter { get; private set; }

        public ParameterRow(InventorParameter p)
        {
            inventorParameter = p;
        }

        private string m_inventorParameterName;
        [Browsable(false)]
        public string inventorParameterName
        {
            get { return m_inventorParameterName; }
            set { m_inventorParameterName = value; }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }


        public string UnitType
        {

            get { return inventorParameter.Unit;}
        }



        public string Equation
        {

            get { return inventorParameter.Value; }
        }


        string m_ValueList;
        public string ValueList
        {
            get { return m_ValueList; }
            set { m_ValueList = value; }
        }

        [Browsable(false)]
        public List<string> Values
        {
            get {
                    char[] delimiters = new[] { ',', ';' };  // List of your delimiters
                    string[] splittedArray = Array.ConvertAll(ValueList.Split(delimiters, StringSplitOptions.RemoveEmptyEntries), p => p.Trim());
                    return splittedArray.ToList();
                 }
        }
    }
}
