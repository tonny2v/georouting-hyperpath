#define DHS
//#define HP

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Windows.Forms.DataVisualization.Charting;
//using System.Collections.Generic;


namespace WinChart
{
    public partial class ChartForm : Form
    {

        public double[] YPoints1 { get; set; }
        public double[] YPoints2 { get; set; }
        public ChartForm()
        {
            InitializeComponent();
        }

        private void ChartForm_Load(object sender, EventArgs e)
        {
            //if (YPoints1.Length == 0 && YPoints2.Length == 0)
            //{
            //    //string[] strs = Console.ReadLine().Split('\t', ',', ' ');
            //    //System.Threading.Thread.Sleep(5000);
            //    //Plot(new int[]{1,3,5,63,3,6,32,2,56,25,62,5});
            //}
            //else PlotGeoHyperStar();
        }

        public void PlotGeoHyperStar()
        {
            chart1.Series.Clear();

            chart1.Series.Add("Ui");
#if DHS
            chart1.Series.Add("Ui + Ca + Hj");
#elif HP
                        chart1.Series.Add("Ui + Ca");
#endif

            for (int pointIndex = 0; pointIndex < YPoints1.Length; pointIndex++)
            {
                double yValue = YPoints1[pointIndex];
                chart1.Series["Ui"].Points.AddY(yValue);
            }
            chart1.Series["Ui"].ChartType = SeriesChartType.FastLine;

            for (int pointIndex = 0; pointIndex < YPoints2.Length; pointIndex++)
            {
                double yValue = YPoints2[pointIndex];
#if DHS
                chart1.Series["Ui + Ca + Hj"].Points.AddY(yValue);
#elif HP
                            chart1.Series["Ui + Ca"].Points.AddY(yValue);
#endif
            }

#if DHS
            chart1.Series["Ui + Ca + Hj"].ChartType = SeriesChartType.FastPoint;
            chart1.Titles.Add("Searching in DHS algorithm");
#elif HP
                        chart1.Series["Ui + Ca"].ChartType = SeriesChartType.FastPoint;
                        chart1.Titles.Add("Searching in HP algorithm");
#endif
        }

        public void Plot(object T, string serialName, string SPName, string ChartAreaName)
        {
            //if (ChartAreaName != "")
            //{
            //    chart1.ChartAreas.Add(ChartAreaName);
            chart1.ChartAreas["ChartArea1"].AxisY.Minimum = Double.NaN;
            //}
            if (chart1.Titles.Count == 0)
            {
                chart1.Titles.Add(new Title(SPName));
                chart1.Titles[0].Font = new Font("Times New Roman", 16f);
            }
            try
            {
                chart1.Series.Remove(chart1.Series["Series1"]);
            }
            catch { }
            chart1.Series.Add(serialName);
            if (T.GetType().IsArray)
            {
                Array m = T as Array;
                for (int i = 0; i < m.Length; i++)
                {
                    var a = m.GetValue(i);
                    chart1.Series[serialName].Points.Add(Convert.ToDouble(a));

                }
                chart1.Series[serialName].ChartType = SeriesChartType.Point;
            }
        }

        public void Plot2(List<double> m, string serialName, string SPName, string ChartAreaName)
        {

            //if (ChartAreaName != "")
            //{
            //    chart1.ChartAreas.Add(ChartAreaName);
            chart1.ChartAreas["ChartArea1"].AxisY.Minimum = Double.NaN;
            //chart1.ChartAreas["ChartArea1"].AxisY.Minimum = 0;
            //chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 1;
            //}
            if (chart1.Titles.Count == 0)
            {
                chart1.Titles.Add(new Title(SPName));
                chart1.Titles[0].Font = new Font("Times New Roman", 16f);
            }
            try
            {
                chart1.Series.Remove(chart1.Series["Series1"]);
            }
            catch { }
            chart1.Series.Add(serialName);

            for (int i = 0; i < m.Count; i++)
            {
                var a = m[i];
                chart1.Series[serialName].Points.Add(Convert.ToDouble(a));

            }
            chart1.Series[serialName].ChartType = SeriesChartType.Point;
        }
    }
}
