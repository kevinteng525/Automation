namespace S1Tool
{
    partial class WebConfigDebugOn
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
            this.DebugOnButton = new System.Windows.Forms.Button();
            this.DebugOffButton = new System.Windows.Forms.Button();
            this.textBoxDebugOn = new System.Windows.Forms.TextBox();
            this.textBoxDebugOff = new System.Windows.Forms.TextBox();
            this._chkListBoxWebConfigs = new System.Windows.Forms.CheckedListBox();
            this._chkBoxCheckAll = new System.Windows.Forms.CheckBox();
            this._panelMain = new System.Windows.Forms.TableLayoutPanel();
            this._panelConfig = new System.Windows.Forms.TableLayoutPanel();
            this._panelBusy = new System.Windows.Forms.Panel();
            this._picBusy = new System.Windows.Forms.PictureBox();
            this._labelBusyText = new System.Windows.Forms.Label();
            this._panelMain.SuspendLayout();
            this._panelConfig.SuspendLayout();
            this._panelBusy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picBusy)).BeginInit();
            this.SuspendLayout();
            // 
            // DebugOnButton
            // 
            this.DebugOnButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DebugOnButton.Location = new System.Drawing.Point(114, 68);
            this.DebugOnButton.Name = "DebugOnButton";
            this.DebugOnButton.Size = new System.Drawing.Size(75, 23);
            this.DebugOnButton.TabIndex = 0;
            this.DebugOnButton.Text = "Debug &ON";
            this.DebugOnButton.UseVisualStyleBackColor = true;
            this.DebugOnButton.Click += new System.EventHandler(this.DebugOnButton_Click);
            // 
            // DebugOffButton
            // 
            this.DebugOffButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DebugOffButton.Location = new System.Drawing.Point(306, 68);
            this.DebugOffButton.Name = "DebugOffButton";
            this.DebugOffButton.Size = new System.Drawing.Size(75, 23);
            this.DebugOffButton.TabIndex = 1;
            this.DebugOffButton.Text = "Debug O&FF";
            this.DebugOffButton.UseVisualStyleBackColor = true;
            this.DebugOffButton.Click += new System.EventHandler(this.DebugOffButton_Click);
            // 
            // textBoxDebugOn
            // 
            this.textBoxDebugOn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDebugOn.Location = new System.Drawing.Point(3, 3);
            this.textBoxDebugOn.Multiline = true;
            this.textBoxDebugOn.Name = "textBoxDebugOn";
            this.textBoxDebugOn.ReadOnly = true;
            this.textBoxDebugOn.Size = new System.Drawing.Size(186, 53);
            this.textBoxDebugOn.TabIndex = 2;
            this.textBoxDebugOn.Text = "<customErrors mode=\"Off\" />\r\n<SafeMode CallStack=\"true\" />\r\n<compilation debug=\"t" +
    "rue\" />";
            // 
            // textBoxDebugOff
            // 
            this.textBoxDebugOff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDebugOff.Location = new System.Drawing.Point(195, 3);
            this.textBoxDebugOff.Multiline = true;
            this.textBoxDebugOff.Name = "textBoxDebugOff";
            this.textBoxDebugOff.ReadOnly = true;
            this.textBoxDebugOff.Size = new System.Drawing.Size(186, 53);
            this.textBoxDebugOff.TabIndex = 3;
            this.textBoxDebugOff.Text = "<customErrors mode=\"On\" />\r\n<SafeMode CallStack=\"false\" />\r\n<compilation debug=\"f" +
    "alse\" />";
            // 
            // _chkListBoxWebConfigs
            // 
            this._chkListBoxWebConfigs.CheckOnClick = true;
            this._chkListBoxWebConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._chkListBoxWebConfigs.FormattingEnabled = true;
            this._chkListBoxWebConfigs.HorizontalScrollbar = true;
            this._chkListBoxWebConfigs.Location = new System.Drawing.Point(5, 30);
            this._chkListBoxWebConfigs.Name = "_chkListBoxWebConfigs";
            this._chkListBoxWebConfigs.Size = new System.Drawing.Size(384, 140);
            this._chkListBoxWebConfigs.TabIndex = 4;
            this._chkListBoxWebConfigs.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._chkListBoxWebConfigs_ItemCheck);
            // 
            // _chkBoxCheckAll
            // 
            this._chkBoxCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._chkBoxCheckAll.AutoSize = true;
            this._chkBoxCheckAll.Location = new System.Drawing.Point(5, 7);
            this._chkBoxCheckAll.Name = "_chkBoxCheckAll";
            this._chkBoxCheckAll.Size = new System.Drawing.Size(71, 17);
            this._chkBoxCheckAll.TabIndex = 5;
            this._chkBoxCheckAll.Text = "Check &All";
            this._chkBoxCheckAll.UseVisualStyleBackColor = true;
            this._chkBoxCheckAll.CheckedChanged += new System.EventHandler(this._chkListBoxWebConfigs_CheckedChanged);
            // 
            // _panelMain
            // 
            this._panelMain.ColumnCount = 1;
            this._panelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._panelMain.Controls.Add(this._chkBoxCheckAll, 0, 0);
            this._panelMain.Controls.Add(this._chkListBoxWebConfigs, 0, 1);
            this._panelMain.Controls.Add(this._panelConfig, 0, 2);
            this._panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelMain.Location = new System.Drawing.Point(0, 0);
            this._panelMain.Name = "_panelMain";
            this._panelMain.Padding = new System.Windows.Forms.Padding(2);
            this._panelMain.RowCount = 3;
            this._panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this._panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this._panelMain.Size = new System.Drawing.Size(394, 275);
            this._panelMain.TabIndex = 6;
            this._panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this._panelMain_Paint);
            // 
            // _panelConfig
            // 
            this._panelConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panelConfig.ColumnCount = 2;
            this._panelConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._panelConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._panelConfig.Controls.Add(this.textBoxDebugOn, 0, 0);
            this._panelConfig.Controls.Add(this.DebugOffButton, 1, 1);
            this._panelConfig.Controls.Add(this.textBoxDebugOff, 1, 0);
            this._panelConfig.Controls.Add(this.DebugOnButton, 0, 1);
            this._panelConfig.Location = new System.Drawing.Point(5, 176);
            this._panelConfig.Name = "_panelConfig";
            this._panelConfig.RowCount = 2;
            this._panelConfig.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._panelConfig.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this._panelConfig.Size = new System.Drawing.Size(384, 94);
            this._panelConfig.TabIndex = 6;
            // 
            // _panelBusy
            // 
            this._panelBusy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panelBusy.Controls.Add(this._labelBusyText);
            this._panelBusy.Controls.Add(this._picBusy);
            this._panelBusy.Location = new System.Drawing.Point(103, 72);
            this._panelBusy.Name = "_panelBusy";
            this._panelBusy.Size = new System.Drawing.Size(180, 90);
            this._panelBusy.TabIndex = 7;
            // 
            // _picBusy
            // 
            this._picBusy.Image = global::WebConfigDebugOn.Properties.Resources._32;
            this._picBusy.Location = new System.Drawing.Point(63, 4);
            this._picBusy.Name = "_picBusy";
            this._picBusy.Size = new System.Drawing.Size(60, 60);
            this._picBusy.TabIndex = 0;
            this._picBusy.TabStop = false;
            // 
            // _labelBusyText
            // 
            this._labelBusyText.AutoSize = true;
            this._labelBusyText.Location = new System.Drawing.Point(64, 69);
            this._labelBusyText.Name = "_labelBusyText";
            this._labelBusyText.Size = new System.Drawing.Size(57, 13);
            this._labelBusyText.TabIndex = 1;
            this._labelBusyText.Text = "Loading ...";
            // 
            // WebConfigDebugOn
            // 
            this.AcceptButton = this.DebugOnButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 275);
            this.Controls.Add(this._panelMain);
            this.Controls.Add(this._panelBusy);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "WebConfigDebugOn";
            this.Text = "Web.Config Debug On/Off";
            this._panelMain.ResumeLayout(false);
            this._panelMain.PerformLayout();
            this._panelConfig.ResumeLayout(false);
            this._panelConfig.PerformLayout();
            this._panelBusy.ResumeLayout(false);
            this._panelBusy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picBusy)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button DebugOnButton;
        private System.Windows.Forms.Button DebugOffButton;
        private System.Windows.Forms.TextBox textBoxDebugOn;
        private System.Windows.Forms.TextBox textBoxDebugOff;
        private System.Windows.Forms.CheckedListBox _chkListBoxWebConfigs;
        private System.Windows.Forms.CheckBox _chkBoxCheckAll;
        private System.Windows.Forms.TableLayoutPanel _panelMain;
        private System.Windows.Forms.TableLayoutPanel _panelConfig;
        private System.Windows.Forms.Panel _panelBusy;
        private System.Windows.Forms.PictureBox _picBusy;
        private System.Windows.Forms.Label _labelBusyText;
    }
}

