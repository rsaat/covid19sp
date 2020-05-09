namespace ExtractCovid19Sp
{
    partial class frmExportCovidToCsv
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
            this.cmdExportCsv = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdExportCsv
            // 
            this.cmdExportCsv.Location = new System.Drawing.Point(87, 57);
            this.cmdExportCsv.Name = "cmdExportCsv";
            this.cmdExportCsv.Size = new System.Drawing.Size(117, 40);
            this.cmdExportCsv.TabIndex = 0;
            this.cmdExportCsv.Text = "Exporta CSV";
            this.cmdExportCsv.UseVisualStyleBackColor = true;
            this.cmdExportCsv.Click += new System.EventHandler(this.cmdExportCsv_Click);
            // 
            // frmExportCovidToCsv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 160);
            this.Controls.Add(this.cmdExportCsv);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmExportCovidToCsv";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exporta Dados Covid para CSV";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdExportCsv;
    }
}

