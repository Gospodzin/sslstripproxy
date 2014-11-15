namespace SSLStripProxy
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
            this.requestsListBox = new System.Windows.Forms.ListBox();
            this.loggingFilterGroupBox = new System.Windows.Forms.GroupBox();
            this.setFilterButton = new System.Windows.Forms.Button();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.removeContentTypeButton = new System.Windows.Forms.Button();
            this.addContentTypeButton = new System.Windows.Forms.Button();
            this.contentTypeComboBox = new System.Windows.Forms.ComboBox();
            this.contentTypeListBox = new System.Windows.Forms.ListBox();
            this.removeMethodButton = new System.Windows.Forms.Button();
            this.addMethodButton = new System.Windows.Forms.Button();
            this.methodComboBox = new System.Windows.Forms.ComboBox();
            this.methodListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nextMatchButton = new System.Windows.Forms.Button();
            this.matchTextBox = new System.Windows.Forms.TextBox();
            this.initMatchingButton = new System.Windows.Forms.Button();
            this.regexTextBox = new System.Windows.Forms.TextBox();
            this.groupNrTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.loggingFilterGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // requestsListBox
            // 
            this.requestsListBox.DisplayMember = "RequestList";
            this.requestsListBox.FormattingEnabled = true;
            this.requestsListBox.Location = new System.Drawing.Point(9, 19);
            this.requestsListBox.Name = "requestsListBox";
            this.requestsListBox.Size = new System.Drawing.Size(749, 329);
            this.requestsListBox.TabIndex = 1;
            this.requestsListBox.ValueMember = "RequestList";
            // 
            // loggingFilterGroupBox
            // 
            this.loggingFilterGroupBox.Controls.Add(this.setFilterButton);
            this.loggingFilterGroupBox.Controls.Add(this.hostTextBox);
            this.loggingFilterGroupBox.Controls.Add(this.hostLabel);
            this.loggingFilterGroupBox.Controls.Add(this.removeContentTypeButton);
            this.loggingFilterGroupBox.Controls.Add(this.addContentTypeButton);
            this.loggingFilterGroupBox.Controls.Add(this.contentTypeComboBox);
            this.loggingFilterGroupBox.Controls.Add(this.contentTypeListBox);
            this.loggingFilterGroupBox.Controls.Add(this.removeMethodButton);
            this.loggingFilterGroupBox.Controls.Add(this.addMethodButton);
            this.loggingFilterGroupBox.Controls.Add(this.methodComboBox);
            this.loggingFilterGroupBox.Controls.Add(this.methodListBox);
            this.loggingFilterGroupBox.Location = new System.Drawing.Point(12, 9);
            this.loggingFilterGroupBox.Name = "loggingFilterGroupBox";
            this.loggingFilterGroupBox.Size = new System.Drawing.Size(366, 222);
            this.loggingFilterGroupBox.TabIndex = 8;
            this.loggingFilterGroupBox.TabStop = false;
            this.loggingFilterGroupBox.Text = "Loggig Filter";
            // 
            // setFilterButton
            // 
            this.setFilterButton.Location = new System.Drawing.Point(198, 25);
            this.setFilterButton.Name = "setFilterButton";
            this.setFilterButton.Size = new System.Drawing.Size(162, 23);
            this.setFilterButton.TabIndex = 19;
            this.setFilterButton.Text = "Set Filter";
            this.setFilterButton.UseVisualStyleBackColor = true;
            this.setFilterButton.Click += new System.EventHandler(this.setFilterButton_Click);
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(38, 26);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(136, 20);
            this.hostTextBox.TabIndex = 17;
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Location = new System.Drawing.Point(6, 30);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(29, 13);
            this.hostLabel.TabIndex = 18;
            this.hostLabel.Text = "Host";
            // 
            // removeContentTypeButton
            // 
            this.removeContentTypeButton.Location = new System.Drawing.Point(285, 90);
            this.removeContentTypeButton.Name = "removeContentTypeButton";
            this.removeContentTypeButton.Size = new System.Drawing.Size(75, 23);
            this.removeContentTypeButton.TabIndex = 15;
            this.removeContentTypeButton.Text = "Remove";
            this.removeContentTypeButton.UseVisualStyleBackColor = true;
            this.removeContentTypeButton.Click += new System.EventHandler(this.removeContentTypeButton_Click);
            // 
            // addContentTypeButton
            // 
            this.addContentTypeButton.Location = new System.Drawing.Point(198, 90);
            this.addContentTypeButton.Name = "addContentTypeButton";
            this.addContentTypeButton.Size = new System.Drawing.Size(75, 23);
            this.addContentTypeButton.TabIndex = 14;
            this.addContentTypeButton.Text = "Add";
            this.addContentTypeButton.UseVisualStyleBackColor = true;
            this.addContentTypeButton.Click += new System.EventHandler(this.addContentTypeButton_Click);
            // 
            // contentTypeComboBox
            // 
            this.contentTypeComboBox.FormattingEnabled = true;
            this.contentTypeComboBox.Items.AddRange(new object[] {
            "application/javascript",
            "application/octet-stream",
            "application/x-javascript",
            "application/x-shockwave-flash",
            "audio/mpeg",
            "image/gif",
            "image/jpeg",
            "image/png",
            "image/x-icon",
            "text/css",
            "text/html",
            "text/javascript",
            "text/plain",
            "text/xml",
            "*"});
            this.contentTypeComboBox.Location = new System.Drawing.Point(198, 63);
            this.contentTypeComboBox.Name = "contentTypeComboBox";
            this.contentTypeComboBox.Size = new System.Drawing.Size(162, 21);
            this.contentTypeComboBox.TabIndex = 13;
            // 
            // contentTypeListBox
            // 
            this.contentTypeListBox.FormattingEnabled = true;
            this.contentTypeListBox.Location = new System.Drawing.Point(198, 119);
            this.contentTypeListBox.Name = "contentTypeListBox";
            this.contentTypeListBox.Size = new System.Drawing.Size(162, 95);
            this.contentTypeListBox.TabIndex = 12;
            // 
            // removeMethodButton
            // 
            this.removeMethodButton.Location = new System.Drawing.Point(99, 90);
            this.removeMethodButton.Name = "removeMethodButton";
            this.removeMethodButton.Size = new System.Drawing.Size(75, 23);
            this.removeMethodButton.TabIndex = 11;
            this.removeMethodButton.Text = "Remove";
            this.removeMethodButton.UseVisualStyleBackColor = true;
            this.removeMethodButton.Click += new System.EventHandler(this.removeMethodButton_Click);
            // 
            // addMethodButton
            // 
            this.addMethodButton.Location = new System.Drawing.Point(12, 90);
            this.addMethodButton.Name = "addMethodButton";
            this.addMethodButton.Size = new System.Drawing.Size(75, 23);
            this.addMethodButton.TabIndex = 10;
            this.addMethodButton.Text = "Add";
            this.addMethodButton.UseVisualStyleBackColor = true;
            this.addMethodButton.Click += new System.EventHandler(this.addMethodButton_Click);
            // 
            // methodComboBox
            // 
            this.methodComboBox.FormattingEnabled = true;
            this.methodComboBox.Items.AddRange(new object[] {
            "GET",
            "POST",
            "*"});
            this.methodComboBox.Location = new System.Drawing.Point(12, 63);
            this.methodComboBox.Name = "methodComboBox";
            this.methodComboBox.Size = new System.Drawing.Size(162, 21);
            this.methodComboBox.TabIndex = 9;
            // 
            // methodListBox
            // 
            this.methodListBox.FormattingEnabled = true;
            this.methodListBox.Location = new System.Drawing.Point(12, 119);
            this.methodListBox.Name = "methodListBox";
            this.methodListBox.Size = new System.Drawing.Size(159, 95);
            this.methodListBox.TabIndex = 8;
            this.methodListBox.SelectedIndexChanged += new System.EventHandler(this.methodsListBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.requestsListBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 241);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(764, 354);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Requests Log";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.groupNrTextBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.nextMatchButton);
            this.groupBox2.Controls.Add(this.matchTextBox);
            this.groupBox2.Controls.Add(this.initMatchingButton);
            this.groupBox2.Controls.Add(this.regexTextBox);
            this.groupBox2.Location = new System.Drawing.Point(384, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 222);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Regex Search";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(170, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Regex";
            // 
            // nextMatchButton
            // 
            this.nextMatchButton.Location = new System.Drawing.Point(90, 24);
            this.nextMatchButton.Name = "nextMatchButton";
            this.nextMatchButton.Size = new System.Drawing.Size(78, 23);
            this.nextMatchButton.TabIndex = 9;
            this.nextMatchButton.Text = "Next Match";
            this.nextMatchButton.UseVisualStyleBackColor = true;
            this.nextMatchButton.Click += new System.EventHandler(this.nextMatchButton_Click_1);
            // 
            // matchTextBox
            // 
            this.matchTextBox.Location = new System.Drawing.Point(6, 53);
            this.matchTextBox.Multiline = true;
            this.matchTextBox.Name = "matchTextBox";
            this.matchTextBox.Size = new System.Drawing.Size(380, 161);
            this.matchTextBox.TabIndex = 8;
            // 
            // initMatchingButton
            // 
            this.initMatchingButton.Location = new System.Drawing.Point(6, 24);
            this.initMatchingButton.Name = "initMatchingButton";
            this.initMatchingButton.Size = new System.Drawing.Size(78, 23);
            this.initMatchingButton.TabIndex = 7;
            this.initMatchingButton.Text = "InitMatching";
            this.initMatchingButton.UseVisualStyleBackColor = true;
            this.initMatchingButton.Click += new System.EventHandler(this.initMatchingButton_Click_1);
            // 
            // regexTextBox
            // 
            this.regexTextBox.Location = new System.Drawing.Point(173, 27);
            this.regexTextBox.Name = "regexTextBox";
            this.regexTextBox.Size = new System.Drawing.Size(180, 20);
            this.regexTextBox.TabIndex = 6;
            // 
            // groupNrTextBox
            // 
            this.groupNrTextBox.Location = new System.Drawing.Point(359, 27);
            this.groupNrTextBox.Name = "groupNrTextBox";
            this.groupNrTextBox.Size = new System.Drawing.Size(22, 20);
            this.groupNrTextBox.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(356, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Nr";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 607);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.loggingFilterGroupBox);
            this.Name = "MainForm";
            this.Text = "SSLStripProxy";
            this.loggingFilterGroupBox.ResumeLayout(false);
            this.loggingFilterGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox requestsListBox;
        private System.Windows.Forms.GroupBox loggingFilterGroupBox;
        private System.Windows.Forms.Button removeMethodButton;
        private System.Windows.Forms.Button addMethodButton;
        private System.Windows.Forms.ComboBox methodComboBox;
        private System.Windows.Forms.ListBox methodListBox;
        private System.Windows.Forms.Button removeContentTypeButton;
        private System.Windows.Forms.Button addContentTypeButton;
        private System.Windows.Forms.ComboBox contentTypeComboBox;
        private System.Windows.Forms.ListBox contentTypeListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button nextMatchButton;
        private System.Windows.Forms.TextBox matchTextBox;
        private System.Windows.Forms.Button initMatchingButton;
        private System.Windows.Forms.TextBox regexTextBox;
        private System.Windows.Forms.Button setFilterButton;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.Label hostLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox groupNrTextBox;
    }
}