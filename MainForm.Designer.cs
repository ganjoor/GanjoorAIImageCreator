namespace GanjoorAIImageCreator
{
    partial class MainForm
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
            label1 = new Label();
            txtPoetId = new TextBox();
            txtAPIKey = new TextBox();
            btnStart = new Button();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(54, 57);
            label1.Name = "label1";
            label1.Size = new Size(93, 32);
            label1.TabIndex = 0;
            label1.Text = "Poet Id:";
            // 
            // txtPoetId
            // 
            txtPoetId.Location = new Point(153, 57);
            txtPoetId.Name = "txtPoetId";
            txtPoetId.Size = new Size(200, 39);
            txtPoetId.TabIndex = 1;
            txtPoetId.Text = "198";
            // 
            // txtAPIKey
            // 
            txtAPIKey.Location = new Point(153, 126);
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.Size = new Size(841, 39);
            txtAPIKey.TabIndex = 2;
            txtAPIKey.Text = "API Key";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(153, 193);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(280, 46);
            btnStart.TabIndex = 3;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(69, 277);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(78, 32);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Ready";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1094, 564);
            Controls.Add(lblStatus);
            Controls.Add(btnStart);
            Controls.Add(txtAPIKey);
            Controls.Add(txtPoetId);
            Controls.Add(label1);
            Name = "MainForm";
            Text = "Ganjoor AI Image Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtPoetId;
        private TextBox txtAPIKey;
        private Button btnStart;
        private Label lblStatus;
    }
}
