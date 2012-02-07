//#define HPDInfinity

using System;
using System.Collections.Generic;
using System.Data;

namespace NetworkLib.Element
{
    [Serializable]
    public partial class Vertex
    {
        int id;
        List<Edge> InEdges;
        List<Edge> OutEdges;
        public Vertex()
        { }
        public Vertex(int _id)
        {
            id = _id;
            InEdges = new List<Edge>();
            OutEdges = new List<Edge>();
        }
    }

    [Serializable]
    public class Edge
    {
        public Vertex From{get;set;}
        public Vertex To { get; set; }
        public Edge() 
        {
            From = new Vertex();
            To = new Vertex();
        }
        public Edge(Vertex _from,Vertex _to) 
        {
            From = _from;
            To = _to;
        }
    
    }

    [Serializable]
    public class Node:ICloneable
    {
        List<Link> inLinks;
        List<Link> outLinks;
        int gid;//与实体对应
        double x;
        double y;

        //可读写属性
        public double Yi { get; set; }
        public double OptHeuristic { get; set; }
        public double PessHeuristic { get; set; }
        public double RegretModifiedHeuristic { get; set; }
        public bool HasProcessed { get; set; }
        public double Ui { get; set; }
        public double UiAddCa { get; set; }
        public double Ui_Infinity { get; set; }
        public double Fi { get; set; }
        public int NextNodeID { get; set; }
        public int ID { get; set; }
        public int SubID { get; set; }
        public int MinID { get; set; }

        //节点流出的Hyperpath
        public List<Link> H { get; set; }
        

        //用于处理0延误的情形
        public List<Link> H_Infinity { get; set; }
        //只读属性
        public int GID { get { return gid; } }
        public List<Link> InLinks { get { return inLinks; } }
        public List<Link> OutLinks { get { return outLinks; } }
        //点坐标，用于backward penalty
        public double X { get { return x; } }
        public double Y { get { return y; } }
        //距离O点的距离，用于衡量是否为backward
        public double DistanceFromO { get; set; }
        public Node() { }
        public Node(int nodeid,double _x,double _y)
        {
            MinID = -1;
            gid = nodeid;
            ID = gid;//用已存在的网络初始化的时候ID与Gid相同，但提取网络后就不同了
            x = _x;
            y = _y;
            inLinks = new List<Link>();
            outLinks = new List<Link>();
            H = null;
            H_Infinity = null;
            Yi = 0;
            Fi = 0;
            Ui = double.PositiveInfinity;
            UiAddCa = double.PositiveInfinity;
            Ui_Infinity = double.PositiveInfinity;
            NextNodeID = -1;
            OptHeuristic = double.PositiveInfinity;
            PessHeuristic = double.PositiveInfinity;
            RegretModifiedHeuristic = double.PositiveInfinity;
            HasProcessed = false;
        }

        public void AddInLink(Link i)
        {
            i.To = this;
            inLinks.Add(i);
        }
        public void AddOutLink(Link i)
        {
            i.From = this;
            outLinks.Add(i);
        }
        public override string ToString()
        {
            return GID.ToString();
        }

        public object Clone()
        {
            Node n = new Node();
            n.DistanceFromO = this.DistanceFromO;
            n.gid = this.gid;
            n.x = this.x;
            n.y = this.y;
            n.inLinks = new List<Link>();
            n.outLinks = new List<Link>();
            n.H = null;
            n.H_Infinity = null;
            n.Yi = 0;
            n.Fi = 0;
            n.Ui = double.PositiveInfinity;
            n.Ui_Infinity = double.PositiveInfinity;
            n.NextNodeID = -1;
            n.OptHeuristic = double.PositiveInfinity;
            n.PessHeuristic = double.PositiveInfinity;
            //n.RegretModifiedHeuristic = double.PositiveInfinity;
            n.RegretModifiedHeuristic = this.RegretModifiedHeuristic;//子网络不做Assign regret，而是延用父网络的
            n.HasProcessed = false;
            n.ID = this.ID;
            return n;
            //throw new NotImplementedException();
        }
    }

    [Serializable]
    public class Link:ICloneable
    {
        /// <summary>
        /// Build Link Class
        /// </summary>
        public static double INFINITE = 999999999D;
        //public static double INFINITE = double.PositiveInfinity;
        //通过datarow得到的数据
        int gid;
        int fromgid;
        int togid;
        double traveltime_fixed;
        double fa;
        int direction;


