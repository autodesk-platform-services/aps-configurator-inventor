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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatalogBuilder.UI
{
    public partial class FrmReadParameters : Form
    {
        public CmdReadParameters m_readParameters { get; }

        public FrmReadParameters(CmdReadParameters cmdReadParams)
        {
            InitializeComponent();
            m_readParameters = cmdReadParams;
        }

        private void FrmReadParameters_Load(object sender, EventArgs e)
        {
            txtJsonDirectory.Text = Globals.JsonDirectory;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void ParameterDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        

        }

        public void ParameterDataGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //m_cmdReadParameters.
            //ParameterDataGrid[e.ColumnIndex, e.RowIndex].Value;
        }
        public void ParameterDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            m_readParameters.UpdateConfigurations();
            //DataGridViewColumn dataGridViewColumn = ParameterDataGrid.Columns[e.ColumnIndex];
            //DataGridViewCell dataGridViewCell = ParameterDataGrid[e.ColumnIndex, e.RowIndex];

            //Document doc = Globals.InventorApplication.ActiveDocument;
            //Parameters parameters;
            //switch (doc.DocumentType)
            //{
            //    case DocumentTypeEnum.kPartDocumentObject:
            //        parameters = ((PartDocument)doc).ComponentDefinition.Parameters;
            //        break;
            //    case DocumentTypeEnum.kAssemblyDocumentObject:
            //        parameters = ((AssemblyDocument)doc).ComponentDefinition.Parameters;
            //        break;
            //    default:
            //        return;
            //}
            //parameters.UserParameters[]
        }

        private void jsonDirectoryBrowserDialog_Load(object sender, EventArgs e)
        {
            
        }

        private void jsonDirectory_Click(object sender, EventArgs e)
        {
            SelectjsonDirectory(jsonDirectoryBrowserDialog);
        }
        private void SelectjsonDirectory(FolderBrowserDialog externaliLogicFolderDialog)
        {
            jsonDirectoryBrowserDialog.SelectedPath = Globals.JsonDirectory;
            if (this.jsonDirectoryBrowserDialog.ShowDialog() == DialogResult.OK)
                txtJsonDirectory.Text = jsonDirectoryBrowserDialog.SelectedPath;
        }

        private void txtJsonDirectory_TextChanged(object sender, EventArgs e)
        {
            Globals.JsonDirectory = txtJsonDirectory.Text;
        }

        private void createJson_Click(object sender, EventArgs e)
        {
            m_readParameters.ExecuteCommand();
        }

        private void close_Click(object sender, EventArgs e)
        {
            m_readParameters.StopCommand();
        }
    }
}
