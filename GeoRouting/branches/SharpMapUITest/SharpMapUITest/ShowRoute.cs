using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpMap.Layers;
using NetworkLib;
using Npgsql;

namespace MapDisplayModule
{
    public partial class ShowRoute : Form
    {
        public SharpMap.Layers.VectorLayer Route_lyr;
        public static SharpMap.Layers.VectorLayer O_lyr;
        public static SharpMap.Layers.VectorLayer D_lyr;

        public ShowRoute()
        {
            InitializeComponent();
        }

        private void ShowRoute_Load(object sender, EventArgs e)
        {
            RouteName_label.Text = Route_lyr.LayerName + " (" + O_lyr.LayerName + "-->" + D_lyr.LayerName + ")";
            O_lyr.Style.PointColor = new SolidBrush(Color.Red);
            D_lyr.Style.PointColor = new SolidBrush(Color.Blue);
            mapImage2.Map.Layers.Add(O_lyr);
            mapImage2.Map.Layers.Add(D_lyr);
            mapImage2.Map.Layers.Add(Route_lyr);
            mapImage2.Map.ZoomToExtents();
            mapImage2.Map.BackColor = System.Drawing.Color.White;
            //mapImage2.ActiveTool = SharpMap.Forms.MapImage.Tools.Pan;
            mapImage2.Refresh();
        }

        private void ShowRoute_SizeChanged(object sender, EventArgs e)
        {

        }

        private void mapImage2_SizeChanged(object sender, EventArgs e)
        {
            if (mapImage2.Size.Width != 0 && mapImage2.Size.Height != 0)
                mapImage2.Refresh();
        }

    }
}
