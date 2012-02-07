using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkLib.Element;

namespace AddEventTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Network net = new Network();
            //Node i = new Node(1);
            //Node j = new Node(2);
   
            net.AllNodes.OnAddEvent += new NodeCol_List.AddEvent(OnAddEvent_Handler);
            //net.AllNodes.Add(i);
            //net.AllNodes.Add(j);
            foreach (int i in Enumerable.Range(0, 100))
            {
                net.AllNodes.Add(new Node(i, 0, 0));
            }
                Console.ReadKey(true);
        }

        static private void OnAddEvent_Handler(object sender, EventArgs e, Node i)
        {
            Console.WriteLine("Node "+ i.GID + " is Rendered.");
        }
    }

    

}
