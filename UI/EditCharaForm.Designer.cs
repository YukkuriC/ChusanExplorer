namespace ChusanExplorer
{
    partial class EditCharaForm
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
            this.btnAddChara = new System.Windows.Forms.Button();
            this.btnRemoveChara = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.illustSwitcher = new System.Windows.Forms.TrackBar();
            this.imgCharaIcon = new System.Windows.Forms.PictureBox();
            this.grpLevel = new System.Windows.Forms.GroupBox();
            this.trackLevel = new System.Windows.Forms.TrackBar();
            this.grpMaxLevel = new System.Windows.Forms.GroupBox();
            this.trackMaxLevel = new System.Windows.Forms.TrackBar();
            this.grpExp = new System.Windows.Forms.GroupBox();
            this.trackExp = new System.Windows.Forms.TrackBar();
            this.panelEditProfile = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.inputPlayCount = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.illustSwitcher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCharaIcon)).BeginInit();
            this.grpLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackLevel)).BeginInit();
            this.grpMaxLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackMaxLevel)).BeginInit();
            this.grpExp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackExp)).BeginInit();
            this.panelEditProfile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputPlayCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddChara
            // 
            this.btnAddChara.Location = new System.Drawing.Point(12, 41);
            this.btnAddChara.Name = "btnAddChara";
            this.btnAddChara.Size = new System.Drawing.Size(75, 23);
            this.btnAddChara.TabIndex = 0;
            this.btnAddChara.Text = "无中生有";
            this.btnAddChara.UseVisualStyleBackColor = true;
            this.btnAddChara.Click += new System.EventHandler(this.btnAddChara_Click);
            // 
            // btnRemoveChara
            // 
            this.btnRemoveChara.Location = new System.Drawing.Point(12, 70);
            this.btnRemoveChara.Name = "btnRemoveChara";
            this.btnRemoveChara.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveChara.TabIndex = 1;
            this.btnRemoveChara.Text = "删除角色";
            this.btnRemoveChara.UseVisualStyleBackColor = true;
            this.btnRemoveChara.Click += new System.EventHandler(this.btnRemoveChara_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(12, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存并退出";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.illustSwitcher);
            this.groupBox1.Location = new System.Drawing.Point(12, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(75, 41);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "立绘";
            // 
            // illustSwitcher
            // 
            this.illustSwitcher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.illustSwitcher.Location = new System.Drawing.Point(3, 17);
            this.illustSwitcher.Maximum = 0;
            this.illustSwitcher.Name = "illustSwitcher";
            this.illustSwitcher.Size = new System.Drawing.Size(69, 21);
            this.illustSwitcher.TabIndex = 0;
            this.illustSwitcher.ValueChanged += new System.EventHandler(this.illustSwitcher_ValueChanged);
            // 
            // imgCharaIcon
            // 
            this.imgCharaIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgCharaIcon.Location = new System.Drawing.Point(93, 12);
            this.imgCharaIcon.Name = "imgCharaIcon";
            this.imgCharaIcon.Size = new System.Drawing.Size(128, 128);
            this.imgCharaIcon.TabIndex = 4;
            this.imgCharaIcon.TabStop = false;
            // 
            // grpLevel
            // 
            this.grpLevel.Controls.Add(this.trackLevel);
            this.grpLevel.Location = new System.Drawing.Point(13, 49);
            this.grpLevel.Name = "grpLevel";
            this.grpLevel.Size = new System.Drawing.Size(209, 40);
            this.grpLevel.TabIndex = 5;
            this.grpLevel.TabStop = false;
            this.grpLevel.Text = "角色等级";
            // 
            // trackLevel
            // 
            this.trackLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackLevel.LargeChange = 1;
            this.trackLevel.Location = new System.Drawing.Point(3, 17);
            this.trackLevel.Maximum = 100;
            this.trackLevel.Minimum = 1;
            this.trackLevel.Name = "trackLevel";
            this.trackLevel.Size = new System.Drawing.Size(203, 20);
            this.trackLevel.TabIndex = 0;
            this.trackLevel.Value = 100;
            this.trackLevel.ValueChanged += new System.EventHandler(this.trackLevel_ValueChanged);
            // 
            // grpMaxLevel
            // 
            this.grpMaxLevel.Controls.Add(this.trackMaxLevel);
            this.grpMaxLevel.Location = new System.Drawing.Point(13, 3);
            this.grpMaxLevel.Name = "grpMaxLevel";
            this.grpMaxLevel.Size = new System.Drawing.Size(209, 40);
            this.grpMaxLevel.TabIndex = 6;
            this.grpMaxLevel.TabStop = false;
            this.grpMaxLevel.Text = "最大等级";
            // 
            // trackMaxLevel
            // 
            this.trackMaxLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackMaxLevel.Location = new System.Drawing.Point(3, 17);
            this.trackMaxLevel.Maximum = 100;
            this.trackMaxLevel.Minimum = 15;
            this.trackMaxLevel.Name = "trackMaxLevel";
            this.trackMaxLevel.Size = new System.Drawing.Size(203, 20);
            this.trackMaxLevel.SmallChange = 5;
            this.trackMaxLevel.TabIndex = 0;
            this.trackMaxLevel.TickFrequency = 5;
            this.trackMaxLevel.Value = 100;
            this.trackMaxLevel.ValueChanged += new System.EventHandler(this.trackMaxLevel_ValueChanged);
            // 
            // grpExp
            // 
            this.grpExp.Controls.Add(this.trackExp);
            this.grpExp.Location = new System.Drawing.Point(13, 95);
            this.grpExp.Name = "grpExp";
            this.grpExp.Size = new System.Drawing.Size(209, 40);
            this.grpExp.TabIndex = 6;
            this.grpExp.TabStop = false;
            this.grpExp.Text = "当前等级经验";
            // 
            // trackExp
            // 
            this.trackExp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackExp.Location = new System.Drawing.Point(3, 17);
            this.trackExp.Maximum = 0;
            this.trackExp.Name = "trackExp";
            this.trackExp.Size = new System.Drawing.Size(203, 20);
            this.trackExp.TabIndex = 0;
            this.trackExp.ValueChanged += new System.EventHandler(this.trackExp_ValueChanged);
            // 
            // panelEditProfile
            // 
            this.panelEditProfile.Controls.Add(this.groupBox2);
            this.panelEditProfile.Controls.Add(this.grpExp);
            this.panelEditProfile.Controls.Add(this.grpLevel);
            this.panelEditProfile.Controls.Add(this.grpMaxLevel);
            this.panelEditProfile.Location = new System.Drawing.Point(-1, 143);
            this.panelEditProfile.Name = "panelEditProfile";
            this.panelEditProfile.Size = new System.Drawing.Size(236, 186);
            this.panelEditProfile.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.inputPlayCount);
            this.groupBox2.Location = new System.Drawing.Point(13, 141);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(209, 40);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "游玩次数";
            // 
            // inputPlayCount
            // 
            this.inputPlayCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputPlayCount.Location = new System.Drawing.Point(3, 17);
            this.inputPlayCount.Maximum = new decimal(new int[] {
            1145141919,
            0,
            0,
            0});
            this.inputPlayCount.Name = "inputPlayCount";
            this.inputPlayCount.Size = new System.Drawing.Size(203, 21);
            this.inputPlayCount.TabIndex = 0;
            this.inputPlayCount.ValueChanged += new System.EventHandler(this.inputPlayCount_ValueChanged);
            // 
            // EditCharaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 333);
            this.Controls.Add(this.imgCharaIcon);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnRemoveChara);
            this.Controls.Add(this.btnAddChara);
            this.Controls.Add(this.panelEditProfile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditCharaForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "编辑角色";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.illustSwitcher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCharaIcon)).EndInit();
            this.grpLevel.ResumeLayout(false);
            this.grpLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackLevel)).EndInit();
            this.grpMaxLevel.ResumeLayout(false);
            this.grpMaxLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackMaxLevel)).EndInit();
            this.grpExp.ResumeLayout(false);
            this.grpExp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackExp)).EndInit();
            this.panelEditProfile.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.inputPlayCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAddChara;
        private System.Windows.Forms.Button btnRemoveChara;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox imgCharaIcon;
        private System.Windows.Forms.TrackBar illustSwitcher;
        private System.Windows.Forms.GroupBox grpLevel;
        private System.Windows.Forms.TrackBar trackLevel;
        private System.Windows.Forms.GroupBox grpMaxLevel;
        private System.Windows.Forms.TrackBar trackMaxLevel;
        private System.Windows.Forms.GroupBox grpExp;
        private System.Windows.Forms.TrackBar trackExp;
        private System.Windows.Forms.Panel panelEditProfile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown inputPlayCount;
    }
}