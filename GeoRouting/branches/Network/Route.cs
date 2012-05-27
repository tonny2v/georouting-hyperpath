using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLib.Element;

namespace NetworkLib
{
    [Serializable]
    public class Route
    {
        public List<Link> Links { get; set; }

        public double Reliability { get; set; }
        
        
        public Route() { }
       
        public Route(Link [] _links) 
        {
            Links = new List<Link>();
            Reliability = -1;
            Links.AddRange(_links);
        }


        public void Add(Link x) 
        {
            Links.Add(x);
        }

        public Link[] ToArray()
        {
            return Links.ToArray();
        }
        /// <summary>
        /// 返回以数字集表示的路径,该路径ID为母网络中的ID
        /// </summary>
        /// <returns></returns>
        public List<int> ToIDs() 
        {
            List<int> IDs = new List<int>();
            foreach (Link i in Links)
            {
                IDs.Add(i.ID);
            }
            return IDs;
        }
        /// <summary>
        /// 返回数字集表示的路径，该路径ID为子网络中的ID
        /// </summary>
        /// <returns></returns>
        public List<int> ToSubIDs()
        {
            List<int> SubIDs = new List<int>();
            foreach (Link i in Links)
            {
                SubIDs.Add(i.SubID);
            }
            return SubIDs;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            for (int i=0;i<Links.Count; i++)
            {
                s.Append(Links[i].ToString()+",");
                if (i!=Links.Count-1)
                    s.Append("\n");
            }
            return s.ToString();
        }
    }
}
