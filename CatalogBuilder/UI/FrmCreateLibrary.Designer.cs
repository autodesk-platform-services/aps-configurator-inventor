namespace CatalogBuilder.UI
{
    partial class FrmCreateLibrary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCreateLibrary));
            this.ConfigurationsDataGrid = new System.Windows.Forms.DataGridView();
            this.close = new System.Windows.Forms.Button();
            this.createLibrary = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationsDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ConfigurationsDataGrid
            // 
            this.ConfigurationsDataGrid.AllowUserToAddRows = false;
            this.ConfigurationsDataGrid.AllowUserToDeleteRows = false;
            this.ConfigurationsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConfigurationsDataGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.ConfigurationsDataGrid.Location = new System.Drawing.Point(0, 0);
            this.ConfigurationsDataGrid.Name = "ConfigurationsDataGrid";
            this.ConfigurationsDataGrid.RowHeadersWidth = 102;
            this.ConfigurationsDataGrid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ConfigurationsDataGrid.Size = new System.Drawing.Size(926, 386);
            this.ConfigurationsDataGrid.TabIndex = 5;
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.close.Location = new System.Drawing.Point(844, 392);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 11;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // createLibrary
            // 
            this.createLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.createLibrary.Location = new System.Drawing.Point(723, 392);
            this.createLibrary.Name = "createLibrary";
            this.createLibrary.Size = new System.Drawing.Size(109, 23);
            this.createLibrary.TabIndex = 10;
            this.createLibrary.Text = "Create Library";
            this.createLibrary.UseVisualStyleBackColor = true;
            this.createLibrary.Click += new System.EventHandler(this.createLibrary_Click);
            // 
            // FrmCreateLibrary
            // 
            this.AcceptButton = this.createLibrary;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.close;
            this.ClientSize = new System.Drawing.Size(926, 422);
            this.Controls.Add(this.close);
            this.Controls.Add(this.createLibrary);
            this.Controls.Add(this.ConfigurationsDataGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "FrmCreateLibrary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmCreateLibrary";
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationsDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView ConfigurationsDataGrid;
        public System.Windows.Forms.Button close;
        public System.Windows.Forms.Button createLibrary;
    }
}