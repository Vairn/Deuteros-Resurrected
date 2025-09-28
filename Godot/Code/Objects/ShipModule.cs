using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class ShipModule
    {
        public Enums.Module_Types ModuleType { get; set; }
        public Enums.ItemTypes ItemStored { get; set; }
        public int ItemCount { get; set; }
    }
}
