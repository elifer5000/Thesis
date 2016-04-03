namespace Stereo3D
{
    partial class MainForm
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
            this.buttonLoadLeftImg = new System.Windows.Forms.Button();
            this.buttonLoadRightImg = new System.Windows.Forms.Button();
            this.buttonCreateMesh = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.checkDebugMode = new System.Windows.Forms.CheckBox();
            this.buttonDebugNext = new System.Windows.Forms.Button();
            this.labelCoordsLeft = new System.Windows.Forms.Label();
            this.labelCoordsRight = new System.Windows.Forms.Label();
            this.textBoxPatternSz = new System.Windows.Forms.TextBox();
            this.textBoxStep = new System.Windows.Forms.TextBox();
            this.textBoxRangeX = new System.Windows.Forms.TextBox();
            this.textBoxRangeY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelPercentage = new System.Windows.Forms.Label();
            this.pictureBoxRightImg = new Stereo3D.PictureBoxExtended();
            this.pictureBoxLeftImg = new Stereo3D.PictureBoxExtended();
            this.groupBoxDebug = new System.Windows.Forms.GroupBox();
            this.textBoxDisparity = new System.Windows.Forms.TextBox();
            this.labelDisparity = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftImg)).BeginInit();
            this.groupBoxDebug.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonLoadLeftImg
            // 
            this.buttonLoadLeftImg.Location = new System.Drawing.Point(23, 12);
            this.buttonLoadLeftImg.Name = "buttonLoadLeftImg";
            this.buttonLoadLeftImg.Size = new System.Drawing.Size(140, 31);
            this.buttonLoadLeftImg.TabIndex = 0;
            this.buttonLoadLeftImg.Text = "Load Left Image";
            this.buttonLoadLeftImg.UseVisualStyleBackColor = true;
            this.buttonLoadLeftImg.Click += new System.EventHandler(this.buttonLoadLeftImg_Click);
            // 
            // buttonLoadRightImg
            // 
            this.buttonLoadRightImg.Location = new System.Drawing.Point(436, 12);
            this.buttonLoadRightImg.Name = "buttonLoadRightImg";
            this.buttonLoadRightImg.Size = new System.Drawing.Size(140, 31);
            this.buttonLoadRightImg.TabIndex = 0;
            this.buttonLoadRightImg.Text = "Load Right Image";
            this.buttonLoadRightImg.UseVisualStyleBackColor = true;
            this.buttonLoadRightImg.Click += new System.EventHandler(this.buttonLoadRightImg_Click);
            // 
            // buttonCreateMesh
            // 
            this.buttonCreateMesh.Location = new System.Drawing.Point(710, 497);
            this.buttonCreateMesh.Name = "buttonCreateMesh";
            this.buttonCreateMesh.Size = new System.Drawing.Size(140, 31);
            this.buttonCreateMesh.TabIndex = 0;
            this.buttonCreateMesh.Text = "Create 3D Model";
            this.buttonCreateMesh.UseVisualStyleBackColor = true;
            this.buttonCreateMesh.Click += new System.EventHandler(this.buttonCreateMesh_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(445, 502);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(247, 26);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 3;
            // 
            // checkDebugMode
            // 
            this.checkDebugMode.AutoSize = true;
            this.checkDebugMode.Location = new System.Drawing.Point(712, 536);
            this.checkDebugMode.Name = "checkDebugMode";
            this.checkDebugMode.Size = new System.Drawing.Size(87, 17);
            this.checkDebugMode.TabIndex = 4;
            this.checkDebugMode.Text = "Debug mode";
            this.checkDebugMode.UseVisualStyleBackColor = true;
            // 
            // buttonDebugNext
            // 
            this.buttonDebugNext.Enabled = false;
            this.buttonDebugNext.Location = new System.Drawing.Point(799, 534);
            this.buttonDebugNext.Name = "buttonDebugNext";
            this.buttonDebugNext.Size = new System.Drawing.Size(51, 19);
            this.buttonDebugNext.TabIndex = 0;
            this.buttonDebugNext.Text = "Next";
            this.buttonDebugNext.UseVisualStyleBackColor = true;
            this.buttonDebugNext.Click += new System.EventHandler(this.buttonDebugNext_Click);
            // 
            // labelCoordsLeft
            // 
            this.labelCoordsLeft.AutoSize = true;
            this.labelCoordsLeft.Location = new System.Drawing.Point(24, 454);
            this.labelCoordsLeft.Name = "labelCoordsLeft";
            this.labelCoordsLeft.Size = new System.Drawing.Size(55, 13);
            this.labelCoordsLeft.TabIndex = 5;
            this.labelCoordsLeft.Text = "Coords:    ";
            // 
            // labelCoordsRight
            // 
            this.labelCoordsRight.AutoSize = true;
            this.labelCoordsRight.Location = new System.Drawing.Point(433, 454);
            this.labelCoordsRight.Name = "labelCoordsRight";
            this.labelCoordsRight.Size = new System.Drawing.Size(55, 13);
            this.labelCoordsRight.TabIndex = 5;
            this.labelCoordsRight.Text = "Coords:    ";
            // 
            // textBoxPatternSz
            // 
            this.textBoxPatternSz.Location = new System.Drawing.Point(89, 30);
            this.textBoxPatternSz.Name = "textBoxPatternSz";
            this.textBoxPatternSz.Size = new System.Drawing.Size(53, 20);
            this.textBoxPatternSz.TabIndex = 6;
            // 
            // textBoxStep
            // 
            this.textBoxStep.Location = new System.Drawing.Point(89, 56);
            this.textBoxStep.Name = "textBoxStep";
            this.textBoxStep.Size = new System.Drawing.Size(53, 20);
            this.textBoxStep.TabIndex = 6;
            // 
            // textBoxRangeX
            // 
            this.textBoxRangeX.Location = new System.Drawing.Point(89, 82);
            this.textBoxRangeX.Name = "textBoxRangeX";
            this.textBoxRangeX.Size = new System.Drawing.Size(53, 20);
            this.textBoxRangeX.TabIndex = 6;
            // 
            // textBoxRangeY
            // 
            this.textBoxRangeY.Location = new System.Drawing.Point(89, 108);
            this.textBoxRangeY.Name = "textBoxRangeY";
            this.textBoxRangeY.Size = new System.Drawing.Size(53, 20);
            this.textBoxRangeY.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Pattern size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Step";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Search range x";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Search range y";
            // 
            // labelPercentage
            // 
            this.labelPercentage.AutoSize = true;
            this.labelPercentage.Location = new System.Drawing.Point(552, 531);
            this.labelPercentage.Name = "labelPercentage";
            this.labelPercentage.Size = new System.Drawing.Size(24, 13);
            this.labelPercentage.TabIndex = 7;
            this.labelPercentage.Text = "0 %";
            // 
            // pictureBoxRightImg
            // 
            this.pictureBoxRightImg.Location = new System.Drawing.Point(436, 61);
            this.pictureBoxRightImg.Name = "pictureBoxRightImg";
            this.pictureBoxRightImg.Size = new System.Drawing.Size(341, 376);
            this.pictureBoxRightImg.TabIndex = 2;
            this.pictureBoxRightImg.TabStop = false;
            // 
            // pictureBoxLeftImg
            // 
            this.pictureBoxLeftImg.Location = new System.Drawing.Point(23, 61);
            this.pictureBoxLeftImg.Name = "pictureBoxLeftImg";
            this.pictureBoxLeftImg.Size = new System.Drawing.Size(341, 376);
            this.pictureBoxLeftImg.TabIndex = 2;
            this.pictureBoxLeftImg.TabStop = false;
            // 
            // groupBoxDebug
            // 
            this.groupBoxDebug.Controls.Add(this.textBoxDisparity);
            this.groupBoxDebug.Controls.Add(this.labelDisparity);
            this.groupBoxDebug.Location = new System.Drawing.Point(799, 278);
            this.groupBoxDebug.Name = "groupBoxDebug";
            this.groupBoxDebug.Size = new System.Drawing.Size(150, 159);
            this.groupBoxDebug.TabIndex = 8;
            this.groupBoxDebug.TabStop = false;
            this.groupBoxDebug.Text = "Debug info";
            // 
            // textBoxDisparity
            // 
            this.textBoxDisparity.Enabled = false;
            this.textBoxDisparity.Location = new System.Drawing.Point(70, 18);
            this.textBoxDisparity.Name = "textBoxDisparity";
            this.textBoxDisparity.Size = new System.Drawing.Size(48, 20);
            this.textBoxDisparity.TabIndex = 1;
            // 
            // labelDisparity
            // 
            this.labelDisparity.AutoSize = true;
            this.labelDisparity.Location = new System.Drawing.Point(8, 22);
            this.labelDisparity.Name = "labelDisparity";
            this.labelDisparity.Size = new System.Drawing.Size(47, 13);
            this.labelDisparity.TabIndex = 0;
            this.labelDisparity.Text = "Disparity";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxPatternSz);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxStep);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxRangeX);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxRangeY);
            this.groupBox1.Location = new System.Drawing.Point(23, 470);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 148);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 630);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxDebug);
            this.Controls.Add(this.labelPercentage);
            this.Controls.Add(this.labelCoordsRight);
            this.Controls.Add(this.labelCoordsLeft);
            this.Controls.Add(this.checkDebugMode);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.pictureBoxRightImg);
            this.Controls.Add(this.pictureBoxLeftImg);
            this.Controls.Add(this.buttonDebugNext);
            this.Controls.Add(this.buttonCreateMesh);
            this.Controls.Add(this.buttonLoadRightImg);
            this.Controls.Add(this.buttonLoadLeftImg);
            this.Name = "MainForm";
            this.Text = "Stereo3D";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRightImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeftImg)).EndInit();
            this.groupBoxDebug.ResumeLayout(false);
            this.groupBoxDebug.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLoadLeftImg;
        private System.Windows.Forms.Button buttonLoadRightImg;
        private PictureBoxExtended pictureBoxLeftImg;
        private PictureBoxExtended pictureBoxRightImg;
        private System.Windows.Forms.Button buttonCreateMesh;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox checkDebugMode;
        private System.Windows.Forms.Button buttonDebugNext;
        private System.Windows.Forms.Label labelCoordsLeft;
        private System.Windows.Forms.Label labelCoordsRight;
        private System.Windows.Forms.TextBox textBoxPatternSz;
        private System.Windows.Forms.TextBox textBoxStep;
        private System.Windows.Forms.TextBox textBoxRangeX;
        private System.Windows.Forms.TextBox textBoxRangeY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelPercentage;
        private System.Windows.Forms.GroupBox groupBoxDebug;
        private System.Windows.Forms.TextBox textBoxDisparity;
        private System.Windows.Forms.Label labelDisparity;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

