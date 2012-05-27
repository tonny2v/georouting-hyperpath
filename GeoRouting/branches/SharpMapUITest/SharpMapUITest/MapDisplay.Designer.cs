namespace MapDisplayModule
{
    partial class MapDisplay
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapDisplay));
            this.Layers_clb = new System.Windows.Forms.CheckedListBox();
            this.main_toolStrip = new System.Windows.Forms.ToolStrip();
            this.ReSet_btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.RenderNode_btn = new System.Windows.Forms.ToolStripButton();
            this.RenderLink_btn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToExtent_btn = new System.Windows.Forms.ToolStripButton();
            this.ZoomIn_btn = new System.Windows.Forms.ToolStripButton();
            this.ZoomOut_btn = new System.Windows.Forms.ToolStripButton();
            this.Pan_btn = new System.Windows.Forms.ToolStripButton();
            this.Query_btn = new System.Windows.Forms.ToolStripButton();
            this.Redirect_cmb = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.NodeInput_tb = new System.Windows.Forms.ToolStripTextBox();
            this.FindElement_btn = new System.Windows.Forms.ToolStripButton();
            this.TestTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.BufferDistanceToolStripMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.bufferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderScanned_btn = new System.Windows.Forms.ToolStripButton();
            this.RenderScannedLinks_btn = new System.Windows.Forms.ToolStripButton();
            this.SaveImage_btn = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.HideRoutes_ckb = new System.Windows.Forms.CheckBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.treeView_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showThisRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MapInfomation_label = new System.Windows.Forms.Label();
            this.mapImage1 = new SharpMap.Forms.MapImage();
            this.Pause_btn = new System.Windows.Forms.Button();
            this.Animation_trackbar = new System.Windows.Forms.TrackBar();
            this.Start_btn = new System.Windows.Forms.Button();
            this.Stop_btn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.mapContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mapContext_refresh = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToExtentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleEarthItToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.main_statusStrip = new System.Windows.Forms.StatusStrip();
            this.CurrentDB_tsl = new System.Windows.Forms.ToolStripStatusLabel();
            this.SelectedLayer_tsl = new System.Windows.Forms.ToolStripStatusLabel();
            this.Status_tsl = new System.Windows.Forms.ToolStripStatusLabel();
            this.SelectedItem_tsl = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.animation_timer = new System.Windows.Forms.Timer(this.components);
            this.setStyle_ColorDialog = new System.Windows.Forms.ColorDialog();
            this.setStyleContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SetColr = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.main_toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.treeView_contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapImage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Animation_trackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.mapContext.SuspendLayout();
            this.main_statusStrip.SuspendLayout();
            this.setStyleContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // Layers_clb
            // 
            this.Layers_clb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Layers_clb.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Layers_clb.FormattingEnabled = true;
            this.Layers_clb.Location = new System.Drawing.Point(15, 32);
            this.Layers_clb.Name = "Layers_clb";
            this.Layers_clb.Size = new System.Drawing.Size(211, 124);
            this.Layers_clb.TabIndex = 4;
            this.Layers_clb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Layers_clb_MouseDown);
            // 
            // main_toolStrip
            // 
            this.main_toolStrip.BackColor = System.Drawing.SystemColors.Control;
            this.main_toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReSet_btn,
            this.toolStripSeparator2,
            this.RenderNode_btn,
            this.RenderLink_btn,
            this.toolStripSeparator1,
            this.ToExtent_btn,
            this.ZoomIn_btn,
            this.ZoomOut_btn,
            this.Pan_btn,
            this.Query_btn,
            this.Redirect_cmb,
            this.toolStripSeparator4,
            this.NodeInput_tb,
            this.FindElement_btn,
            this.TestTools,
            this.RenderScanned_btn,
            this.RenderScannedLinks_btn,
            this.SaveImage_btn});
            this.main_toolStrip.Location = new System.Drawing.Point(0, 0);
            this.main_toolStrip.Name = "main_toolStrip";
            this.main_toolStrip.Size = new System.Drawing.Size(1073, 25);
            this.main_toolStrip.TabIndex = 10;
            this.main_toolStrip.Text = "toolStrip1";
            // 
            // ReSet_btn
            // 
            this.ReSet_btn.Image = ((System.Drawing.Image)(resources.GetObject("ReSet_btn.Image")));
            this.ReSet_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ReSet_btn.Name = "ReSet_btn";
            this.ReSet_btn.Size = new System.Drawing.Size(56, 22);
            this.ReSet_btn.Text = "&ReSet";
            this.ReSet_btn.Click += new System.EventHandler(this.Refresth_btn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // RenderNode_btn
            // 
            this.RenderNode_btn.Image = ((System.Drawing.Image)(resources.GetObject("RenderNode_btn.Image")));
            this.RenderNode_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RenderNode_btn.Name = "RenderNode_btn";
            this.RenderNode_btn.Size = new System.Drawing.Size(93, 22);
            this.RenderNode_btn.Text = "Render&Node";
            this.RenderNode_btn.Click += new System.EventHandler(this.ShowNodeLabel_btn_Click);
            // 
            // RenderLink_btn
            // 
            this.RenderLink_btn.Image = ((System.Drawing.Image)(resources.GetObject("RenderLink_btn.Image")));
            this.RenderLink_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RenderLink_btn.Name = "RenderLink_btn";
            this.RenderLink_btn.Size = new System.Drawing.Size(86, 22);
            this.RenderLink_btn.Text = "Render&Link";
            this.RenderLink_btn.Click += new System.EventHandler(this.ShowLinkLabel_btn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ToExtent_btn
            // 
            this.ToExtent_btn.Image = ((System.Drawing.Image)(resources.GetObject("ToExtent_btn.Image")));
            this.ToExtent_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToExtent_btn.Name = "ToExtent_btn";
            this.ToExtent_btn.Size = new System.Drawing.Size(73, 22);
            this.ToExtent_btn.Text = "To&Extent";
            this.ToExtent_btn.Click += new System.EventHandler(this.ZoomToExtent_Click);
            // 
            // ZoomIn_btn
            // 
            this.ZoomIn_btn.Image = ((System.Drawing.Image)(resources.GetObject("ZoomIn_btn.Image")));
            this.ZoomIn_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ZoomIn_btn.Name = "ZoomIn_btn";
            this.ZoomIn_btn.Size = new System.Drawing.Size(69, 22);
            this.ZoomIn_btn.Text = "Zoom&In";
            this.ZoomIn_btn.Click += new System.EventHandler(this.ZoomIn_btn_Click);
            // 
            // ZoomOut_btn
            // 
            this.ZoomOut_btn.Image = ((System.Drawing.Image)(resources.GetObject("ZoomOut_btn.Image")));
            this.ZoomOut_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ZoomOut_btn.Name = "ZoomOut_btn";
            this.ZoomOut_btn.Size = new System.Drawing.Size(79, 22);
            this.ZoomOut_btn.Text = "Zoom&Out";
            this.ZoomOut_btn.Click += new System.EventHandler(this.ZoomOut_btn_Click);
            // 
            // Pan_btn
            // 
            this.Pan_btn.Image = ((System.Drawing.Image)(resources.GetObject("Pan_btn.Image")));
            this.Pan_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Pan_btn.Name = "Pan_btn";
            this.Pan_btn.Size = new System.Drawing.Size(47, 22);
            this.Pan_btn.Text = "&Pan";
            this.Pan_btn.Click += new System.EventHandler(this.Pan_btn_Click);
            // 
            // Query_btn
            // 
            this.Query_btn.Image = ((System.Drawing.Image)(resources.GetObject("Query_btn.Image")));
            this.Query_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Query_btn.Name = "Query_btn";
            this.Query_btn.Size = new System.Drawing.Size(59, 22);
            this.Query_btn.Text = "&Query";
            this.Query_btn.Click += new System.EventHandler(this.Query_btn_Click);
            // 
            // Redirect_cmb
            // 
            this.Redirect_cmb.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Redirect_cmb.Items.AddRange(new object[] {
            "Test Network",
            "Mesh533935",
            "US. Network",
            "DataMeshes"});
            this.Redirect_cmb.Name = "Redirect_cmb";
            this.Redirect_cmb.Size = new System.Drawing.Size(120, 25);
            this.Redirect_cmb.SelectedIndexChanged += new System.EventHandler(this.Redirect_cmb_SelectedIndexChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // NodeInput_tb
            // 
            this.NodeInput_tb.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.NodeInput_tb.Name = "NodeInput_tb";
            this.NodeInput_tb.Size = new System.Drawing.Size(50, 25);
            this.NodeInput_tb.ToolTipText = "Input Node ID";
            this.NodeInput_tb.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NodeInput_tb_KeyDown);
            // 
            // FindElement_btn
            // 
            this.FindElement_btn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.FindElement_btn.BackColor = System.Drawing.Color.Transparent;
            this.FindElement_btn.Image = ((System.Drawing.Image)(resources.GetObject("FindElement_btn.Image")));
            this.FindElement_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FindElement_btn.Name = "FindElement_btn";
            this.FindElement_btn.Size = new System.Drawing.Size(50, 22);
            this.FindElement_btn.Text = "&Find";
            this.FindElement_btn.Click += new System.EventHandler(this.FindGeometry_btn_Click_1);
            // 
            // TestTools
            // 
            this.TestTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BufferDistanceToolStripMenuItem,
            this.bufferToolStripMenuItem,
            this.testToolStripMenuItem});
            this.TestTools.Image = ((System.Drawing.Image)(resources.GetObject("TestTools.Image")));
            this.TestTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TestTools.Name = "TestTools";
            this.TestTools.Size = new System.Drawing.Size(65, 22);
            this.TestTools.Text = "Tools";
            // 
            // BufferDistanceToolStripMenuItem
            // 
            this.BufferDistanceToolStripMenuItem.Name = "BufferDistanceToolStripMenuItem";
            this.BufferDistanceToolStripMenuItem.Size = new System.Drawing.Size(152, 23);
            this.BufferDistanceToolStripMenuItem.Text = "Set Buffer Distance";
            this.BufferDistanceToolStripMenuItem.ToolTipText = "BufferDistance";
            // 
            // bufferToolStripMenuItem
            // 
            this.bufferToolStripMenuItem.Name = "bufferToolStripMenuItem";
            this.bufferToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.bufferToolStripMenuItem.Text = "&Intersect Buffer";
            this.bufferToolStripMenuItem.Click += new System.EventHandler(this.bufferToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.testToolStripMenuItem.Text = "&Classify Road Layer";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // RenderScanned_btn
            // 
            this.RenderScanned_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RenderScanned_btn.Image = ((System.Drawing.Image)(resources.GetObject("RenderScanned_btn.Image")));
            this.RenderScanned_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RenderScanned_btn.Name = "RenderScanned_btn";
            this.RenderScanned_btn.Size = new System.Drawing.Size(23, 22);
            this.RenderScanned_btn.Text = "Render Scanned Nodes";
            this.RenderScanned_btn.Click += new System.EventHandler(this.RenderScanned_btn_Click);
            // 
            // RenderScannedLinks_btn
            // 
            this.RenderScannedLinks_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RenderScannedLinks_btn.Image = ((System.Drawing.Image)(resources.GetObject("RenderScannedLinks_btn.Image")));
            this.RenderScannedLinks_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RenderScannedLinks_btn.Name = "RenderScannedLinks_btn";
            this.RenderScannedLinks_btn.Size = new System.Drawing.Size(23, 22);
            this.RenderScannedLinks_btn.Text = "Render Scanned Links";
            this.RenderScannedLinks_btn.Click += new System.EventHandler(this.RenderScannedLinks_btn_Click);
            // 
            // SaveImage_btn
            // 
            this.SaveImage_btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveImage_btn.Image = global::MapDisplayModule.Properties.Resources.save;
            this.SaveImage_btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveImage_btn.Name = "SaveImage_btn";
            this.SaveImage_btn.Size = new System.Drawing.Size(23, 22);
            this.SaveImage_btn.Text = "Save Image";
            this.SaveImage_btn.Click += new System.EventHandler(this.SaveImage_btn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1073, 410);
            this.splitContainer1.SplitterDistance = 357;
            this.splitContainer1.TabIndex = 11;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(12, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.HideRoutes_ckb);
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.Layers_clb);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.MapInfomation_label);
            this.splitContainer2.Panel2.Controls.Add(this.mapImage1);
            this.splitContainer2.Panel2.Controls.Add(this.Pause_btn);
            this.splitContainer2.Panel2.Controls.Add(this.Animation_trackbar);
            this.splitContainer2.Panel2.Controls.Add(this.Start_btn);
            this.splitContainer2.Panel2.Controls.Add(this.Stop_btn);
            this.splitContainer2.Size = new System.Drawing.Size(1061, 351);
            this.splitContainer2.SplitterDistance = 242;
            this.splitContainer2.TabIndex = 12;
            // 
            // HideRoutes_ckb
            // 
            this.HideRoutes_ckb.AutoSize = true;
            this.HideRoutes_ckb.Location = new System.Drawing.Point(139, 167);
            this.HideRoutes_ckb.Name = "HideRoutes_ckb";
            this.HideRoutes_ckb.Size = new System.Drawing.Size(85, 17);
            this.HideRoutes_ckb.TabIndex = 8;
            this.HideRoutes_ckb.Text = "Hide Routes";
            this.HideRoutes_ckb.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.ContextMenuStrip = this.treeView_contextMenuStrip;
            this.treeView1.Location = new System.Drawing.Point(15, 200);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(211, 139);
            this.treeView1.TabIndex = 7;
            // 
            // treeView_contextMenuStrip
            // 
            this.treeView_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showThisRouteToolStripMenuItem});
            this.treeView_contextMenuStrip.Name = "treeView_contextMenuStrip";
            this.treeView_contextMenuStrip.Size = new System.Drawing.Size(153, 48);
            // 
            // showThisRouteToolStripMenuItem
            // 
            this.showThisRouteToolStripMenuItem.Name = "showThisRouteToolStripMenuItem";
            this.showThisRouteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showThisRouteToolStripMenuItem.Text = "Show";
            this.showThisRouteToolStripMenuItem.Click += new System.EventHandler(this.showThisRouteToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Trebuchet MS", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 22);
            this.label2.TabIndex = 6;
            this.label2.Text = "Memory layers";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Trebuchet MS", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 22);
            this.label1.TabIndex = 5;
            this.label1.Text = "DB layers";
            // 
            // MapInfomation_label
            // 
            this.MapInfomation_label.AutoSize = true;
            this.MapInfomation_label.Location = new System.Drawing.Point(19, 7);
            this.MapInfomation_label.Name = "MapInfomation_label";
            this.MapInfomation_label.Size = new System.Drawing.Size(65, 13);
            this.MapInfomation_label.TabIndex = 7;
            this.MapInfomation_label.Text = "Map Display";
            // 
            // mapImage1
            // 
            this.mapImage1.ActiveTool = SharpMap.Forms.MapImage.Tools.Pan;
            this.mapImage1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapImage1.BackColor = System.Drawing.Color.White;
            this.mapImage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapImage1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.mapImage1.FineZoomFactor = 10D;
            this.mapImage1.Location = new System.Drawing.Point(22, 32);
            this.mapImage1.Name = "mapImage1";
            this.mapImage1.PanOnClick = false;
            this.mapImage1.QueryLayerIndex = 0;
            this.mapImage1.Size = new System.Drawing.Size(781, 307);
            this.mapImage1.TabIndex = 0;
            this.mapImage1.TabStop = false;
            this.mapImage1.WheelZoomMagnitude = 2D;
            this.mapImage1.ZoomOnDblClick = false;
            this.mapImage1.MouseMove += new SharpMap.Forms.MapImage.MouseEventHandler(this.mapImage1_MouseMove);
            this.mapImage1.SizeChanged += new System.EventHandler(this.mapImage1_SizeChanged);
            this.mapImage1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapImage1_MouseClick);
            // 
            // Pause_btn
            // 
            this.Pause_btn.Image = ((System.Drawing.Image)(resources.GetObject("Pause_btn.Image")));
            this.Pause_btn.Location = new System.Drawing.Point(184, 3);
            this.Pause_btn.Name = "Pause_btn";
            this.Pause_btn.Size = new System.Drawing.Size(20, 20);
            this.Pause_btn.TabIndex = 9;
            this.Pause_btn.UseVisualStyleBackColor = true;
            this.Pause_btn.Click += new System.EventHandler(this.Pause_btn_Click);
            // 
            // Animation_trackbar
            // 
            this.Animation_trackbar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Animation_trackbar.Location = new System.Drawing.Point(236, 3);
            this.Animation_trackbar.Name = "Animation_trackbar";
            this.Animation_trackbar.Size = new System.Drawing.Size(559, 45);
            this.Animation_trackbar.TabIndex = 11;
            // 
            // Start_btn
            // 
            this.Start_btn.Image = ((System.Drawing.Image)(resources.GetObject("Start_btn.Image")));
            this.Start_btn.Location = new System.Drawing.Point(158, 3);
            this.Start_btn.Name = "Start_btn";
            this.Start_btn.Size = new System.Drawing.Size(20, 20);
            this.Start_btn.TabIndex = 8;
            this.Start_btn.UseVisualStyleBackColor = true;
            this.Start_btn.Click += new System.EventHandler(this.Start_btn_Click);
            // 
            // Stop_btn
            // 
            this.Stop_btn.Image = ((System.Drawing.Image)(resources.GetObject("Stop_btn.Image")));
            this.Stop_btn.Location = new System.Drawing.Point(210, 3);
            this.Stop_btn.Name = "Stop_btn";
            this.Stop_btn.Size = new System.Drawing.Size(20, 20);
            this.Stop_btn.TabIndex = 10;
            this.Stop_btn.UseVisualStyleBackColor = true;
            this.Stop_btn.Click += new System.EventHandler(this.Stop_btn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1067, 43);
            this.dataGridView1.TabIndex = 0;
            // 
            // mapContext
            // 
            this.mapContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapContext_refresh,
            this.zoomToExtentToolStripMenuItem,
            this.googleEarthItToolStripMenuItem});
            this.mapContext.Name = "mapContext";
            this.mapContext.Size = new System.Drawing.Size(159, 70);
            // 
            // mapContext_refresh
            // 
            this.mapContext_refresh.Name = "mapContext_refresh";
            this.mapContext_refresh.Size = new System.Drawing.Size(158, 22);
            this.mapContext_refresh.Text = "&Refresh";
            this.mapContext_refresh.Click += new System.EventHandler(this.mapContext_refresh_Click);
            // 
            // zoomToExtentToolStripMenuItem
            // 
            this.zoomToExtentToolStripMenuItem.Name = "zoomToExtentToolStripMenuItem";
            this.zoomToExtentToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.zoomToExtentToolStripMenuItem.Text = "&Zoom To Extent";
            this.zoomToExtentToolStripMenuItem.Click += new System.EventHandler(this.zoomToExtentToolStripMenuItem_Click);
            // 
            // googleEarthItToolStripMenuItem
            // 
            this.googleEarthItToolStripMenuItem.Name = "googleEarthItToolStripMenuItem";
            this.googleEarthItToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.googleEarthItToolStripMenuItem.Text = "&Google Earth it";
            this.googleEarthItToolStripMenuItem.Click += new System.EventHandler(this.googleEarthItToolStripMenuItem_Click);
            // 
            // main_statusStrip
            // 
            this.main_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CurrentDB_tsl,
            this.SelectedLayer_tsl,
            this.Status_tsl,
            this.SelectedItem_tsl});
            this.main_statusStrip.Location = new System.Drawing.Point(0, 440);
            this.main_statusStrip.Name = "main_statusStrip";
            this.main_statusStrip.Size = new System.Drawing.Size(1073, 22);
            this.main_statusStrip.TabIndex = 12;
            this.main_statusStrip.Text = "statusStrip1";
            // 
            // CurrentDB_tsl
            // 
            this.CurrentDB_tsl.Image = ((System.Drawing.Image)(resources.GetObject("CurrentDB_tsl.Image")));
            this.CurrentDB_tsl.Name = "CurrentDB_tsl";
            this.CurrentDB_tsl.Size = new System.Drawing.Size(114, 17);
            this.CurrentDB_tsl.Text = "Current DataBase";
            // 
            // SelectedLayer_tsl
            // 
            this.SelectedLayer_tsl.Image = ((System.Drawing.Image)(resources.GetObject("SelectedLayer_tsl.Image")));
            this.SelectedLayer_tsl.Name = "SelectedLayer_tsl";
            this.SelectedLayer_tsl.Size = new System.Drawing.Size(94, 17);
            this.SelectedLayer_tsl.Text = "Current Layer";
            // 
            // Status_tsl
            // 
            this.Status_tsl.Image = ((System.Drawing.Image)(resources.GetObject("Status_tsl.Image")));
            this.Status_tsl.Name = "Status_tsl";
            this.Status_tsl.Size = new System.Drawing.Size(55, 17);
            this.Status_tsl.Text = "Status";
            // 
            // SelectedItem_tsl
            // 
            this.SelectedItem_tsl.Image = ((System.Drawing.Image)(resources.GetObject("SelectedItem_tsl.Image")));
            this.SelectedItem_tsl.Name = "SelectedItem_tsl";
            this.SelectedItem_tsl.Size = new System.Drawing.Size(122, 17);
            this.SelectedItem_tsl.Text = "Selected Geometry";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // animation_timer
            // 
            this.animation_timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // setStyleContext
            // 
            this.setStyleContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetColr,
            this.zoomToLayerToolStripMenuItem});
            this.setStyleContext.Name = "setStyleContext";
            this.setStyleContext.Size = new System.Drawing.Size(155, 48);
            // 
            // SetColr
            // 
            this.SetColr.Name = "SetColr";
            this.SetColr.Size = new System.Drawing.Size(154, 22);
            this.SetColr.Text = "Set &Color";
            this.SetColr.Click += new System.EventHandler(this.SetColr_Click);
            // 
            // zoomToLayerToolStripMenuItem
            // 
            this.zoomToLayerToolStripMenuItem.Name = "zoomToLayerToolStripMenuItem";
            this.zoomToLayerToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.zoomToLayerToolStripMenuItem.Text = "&Zoom To Layer";
            this.zoomToLayerToolStripMenuItem.Click += new System.EventHandler(this.zoomToLayerToolStripMenuItem_Click);
            // 
            // MapDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 462);
            this.Controls.Add(this.main_statusStrip);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.main_toolStrip);
            this.Name = "MapDisplay";
            this.Text = "MapDisplay(CopyRight by J.S. Ma)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.main_toolStrip.ResumeLayout(false);
            this.main_toolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.treeView_contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mapImage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Animation_trackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.mapContext.ResumeLayout(false);
            this.main_statusStrip.ResumeLayout(false);
            this.main_statusStrip.PerformLayout();
            this.setStyleContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpMap.Forms.MapImage mapImage1;
        private System.Windows.Forms.ToolStrip main_toolStrip;
        private System.Windows.Forms.ToolStripButton ReSet_btn;
        private System.Windows.Forms.ToolStripButton RenderNode_btn;
        private System.Windows.Forms.ToolStripButton RenderLink_btn;
        private System.Windows.Forms.ToolStripButton ToExtent_btn;
        private System.Windows.Forms.ToolStripButton ZoomIn_btn;
        private System.Windows.Forms.ToolStripButton ZoomOut_btn;
        private System.Windows.Forms.ToolStripButton Pan_btn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton Query_btn;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.StatusStrip main_statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel Status_tsl;
        private System.Windows.Forms.ToolStripStatusLabel SelectedItem_tsl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripStatusLabel CurrentDB_tsl;
        private System.Windows.Forms.CheckedListBox Layers_clb;
        private System.Windows.Forms.ToolStripComboBox Redirect_cmb;
        private System.Windows.Forms.ToolStripStatusLabel SelectedLayer_tsl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripTextBox NodeInput_tb;
        private System.Windows.Forms.ToolStripButton FindElement_btn;
        private System.Windows.Forms.Timer animation_timer;
        private System.Windows.Forms.Label MapInfomation_label;
        private System.Windows.Forms.Button Start_btn;
        private System.Windows.Forms.Button Stop_btn;
        private System.Windows.Forms.Button Pause_btn;
        private System.Windows.Forms.TrackBar Animation_trackbar;
        private System.Windows.Forms.ToolStripDropDownButton TestTools;
        private System.Windows.Forms.ToolStripMenuItem bufferToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox BufferDistanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ColorDialog setStyle_ColorDialog;
        private System.Windows.Forms.ContextMenuStrip setStyleContext;
        private System.Windows.Forms.ToolStripMenuItem SetColr;
        private System.Windows.Forms.ContextMenuStrip mapContext;
        private System.Windows.Forms.ToolStripMenuItem mapContext_refresh;
        private System.Windows.Forms.ToolStripMenuItem zoomToLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToExtentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem googleEarthItToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton RenderScanned_btn;
        private System.Windows.Forms.ToolStripButton RenderScannedLinks_btn;
        private System.Windows.Forms.ToolStripButton SaveImage_btn;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox HideRoutes_ckb;
        private System.Windows.Forms.ContextMenuStrip treeView_contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showThisRouteToolStripMenuItem;
    }
}

