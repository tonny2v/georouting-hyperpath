using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.KML;
using ProjNet.CoordinateSystems;
using Npgsql;
using System.Data;
using System.IO;
using ProjNet.CoordinateSystems.Transformations;

namespace MapDisplayModule
{
    class GoogleEarthKML
    {
        geDocument doc = new geDocument();
        string ConnStr;
        string[] lyrNames;
        string path;
        public GoogleEarthKML(string _ConnStr, string[] _lyrName,string _path) 
        {
            ConnStr = _ConnStr;
            lyrNames = _lyrName;
            path = _path;
        }
        public bool WriteKMLFile()
        {
            
            byte[] bytesToWrite = GenKML().ToKML();
            try
            {
                File.WriteAllBytes(path, bytesToWrite);
                return true;
            }
            catch (IOException){ return false; }
        }

        private geKML GenKML()
        {
            List<geCoordinates> linepoints = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnStr))
            {


                //加入几种不同的Style以供使用

                // p >= 0.5
                //Lets add a Line somewhere too...
                geStyle myLineStyle1 = new geStyle("myLineStyle1");
                myLineStyle1.LineStyle = new geLineStyle();
                myLineStyle1.LineStyle.Color.SysColor = System.Drawing.Color.Green;
                myLineStyle1.LineStyle.Width = 8;  //This may or may not work, depends on the end user's video card
                doc.StyleSelectors.Add(myLineStyle1);
                // 0.2 =< p < 0.5
                geStyle myLineStyle2 = new geStyle("myLineStyle2");
                myLineStyle2.LineStyle = new geLineStyle();
                myLineStyle2.LineStyle.Color.SysColor = System.Drawing.Color.Blue;
                myLineStyle2.LineStyle.Width = 5;  //This may or may not work, depends on the end user's video card
                doc.StyleSelectors.Add(myLineStyle2);
                // p < 0.2
                geStyle myLineStyle3 = new geStyle("myLineStyle3");
                myLineStyle3.LineStyle = new geLineStyle();
                myLineStyle3.LineStyle.Color.SysColor = System.Drawing.Color.Red;
                myLineStyle3.LineStyle.Width = 3;  //This may or may not work, depends on the end user's video card
                doc.StyleSelectors.Add(myLineStyle3);

                geStyle normalLightBlue = new geStyle("normalLightBlue");
                normalLightBlue.LineStyle = new geLineStyle();
                normalLightBlue.LineStyle.Color.SysColor = System.Drawing.Color.LightBlue;
                normalLightBlue.LineStyle.Width = 1.5f;  //This may or may not work, depends on the end user's video card
                doc.StyleSelectors.Add(normalLightBlue);
                geStyle normalRed = new geStyle("normalRed");
                normalRed.LineStyle = new geLineStyle();
                normalRed.LineStyle.Color.SysColor = System.Drawing.Color.Red;
                normalRed.LineStyle.Width = 3;  //This may or may not work, depends on the end user's video card
                doc.StyleSelectors.Add(normalRed);

                NpgsqlCommand cmd = null;
                NpgsqlDataReader reader = null;
                //查询每个图层，并将它们逐个添加到kml中
                foreach (string lyrName in lyrNames)
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    cmd = new NpgsqlCommand("select gid,ST_ASEWKT(ST_Transform(the_geom,4326)),choice_possibility from " + lyrName + ";", conn);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int gid = reader.GetInt32(reader.GetOrdinal("gid"));
                        string geomwkt = reader.GetString(reader.GetOrdinal("st_asewkt"));
                        string[] points = geomwkt.Substring(geomwkt.IndexOf("((") + 2, geomwkt.Length - geomwkt.IndexOf("((") - 4).Split(',');
                        linepoints = new List<geCoordinates>();
                        foreach (string pointstr in points)
                        {
                            double[] point = new double[2];
                            var pointcor = pointstr.Split(' ');
                            //经度
                            point[0] = Convert.ToDouble(pointcor[0]);
                            //纬度
                            point[1] = Convert.ToDouble(pointcor[1]);

                            linepoints.Add(new geCoordinates(new geAngle90(point[1]), new geAngle180(point[0])));
                        }

                        gePlacemark pmLine = new gePlacemark();
                        if (lyrName == "hyperpath_lyr")
                        {
                            double choice_possibility = reader.GetDouble(reader.GetOrdinal("choice_possibility"));
                            if (choice_possibility >= 0.5)
                                pmLine.StyleUrl = "myLineStyle1";
                            else if (choice_possibility >= 0.2)
                                pmLine.StyleUrl = "myLineStyle2";
                            else pmLine.StyleUrl = "myLineStyle3";
                            pmLine.Name = "ID: " + gid.ToString() + " Possibility: " + Math.Round(choice_possibility, 2).ToString();
                            pmLine.Description = "This is a " + lyrNames + "link";
                        }
                        else
                        {
                            if(lyrName=="popath_lyr")
                                pmLine.StyleUrl = "normalLightBlue";
                            else if (lyrName=="regretpath_lyr")
                                pmLine.StyleUrl = "normalRed";
                            pmLine.Name = "ID: " + gid.ToString();
                            pmLine.Description = "This is a " + lyrName + " link";
                        }
                        //gePlacemark hyperpath = new gePlacemark();
                        pmLine.Geometry = new geLineString(linepoints);
                        pmLine.ID = (doc.Features.Count + 1).ToString();
                        doc.Features.Add(pmLine);
                    }
                }
                reader.Close();
                cmd.Dispose();
                geKML kml = new geKML(doc);
                return kml;
            }
        }

        private geKML GetKML(int gid, string wkt, double p)
        {
            // Use a Document as the root of the KML
            geDocument doc = new geDocument();
            doc.Name = "My Root Document";

            //Create a Placemark to put in the document
            //This placemark is going to be a point
            //but it could be anything in the Geometry class
            gePlacemark pm = new gePlacemark();

            //Create some coordinates for the point at which
            //this placemark will sit. (Lat / Lon)
            geCoordinates coords = new geCoordinates(
                new geAngle90(37.422067),
            new geAngle180(-122.084437));

            //Create a point with these new coordinates
            gePoint point = new gePoint(coords);

            //Assign the point to the Geometry property of the
            //placemark.
            pm.Geometry = point;

            //Now lets add some other properties to our placemark
            pm.Name = "My Placemark";
            pm.Snippet = "This is where I put my Placemark";
            pm.Description =
                "I wonder where this is...";

            //Finally, add the placemark to the document
            doc.Features.Add(pm);

            //Now that we have our document, lets create our KML
            geKML kml = new geKML(doc);


            return kml;
        }

        #region 这里涉及到利用Proj4类库进行坐标转换，但是没有成功，所以在代码中没有用到，而是用postgis内提供的转换
        private IProjectedCoordinateSystem CreateUtmProjection(int utmZone)
        {
            CoordinateSystemFactory cFac = new ProjNet.CoordinateSystems.CoordinateSystemFactory();

            IEllipsoid ellipsoid = cFac.CreateFlattenedSphere("WGS 84",
   6378137, 298.257223563, LinearUnit.Metre);
            IHorizontalDatum datum = cFac.CreateHorizontalDatum("WGS_1984",
             DatumType.HD_Geocentric, ellipsoid, null);
            IGeographicCoordinateSystem gcs = cFac.CreateGeographicCoordinateSystem(
                                 "WGS 84", AngularUnit.Degrees, datum,
                                 PrimeMeridian.Greenwich,
                                 new AxisInfo("Lon", AxisOrientationEnum.East),
                                 new AxisInfo("Lat", AxisOrientationEnum.North));
            //Create UTM projection
            List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
            parameters.Add(new ProjectionParameter("latitude_of_origin", 0));
            parameters.Add(new ProjectionParameter("central_meridian", -183 + 6 * utmZone));
            parameters.Add(new ProjectionParameter("scale_factor", 0.9996));
            parameters.Add(new ProjectionParameter("false_easting", 500000));
            parameters.Add(new ProjectionParameter("false_northing", 0.0));
            IProjection projection = cFac.CreateProjection(
            "Transverse Mercator", "Transverse_Mercator", parameters);
            return cFac.CreateProjectedCoordinateSystem(
                     "WGS 84 / UTM zone " + utmZone.ToString() + "N", gcs,
            projection, LinearUnit.Metre,
            new AxisInfo("East", AxisOrientationEnum.East),
            new AxisInfo("North", AxisOrientationEnum.North));
        }

        private ICoordinateSystem CreateCoordinateSystemFromWKT(string wkt)
        {
            CoordinateSystemFactory cFac = new CoordinateSystemFactory();
            return cFac.CreateFromWkt(wkt);
        }

        private double[] Proj2WGS(int utm, double[] point)
        {
            //Create zone UTM 32N projection
            IProjectedCoordinateSystem utmProj = CreateUtmProjection(54);
            //Create geographic coordinate system (lets just reuse the CS from the projection)
            IGeographicCoordinateSystem geoCS = utmProj.GeographicCoordinateSystem;
            //Create transformation
            CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();

            ICoordinateTransformation transform =
               ctFac.CreateFromCoordinateSystems(utmProj, geoCS);

            double[] CRSCoor = transform.MathTransform.Transform(point);
            return CRSCoor;

        }
        #endregion

    }
}
