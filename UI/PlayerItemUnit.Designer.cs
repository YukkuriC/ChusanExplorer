namespace ChusanExplorer
{
    partial class PlayerItemUnit
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.radioChoose = new System.Windows.Forms.RadioButton();
            this.imgMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgMain)).BeginInit();
            this.SuspendLayout();
            // 
            // radioChoose
            // 
            this.radioChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioChoose.Location = new System.Drawing.Point(0, 100);
            this.radioChoose.Margin = new System.Windows.Forms.Padding(0);
            this.radioChoose.Name = "radioChoose";
            this.radioChoose.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.radioChoose.Size = new System.Drawing.Size(256, 16);
            this.radioChoose.TabIndex = 0;
            this.radioChoose.TabStop = true;
            this.radioChoose.Text = "李田所";
            this.radioChoose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioChoose.UseVisualStyleBackColor = true;
            this.radioChoose.CheckedChanged += new System.EventHandler(this.radioChoose_CheckedChanged);
            // 
            // imgMain
            // 
            this.imgMain.Location = new System.Drawing.Point(0, 0);
            this.imgMain.Margin = new System.Windows.Forms.Padding(0);
            this.imgMain.Name = "imgMain";
            this.imgMain.Size = new System.Drawing.Size(256, 100);
            this.imgMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgMain.TabIndex = 1;
            this.imgMain.TabStop = false;
            this.imgMain.Click += new System.EventHandler(this.imgMain_Click);
            // 
            // PlayerItemUnit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.imgMain);
            this.Controls.Add(this.radioChoose);
            this.Name = "PlayerItemUnit";
            this.Size = new System.Drawing.Size(256, 116);
            ((System.ComponentModel.ISupportInitialize)(this.imgMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioChoose;
        private System.Windows.Forms.PictureBox imgMain;
    }
}