        //可读写属性
        public double UiAddCa { get; set; }
        public double UiAddCa_Infinity { get; set; }
        public double Pa { get; set; }
        public bool Hasbeenremoved { get; set; }
        public bool InPathsCollection { get; set; }
        public bool HasUpdated { get; set; }
        public int ID { get; set; }
        public int SubID { get; set; }
        //用在计算local search的regret中
        public double Cost { get; set; }
        //用在为各条路段分配Regret值中
        public double Regret { get; set; }
        public double TravelTime_variable { get; set; }//广义费用
        

        public Node From { get; set; }
        public Node To { get; set; }

        //只读属性
        public int GID { get { return gid; } }
        public int FromGID { get { return fromgid; } }
        public int ToGID { get { return togid; } }
        public double TravelTime_Fixed { get { return traveltime_fixed; } }
        public double Fa { get { return fa; } }
        public int Direction { get { return direction; } }

        //构造函数
        public Link() { }
        public Link(int id, DataRow _attrdr, double maxdelaylevel, bool follow)
        {
            UiAddCa = double.PositiveInfinity;
            UiAddCa_Infinity = double.PositiveInfinity;
            Pa = 0;
            Hasbeenremoved = false;
            InPathsCollection = false;
            HasUpdated = false;
            ID = id;
            Regret = double.PositiveInfinity;
            gid = Convert.ToInt32(_attrdr["gid"]);
            traveltime_fixed = Convert.ToDouble(_attrdr["time"]);
            TravelTime_variable = traveltime_fixed;
            //考虑最大延误为0的情形

#if HPDInfinity
            if (maxdelaylevel*(double)_attrdr["maxdelay"] == 0) fa = double.PositiveInfinity;
#else
            if (maxdelaylevel * Convert.ToDouble(_attrdr["maxdelay"]) == 0) fa = Link.INFINITE;
#endif
            else fa = 1 / (maxdelaylevel * Convert.ToDouble(_attrdr["maxdelay"]));
            direction = Convert.ToInt32(_attrdr["direction"]);
            if (follow)
            {
                fromgid = Convert.ToInt32(_attrdr["source"]);
                togid = Convert.ToInt32(_attrdr["target"]);
            }
            else
            {
                fromgid = Convert.ToInt32(_attrdr["target"]);
                togid = Convert.ToInt32(_attrdr["source"]);
            }
        }
        //创建浅表副本,值独立，引用相同
        public Link ShallowCopy()
        {
            return this.MemberwiseClone() as Link;
        }
        

        public override string ToString()
        {
            return ID.ToString();
        }

        //实现IClonable
        public object Clone()
        {
            Link l = new Link();
            l.Cost = this.Cost;
            l.direction = this.direction;
            l.fa = this.fa;
            l.From = null;
            l.fromgid = this.fromgid;
            l.gid = this.gid;
            l.Hasbeenremoved = false;
            l.HasUpdated = false;
            l.ID = this.ID;
            l.InPathsCollection = true;
            l.Pa = this.Pa;
            //l.Regret = double.PositiveInfinity;
            l.Regret = this.Regret;//子网络不做assign regret，延用父网络的
            l.To = null;
            l.togid = this.togid;
            l.traveltime_fixed = this.traveltime_fixed;
            l.TravelTime_variable = this.traveltime_fixed;
            l.UiAddCa = double.PositiveInfinity;
            l.UiAddCa_Infinity = double.PositiveInfinity;
            return l;
        }
    }

    [Serializable]
    public class LinkCol_List : List<Link>
    {
        public LinkCol_List() { }

        public delegate void AddEvent(object sender, EventArgs e, Link i);

        public event AddEvent OnAddEvent;

        public new void Add(Link i)
        {
            base.Add(i);
            if (OnAddEvent != null)
                OnAddEvent(this, new EventArgs(), i);
        }

    }

    [Serializable]
    public class NodeCol_List : List<Node>
    {
        public NodeCol_List() { }

        public delegate void AddEvent(object sender, EventArgs e, Node i);

        public event AddEvent OnAddEvent;

        public new void Add(Node i)
        {
            base.Add(i);
            if (OnAddEvent != null)
                OnAddEvent(this, new EventArgs(), i);
        }
    }

