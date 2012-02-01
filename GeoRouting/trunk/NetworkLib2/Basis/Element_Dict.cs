using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkLib2.Basis
{

    public class Element_Dict <K, T>: Dictionary<K, T>
    {
        public delegate void AddEvent(object sender, EventArgs e, T i);

        public event AddEvent OnAddEvent;

        public new void Add(K key, T i)
        {
            base.Add(key, i);
            if (OnAddEvent != null)
                OnAddEvent(this, new EventArgs(), i);
        }

        public override string ToString()
        {
            string s = string.Empty;
            if (this.Count != 0)
            {
                foreach (K i in this.Keys)
                {
                    if (s != string.Empty)
                        s = s + "," + i;
                    else s += i;
                }
                return s;
            }
            else return "Empty";
        }
    }
}
