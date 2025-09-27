using Deuteros.Code.Platform.Screens;
using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class Factory
    {
        //The item being produced, and the number of days it has been in production
        public List<ProductionItem> ProductionQueue { get; set; }
        public Staff Builder { get; set; }
        public bool AOC { get; set; }

        public Factory() 
        {
            ProductionQueue = new List<ProductionItem>();
        }

        public ProductionItem CurrentProductionItem()
        {
            return ProductionQueue.SingleOrDefault(T => T.Active);
        }

        public void IncrementCurrentProd()
        {
            if (CurrentProductionItem() != null)
            {
                CurrentProductionItem().DaysProduced += 1;
            }
        }

        public void ClearQueue()
        {
            ProductionQueue.Clear();
        }
    }
}