    [Serializable]
    public class Network : IDisposable,ICloneable
    {
        public NodeCol_List AllNodes;
        public LinkCol_List AllLinks;

        public Network()
        {
            AllNodes = new NodeCol_List();
            AllLinks = new LinkCol_List();
        }

        //由已经读取好的节点集和路段集构建网络(静态网络)
        public Network(NodeCol_List allnodes, LinkCol_List alllinks)
        {
            //AllNodes = new NodeCol_List();
            //AllLinks = new LinkCol_List();
            AllNodes= allnodes;
            AllLinks = alllinks;
        }

        #region ReDesign
        public void AddNode(double x,double y) 
        {
            Node node = new Node(AllNodes.Count+1, x, y);
            AllNodes.Add(node);
        }
        //虽然暂时没有用到，但是在理念上非常有用
        //先建立节点表,然后以此方式来建立路段表
        public void AddLink(int fromid, int toid)
        {
            Link link = new Link();
            link.ID = AllLinks.Count + 1;
            link.From = AllNodes[fromid - 1];
            AllNodes[fromid - 1].OutLinks.Add(link);

            link.To = AllNodes[toid - 1];
            AllNodes[toid - 1].InLinks.Add(link);
            AllLinks.Add(link);
        }
        public void RemoveNode(Node node)
        {
            //移除节点的关联边
            foreach (Link i in node.InLinks)
            {
                AllLinks.Remove(i);
            }
            foreach (Link i in node.OutLinks)
            {
                AllLinks.Remove(i);
            }
            //移除节点本身
            AllNodes.Remove(node);
        }
        public void RemoveLink(Link link)
        {
            //检查link的端点是否有其他link连接，若没有，则同时移除节点
            if (link.From.OutLinks.Count == 1 || link.From.InLinks.Count == 0)
                AllNodes.Remove(link.From);
            if (link.To.InLinks.Count == 1 || link.To.OutLinks.Count == 0)
                AllNodes.Remove(link.To);
            AllLinks.Remove(link);
        }
        #endregion

        public Network CreateSubNetwork(Link[] LinksInvolved)
        {
            //首先对父网络中的每个路段和节点进行深度复制
            //然后根据GID的对应关系重建网络拓扑
            //新的子网络中的ID在构建网络拓扑的时候重新生成
            //注意ID是父子网络的关联，而SubID是子网络中的索引
            Network NewNetwork = new Network();
            HashSet<Node> NodesInvolved = new HashSet<Node>();
            Dictionary<int, int> ID_SubID = new Dictionary<int, int>();
            foreach (Link l in LinksInvolved)
            {
                //这里的NodesInvolved和LinksInvolved都是对原网络的引用
                NodesInvolved.Add(l.From);
                NodesInvolved.Add(l.To);
            }
            //添加网络节点
            foreach (Node node in NodesInvolved)
            {
                Node newnode = node.Clone() as Node;
                newnode.SubID = NewNetwork.AllNodes.Count + 1;
                ID_SubID.Add(newnode.ID, newnode.SubID);
                NewNetwork.AllNodes.Add(newnode);
            }
           
            foreach (Link link in LinksInvolved)
            {
                Link newlink = link.Clone() as Link;
                int fromid = ID_SubID[link.From.ID];//用对应的id来索引，因为id和Gid是不同的
                newlink.From = NewNetwork.AllNodes[fromid - 1];
                NewNetwork.AllNodes[fromid - 1].OutLinks.Add(newlink);
                int toid = ID_SubID[link.To.ID];
                newlink.To = NewNetwork.AllNodes[toid - 1];
                NewNetwork.AllNodes[toid - 1].InLinks.Add(newlink);
                newlink.SubID = NewNetwork.AllLinks.Count + 1;
                NewNetwork.AllLinks.Add(newlink);
            }
            return NewNetwork;
        }

        public override string ToString()
        {
            return AllNodes.Count.ToString() + " Nodes, " + AllLinks.Count.ToString() + " Links";
        }

        #region IDisposable 成员

        public void Dispose()
        {
            AllLinks = null;
            AllNodes = null;
        }

        #endregion

        #region ICloneable 成员
        //通过Create subnetwork来复制网络(与父网络完全相同的子网络)
        public object Clone()
        {
            Network newnet = new Network();
            newnet = this.CreateSubNetwork(this.AllLinks.ToArray());
            return newnet;
        }

        #endregion
    }
}
