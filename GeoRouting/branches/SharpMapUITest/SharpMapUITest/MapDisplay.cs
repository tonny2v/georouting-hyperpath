
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Npgsql;
using System.Drawing.Drawing2D;
using SharpMap.Forms;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using GisSharpBlog.NetTopologySuite;
using System.Drawing.Imaging;


namespace SharpMapUITest
{
    public partial class MapDisplay : Form
    {
        
        public MapDisplay()
        {
            InitializeComponent();
        }

        private static string database = "postgis";
        //Default connection
        public static string ConnStr = "Server=localhost"
        + ";DataBase=" + database
        + ";Port=5432"
        + ";Userid= postgres"
        + ";password=password"
        + ";Protocol=3;SSL=false;Pooling=true;MinPoolSize=1;MaxPoolSize=20;EnCoding=UNICODE;Timeout=15;SslMode=Disable";

        SharpMap.Layers.LabelLayer verticesAsLabel;//node label style
        SharpMap.Layers.LabelLayer roadlayASLabel;//link label style
 
        
        public int[] Nodes_Animation;
        public int[] Links_Animation;
        Graphics g;
        int timecounter = 0;
        double fullzoom = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            //从数据库中获取几何表，并将其添加到列表框中
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnStr))
            {
                string QueryGeometricLayers = "SELECT f_table_name FROM geometry_columns;";
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = QueryGeometricLayers;
                NpgsqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                { 
                    Layers_clb.Items.Add(dr["f_table_name"].ToString());
                }
                dr.Dispose();
                cmd.Dispose();
            }
            g = mapImage1.CreateGraphics();
            mapImage1.MouseEnter += new EventHandler(Map_MouseEnterEventHandler);
            mapImage1.ZoomOnDblClick = true;
        }

        private SharpMap.Map GetCurrentMap(string sqlconn, string[] tableName)
        {
            SharpMap.Map myMap = new SharpMap.Map();

            using (NpgsqlConnection conn = new NpgsqlConnection(sqlconn))
            {
                conn.Open();
                myMap.Size = new System.Drawing.Size(this.mapImage1.Size.Width, this.mapImage1.Height);
                //myMap.MinimumZoom = 10;
                if (tableName.Contains("vertices_tmp"))
                {
                    //Add node layer
                    verticesAsLabel = new SharpMap.Layers.LabelLayer("nodes");
                    SharpMap.Layers.VectorLayer vertices_tmp = new SharpMap.Layers.VectorLayer("vertices_tmp");
                    vertices_tmp.DataSource = new SharpMap.Data.Providers.PostGIS(conn.ConnectionString, "vertices_tmp", "the_geom");

                    vertices_tmp.Style.Symbol = SharpMap.Styles.VectorStyle.DefaultSymbol;
                    //vertices_tmp.Style.Symbol = Image.FromFile("..\\..\\Node.bmp");
                    vertices_tmp.Style.SymbolScale = (float)0.32;
                    //不消除锯齿
                    vertices_tmp.SmoothingMode = SmoothingMode.None;
                    myMap.Layers.Add(vertices_tmp);

                    //设置节点Label层样式
                    verticesAsLabel.DataSource = vertices_tmp.DataSource;
                    verticesAsLabel.LabelColumn = "id";
                    verticesAsLabel.Style.Font = new Font("Arial", 6, FontStyle.Bold);
                    verticesAsLabel.Style = new SharpMap.Styles.LabelStyle();
                    verticesAsLabel.Style.ForeColor = Color.Red;
                    verticesAsLabel.Style.Offset = new PointF(10, 0);
                    verticesAsLabel.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    verticesAsLabel.SmoothingMode = SmoothingMode.AntiAlias;
                    //layASLabel.SRID = 3945; //This is your spatial ref no
                    verticesAsLabel.LabelFilter = SharpMap.Rendering.LabelCollisionDetection.ThoroughCollisionDetection;
                    verticesAsLabel.Style.CollisionDetection = true;
                }

                if (tableName.Contains("road"))
                {
                    roadlayASLabel = new SharpMap.Layers.LabelLayer("road");
                    //设置road图层的数据源和样式
                    SharpMap.Layers.VectorLayer road = new SharpMap.Layers.VectorLayer("road");
                    road.DataSource = new SharpMap.Data.Providers.PostGIS(conn.ConnectionString, "road", "the_geom");
                    //conn.Open();
                    //var indexes = road.DataSource.GetObjectIDsInView(road.DataSource.GetExtents());
                    road.Style.Line = new System.Drawing.Pen(System.Drawing.Color.LightBlue, 1);
                    //不消除锯齿
                    road.SmoothingMode = SmoothingMode.None;
                    myMap.Layers.Add(road);

                    //设置路段Label层样式
                    roadlayASLabel.DataSource = road.DataSource;
                    roadlayASLabel.LabelColumn = "gid";
                    roadlayASLabel.Style.Font = new Font("Arial", 6, FontStyle.Bold);
                    roadlayASLabel.Style = new SharpMap.Styles.LabelStyle();
                    roadlayASLabel.Style.ForeColor = Color.Black;
                    roadlayASLabel.Style.Offset = new PointF(10, 0);
                    roadlayASLabel.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    roadlayASLabel.SmoothingMode = SmoothingMode.AntiAlias;
                    //layASLabel.SRID = 3945; //This is your spatial ref no
                    roadlayASLabel.LabelFilter = SharpMap.Rendering.LabelCollisionDetection.ThoroughCollisionDetection;
                    roadlayASLabel.Style.CollisionDetection = true;
                }
                if (tableName.Contains("hyperpath_lyr"))
                {
                    //设置hyperpath_lyr图层的数据源和样式
                    SharpMap.Layers.VectorLayer hyperpath = new SharpMap.Layers.VectorLayer("hyperpath_lyr");
                    hyperpath.DataSource = new SharpMap.Data.Providers.PostGIS(conn.ConnectionString, "hyperpath_lyr", "the_geom");
                    hyperpath.Style.Line = new System.Drawing.Pen(System.Drawing.Color.GreenYellow, 5);
                    myMap.Layers.Add(hyperpath);

                    //设置主题
                    SharpMap.Styles.VectorStyle firstStyle = new SharpMap.Styles.VectorStyle();
                    SharpMap.Styles.VectorStyle lastStyle = new SharpMap.Styles.VectorStyle();
                    firstStyle.Line = new System.Drawing.Pen(System.Drawing.Color.Green, 1);
                    lastStyle.Line = new System.Drawing.Pen(System.Drawing.Color.Blue, 5);
                    //firstStyle.Outline = new Pen(Color.Black);
                    //firstStyle.EnableOutline = true;
                    //lastStyle.Outline = new Pen(Color.Black);
                    //lastStyle.EnableOutline = true;
                    SharpMap.Rendering.Thematics.GradientTheme Theme = new SharpMap.Rendering.Thematics.GradientTheme("choice_possibility", 0, 1, firstStyle, lastStyle);
                    //Theme.FillColorBlend = SharpMap.Rendering.Thematics.ColorBlend.ThreeColors(Color.Yellow, Color.SkyBlue, Color.HotPink);
                    hyperpath.Theme = Theme;
                }

                if (tableName.Contains("popath_lyr"))
                {
                    //设置road图层的数据源和样式
                    SharpMap.Layers.VectorLayer popath_lyr = new SharpMap.Layers.VectorLayer("popath_lyr");
                    popath_lyr.DataSource = new SharpMap.Data.Providers.PostGIS(conn.ConnectionString, "popath_lyr", "the_geom");
                    //conn.Open();
                    //var indexes = road.DataSource.GetObjectIDsInView(road.DataSource.GetExtents());
                    popath_lyr.Style.Line = new System.Drawing.Pen(System.Drawing.Color.LightBlue, 1);
                    //不消除锯齿
                    popath_lyr.SmoothingMode = SmoothingMode.None;
                    SharpMap.Styles.VectorStyle mystyle = new SharpMap.Styles.VectorStyle();
                    mystyle.Line.Color = Color.Blue;
                    mystyle.Line.Width = 2.0f;
                    popath_lyr.Style = mystyle;
                    myMap.Layers.Add(popath_lyr);

                    
                    
                }

                if (tableName.Contains("regretpath_lyr"))
                {
                    //设置road图层的数据源和样式
                    SharpMap.Layers.VectorLayer regretpath_lyr = new SharpMap.Layers.VectorLayer("regretpath_lyr");
                    regretpath_lyr.DataSource = new SharpMap.Data.Providers.PostGIS(conn.ConnectionString, "regretpath_lyr", "the_geom");
                    //conn.Open();
                    //var indexes = road.DataSource.GetObjectIDsInView(road.DataSource.GetExtents());
                    regretpath_lyr.Style.Line = new System.Drawing.Pen(System.Drawing.Color.LightBlue, 1);
                    //不消除锯齿
                    regretpath_lyr.SmoothingMode = SmoothingMode.None;
                    SharpMap.Styles.VectorStyle mystyle = new SharpMap.Styles.VectorStyle();
                    mystyle.Line.Color = Color.Red;
                    mystyle.Line.Width = 3.5f;
                    regretpath_lyr.Style = mystyle;
                    myMap.Layers.Add(regretpath_lyr);
                }

            }
            return myMap;
        }

        #region ToolStripButton Events

        private void Refresth_btn_Click(object sender, EventArgs e)
        {
            timer1.Dispose();
            if (ConnStr == string.Empty) MessageBox.Show("No Sql Connection!");
            List<string> lyrs = new List<string>();
            foreach (var i in Layers_clb.Items)
            {
                if(Layers_clb.CheckedItems.Contains(i))
                    lyrs.Add(i.ToString());
            }
            
            CurrentDB_tsl.Text = "Connection: localhost//"+database;
            //mapImage1.Map.Size = Size;
            mapImage1.Map = GetCurrentMap(ConnStr, lyrs.ToArray());
            Cursor = Cursors.Default;
            mapImage1.ActiveTool = MapImage.Tools.None;
            //mapImage1.ActiveTool = MapImage.Tools.Pan;
            mapImage1.Map.ZoomToExtents();
            fullzoom = mapImage1.Map.Zoom;
            //mapImage1.Map.GetMap();

            SelectedLayer_tsl.Text = "Current Layer: "+Layers_clb.SelectedItem.ToString();
            //mapImage1.Cursor = mic;
            
            mapImage1.Refresh();
        }

        private void mapImage1_SizeChanged(object sender, EventArgs e)
        {
            if (mapImage1.Size.Width != 0&&mapImage1.Size.Height!=0)
                mapImage1.Refresh();
        }

        //private void DrawRectange(System.Drawing.PointF start, System.Drawing.PointF end, Graphics g)
        //{
        //    g.DrawRectangle(new Pen(Color.Red), start.X, start.Y, start.X - end.X, start.Y - end.Y);
        //}

        private void ShowNodeLabel_btn_Click(object sender, EventArgs e)
        {
            if (Layers_clb.CheckedItems.Contains("vertices_tmp"))
                verticesAsLabel.Render(g, mapImage1.Map);
        }

        private void ShowLinkLabel_btn_Click(object sender, EventArgs e)
        {
            if (Layers_clb.CheckedItems.Contains("road"))
                roadlayASLabel.Render(g, mapImage1.Map);
        }

        private void ZoomToExtent_Click(object sender, EventArgs e)
        {
            this.mapImage1.Map.ZoomToExtents();
            this.mapImage1.Refresh();
        }

        private void ZoomIn_btn_Click(object sender, EventArgs e)
        {
            mapImage1.ActiveTool = MapImage.Tools.ZoomIn;
        }

        private void ZoomOut_btn_Click(object sender, EventArgs e)
        {
            mapImage1.ActiveTool = MapImage.Tools.ZoomOut;
        }

        private void Pan_btn_Click(object sender, EventArgs e)
        {
            mapImage1.ActiveTool = MapImage.Tools.Pan;
        }

        private void Query_btn_Click(object sender, EventArgs e)
        {
            mapImage1.ActiveTool = MapImage.Tools.SelfDefinedQuery;
        }

        #endregion

        #region MapImage1 Events

        private void mapImage1_MouseClick(object sender, MouseEventArgs e)
        {
            if (mapImage1.ActiveTool == MapImage.Tools.SelfDefinedQuery)
            {
                PointF p = new PointF(e.X, e.Y);
                var pos = mapImage1.Map.ImageToWorld(p);
                double allowedAccuracy = mapImage1.Map.PixelSize * 5;
                string lyrName=Layers_clb.SelectedItem.ToString();
                var QueryLayer = mapImage1.Map.Layers[lyrName] as SharpMap.Layers.VectorLayer;
                SharpMap.Data.FeatureDataRow QueryResult = null;
                if (QueryLayer is SharpMap.Layers.ICanQueryLayer)
                {
                    QueryResult = PickUpGeometry(pos, QueryLayer, allowedAccuracy);
                }
                if (QueryResult != null)
                {
                    mapImage1.Refresh();
                    RenderFeatureDataRow(QueryResult, g, Color.Blue, Color.Orange);

                    DataRow dr = QueryResult as DataRow;

                    SelectedItem_tsl.Text = "Selected Geometry: "+QueryResult.Geometry.GeometryType.ToString() +" GID = "+dr[0].ToString();
                }
                else SelectedItem_tsl.Text = "No item is selected";
            }
        }

        private void mapImage1_MouseMove(SharpMap.Geometries.Point WorldPos, MouseEventArgs ImagePos)
        {
            Status_tsl.Text = string.Format("WCRS: X: {0}, Y:{1}", WorldPos.X, WorldPos.Y);
        }
       
        #endregion

        #region Defined Function
        
        //用NetTopologySuite找出距离目标点最近的元素
        SharpMap.Data.FeatureDataRow PickUpGeometry(SharpMap.Geometries.Point pos, SharpMap.Layers.VectorLayer layer, double amountGrow)
        {
            SharpMap.Geometries.BoundingBox bbox = pos.GetBoundingBox().Grow(amountGrow);
            SharpMap.Data.FeatureDataSet ds = new SharpMap.Data.FeatureDataSet();
            layer.DataSource.ExecuteIntersectionQuery(bbox, ds);
            DataTable tbl = ds.Tables[0] as SharpMap.Data.FeatureDataTable;
            dataGridView1.DataSource = tbl;
            GisSharpBlog.NetTopologySuite.IO.WKTReader reader = new GisSharpBlog.NetTopologySuite.IO.WKTReader();
            GeoAPI.Geometries.IGeometry point = reader.Read(pos.ToString());
            //如果查询结果为空，则返回空值
            if (tbl.Rows.Count == 0)
                return null;

            double distance = point.Distance(reader.Read((tbl.Rows[0] as SharpMap.Data.FeatureDataRow).Geometry.ToString()));
            SharpMap.Data.FeatureDataRow selectedFeature = tbl.Rows[0] as SharpMap.Data.FeatureDataRow;

            //如果查询结果超过1条，则返回距离点击位置最近的元素
            if (tbl.Rows.Count > 1)
                for (int i = 1; i < tbl.Rows.Count; i++)
                {
                    GeoAPI.Geometries.IGeometry line = reader.Read((tbl.Rows[i] as SharpMap.Data.FeatureDataRow).Geometry.ToString());
                    if (point.Distance(line) < distance)
                    {
                        distance = point.Distance(line);
                        selectedFeature = tbl.Rows[i] as SharpMap.Data.FeatureDataRow;
                    }
                }
            return selectedFeature;
        }

        //渲染地理元素
        private void RenderGeometry(SharpMap.Geometries.IGeometry item, string type)
        {
            if(type=="Point")
                SharpMap.Rendering.VectorRenderer.DrawPoint(g, item as SharpMap.Geometries.Point, new SolidBrush(Color.Red), 10f, new PointF(0f, 0f), mapImage1.Map);
            else if(type=="Polygon")
            {
                Brush brush = new SolidBrush(Color.FromArgb(20, Color.Brown));
                
                SharpMap.Rendering.VectorRenderer.DrawPolygon(g, item as SharpMap.Geometries.Polygon, brush , new Pen(Color.Brown),true, mapImage1.Map);
            }
        }

        //渲染数据行
        private void RenderFeatureDataRow(SharpMap.Data.FeatureDataRow item,Graphics g,Color PointColor,Color PolyLineColor)
        {
            //Graphics g = mapImage1.CreateGraphics();
            //if (PointColor == null) PointColor = Color.Blue;
            //if (PolyLineColor == null) PolyLineColor = Color.Orange;
            Font aFont = new Font("Times New Roman", 18, FontStyle.Regular | FontStyle.Regular);
            switch (item.Geometry.GeometryType)
            {
                case SharpMap.Geometries.GeometryType2.Point:
                    SharpMap.Rendering.VectorRenderer.DrawPoint(g, item.Geometry as SharpMap.Geometries.Point, new SolidBrush(PointColor), 6f, new PointF(0f, 0f), mapImage1.Map);
                    
                // render label at the same time       
                    //PointF pos = mapImage1.Map.WorldToImage(item.Geometry as SharpMap.Geometries.Point);
                    //SharpMap.Rendering.VectorRenderer.DrawLabel(g, pos, new PointF(2f, 2f), aFont, Color.Black, new SolidBrush(Color.Empty), new Pen(new SolidBrush(Color.Empty)), 0f, item["id"].ToString(), mapImage1.Map);
                    aFont.Dispose();
                    break;
                //case SharpMap.Geometries.GeometryType2.LineString:
                //    SharpMap.Rendering.VectorRenderer.DrawLineString(g, item.Geometry as SharpMap.Geometries.LineString, new Pen(Color.Orange,3), mapImage1.Map);
                //    break;
                case SharpMap.Geometries.GeometryType2.MultiLineString:
                    SharpMap.Rendering.VectorRenderer.DrawMultiLineString(g, item.Geometry as SharpMap.Geometries.MultiLineString, new Pen(PolyLineColor,1.5f), mapImage1.Map, 0f);
                // render label at the same time   
                   //SharpMap.Geometries.MultiLineString multiline=item.Geometry as SharpMap.Geometries.MultiLineString;
                    //var p=multiline.LineStrings[multiline.LineStrings.Count / 2].Vertices;
                    //PointF pos2 = mapImage1.Map.WorldToImage(p[p.Count/2]);
                    //SharpMap.Rendering.VectorRenderer.DrawLabel(g, pos2, new PointF(2f, 2f), aFont, Color.Black, new SolidBrush(Color.Empty), new Pen(new SolidBrush(Color.Empty)), 0f, item["gid"].ToString(), mapImage1.Map);
                    //aFont.Dispose();
                    
                    break;
                default:
                    MessageBox.Show(this, "Error", "Render doesn't support this type of geometry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }


        }

        //渲染数据表中的所有行
        private void RenderFeatureDataTable(SharpMap.Data.FeatureDataTable table)
        {
            foreach (FeatureDataRow item in table.Rows)
            {
                RenderFeatureDataRow(item, g, Color.Blue, Color.Orange);
            }
        }

        

        //渲染选择ID对应的Geometry
        private void RenderElementByID(int _id,Color _PointColor,string _type,bool _refresh,Graphics g) 
        {
            
            SharpMap.Layers.VectorLayer query_lyr;
           
            if (_type == "Point")
                query_lyr = this.mapImage1.Map.GetLayerByName("Vertices_tmp") as SharpMap.Layers.VectorLayer;
            else
                query_lyr = this.mapImage1.Map.GetLayerByName("road") as SharpMap.Layers.VectorLayer;
            
            if (query_lyr == null) 
            { 
                throw (new ApplicationException("An attempt was made to read from a closed datasource")); 
            } 
 
            if (!query_lyr.DataSource.IsOpen) query_lyr.DataSource.Open(); 
 
            //get all the features in the query_lyr. 
            SharpMap.Data.FeatureDataSet ds = new SharpMap.Data.FeatureDataSet(); 
            query_lyr.DataSource.ExecuteIntersectionQuery(query_lyr.Envelope, ds); 
 
            query_lyr.DataSource.Close(); 
            if (ds == null) return; 
             
            
 
            //hignlight the geometry 
            //SharpMap.Layers.VectorLayer selectedLyr = new SharpMap.Layers.VectorLayer("selectedLyr");
            if (_type == "Point")
            {
                DataRow[] query = ds.Tables[0].Select("","id");
                //EnumerableRowCollection<DataRow> query = from id in ds.Tables[0].AsEnumerable()
                //                                         orderby id.Field<int>("id")
                //                                         select id;

                FeatureDataRow fdr = query.ElementAt(_id - 1) as FeatureDataRow;



                if (_refresh)
                {
                    mapImage1.Map.Center = fdr.Geometry as SharpMap.Geometries.Point;
                    mapImage1.Refresh();
                }
                RenderFeatureDataRow(fdr, g, _PointColor, Color.Orange);
                
                //selectedLyr.DataSource = new SharpMap.Data.Providers.GeometryProvider(fdr);

            }
            else if (_type == "PolyLine")
            {
                DataRow[] query = ds.Tables[0].Select("", "gid");
                //EnumerableRowCollection<DataRow> query = from id in ds.Tables[0].AsEnumerable()
                //                                         orderby id.Field<int>("gid")
                //                                         select id;
                FeatureDataRow fdr = query.ElementAt(_id - 1) as FeatureDataRow;
                
                var multiline = fdr.Geometry as SharpMap.Geometries.MultiLineString;
                var p = multiline.LineStrings[multiline.LineStrings.Count / 2].Vertices;

                if (_refresh)
                {
                    mapImage1.Map.Center = p[p.Count / 2];
                    mapImage1.Refresh();
                }
                RenderFeatureDataRow(fdr, g, _PointColor, Color.Orange);

            }
            #region 可以用添加图层的方法实现，但是每次都要刷新地图，太耗时间
            /*
            selectedLyr.Style.EnableOutline = true; 
            selectedLyr.Style.Outline = new Pen(Color.GreenYellow, 5);
            selectedLyr.Style.PointColor = new SolidBrush(_color);
            
            selectedLyr.Style.Fill = new SolidBrush(Color.Transparent); 
            //selectedLyr.Style.Fill = new SolidBrush(_color); 
            selectedLyr.SRID = 2450; 
 
            //set the zoom of map 
            this.mapImage1.Map.Layers.Add(selectedLyr); 
            //this.mapImage1.Map.ZoomToBox(ds.Tables[0][rowID].Geometry.GetBoundingBox());
            mapImage1.Map.GetMap();
            mapImage1.Refresh();
            */
            #endregion

        }


        //private void RenderNodeByGID_GDI(int _id, Color _color)
        //{
        //    SharpMap.Layers.VectorLayer query_lyr = this.mapImage1.Map.GetLayerByName("Vertices_tmp") as SharpMap.Layers.VectorLayer;
            

        //    if (query_lyr == null)
        //    {
        //        throw (new ApplicationException("An attempt was made to read from a closed datasource"));
        //    }

        //    if (!query_lyr.DataSource.IsOpen) query_lyr.DataSource.Open();

        //    //get all the features in the query_lyr. 
        //    SharpMap.Data.FeatureDataSet ds = new SharpMap.Data.FeatureDataSet();
        //    query_lyr.DataSource.ExecuteIntersectionQuery(query_lyr.Envelope, ds);
            
        //    query_lyr.DataSource.Close();
        //    if (ds == null) return;
        //    else
        //    {
        //        foreach (DataRow i in ds.Tables[0])
        //        {
        //            FeatureDataRow feature = i as FeatureDataRow;
        //            if(feature.Geometry.)
        //        }

        //    }
           

           
        //}

       


        private void StartAnimation()
        {           
            timer1.Interval = 2;
            timer1.Start();
        }
        private void PauseAnimation()
        {
            timer1.Stop();
        }
    

        #endregion
        //重定向数据连接，选择不同的数据源
        private void Redirect_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Redirect_cmb.SelectedIndex)
            {
                case 0: database = "postgis";
                    CurrentDB_tsl.Text = "Connection: //localhost/"+database;
                    break;
                case 1: database="postgis2";
                    CurrentDB_tsl.Text = "Connection: //localhost/" + database;
                    break;
                case 2:
                    database = "postgis3";
                    CurrentDB_tsl.Text = "Connection: //localhost/" + database;
                    break;
                case 3:
                    database = "postgis4";
                    CurrentDB_tsl.Text = "Connection: //localhost/" + database;
                    break;
                default:
                    break;
            }
            ConnStr = "Server=localhost"
        + ";DataBase=" + database
        + ";Port=5432"
        + ";Userid= postgres"
        + ";password=password"
        + ";Protocol=3;SSL=false;Pooling=true;MinPoolSize=1;MaxPoolSize=20;EnCoding=UNICODE;Timeout=15;SslMode=Disable";
        }

        //根据ID查找点
        private void FindGeometry_btn_Click_1(object sender, EventArgs e)
        {
            int i = -1;
            int.TryParse(NodeInput_tb.Text.Trim(), out i);
           
            if (i != -1)
            {
                if (Layers_clb.SelectedItem.ToString() == "vertices_tmp")
                    RenderElementByID(i, Color.Blue, "Point",true,g);
                else if (Layers_clb.SelectedItem.ToString() == "road")
                    RenderElementByID(i, Color.Blue, "PolyLine",true,g);
            }
        }
        //演示动画的时间动作
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timecounter < Nodes_Animation.Length)
            {
                RenderElementByID(Nodes_Animation[timecounter], Color.Blue, "Point",false,g);
                
            }

            if (timecounter < Links_Animation.Length)
            {
                RenderElementByID(Links_Animation[timecounter], Color.Blue, "PolyLine",false,g);
                Animation_trackbar.Value = timecounter;
                timecounter++;
            }

            else
            {
                timer1.Stop();
                timecounter = 0;
                Animation_trackbar.Value = 0;
                mapImage1.Refresh();
            }
        }
        //开始演示动画
        private void Start_btn_Click(object sender, EventArgs e)
        {
            Animation_trackbar.Maximum = Links_Animation.Length;
            StartAnimation();
        }
        //暂停动画
        private void Pause_btn_Click(object sender, EventArgs e)
        {
            PauseAnimation();
        }
        //停止动画
        private void Stop_btn_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Dispose();
            //mapImage1.Map.Layers.Clear();
            
            mapImage1.Refresh();
        }

        //获取图层_lyr与地理元素_geom的相交集合
        private SharpMap.Geometries.MultiPoint GetIntersections(SharpMap.Geometries.Geometry _geom, SharpMap.Layers.ILayer _lyr)
        {
            SharpMap.Data.FeatureDataSet ds = new SharpMap.Data.FeatureDataSet();
            //将多边形转换为GeoAPI的类型
            GisSharpBlog.NetTopologySuite.IO.WKTReader reader = new GisSharpBlog.NetTopologySuite.IO.WKTReader();
            GeoAPI.Geometries.IGeometry polygon = reader.Read(_geom.ToString());

            //得到_lyr中的所有地理元素并转换为GeoAPI的格式
            SharpMap.Layers.VectorLayer query_lyr;
            query_lyr = this.mapImage1.Map.GetLayerByName("Vertices_tmp") as SharpMap.Layers.VectorLayer;
            if (!query_lyr.DataSource.IsOpen) query_lyr.DataSource.Open();
            SharpMap.Data.FeatureDataSet ds2 = new SharpMap.Data.FeatureDataSet();
            query_lyr.DataSource.ExecuteIntersectionQuery(query_lyr.Envelope, ds2);
            query_lyr.DataSource.Close();
            if (ds2 == null) return null;
            DataRow[] query = ds2.Tables[0].Select("", "id");
            FeatureDataTable fdt = ds2.Tables[0];

            List<GisSharpBlog.NetTopologySuite.Geometries.Point> points = new List<GisSharpBlog.NetTopologySuite.Geometries.Point>();
            foreach (FeatureDataRow i in fdt)
            {
                GeoAPI.Geometries.IGeometry point = reader.Read(i.Geometry.ToString());
                points.Add(point as GisSharpBlog.NetTopologySuite.Geometries.Point);
            }
            GisSharpBlog.NetTopologySuite.Geometries.MultiPoint multipoints = new GisSharpBlog.NetTopologySuite.Geometries.MultiPoint(points.ToArray());
            //GeoAPI相交查询
            GisSharpBlog.NetTopologySuite.Geometries.MultiPoint Interpoints = (polygon.Intersection(multipoints)) as GisSharpBlog.NetTopologySuite.Geometries.MultiPoint;

            SharpMap.Geometries.MultiPoint mp = SharpMap.Converters.NTS.GeometryConverter.ToSharpMapGeometry(Interpoints) as SharpMap.Geometries.MultiPoint;
            //Execute boundingbox intersection query, geometry intersection is not well supported
            //(_lyr as SharpMap.Layers.VectorLayer).DataSource.ExecuteIntersectionQuery(_geom.GetBoundingBox(), ds);

            return mp;
            //SharpMap.Layers.VectorLayer laySelected = new SharpMap.Layers.VectorLayer("Selection");


            //laySelected.DataSource = new GeometryProvider(mp);
            //laySelected.Style.Fill = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow);
            //return laySelected;

        }

        //用GeoAPI为节点生成缓冲区
        private SharpMap.Geometries.Geometry CreateBuffer(int _id, double distance)
        {
            SharpMap.Layers.VectorLayer query_lyr;
            query_lyr = this.mapImage1.Map.GetLayerByName("Vertices_tmp") as SharpMap.Layers.VectorLayer;
            if (!query_lyr.DataSource.IsOpen) query_lyr.DataSource.Open();

            //get all the features in the query_lyr. 
            SharpMap.Data.FeatureDataSet ds = new SharpMap.Data.FeatureDataSet();
            query_lyr.DataSource.ExecuteIntersectionQuery(query_lyr.Envelope, ds);

            query_lyr.DataSource.Close();
            if (ds == null) return null;

            DataRow[] query = ds.Tables[0].Select("", "id");
            //EnumerableRowCollection<DataRow> query = from id in ds.Tables[0].AsEnumerable()
            //orderby id.Field<int>("id")

            //fdr是用于创建缓冲区的数据行，对应地图中的一个节点

            FeatureDataRow fdr = query.ElementAt(_id - 1) as FeatureDataRow;
            mapImage1.Map.Center = fdr.Geometry as SharpMap.Geometries.Point;

            //转换为NetTopologySuite可以识别的点
            GisSharpBlog.NetTopologySuite.IO.WKTReader reader = new GisSharpBlog.NetTopologySuite.IO.WKTReader();
            GeoAPI.Geometries.IGeometry point = reader.Read(fdr.Geometry.ToString());

            GeoAPI.Geometries.IGeometry BufArea_GeoAPI = point.Buffer(distance);

            //将GeoAPI产生的buffer转换为Sharpmap的格式
            SharpMap.Geometries.Geometry BufArea_SharpMap = SharpMap.Converters.NTS.GeometryConverter.ToSharpMapGeometry(BufArea_GeoAPI as GisSharpBlog.NetTopologySuite.Geometries.Geometry);
            return BufArea_SharpMap;
        }

        private void bufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpMap.Geometries.Geometry bufArea=null;
            try
            {
                bufArea = CreateBuffer(Convert.ToInt16(NodeInput_tb.Text.Trim()), Convert.ToDouble(BufferDistanceToolStripMenuItem.Text.Trim()));
            }
            catch { MessageBox.Show(this, "Make sure the NodeID(Find Node) and Buffer distance are in right format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            SharpMap.Geometries.BoundingBox box = bufArea.GetBoundingBox();
            mapImage1.Map.ZoomToBox(box);
            mapImage1.Refresh();
            //渲染缓冲区
            RenderGeometry(bufArea, "Polygon");
            
            SharpMap.Geometries.MultiPoint intersection = GetIntersections(bufArea, mapImage1.Map.Layers["vertices_tmp"]);

            // Create image.
            Image newImage = Image.FromFile(@"D:\Past Desktop\GeoHyperstar(for RSA)\GeoHyperstar\Ball_blue.png");

            if(intersection!=null)
            SharpMap.Rendering.VectorRenderer.DrawMultiPoint(g, intersection, newImage, 0.1f, new PointF(0,0), 0, mapImage1.Map);

            ////渲染相交点
            //if(intersection!=null)
            //    intersection.Render(g, mapImage1.Map);
            RenderElementByID(Convert.ToInt16(NodeInput_tb.Text.Trim()), Color.Red, "Point", false, g);
        }

        //查找节点时输入ID号后按下回车键的默认操作
        private void NodeInput_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                FindElement_btn.PerformClick();
        }

        #region 未使用的代码
        //用不同主题填充不同值范围
        private SharpMap.Styles.VectorStyle GetRoadStyle(SharpMap.Data.FeatureDataRow row)
        {
            SharpMap.Styles.VectorStyle style = new SharpMap.Styles.VectorStyle();
            int x = int.Parse(row["ID"].ToString());
            if (x <= 500)
            {
                style.Fill = Brushes.Green;
                return style;
            }
            else if (x <= 1000)
            {
                style.Fill = Brushes.Blue;
                style.Outline = Pens.Red;
                return style;
            }
            else
            {
                style.Fill = Brushes.Red;
                return style;
            }
        }
        //将地图图层按照道路等级分级,速度慢，可以考虑在数据库中分好层，然后导入sharpmap时设置各图层
        //的MinVisibility和MaxVisibility
        //分过层的数据在查询的时候需要遍历各数据分层，而不是在单一图层中查询。难度比较大。。。
        /*
         * Define DS;
         * Foreach (Layer i as VectorLayer in Map.Layers)
         * {
         *      i.ExecuteIntersects(Envelope,DS)
         * }
         * 
         * 
         */
        private SharpMap.Map ClassifiedRoadMap(SharpMap.Layers.Layer _mylyr, SharpMap.Map m) 
        {
            VectorLayer[] ClassifiedLayer=new VectorLayer[8];
            VectorLayer query_lyr = _mylyr as VectorLayer;
            if (!query_lyr.DataSource.IsOpen) query_lyr.DataSource.Open();

            //get all the features in the query_lyr. 
            SharpMap.Data.FeatureDataSet ds = new SharpMap.Data.FeatureDataSet();
            query_lyr.DataSource.ExecuteIntersectionQuery(query_lyr.Envelope, ds);
            
            query_lyr.DataSource.Close();
            if (ds == null) return null;
            Collection<SharpMap.Geometries.Geometry>[] GeomCols = new Collection<SharpMap.Geometries.Geometry>[8];
            
            for (int i = 1; i <= 8; i++)
            {
                GeomCols[i - 1] = new Collection<SharpMap.Geometries.Geometry>();
               
                if(i!=8)
                {
                    ClassifiedLayer[i - 1] = new VectorLayer("roadkind_"+i.ToString());
                    foreach(FeatureDataRow j in ds.Tables[0].Rows)
                    {
                        if (Convert.ToInt16(j["roadkind"]) == i)
                        {
                            GeomCols[i-1].Add(j.Geometry);
                        }
                    }
                }
                else
                {
                    ClassifiedLayer[i - 1] = new VectorLayer("roadkind_9");
                    foreach (FeatureDataRow j in ds.Tables[0].Rows)
                    {
                        if (Convert.ToInt16(j["roadkind"]) == 9)
                        {
                            GeomCols[i - 1].Add(j.Geometry);
                         
                        }
                    }
                }
                ClassifiedLayer[i - 1].DataSource = new SharpMap.Data.Providers.GeometryFeatureProvider(GeomCols[i-1]);
                ClassifiedLayer[i - 1].Style.Line = new System.Drawing.Pen(System.Drawing.Color.LightBlue, 1);
                ClassifiedLayer[i - 1].SRID = 2450;
            }
            for (int i = 1; i <= 8;i++ )
            {
                //设置roadkind_9的最大可是的比例尺，这里设置为ZoomtoExtent时比例尺的1/5，即地图放大5倍后才能看到
                if(i==8)
                    ClassifiedLayer[i-1].MaxVisible = fullzoom *  1 / 5;
            }

            m.Layers.Clear();
            foreach (var i in ClassifiedLayer)
            {
                m.Layers.Add(i);
            }
            return m;
        }

        #endregion
        //显示根据roadkind分层后的road图层
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassifiedRoadMap(mapImage1.Map.Layers["road"] as VectorLayer, mapImage1.Map);
            mapImage1.Map.ZoomToExtents();
            mapImage1.Refresh();
        }

        //设置线层颜色
        private void SetColr_Click(object sender, EventArgs e)
        {
            string lyr_name = Layers_clb.SelectedItem.ToString();
            var selctedlyr = this.mapImage1.Map.GetLayerByName(lyr_name) as SharpMap.Layers.VectorLayer;
            colorDialog1.ShowDialog(this);
            selctedlyr.Style.Line.Color = colorDialog1.Color;
            mapImage1.Refresh();
        }

     
        //隐藏未选中图层，刷新Map
        private void mapContext_refresh_Click(object sender, EventArgs e)
        {
            if (mapImage1.Map != null)
            {
                HashSet<string> Enabled = new HashSet<string>();
                for (int i = 0; i < Layers_clb.CheckedIndices.Count; i++)
                {
                    string lyrname = Layers_clb.Items[Layers_clb.CheckedIndices[i]].ToString();
                    Enabled.Add(lyrname);
                }
                foreach (var lyr in mapImage1.Map.Layers)
                {
                    if (!Enabled.Contains(lyr.LayerName))
                        lyr.Enabled = false;
                    else lyr.Enabled = true;
                }
                mapImage1.Refresh();
            }
            else { MessageBox.Show(this, "No layers in map,please add the layers first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void Layers_clb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < Layers_clb.Items.Count; i++)
                {
                    Rectangle ItemRect = Layers_clb.GetItemRectangle(i);
                    if (ItemRect.Contains(new Point(e.X, e.Y)))
                    {
                        Layers_clb.SelectedItem = Layers_clb.Items[i];
                        Layers_clb.ContextMenuStrip = setStyleContext;
                        break;
                    }
                    else
                        Layers_clb.ContextMenuStrip = null;
                }
            }
        }

        private void zoomToLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string lyr_name = Layers_clb.SelectedItem.ToString();
            var selctedlyr = this.mapImage1.Map.GetLayerByName(lyr_name) as SharpMap.Layers.VectorLayer;
            mapImage1.Map.ZoomToBox(selctedlyr.Envelope);
            mapImage1.Refresh();
        }

        private void zoomToExtentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapImage1.Map.ZoomToExtents();
            mapImage1.Refresh();
        }

        private void googleEarthItToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Enabled = new List<string>();
            foreach (var lyr in mapImage1.Map.Layers)
            {
                if (lyr.Enabled)
                {
                    Enabled.Add(lyr.LayerName);
                }
            }
            string path = @"D:\mymap.kml";
            GoogleEarthKML kml = new GoogleEarthKML(ConnStr, Enabled.ToArray(), path);
            kml.WriteKMLFile();
            ProcessStartInfo psInfNotepad = new ProcessStartInfo("notepad.exe", path);
            Process.Start(psInfNotepad);
            ProcessStartInfo psInfGE = new ProcessStartInfo(@"C:\Program Files\Google\Google Earth\client\googleearth.exe", path);
            Process.Start(psInfGE);
        }
        //鼠标进入时设置鼠标右键绑定，如果有激活的图层，则设置绑定，否则不绑定
        private void Map_MouseEnterEventHandler(object sender, EventArgs e)
        {
            int count=0;
            foreach (var lyr in mapImage1.Map.Layers)
            {
                if(lyr.Enabled)count++;
            }
            if (count == 0) mapImage1.ContextMenu = null;
            else mapImage1.ContextMenuStrip = mapContext;
        }

        private void RenderScanned_btn_Click(object sender, EventArgs e)
        {
            HashSet<int> GIDs = new HashSet<int>();
            //using (StreamReader sr = new StreamReader("..\\..\\NDHEU_scanned.txt"))
            using (StreamReader sr = new StreamReader("..\\..\\ScannedNodes.txt"))
            {
                while (sr.Peek() > 0)
                {
                    int i = int.Parse(sr.ReadLine());
                    GIDs.Add(i);
                }
            }
            foreach (int i in GIDs)
            {
                RenderElementByID(i, Color.FromArgb(120, 0, 0, 167), "Point", false, g);//渲染点，不刷新地图0,0,167
            }
        }

        private void RenderScannedLinks_btn_Click(object sender, EventArgs e)
        {
            HashSet<int> GIDs = new HashSet<int>();
            using (StreamReader sr = new StreamReader("..\\..\\ScannedLinks.txt"))
            {
                while (sr.Peek() > 0)
                {
                    int i = int.Parse(sr.ReadLine());
                    GIDs.Add(i);
                }
            }
            foreach (int i in GIDs)
            {
                RenderElementByID(i, Color.FromArgb(120, 0, 0, 167), "PolyLine", false, g);//渲染点，不刷新地图0,0,167
            }
        }

        private void SaveImage_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd=new SaveFileDialog();
            sfd.InitialDirectory = Path.GetFullPath("..\\..\\");
            //sfd.Filter = "emf file|*.emf";
            sfd.Title = "Save picture";
            if (sfd.ShowDialog()==DialogResult.OK)
            {
                using (FileStream fs = new FileStream(sfd.FileName, FileMode.OpenOrCreate))
                {
                    Metafile mf = mapImage1.Map.GetMapAsMetafile();
                    mf.Save(fs,ImageFormat.Tiff);
                }
            }
        }


    }
}
