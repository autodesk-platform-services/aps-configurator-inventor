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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatalogBuilder.UI
{
    public partial class FrmCreateLibrary : Form
    {
        public CmdCreateLibrary m_createLibrary { get; }

        public FrmCreateLibrary(CmdCreateLibrary createLibrary)
        {
            InitializeComponent();
            m_createLibrary = createLibrary;
        }

        private void createLibrary_Click(object sender, EventArgs e)
        {
            m_createLibrary.ExecuteCommand();
        }

        private void close_Click(object sender, EventArgs e)
        {
            m_createLibrary.StopCommand();
        }
    }
}
