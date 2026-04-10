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
		public Enums.StellarBodies PlanetId { get; set; }
		public Factory Factory { get; set; }
		public int ShuttleState { get; set; }
		public int StarShipState { get; set; }


		public SpaceStation(Enums.StellarBodies planet_id)
		{
			Resources = new StationResource();
			Factory = new Factory();
			PlanetId = planet_id;
		}
	}
}
