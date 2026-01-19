using Godot;

namespace Deuteros.Code.Objects.Interfaces
{
    public interface IPlanet
    {
        PlanetResource PlanetResources { get; set; }
        bool IsMoon { get; set; }
        bool ActiveMethanoid { get; set; }
        SpaceStation Station { get; set; }
        Enums.Planetoids PlanetId { get; set; }
        Enums.Planetoids MoonParentPlanetId { get; set; }
        Enums.Stars ParentStar { get; set; }
        public int ShuttleState { get; set; }
        public int StarShipState { get; set; }
    }
}