namespace qUTei
{
    partial class ChoiceForm
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
            this.InstructionsLabel = new System.Windows.Forms.Label();
            this.ChoicesComboBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InstructionsLabel
            // 
            this.InstructionsLabel.AutoSize = true;
            this.InstructionsLabel.Location = new System.Drawing.Point(13, 13);
            this.InstructionsLabel.Name = "InstructionsLabel";
            this.InstructionsLabel.Size = new System.Drawing.Size(168, 13);
            this.InstructionsLabel.TabIndex = 0;
            this.InstructionsLabel.Text = "Code Generated Instructions Here";
            // 
            // ChoicesComboBox
            // 
            this.ChoicesComboBox.FormattingEnabled = true;
            this.ChoicesComboBox.Location = new System.Drawing.Point(16, 29);
            this.ChoicesComboBox.Name = "ChoicesComboBox";
            this.ChoicesComboBox.Size = new System.Drawing.Size(121, 21);
            this.ChoicesComboBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 93);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ChoicesComboBox);
            this.Controls.Add(this.InstructionsLabel);
            this.Name = "ChoiceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "qUTei - Choice";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label InstructionsLabel;
        public System.Windows.Forms.ComboBox ChoicesComboBox;
        private System.Windows.Forms.Button button1;
    }
}