using Godot;
using System;

namespace Deuteros.Code.Objects
{
    public partial class SpaceStation
    {
        public StationResource Resources { get; set; }
        public int BuildParts { get; set; }
        public int Type { get; set; }
        public bool Built { get; set; }
        public int PlanetId { get; set; }
        public Factory Factory { get; set; }

        public SpaceStation()
        {
            Resources = new StationResource();
            Factory = new Factory();
        }
    }
}