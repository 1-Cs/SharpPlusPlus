namespace WindowsFormsApplication
{
    partial class BaseForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.baseTree = new System.Windows.Forms.TreeView();
            this.keyValue = new System.Windows.Forms.ListView();
            this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hexWindow = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.filename = new System.Windows.Forms.TextBox();
            this.OpenFile = new System.Windows.Forms.Button();
            this.picture = new System.Windows.Forms.PictureBox();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            this.SuspendLayout();
            // 
            // baseTree
            // 
            this.baseTree.Location = new System.Drawing.Point(2, 80);
            this.baseTree.Name = "baseTree";
            this.baseTree.Size = new System.Drawing.Size(355, 479);
            this.baseTree.TabIndex = 0;
            this.baseTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // keyValue
            // 
            this.keyValue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameHeader,
            this.Value});
            this.keyValue.FullRowSelect = true;
            this.keyValue.GridLines = true;
            this.keyValue.Location = new System.Drawing.Point(447, 80);
            this.keyValue.Name = "keyValue";
            this.keyValue.Size = new System.Drawing.Size(687, 204);
            this.keyValue.TabIndex = 1;
            this.keyValue.UseCompatibleStateImageBehavior = false;
            this.keyValue.View = System.Windows.Forms.View.Details;
            this.keyValue.Visible = false;
            // 
            // NameHeader
            // 
            this.NameHeader.Text = "Name";
            this.NameHeader.Width = 100;
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.Width = 100;
            // 
            // hexWindow
            // 
            this.hexWindow.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexWindow.Location = new System.Drawing.Point(377, 80);
            this.hexWindow.Multiline = true;
            this.hexWindow.Name = "hexWindow";
            this.hexWindow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.hexWindow.Size = new System.Drawing.Size(757, 479);
            this.hexWindow.TabIndex = 2;
            this.hexWindow.Visible = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // filename
            // 
            this.filename.Font = new System.Drawing.Font("Open Sans", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filename.Location = new System.Drawing.Point(2, 33);
            this.filename.Name = "filename";
            this.filename.Size = new System.Drawing.Size(617, 28);
            this.filename.TabIndex = 3;
            this.filename.Text = "Filename";
            this.filename.TextChanged += new System.EventHandler(this.filename_TextChanged);
            // 
            // OpenFile
            // 
            this.OpenFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenFile.Location = new System.Drawing.Point(641, 33);
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(107, 23);
            this.OpenFile.TabIndex = 4;
            this.OpenFile.Text = "Durchsuchen";
            this.OpenFile.UseVisualStyleBackColor = true;
            this.OpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // picture
            // 
            this.picture.Location = new System.Drawing.Point(389, 80);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(271, 204);
            this.picture.TabIndex = 5;
            this.picture.TabStop = false;
            // 
            // webBrowser
            // 
            this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser.Location = new System.Drawing.Point(377, 80);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(820, 479);
            this.webBrowser.TabIndex = 7;
            this.webBrowser.Visible = false;
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 558);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.picture);
            this.Controls.Add(this.OpenFile);
            this.Controls.Add(this.filename);
            this.Controls.Add(this.hexWindow);
            this.Controls.Add(this.keyValue);
            this.Controls.Add(this.baseTree);
            this.Name = "BaseForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView baseTree;
        private System.Windows.Forms.ListView keyValue;
        private System.Windows.Forms.TextBox hexWindow;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox filename;
        private System.Windows.Forms.Button OpenFile;
        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ColumnHeader NameHeader;
        private System.Windows.Forms.ColumnHeader Value;
    }
}

