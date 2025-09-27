using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class ProductionItem
    {
        public Item Product { get; set; }
        public int DaysProduced { get; set; }
        public bool AOCRepeat { get; set; }
        public bool AOCOneTime { get; set; }
        public bool Active { get; set; }

        public ProductionItem(Item product) 
        {
            Product = product;
            DaysProduced = 0;
        }
    }
}
