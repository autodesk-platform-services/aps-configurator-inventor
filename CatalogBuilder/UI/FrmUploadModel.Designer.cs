namespace CatalogBuilder
{
    partial class FrmUploadModel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUploadModel));
            this.uploadPicture = new System.Windows.Forms.PictureBox();
            this.BtnUpload = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.uploadPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // uploadPicture
            // 
            this.uploadPicture.Dock = System.Windows.Forms.DockStyle.Top;
            this.uploadPicture.Location = new System.Drawing.Point(0, 0);
            this.uploadPicture.Name = "uploadPicture";
            this.uploadPicture.Padding = new System.Windows.Forms.Padding(11);
            this.uploadPicture.Size = new System.Drawing.Size(522, 500);
            this.uploadPicture.TabIndex = 0;
            this.uploadPicture.TabStop = false;
            this.uploadPicture.WaitOnLoad = true;
            // 
            // BtnUpload
            // 
            this.BtnUpload.Location = new System.Drawing.Point(343, 518);
            this.BtnUpload.Name = "BtnUpload";
            this.BtnUpload.Size = new System.Drawing.Size(75, 23);
            this.BtnUpload.TabIndex = 1;
            this.BtnUpload.Text = "Upload";
            this.BtnUpload.UseVisualStyleBackColor = true;
            this.BtnUpload.Click += new System.EventHandler(this.Upload_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnClose.Location = new System.Drawing.Point(437, 518);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 2;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // FrmUploadModel
            // 
            this.AcceptButton = this.BtnUpload;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnClose;
            this.ClientSize = new System.Drawing.Size(522, 551);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnUpload);
            this.Controls.Add(this.uploadPicture);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUploadModel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Upload Model";
            this.Load += new System.EventHandler(this.FrmUploadModel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.uploadPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox uploadPicture;
        public System.Windows.Forms.Button BtnUpload;
        public System.Windows.Forms.Button BtnClose;
    }
}