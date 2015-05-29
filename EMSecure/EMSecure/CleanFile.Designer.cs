namespace EMSecure
{
    partial class CleanFile
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CleanFile));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.directoryTree = new System.Windows.Forms.TreeView();
            this.directoryIcons = new System.Windows.Forms.ImageList(this.components);
            this.filesList = new System.Windows.Forms.ListView();
            this.filesIcons = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::EMSecure.Properties.Resources.panel6;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Location = new System.Drawing.Point(4, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(49, 50);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(59, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 19);
            this.label1.TabIndex = 10;
            this.label1.Text = "label1";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DodgerBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(576, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(55, 42);
            this.button1.TabIndex = 11;
            this.button1.Text = "配置选项";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // directoryTree
            // 
            this.directoryTree.BackColor = System.Drawing.Color.White;
            this.directoryTree.ImageIndex = 0;
            this.directoryTree.ImageList = this.directoryIcons;
            this.directoryTree.Location = new System.Drawing.Point(4, 68);
            this.directoryTree.Name = "directoryTree";
            this.directoryTree.SelectedImageIndex = 0;
            this.directoryTree.Size = new System.Drawing.Size(188, 280);
            this.directoryTree.TabIndex = 13;
            this.directoryTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.directoryTree_BeforeExpand);
            this.directoryTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.directoryTree_AfterExpand);
            this.directoryTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.directoryTree_AfterSelect);
            // 
            // directoryIcons
            // 
            this.directoryIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("directoryIcons.ImageStream")));
            this.directoryIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.directoryIcons.Images.SetKeyName(0, "Computer.ico");
            this.directoryIcons.Images.SetKeyName(1, "Closed Folder.ico");
            this.directoryIcons.Images.SetKeyName(2, "Open Folder.ico");
            this.directoryIcons.Images.SetKeyName(3, "fixed drive.ico");
            this.directoryIcons.Images.SetKeyName(4, "My Documents.ico");
            // 
            // filesList
            // 
            this.filesList.BackColor = System.Drawing.Color.White;
            this.filesList.Location = new System.Drawing.Point(198, 68);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(491, 280);
            this.filesList.SmallImageList = this.filesIcons;
            this.filesList.TabIndex = 14;
            this.filesList.UseCompatibleStateImageBehavior = false;
            this.filesList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.filesList_MouseClick);
            // 
            // filesIcons
            // 
            this.filesIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("filesIcons.ImageStream")));
            this.filesIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.filesIcons.Images.SetKeyName(0, "Closed Folder.ico");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.deleteFileToolStripMenuItem,
            this.cleanFileToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 70);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.openFileToolStripMenuItem.Text = "打开文件";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // deleteFileToolStripMenuItem
            // 
            this.deleteFileToolStripMenuItem.Name = "deleteFileToolStripMenuItem";
            this.deleteFileToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.deleteFileToolStripMenuItem.Text = "删除文件";
            this.deleteFileToolStripMenuItem.Click += new System.EventHandler(this.deleteFileToolStripMenuItem_Click);
            // 
            // cleanFileToolStripMenuItem
            // 
            this.cleanFileToolStripMenuItem.Name = "cleanFileToolStripMenuItem";
            this.cleanFileToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.cleanFileToolStripMenuItem.Text = "粉碎文件";
            this.cleanFileToolStripMenuItem.Click += new System.EventHandler(this.cleanFileToolStripMenuItem_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(637, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(41, 24);
            this.comboBox1.TabIndex = 15;
            // 
            // CleanFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(716, 360);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.filesList);
            this.Controls.Add(this.directoryTree);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CleanFile";
            this.Text = "CleanFile";
            this.Load += new System.EventHandler(this.CleanFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TreeView directoryTree;
        private System.Windows.Forms.ImageList directoryIcons;
        private System.Windows.Forms.ListView filesList;
        private System.Windows.Forms.ImageList filesIcons;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanFileToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}