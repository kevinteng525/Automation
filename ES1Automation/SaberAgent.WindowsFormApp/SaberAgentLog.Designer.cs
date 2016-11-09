namespace SaberAgent.WindowsFormApp
{
    partial class ProgressDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDetail));
            this.SaberAgentTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgressMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ProgressLogGridView = new System.Windows.Forms.DataGridView();
            this.TrayMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressLogGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SaberAgentTrayIcon
            // 
            this.SaberAgentTrayIcon.ContextMenuStrip = this.TrayMenu;
            this.SaberAgentTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("SaberAgentTrayIcon.Icon")));
            this.SaberAgentTrayIcon.Text = "Galaxy Saber Agent";
            this.SaberAgentTrayIcon.Visible = true;
            // 
            // TrayMenu
            // 
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExitMenu,
            this.ProgressMenu,
            this.HelpMenu});
            this.TrayMenu.Name = "TrayMenu";
            this.TrayMenu.Size = new System.Drawing.Size(117, 70);
            // 
            // ExitMenu
            // 
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.Size = new System.Drawing.Size(116, 22);
            this.ExitMenu.Text = "Exit";
            this.ExitMenu.ToolTipText = "Exit the agent";
            this.ExitMenu.Click += new System.EventHandler(this.ExitMenu_Click);
            // 
            // ProgressMenu
            // 
            this.ProgressMenu.Name = "ProgressMenu";
            this.ProgressMenu.Size = new System.Drawing.Size(116, 22);
            this.ProgressMenu.Text = "Progress";
            this.ProgressMenu.ToolTipText = "Show the progress detail";
            this.ProgressMenu.Click += new System.EventHandler(this.ProgressMenu_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(116, 22);
            this.HelpMenu.Text = "Help";
            this.HelpMenu.ToolTipText = "Help and About";
            this.HelpMenu.Click += new System.EventHandler(this.HelpMenu_Click);
            // 
            // ProgressLogGridView
            // 
            this.ProgressLogGridView.AllowUserToAddRows = false;
            this.ProgressLogGridView.AllowUserToDeleteRows = false;
            this.ProgressLogGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.ProgressLogGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ProgressLogGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressLogGridView.Location = new System.Drawing.Point(0, 0);
            this.ProgressLogGridView.Name = "ProgressLogGridView";
            this.ProgressLogGridView.Size = new System.Drawing.Size(492, 573);
            this.ProgressLogGridView.TabIndex = 1;
            // 
            // ProgressDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(492, 573);
            this.Controls.Add(this.ProgressLogGridView);
            this.Location = new System.Drawing.Point(300, 300);
            this.MinimumSize = new System.Drawing.Size(400, 600);
            this.Name = "ProgressDetail";
            this.Text = "Saber Agent";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.TrayMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ProgressLogGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon SaberAgentTrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitMenu;
        private System.Windows.Forms.ToolStripMenuItem ProgressMenu;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.DataGridView ProgressLogGridView;
    }
}

