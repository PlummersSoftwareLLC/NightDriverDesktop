namespace NightDriver
{
    partial class StripDetails
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
            labelName = new Label();
            groupBox1 = new GroupBox();
            comboChannel = new ComboBox();
            textOffset = new TextBox();
            textHeight = new TextBox();
            labelBatchSize = new Label();
            textBatchSize = new TextBox();
            textWidth = new TextBox();
            labelOffset = new Label();
            textHostName = new TextBox();
            labelHeight = new Label();
            labelChannel = new Label();
            labelWidth = new Label();
            labelHostName = new Label();
            textName = new TextBox();
            groupBox2 = new GroupBox();
            checkSwapRedGreen = new CheckBox();
            checkReverse = new CheckBox();
            checkCompress = new CheckBox();
            buttonApply = new Button();
            buttonCancel = new Button();
            groupBox3 = new GroupBox();
            comboLocation = new ComboBox();
            label1 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new Point(55, 21);
            labelName.Name = "labelName";
            labelName.Size = new Size(39, 15);
            labelName.TabIndex = 0;
            labelName.Text = "Name";
            labelName.TextAlign = ContentAlignment.TopRight;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboChannel);
            groupBox1.Controls.Add(textOffset);
            groupBox1.Controls.Add(textHeight);
            groupBox1.Controls.Add(labelBatchSize);
            groupBox1.Controls.Add(textBatchSize);
            groupBox1.Controls.Add(textWidth);
            groupBox1.Controls.Add(labelOffset);
            groupBox1.Controls.Add(textHostName);
            groupBox1.Controls.Add(labelHeight);
            groupBox1.Controls.Add(labelChannel);
            groupBox1.Controls.Add(labelWidth);
            groupBox1.Controls.Add(labelHostName);
            groupBox1.Controls.Add(textName);
            groupBox1.Controls.Add(labelName);
            groupBox1.Location = new Point(12, 66);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(525, 138);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "LED  Details";
            // 
            // comboChannel
            // 
            comboChannel.FormattingEnabled = true;
            comboChannel.Items.AddRange(new object[] { "0 (All)", "1", "2", "3", "4", "5", "6", "7", "8" });
            comboChannel.Location = new Point(100, 106);
            comboChannel.Name = "comboChannel";
            comboChannel.Size = new Size(81, 23);
            comboChannel.TabIndex = 2;
            // 
            // textOffset
            // 
            textOffset.Location = new Point(438, 74);
            textOffset.Name = "textOffset";
            textOffset.Size = new Size(81, 23);
            textOffset.TabIndex = 1;
            // 
            // textHeight
            // 
            textHeight.Location = new Point(267, 74);
            textHeight.Name = "textHeight";
            textHeight.Size = new Size(81, 23);
            textHeight.TabIndex = 1;
            // 
            // labelBatchSize
            // 
            labelBatchSize.AutoSize = true;
            labelBatchSize.Location = new Point(201, 109);
            labelBatchSize.Name = "labelBatchSize";
            labelBatchSize.Size = new Size(60, 15);
            labelBatchSize.TabIndex = 0;
            labelBatchSize.Text = "Batch Size";
            labelBatchSize.TextAlign = ContentAlignment.TopRight;
            // 
            // textBatchSize
            // 
            textBatchSize.Location = new Point(267, 103);
            textBatchSize.Name = "textBatchSize";
            textBatchSize.Size = new Size(81, 23);
            textBatchSize.TabIndex = 1;
            // 
            // textWidth
            // 
            textWidth.Location = new Point(100, 74);
            textWidth.Name = "textWidth";
            textWidth.Size = new Size(81, 23);
            textWidth.TabIndex = 1;
            // 
            // labelOffset
            // 
            labelOffset.AutoSize = true;
            labelOffset.Location = new Point(393, 77);
            labelOffset.Name = "labelOffset";
            labelOffset.Size = new Size(39, 15);
            labelOffset.TabIndex = 0;
            labelOffset.Text = "Offset";
            labelOffset.TextAlign = ContentAlignment.TopRight;
            // 
            // textHostName
            // 
            textHostName.Location = new Point(100, 45);
            textHostName.Name = "textHostName";
            textHostName.Size = new Size(419, 23);
            textHostName.TabIndex = 1;
            // 
            // labelHeight
            // 
            labelHeight.AutoSize = true;
            labelHeight.Location = new Point(222, 77);
            labelHeight.Name = "labelHeight";
            labelHeight.Size = new Size(43, 15);
            labelHeight.TabIndex = 0;
            labelHeight.Text = "Height";
            labelHeight.TextAlign = ContentAlignment.TopRight;
            // 
            // labelChannel
            // 
            labelChannel.AutoSize = true;
            labelChannel.Location = new Point(43, 109);
            labelChannel.Name = "labelChannel";
            labelChannel.Size = new Size(51, 15);
            labelChannel.TabIndex = 0;
            labelChannel.Text = "Channel";
            labelChannel.TextAlign = ContentAlignment.TopRight;
            // 
            // labelWidth
            // 
            labelWidth.AutoSize = true;
            labelWidth.Location = new Point(55, 77);
            labelWidth.Name = "labelWidth";
            labelWidth.Size = new Size(39, 15);
            labelWidth.TabIndex = 0;
            labelWidth.Text = "Width";
            labelWidth.TextAlign = ContentAlignment.TopRight;
            // 
            // labelHostName
            // 
            labelHostName.AutoSize = true;
            labelHostName.Location = new Point(17, 48);
            labelHostName.Name = "labelHostName";
            labelHostName.Size = new Size(77, 15);
            labelHostName.TabIndex = 0;
            labelHostName.Text = "Hostname/IP";
            labelHostName.TextAlign = ContentAlignment.TopRight;
            // 
            // textName
            // 
            textName.Location = new Point(100, 16);
            textName.Name = "textName";
            textName.Size = new Size(419, 23);
            textName.TabIndex = 1;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(checkSwapRedGreen);
            groupBox2.Controls.Add(checkReverse);
            groupBox2.Controls.Add(checkCompress);
            groupBox2.Location = new Point(12, 210);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(525, 55);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "LED  Options";
            // 
            // checkSwapRedGreen
            // 
            checkSwapRedGreen.AutoSize = true;
            checkSwapRedGreen.Location = new Point(393, 22);
            checkSwapRedGreen.Name = "checkSwapRedGreen";
            checkSwapRedGreen.Size = new Size(113, 19);
            checkSwapRedGreen.TabIndex = 0;
            checkSwapRedGreen.Text = "Swap Red-Green";
            checkSwapRedGreen.UseVisualStyleBackColor = true;
            // 
            // checkReverse
            // 
            checkReverse.AutoSize = true;
            checkReverse.Location = new Point(243, 22);
            checkReverse.Name = "checkReverse";
            checkReverse.Size = new Size(66, 19);
            checkReverse.TabIndex = 0;
            checkReverse.Text = "Reverse";
            checkReverse.UseVisualStyleBackColor = true;
            // 
            // checkCompress
            // 
            checkCompress.AutoSize = true;
            checkCompress.Location = new Point(75, 22);
            checkCompress.Name = "checkCompress";
            checkCompress.Size = new Size(106, 19);
            checkCompress.TabIndex = 0;
            checkCompress.Text = "Compress Data";
            checkCompress.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            buttonApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonApply.Location = new Point(459, 278);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(75, 23);
            buttonApply.TabIndex = 2;
            buttonApply.Text = "Apply";
            buttonApply.UseVisualStyleBackColor = true;
            buttonApply.Click += buttonApply_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(378, 278);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(comboLocation);
            groupBox3.Controls.Add(label1);
            groupBox3.Location = new Point(12, 5);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(525, 55);
            groupBox3.TabIndex = 1;
            groupBox3.TabStop = false;
            groupBox3.Text = "LED Location";
            // 
            // comboLocation
            // 
            comboLocation.FormattingEnabled = true;
            comboLocation.Location = new Point(100, 22);
            comboLocation.Name = "comboLocation";
            comboLocation.Size = new Size(419, 23);
            comboLocation.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 26);
            label1.Name = "label1";
            label1.Size = new Size(53, 15);
            label1.TabIndex = 0;
            label1.Text = "Location";
            label1.TextAlign = ContentAlignment.TopRight;
            // 
            // StripDetails
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(546, 313);
            Controls.Add(buttonCancel);
            Controls.Add(buttonApply);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "StripDetails";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "LED Strip/Matrix Details";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label labelName;
        private GroupBox groupBox1;
        private TextBox textHostName;
        private Label labelHostName;
        private TextBox textName;
        private TextBox textWidth;
        private Label labelWidth;
        private TextBox textOffset;
        private TextBox textHeight;
        private Label labelOffset;
        private Label labelHeight;
        private GroupBox groupBox2;
        private Label labelBatchSize;
        private TextBox textBatchSize;
        private CheckBox checkCompress;
        private CheckBox checkSwapRedGreen;
        private CheckBox checkReverse;
        private ComboBox comboChannel;
        private Label labelChannel;
        private Button buttonApply;
        private Button buttonCancel;
        private GroupBox groupBox3;
        private ComboBox comboLocation;
        private Label label1;
    }
}