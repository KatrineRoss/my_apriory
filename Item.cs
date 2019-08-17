using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApriory
{
    public class Item : IComparable<Item>
    {
        #region Public Properties

        public string Name { get; set; }
        public double Support { get; set; }
        
        public string[] ProductSet { get; set; }

        #endregion

        #region IComparable

        public int CompareTo(Item other)
        {
            return Support.CompareTo(other.Support);
        }

        #endregion
    }
}
