using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deuteros.Code.Enums;

namespace Deuteros.Code.Objects
{
	public class Asteroid
	{
		public static List<Enums.ItemTypes> ResourceTypeList = new List<Enums.ItemTypes>() { Enums.ItemTypes.titanium, Enums.ItemTypes.aluminium, Enums.ItemTypes.carbon, ItemTypes.paladium, ItemTypes.platinum, ItemTypes.silver, ItemTypes.silica };
		public static List<int> ResourceMassList = new List<int>() { 50, 100, 250, 5000, 10000, 25000, 60000 };
		public static List<string> ResourceMassNameList = new List<string>() { "Small", "Small", "Small", "Medium", "Large", "Large", "Large" };

		public Enums.ItemTypes Type { get; set; }
		public int Mass { get; set; }
		public string MassName { get; set; }
		public int DayCount { get; set; }

		//Crappy temporary code
		public static Asteroid ScanAsteroids(Asteroid currentAsteroid)
		{
			if (currentAsteroid != null)
			{
				//Check if we should change the current asteroid
				var asteroidChangeChance = Random.Shared.Next(0, 10-currentAsteroid.DayCount);

				//It's a new one!
				if (asteroidChangeChance == 0)
					currentAsteroid = GenerateAsteroid();
				else
					currentAsteroid.DayCount++;

				return currentAsteroid;
			}
			else
			{
				var asteroidChance = Random.Shared.Next(0, 5);

				if (asteroidChance == 0)
				{
					return GenerateAsteroid();
				}
				else
				{
					return null;
				}
			}
		}

		public static Asteroid GenerateAsteroid()
		{
			var asteroidType = Random.Shared.Next(0, 6);
			var asteroidSize = Random.Shared.Next(0, 6);

			var newAsteroid = new Asteroid(); newAsteroid.Type = ResourceTypeList[asteroidType];
			newAsteroid.Mass = ResourceMassList[asteroidSize];
			newAsteroid.MassName = ResourceMassNameList[asteroidSize];
			return newAsteroid;
		}
	}
}