namespace GeoHyperstar.Forms
{
    partial class DBSetting_Form
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
            this.label1 = new System.Windows.Forms.Label();
            this.server_tb = new System.Windows.Forms.TextBox();
            this.port_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.database_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.user_tb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.password_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SetDefault_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // server_tb
            // 
            this.server_tb.Location = new System.Drawing.Point(79, 23);
            this.server_tb.Name = "server_tb";
            this.server_tb.Size = new System.Drawing.Size(100, 20);
            this.server_tb.TabIndex = 1;
            this.server_tb.Text = "localhost";
            // 
            // port_tb
            // 
            this.port_tb.Location = new System.Drawing.Point(79, 62);
            this.port_tb.Name = "port_tb";
            this.port_tb.Size = new System.Drawing.Size(100, 20);
            this.port_tb.TabIndex = 3;
            this.port_tb.Text = "5432";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // database_tb
            // 
            this.database_tb.Location = new System.Drawing.Point(79, 102);
            this.database_tb.Name = "database_tb";
            this.database_tb.Size = new System.Drawing.Size(100, 20);
            this.database_tb.TabIndex = 5;
            this.database_tb.Text = "postgis";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Database";
            // 
            // user_tb
            // 
            this.user_tb.Location = new System.Drawing.Point(79, 138);
            this.user_tb.Name = "user_tb";
            this.user_tb.Size = new System.Drawing.Size(100, 20);
            this.user_tb.TabIndex = 7;
            this.user_tb.Text = "postgres";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "User";
            // 
            // password_tb
            // 
            this.password_tb.Location = new System.Drawing.Point(79, 171);
            this.password_tb.Name = "password_tb";
            this.password_tb.PasswordChar = '*';
            this.password_tb.Size = new System.Drawing.Size(100, 20);
            this.password_tb.TabIndex = 9;
            this.password_tb.Text = "wangyiqi";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 174);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Password";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(104, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SetDefault_btn
            // 
            this.SetDefault_btn.Location = new System.Drawing.Point(23, 197);
            this.SetDefault_btn.Name = "SetDefault_btn";
            this.SetDefault_btn.Size = new System.Drawing.Size(75, 23);
            this.SetDefault_btn.TabIndex = 11;
            this.SetDefault_btn.Text = "SetDefault";
            this.SetDefault_btn.UseVisualStyleBackColor = true;
            this.SetDefault_btn.Click += new System.EventHandler(this.SetDefault_btn_Click);
            // 
            // DBSetting_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 229);
            this.Controls.Add(this.SetDefault_btn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.password_tb);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.user_tb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.database_tb);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.port_tb);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.server_tb);
            this.Controls.Add(this.label1);
            this.Name = "DBSetting_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DBSetting";
            this.Load += new System.EventHandler(this.DBSetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox server_tb;
        private System.Windows.Forms.TextBox port_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox database_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox user_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox password_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button SetDefault_btn;
    }
}