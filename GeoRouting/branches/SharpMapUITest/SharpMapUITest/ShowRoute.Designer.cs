namespace MapDisplayModule
{
    partial class ShowRoute
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
            this.mapImage2 = new SharpMap.Forms.MapImage();
            this.mapBox1 = new SharpMap.Forms.MapBox();
            this.RouteName_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mapImage2)).BeginInit();
            this.SuspendLayout();
            // 
            // mapImage2
            // 
            this.mapImage2.ActiveTool = SharpMap.Forms.MapImage.Tools.Pan;
            this.mapImage2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapImage2.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.mapImage2.FineZoomFactor = 10D;
            this.mapImage2.Location = new System.Drawing.Point(12, 39);
            this.mapImage2.Name = "mapImage2";
            this.mapImage2.PanOnClick = false;
            this.mapImage2.QueryLayerIndex = 0;
            this.mapImage2.Size = new System.Drawing.Size(560, 538);
            this.mapImage2.TabIndex = 2;
            this.mapImage2.TabStop = false;
            this.mapImage2.WheelZoomMagnitude = 2D;
            this.mapImage2.ZoomOnDblClick = false;
            this.mapImage2.SizeChanged += new System.EventHandler(this.mapImage2_SizeChanged);
            // 
            // mapBox1
            // 
            this.mapBox1.ActiveTool = SharpMap.Forms.MapBox.Tools.None;
            this.mapBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.mapBox1.FineZoomFactor = 10D;
            this.mapBox1.Location = new System.Drawing.Point(41, 65);
            this.mapBox1.Name = "mapBox1";
            this.mapBox1.QueryLayerIndex = 0;
            this.mapBox1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mapBox1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.mapBox1.ShowProgressUpdate = false;
            this.mapBox1.Size = new System.Drawing.Size(56, 18);
            this.mapBox1.TabIndex = 0;
            this.mapBox1.Text = "mapBox1";
            this.mapBox1.WheelZoomMagnitude = 2D;
            // 
            // RouteName_label
            // 
            this.RouteName_label.AutoSize = true;
            this.RouteName_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RouteName_label.Location = new System.Drawing.Point(12, 9);
            this.RouteName_label.Name = "RouteName_label";
            this.RouteName_label.Size = new System.Drawing.Size(66, 24);
            this.RouteName_label.TabIndex = 3;
            this.RouteName_label.Text = "label1";
            // 
            // ShowRoute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 589);
            this.Controls.Add(this.RouteName_label);
            this.Controls.Add(this.mapImage2);
            this.Controls.Add(this.mapBox1);
            this.Name = "ShowRoute";
            this.Text = "ShowRoute";
            this.Load += new System.EventHandler(this.ShowRoute_Load);
            this.SizeChanged += new System.EventHandler(this.ShowRoute_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.mapImage2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpMap.Forms.MapBox mapBox1;
        private SharpMap.Forms.MapImage mapImage2;
        private System.Windows.Forms.Label RouteName_label;
    }
}