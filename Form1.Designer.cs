namespace SimplePaint
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            lblAppName = new Label();
            groupShape = new GroupBox();
            btnCircle = new Button();
            btnRectangle = new Button();
            btnLine = new Button();
            groupColor = new GroupBox();
            cmbColor = new ComboBox();
            groupLineSize = new GroupBox();
            trbLineWidth = new TrackBar();
            btnOpenFile = new Button();
            btnSaveFile = new Button();
            picCanvas = new PictureBox();
            groupShape.SuspendLayout();
            groupColor.SuspendLayout();
            groupLineSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trbLineWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCanvas).BeginInit();
            SuspendLayout();
            // 
            // lblAppName
            // 
            lblAppName.AutoSize = true;
            lblAppName.Font = new Font("맑은 고딕", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblAppName.ForeColor = Color.Blue;
            lblAppName.Location = new Point(12, 9);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new Size(230, 47);
            lblAppName.TabIndex = 0;
            lblAppName.Text = "Simple Paint";
            // 
            // groupShape
            // 
            groupShape.Controls.Add(btnCircle);
            groupShape.Controls.Add(btnRectangle);
            groupShape.Controls.Add(btnLine);
            groupShape.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            groupShape.Location = new Point(9, 70);
            groupShape.Name = "groupShape";
            groupShape.Size = new Size(200, 82);
            groupShape.TabIndex = 1;
            groupShape.TabStop = false;
            groupShape.Text = "도형 선택";
            // 
            // btnCircle
            // 
            btnCircle.Image = (Image)resources.GetObject("btnCircle.Image");
            btnCircle.ImageAlign = ContentAlignment.TopCenter;
            btnCircle.Location = new Point(135, 22);
            btnCircle.Name = "btnCircle";
            btnCircle.Size = new Size(61, 54);
            btnCircle.TabIndex = 2;
            btnCircle.Text = "원";
            btnCircle.TextAlign = ContentAlignment.BottomCenter;
            btnCircle.UseVisualStyleBackColor = true;
            btnCircle.Click += btnCircle_Click;
            // 
            // btnRectangle
            // 
            btnRectangle.Image = (Image)resources.GetObject("btnRectangle.Image");
            btnRectangle.ImageAlign = ContentAlignment.TopCenter;
            btnRectangle.Location = new Point(69, 22);
            btnRectangle.Name = "btnRectangle";
            btnRectangle.Size = new Size(61, 54);
            btnRectangle.TabIndex = 1;
            btnRectangle.Text = "사각형";
            btnRectangle.TextAlign = ContentAlignment.BottomCenter;
            btnRectangle.UseVisualStyleBackColor = true;
            btnRectangle.Click += btnRectangle_Click;
            // 
            // btnLine
            // 
            btnLine.Image = (Image)resources.GetObject("btnLine.Image");
            btnLine.ImageAlign = ContentAlignment.TopCenter;
            btnLine.Location = new Point(4, 22);
            btnLine.Name = "btnLine";
            btnLine.Size = new Size(61, 54);
            btnLine.TabIndex = 0;
            btnLine.Text = "직선";
            btnLine.TextAlign = ContentAlignment.BottomCenter;
            btnLine.UseVisualStyleBackColor = true;
            btnLine.Click += btnLine_Click;
            // 
            // groupColor
            // 
            groupColor.Controls.Add(cmbColor);
            groupColor.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            groupColor.Location = new Point(227, 70);
            groupColor.Name = "groupColor";
            groupColor.Size = new Size(122, 82);
            groupColor.TabIndex = 2;
            groupColor.TabStop = false;
            groupColor.Text = "색 선택";
            // 
            // cmbColor
            // 
            cmbColor.FormattingEnabled = true;
            cmbColor.Items.AddRange(new object[] { "Black(검정)", "Red(빨강)", "Blue(파랑)", "Green(초록)" });
            cmbColor.Location = new Point(13, 36);
            cmbColor.Name = "cmbColor";
            cmbColor.Size = new Size(96, 23);
            cmbColor.TabIndex = 0;
            cmbColor.SelectedIndexChanged += cmbColor_SelectedIndexChanged;
            // 
            // groupLineSize
            // 
            groupLineSize.Controls.Add(trbLineWidth);
            groupLineSize.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            groupLineSize.Location = new Point(374, 70);
            groupLineSize.Name = "groupLineSize";
            groupLineSize.Size = new Size(159, 82);
            groupLineSize.TabIndex = 2;
            groupLineSize.TabStop = false;
            groupLineSize.Text = "선 두께";
            // 
            // trbLineWidth
            // 
            trbLineWidth.Location = new Point(6, 31);
            trbLineWidth.Name = "trbLineWidth";
            trbLineWidth.Size = new Size(147, 45);
            trbLineWidth.TabIndex = 0;
            trbLineWidth.Scroll += trbLineWidth_Scroll;
            trbLineWidth.ValueChanged += trbLineWidth_ValueChanged;
            // 
            // btnOpenFile
            // 
            btnOpenFile.BackColor = Color.FromArgb(255, 255, 128);
            btnOpenFile.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            btnOpenFile.Location = new Point(549, 87);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(73, 59);
            btnOpenFile.TabIndex = 3;
            btnOpenFile.Text = "열기";
            btnOpenFile.UseVisualStyleBackColor = false;
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // btnSaveFile
            // 
            btnSaveFile.BackColor = Color.FromArgb(192, 255, 255);
            btnSaveFile.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            btnSaveFile.Location = new Point(633, 87);
            btnSaveFile.Name = "btnSaveFile";
            btnSaveFile.Size = new Size(73, 59);
            btnSaveFile.TabIndex = 4;
            btnSaveFile.Text = "저장";
            btnSaveFile.UseVisualStyleBackColor = false;
            btnSaveFile.Click += btnSaveFile_Click;
            // 
            // picCanvas
            // 
            picCanvas.Location = new Point(10, 160);
            picCanvas.Name = "picCanvas";
            picCanvas.Size = new Size(698, 287);
            picCanvas.TabIndex = 5;
            picCanvas.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 459);
            Controls.Add(picCanvas);
            Controls.Add(btnSaveFile);
            Controls.Add(btnOpenFile);
            Controls.Add(groupColor);
            Controls.Add(groupLineSize);
            Controls.Add(groupShape);
            Controls.Add(lblAppName);
            Name = "Form1";
            Text = "Simple Paint v1.0";
            groupShape.ResumeLayout(false);
            groupColor.ResumeLayout(false);
            groupLineSize.ResumeLayout(false);
            groupLineSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trbLineWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCanvas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAppName;
        private GroupBox groupShape;
        private Button btnCircle;
        private Button btnRectangle;
        private Button btnLine;
        private GroupBox groupColor;
        private ComboBox cmbColor;
        private GroupBox groupLineSize;
        private TrackBar trbLineWidth;
        private Button btnOpenFile;
        private Button btnSaveFile;
        private PictureBox picCanvas;
    }
}
