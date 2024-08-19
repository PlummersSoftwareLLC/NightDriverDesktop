namespace NightDriver
{
    partial class DontShowAgainMessage
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
            textMain = new TextBox();
            checkDontShowAgain = new CheckBox();
            buttonOK = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // textMain
            // 
            textMain.BorderStyle = BorderStyle.None;
            textMain.Location = new Point(12, 7);
            textMain.Multiline = true;
            textMain.Name = "textMain";
            textMain.Size = new Size(421, 94);
            textMain.TabIndex = 0;
            // 
            // checkDontShowAgain
            // 
            checkDontShowAgain.AutoSize = true;
            checkDontShowAgain.Location = new Point(11, 120);
            checkDontShowAgain.Name = "checkDontShowAgain";
            checkDontShowAgain.Size = new Size(199, 19);
            checkDontShowAgain.TabIndex = 1;
            checkDontShowAgain.Text = "Do not show this message again.";
            checkDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(358, 117);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(75, 23);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(277, 117);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // DontShowAgainMessage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(445, 147);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(checkDontShowAgain);
            Controls.Add(textMain);
            Name = "DontShowAgainMessage";
            Text = "DontShowAgainMessage";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textMain;
        private CheckBox checkDontShowAgain;
        private Button buttonOK;
        private Button buttonCancel;
    }
}