using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Deuteros.Code.Enums;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class Store
    {
        public Dictionary<ItemTypes, int> Items { get; set; }

        public Store()
        {
            Items = new Dictionary<ItemTypes, int>();
        }

        public int this[ItemTypes itemType]
        {
            get
            {
                //TODO DEBUG
                if (GameCore.SingletonInstance.InfiniteResources)
                    return 50000;

                return Items.ContainsKey(itemType) ? Items[itemType] : 0;
            }
            set 
            {
                if (Items.ContainsKey(itemType))
                    Items[itemType] = value;
                else
                    Items.Add(itemType, value);
            }
        }
    }
}