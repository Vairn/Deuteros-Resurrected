using Deuteros.Code.Objects.Interfaces;
using Deuteros.Code.Platform.Base;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deuteros.Code.Objects
{
    public class ACC
    {
        public Enums.StellarBodies Source { get; set; }
        public Enums.StellarBodies Destination { get; set; }

        public List<Enums.ItemTypes> SourceItems { get; set; }
        public List<Enums.ItemTypes> DestinationItems { get; set; }

        public Enums.ItemTypes CurrentSource { get; set; }
        public Enums.ItemTypes CurrentDestination { get; set; }

        public bool Active { get; set; }
        public bool CycleMode { get; set; }

        public IShip Ship { get; set; }
    }
}