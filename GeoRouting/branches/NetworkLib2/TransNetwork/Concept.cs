using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NetworkLib2.Basis;

namespace NetworkLib2.TransNetwork.Concept
{
    [Serializable]
    public class Node:Basis.Element
    {
        public double OptHeuristic { get; set; }

        public double PessHeuristic { get; set; }

        public Element_Dict<string,Link> OutLinks { get; set; }

        public Element_Dict<string, Link> InLinks { get; set; }

        public override string ToString()
        {
            return ID;
        }

        public Node(string _id)
        {
            ID = _id;
            InLinks = new Element_Dict<string,Link>();
            OutLinks = new Element_Dict<string,Link>();
        }

        public string SP_Next { get; set; }

        public bool HasProcessed { get; set; }
    }

    [Serializable]
    public class Link:Basis.Element
    {
        public static double INFINITE = 999999999D;

        //原始数据
        public RawLinkDataRow RawData;

        //To be initialized when building GNetwork
        public Node From { get; set; }
        public Node To { get; set; }


        //To be initialized before running algorithm
        public double TravelTime { get; set; }
        public double Delay { get; set; }
        public double UiAddCa { get; set; }
        public double Pa { get; set; }
        public bool Hasbeenremoved { get; set; }
        public bool InPathsCollection { get; set; }
        public bool HasUpdated { get; set; }
        public double Fa { get; set; }

        //构造函数
        public Link(string _id)
        {
            ID = _id;
        }
        public Link(RawLinkDataRow _rawdata)
        {
            RawData = _rawdata;
            TravelTime = RawData.traveltime;
            Delay = RawData.maxdelay;
        }

        public override string ToString()
        {
            return ID + ": " + From.ID + "->" + To.ID;
        }
    }

    [Serializable]
    public struct RawLinkDataRow
    {
        //从外部数据文件或数据库中得到的数据
        public readonly string gid;
        public readonly string fromid;
        public readonly string toid;
        public double traveltime;
        public double maxdelay;
        public readonly int direction;
        public readonly double mean;
        public readonly string geomwkt;
        public double n;
        //Probe data
        public double expected;
       
        public readonly double length;

        //用从数据库中读取的数据
        public RawLinkDataRow(DataRow dr)
        {
            gid = dr["gid"].ToString();
            fromid = dr["source"].ToString();
            toid = dr["target"].ToString();
            traveltime = (double)dr["traveltime"];
            maxdelay = (double)dr["maxdelay"];
            direction = (int)dr["direction"];
            mean = (double)dr["mean"];
            n = (int)dr["n"];
            geomwkt = (string)dr["geomwkt"];
            length = (double)dr["length"];
            expected = (double)dr["expected"];
        }
        
        //以文本方式读取的数据,colName_array为gid,fromid,toid,traveltime,maxdelay和direction在string数组中的索引
        public RawLinkDataRow(string[] str_array,int[] colName_array)
        {
            
            gid = str_array[colName_array[0]];
            fromid = str_array[colName_array[1]];
            toid = str_array[colName_array[2]];
            length=Convert.ToDouble(str_array[colName_array[3]]);
            expected = Convert.ToDouble(str_array[colName_array[4]]);
            traveltime = Convert.ToDouble(str_array[colName_array[5]]);
            maxdelay = Convert.ToDouble(str_array[colName_array[6]]);
            direction = Convert.ToInt32(str_array[colName_array[7]]);
            mean = Convert.ToDouble(str_array[colName_array[8]]);
            n = Convert.ToInt32(str_array[colName_array[9]]);
            geomwkt = str_array[colName_array[10]];
           
        }

       

    }

    [Serializable]
    public class TNetwork : Basis.Network<string, Node, Link>
    {

        #region ReDesign

        public override void AddV(Node _Node)
        {
            Vertices.Add(_Node.ID, _Node);
        }

        public override void AddE(Link _Link)
        {

            Edges.Add(_Link.ID, _Link);
            _Link.From.OutLinks.Add(_Link.ID, _Link);
            _Link.To.InLinks.Add(_Link.ID, _Link);

        }

        public override void RemoveV(Node _Node)
        {
            //移除节点的关联边
            foreach (Link i in _Node.InLinks.Values)
            {
                Vertices.Remove(i.ID);
            }
            foreach (Link i in _Node.OutLinks.Values)
            {
                Vertices.Remove(i.ID);
            }
            //移除节点本身
            Vertices.Remove(_Node.ID);
        }

        public override void RemoveE(Link _Link)
        {
            //检查link的端点是否有其他link连接，若没有，则同时移除节点
            if (_Link.From.OutLinks.Count == 1 || _Link.From.InLinks.Count == 0)
                Vertices.Remove(_Link.From.ID);
            if (_Link.To.InLinks.Count == 1 || _Link.To.OutLinks.Count == 0)
                Vertices.Remove(_Link.To.ID);
            Edges.Remove(_Link.ID);
        }

        #endregion


        public override string ToString()
        {
            return Vertices.Count.ToString() + " Nodes, " + Edges.Count.ToString() + " Links";
        }

        public override void RemoveV(string _id)
        {
            //移除节点的关联边
            foreach (Link i in Vertices[_id].InLinks.Values)
            {
                Vertices.Remove(i.ID);
            }
            foreach (Link i in Vertices[_id].OutLinks.Values)
            {
                Vertices.Remove(i.ID);
            }
            //移除节点本身
            Vertices.Remove(Vertices[_id].ID);
        }

        public override void RemoveE(string _id)
        {
            //检查link的端点是否有其他link连接，若没有，则同时移除节点
            if (Edges[_id].From.OutLinks.Count == 1 || Edges[_id].From.InLinks.Count == 0)
                Vertices.Remove(Edges[_id].From.ID);
            if (Edges[_id].To.InLinks.Count == 1 || Edges[_id].To.OutLinks.Count == 0)
                Vertices.Remove(Edges[_id].To.ID);
            Vertices.Remove(Edges[_id].ID);
        }

        public override void Clear()
        {
            Vertices.Clear();
            Vertices.Clear();
        }

       

        #region IDisposable 成员

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
