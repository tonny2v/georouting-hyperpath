using System;
using System.Collections.Generic;
using NetworkLib2.TransNetwork.Concept;
using System.IO;


//TopoBuilder只负责构建网络的拓扑结构，并且读取原始数据到各元素中，但不对算法所需的任何字段进行初始化

namespace NetworkLib2.TransNetwork.TopoBuilder
{
    public class DataBuilder 
    {
        public DataBuilder() { Net = new TNetwork(); }
        public string DataSource { get; set; }
        public TNetwork Net { get; set; }
        public virtual void GetNetwork() { }
    }

    public class FileBuilder: DataBuilder
    {
        bool FirstLineAsData { get; set; }
        public FileBuilder(string _filepath,bool _firstLineAsData) 
        {
            DataSource = _filepath;
            FirstLineAsData = _firstLineAsData;
        }

        public override void GetNetwork()
        {
            //不重复的节点id集合，用于判断节点是否已经添加
            HashSet<string> nodeids = new HashSet<string>();
            StreamReader sr = new StreamReader(DataSource);
            string[] oneline = null;
            /*数据格式必须是
             
             * Linkdata.txt
             (direction 若为1, follow,若为2, opposite, 若为0，双向，在文本文件中无此考虑，统一设置为-1, 路段的gid与id相同)
             gid    source  target   time   maxdelay   direction
              1        1       2     22.1    13.3        -1
             ...      ...     ...     ...     ...       ...
             */
            //如果第一行不是数据行，则跳过第一行
            int[] colName_indexarray = new int[11];
            string colName=string.Empty;
            if (!FirstLineAsData)
            {
                colName = sr.ReadLine();
            }
            //找出关键列的索引
            string[] colName_array = null;
            if (DataSource.EndsWith("csv"))
                colName_array = colName.Split(',');
            else if (DataSource.EndsWith("txt"))
                colName_array = colName.Split('\t');

            //搜索列名（第一行），为列名索引数组赋值
            for (int i=0;i<colName_array.Length;i++)
            {
                switch (colName_array[i])
                {
                    case "gid":
                        colName_indexarray[0] = i;
                        break;
                    case "source":
                        colName_indexarray[1] = i;
                        break;
                    case "target":
                        colName_indexarray[2] = i;
                        break;
                    case "length":
                        colName_indexarray[3] = i;
                        break;
                    case "expected":
                        colName_indexarray[4] = i;
                        break;
                    case "time":
                        colName_indexarray[5] = i;
                        break;
                    case "maxdelay":
                        colName_indexarray[6] = i;
                        break;
                    case "direction":
                        colName_indexarray[7] = i;
                        break;
                    case "mean":
                        colName_indexarray[8] = i;
                        break;
                    case "n":
                        colName_indexarray[9] = i;
                        break;
                    case "geomwkt":
                        colName_indexarray[10] = i;
                        break;
                }
            }
            
            while (sr.Peek() > 0)
            {
                //判断文本文件格式并读取数据
                if (DataSource.EndsWith("csv")) //csv文件数据列geomwkt中包含逗号，用以Tab为分隔的txt文件更好
                    oneline = sr.ReadLine().Split(',');
                else if (DataSource.EndsWith("txt"))
                    oneline = sr.ReadLine().Split('\t');
               
                
                if (!nodeids.Contains(oneline[colName_indexarray[1]]))
                {
                    Node onenode = new Node(oneline[colName_indexarray[1]]);
                    nodeids.Add(oneline[colName_indexarray[1]]);//source id 
                    Net.AddV(onenode);
                }

                if (!nodeids.Contains(oneline[colName_indexarray[2]]))
                {
                    Node onenode = new Node(oneline[colName_indexarray[2]]);
                    nodeids.Add(oneline[colName_indexarray[2]]);//target id 
                    Net.AddV(onenode);
                }

                RawLinkDataRow onedata = new RawLinkDataRow(oneline,colName_indexarray);
                Link onelink = new Link(onedata);
                onelink.ID = onedata.gid;
                onelink.From = Net.Vertices[onedata.fromid];
                onelink.To = Net.Vertices[onedata.toid];
                Net.AddE(onelink);
                
            }
        }

    }

    public class SyntheticBuilder:DataBuilder
    {

        public enum NetworkType { GridNetwork, RadialNetwork, RandomNetwork };
        static int nodenumber;
        static int linknumber;

        public TNetwork GenNetwork(NetworkType _type, int _nodenum, int _linknum, int mint, int maxt, int mind, int maxd)
        {
            nodenumber = _nodenum;
            linknumber = _linknum;
            Net = null;
            switch (_type)
            {
                case NetworkType.GridNetwork:
                    break;
                case NetworkType.RadialNetwork:
                    break;
                case NetworkType.RandomNetwork:
                    Net = RandomNetwork(mint, maxt, mind, maxd);
                    break;
                default:
                    break;
            }
            return Net;
        }

        TNetwork RandomNetwork(int t_min, int t_max, int d_min, int d_max)
        {
            TNetwork net = new TNetwork();
            //节点
            for (int i = 1; i <= nodenumber; i++)
            {
                Node Vi = new Node(i.ToString());
                net.AddV(Vi);
            }
            //边
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 1; i <= linknumber; i++)
            {
                Link Ei = new Link(i.ToString());
                Ei.From = net.Vertices[((int)rand.Next(1, nodenumber + 1)).ToString()];
                Ei.To = net.Vertices[((int)rand.Next(1, nodenumber + 1)).ToString()];
                Ei.TravelTime = t_min + (t_max - t_min) * rand.NextDouble();
                Ei.Delay = d_min + (d_max - d_min) * rand.NextDouble();
                net.AddE(Ei);
            }
            return net;
        }
    }

    public class DataBaseBuilder:DataBuilder
    {
        string ConnectionStr { get; set; }
    }

    public class BinaryDataBuilder:DataBuilder
    {}

  
}
