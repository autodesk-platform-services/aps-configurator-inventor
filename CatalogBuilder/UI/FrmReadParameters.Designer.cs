namespace CatalogBuilder.UI
{
    partial class FrmReadParameters
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmReadParameters));
            this.ParameterDataGrid = new System.Windows.Forms.DataGridView();
            this.txtTotalConfigurations = new System.Windows.Forms.Label();
            this.txtJsonDirectory = new System.Windows.Forms.TextBox();
            this.createJson = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.jsonDirectoryBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.ParameterDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ParameterDataGrid
            // 
            this.ParameterDataGrid.AllowUserToAddRows = false;
            this.ParameterDataGrid.AllowUserToDeleteRows = false;
            this.ParameterDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ParameterDataGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.ParameterDataGrid.Location = new System.Drawing.Point(0, 0);
            this.ParameterDataGrid.Name = "ParameterDataGrid";
            this.ParameterDataGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ParameterDataGrid.Size = new System.Drawing.Size(800, 386);
            this.ParameterDataGrid.TabIndex = 4;
            this.ParameterDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParameterDataGrid_CellContentClick);
            this.ParameterDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParameterDataGrid_CellValueChanged);
            // 
            // txtTotalConfigurations
            // 
            this.txtTotalConfigurations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalConfigurations.AutoSize = true;
            this.txtTotalConfigurations.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotalConfigurations.Location = new System.Drawing.Point(632, 389);
            this.txtTotalConfigurations.Name = "txtTotalConfigurations";
            this.txtTotalConfigurations.Size = new System.Drawing.Size(144, 24);
            this.txtTotalConfigurations.TabIndex = 5;
            this.txtTotalConfigurations.Text = "Configurations";
            this.txtTotalConfigurations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtJsonDirectory
            // 
            this.txtJsonDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJsonDirectory.Location = new System.Drawing.Point(12, 420);
            this.txtJsonDirectory.Name = "txtJsonDirectory";
            this.txtJsonDirectory.Size = new System.Drawing.Size(574, 20);
            this.txtJsonDirectory.TabIndex = 7;
            this.txtJsonDirectory.TextChanged += new System.EventHandler(this.txtJsonDirectory_TextChanged);
            // 
            // createJson
            // 
            this.createJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.createJson.Location = new System.Drawing.Point(592, 418);
            this.createJson.Name = "createJson";
            this.createJson.Size = new System.Drawing.Size(109, 23);
            this.createJson.TabIndex = 8;
            this.createJson.Text = "Create Json Files";
            this.createJson.UseVisualStyleBackColor = true;
            this.createJson.Click += new System.EventHandler(this.createJson_Click);
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(713, 418);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 9;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // jsonDirectoryBrowserDialog
            // 
            this.jsonDirectoryBrowserDialog.SelectedPath = global::CatalogBuilder.Properties.Settings.Default.SelectJsonDirectory;
            this.jsonDirectoryBrowserDialog.ShowNewFolderButton = global::CatalogBuilder.Properties.Settings.Default.createNewFolder;
            this.jsonDirectoryBrowserDialog.HelpRequest += new System.EventHandler(this.jsonDirectoryBrowserDialog_Load);
            // 
            // FrmReadParameters
            // 
            this.AcceptButton = this.createJson;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.close);
            this.Controls.Add(this.createJson);
            this.Controls.Add(this.txtJsonDirectory);
            this.Controls.Add(this.txtTotalConfigurations);
            this.Controls.Add(this.ParameterDataGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmReadParameters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parameters";
            this.Load += new System.EventHandler(this.FrmReadParameters_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ParameterDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.DataGridView ParameterDataGrid;
        public System.Windows.Forms.Label txtTotalConfigurations;
        private System.Windows.Forms.TextBox txtJsonDirectory;
        public System.Windows.Forms.Button createJson;
        public System.Windows.Forms.Button close;
        public System.Windows.Forms.FolderBrowserDialog jsonDirectoryBrowserDialog;
    }
}