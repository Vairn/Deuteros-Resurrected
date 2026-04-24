using Deuteros.Code.Objects;
using Deuteros.Code.Objects.GameData;
using Deuteros.Code.Objects.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using static Deuteros.Code.Enums;

namespace Deuteros.Code
{
    public partial class CoreData
    {
        public SaveFile ActiveSaveFile { get; set; }
        public static BaseData StaticGameData { get; set; }

        public Item GetItem(Enums.ItemTypes itemType)
        {
            return ActiveSaveFile.BaseGameData.ItemList.First(T => T.ItemType == itemType);
        }

        public string GetNextPersonName()
        {
            if (ActiveSaveFile.NextPersonIndex >= ActiveSaveFile.BaseGameData.PersonNames.Count()) ActiveSaveFile.NextPersonIndex = 1;
            return ActiveSaveFile.BaseGameData.PersonNames[ActiveSaveFile.NextPersonIndex++];
        }

        public IEnumerable<Item> GetAllActiveItems()
        {
            return ActiveSaveFile.BaseGameData.ItemList.Where(T => T.Locked == false && (T.Research == null || T.Research.Researched));
        }

        public static SaveFile CreateNewSaveFile()
        {
            var newGameSave = new SaveFile();
            newGameSave.BaseGameData = StaticGameData;
            newGameSave.GameConfig = new Config();
            newGameSave.CurrentPlanet = Enums.StellarBodies.earth;
            newGameSave.Unlocks = new List<Enums.Game_Unlocks>();
            newGameSave.Ships = new List<IShip>();
            newGameSave.CurrentDay = 0;
            newGameSave.GameConfig.ShuttleRefuelThreshold = 50;
            newGameSave.GameConfig.IOSRefuelThreshold = 200;
            newGameSave.NextPersonIndex = Random.Shared.Next(StaticGameData.PersonNames.Count() + 1);

            return newGameSave;
        }

        public static void CreateBaseGameData()
        {
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data") || 1 == 1)
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Data");

                StaticGameData = new BaseData();

                StaticGameData.Planets = new Dictionary<Enums.StellarBodies, Objects.Interfaces.IPlanet>();
                StaticGameData.Stars = new Dictionary<StellarBodies, Star>();
                StaticGameData.ResourceLevels_Survey_Multiplier = new Dictionary<Enums.ItemTypes, int>();
                StaticGameData.ResourceRate_Per_Derrick = new Dictionary<Enums.ItemTypes, int>();
                StaticGameData.PersonNames = new List<string>() { "None","Jones","Jackson","Straker","Collins","Johnson","Emerson",
                    "Benson","Brubaker","Nilson","Cummins","Edberg","Fischer","Lasky",
                    "Sheppard","Zapasnik","Delaney","Rimmer","Olson","Gregory","Adamson",
                    "Prescott","Palmer","Blake","Devlin","Bean","Sweeney","Bates","Mannion",
                    "Matusiak","Thomson","Scott","Langer","James","Urlich","Seth","Rogers",
                    "Zuccker","Foster","Allen","Loakes","Peterson","Edmunds","Rooney","Polanski",
                    "Quinn","Wheeler","Lister","Andrews","Dawson","Goldberg","Spilaney","Singh",
                    "Taylor","Hunter","Jarre","Clarke","Ash","Lyons","Wright","Cousins","Lennox",
                    "Biggs","Galagher","Cooper","Yuen","Sykes","Sellers","Fowler","Anderson","Moore",
                    "Lovell","Zaranoff","Dalton","Berry","Morgan","Nimitz","Hall","Tindel","Cavell",
                    "Redman","Blunket","Raphael","Morse","Kingston","Floyd","Thackray","Chan","Quigley",
                    "Deering","Keegan","Packer","Chandi","Price","Curtis","Sharma","Dodd","Rosso","Arnold",
                    "Turner","Wells","Nipper","Campbell","Corrigan","Grant","Appleby","Foreman","Lee","Smith",
                    "Hill","Dexter","Phillips","Bishop","Haggerty","Walsh","Green","Cushing","Harris",
                    "Darcy","Farquhar","Townsend","Booker","Ould","Hobbs","Templer","Metcalfe","Davis",
                    "White","Garner","Finch","Pablov","Rigsby","Ford","Lyons","Ashforth","Richards",
                    "Gibbon","Snipe","Schmidt","Dempsey","Nauls","Cohen","Proctor","Skupski",
                    "Docherty","Yamahata","Barber","Ives","Jansen","Callan","Wilkes","Tkaczuk",
                    "Andreas","Nichols","Ingram","Connors","Nunn","Blood","Sommers","Nelson",
                    "Lander","Suchon","Pinky","Vaccaro","Gill","Lloyd","Dade","Matthews","Forsyth","Goode"
                };

                #region ItemList

                StaticGameData.ItemList = new List<Item>();

                #region Items

                var unknownitem = new Item();
                unknownitem.FullName = "";
                unknownitem.ItemCategory = Enums.ItemCategory.item;
                unknownitem.ItemType = Enums.ItemTypes.aluminium;
                unknownitem.Mass = 2000 ;
                unknownitem.ToolPod = false;
                unknownitem.Locked = true;

                unknownitem.Research = new ResearchItem(Enums.ItemTypes.none, 1, 1);
                unknownitem.Research.Researched = false;
                unknownitem.Research.Locked = true;

                StaticGameData.ItemList.Add(unknownitem);

                var derrick = new Item();
                derrick.FullName = "Resource Mining Rig";
                derrick.ShortName = "Derrick";
                derrick.ItemCategory = Enums.ItemCategory.item;
                derrick.ItemType = Enums.ItemTypes.derrick;
                derrick.Mass = 8;
                derrick.ToolPod = true;

                derrick.Research = new ResearchItem(Enums.ItemTypes.derrick, 2, 1);
                derrick.Research.Researched = true;
                derrick.Research.Locked = false;
                derrick.Research.ResearchPercentageComplete = 100;
                derrick.Research.ResearchOrder = 1;

                derrick.Locked = false;
                derrick.OrbitOnly = false;
                derrick.BuildRequirements = new List<BuildRequirement>();
                derrick.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 3));
                derrick.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 4));
                derrick.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 1));

                StaticGameData.ItemList.Add(derrick);

                var shuttlechas = new Item();
                shuttlechas.FullName = "Shuttle Chassis";
                shuttlechas.ShortName = "S Chassis";
                shuttlechas.ItemCategory = Enums.ItemCategory.item;
                shuttlechas.ItemType = Enums.ItemTypes.s_chassis;
                shuttlechas.Mass = 130;

                shuttlechas.Research = new ResearchItem(Enums.ItemTypes.s_chassis, 3, 1);
                shuttlechas.Research.Locked = false;

                shuttlechas.Locked = true;
                shuttlechas.OrbitOnly = false;
                shuttlechas.BuildRequirements = new List<BuildRequirement>();
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 20));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 50));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 35));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 10));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 15));

                StaticGameData.ItemList.Add(shuttlechas);

                var shuttledrive = new Item();
                shuttledrive.FullName = "Shuttle Drive Unit";
                shuttledrive.ShortName = "S Drive";
                shuttledrive.ItemCategory = Enums.ItemCategory.item;
                shuttledrive.ItemType = Enums.ItemTypes.s_drive;
                shuttledrive.Mass = 20;

                shuttledrive.Research = new ResearchItem(Enums.ItemTypes.s_drive, 4, 1);
                shuttledrive.Research.Locked = false;
                shuttledrive.Research.ResearchMultiplier = 96;
                shuttledrive.Research.ResearchValue = 32;

                shuttledrive.Locked = true;
                shuttledrive.OrbitOnly = false;
                shuttledrive.BuildRequirements = new List<BuildRequirement>();
                shuttledrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 6));
                shuttledrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 10));
                shuttledrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 4));

                StaticGameData.ItemList.Add(shuttledrive);

                var ofFrame = new Item();
                ofFrame.FullName = "Orbital Factory Section";
                ofFrame.ShortName = "OF Frame";
                ofFrame.ItemCategory = Enums.ItemCategory.item;
                ofFrame.ItemType = Enums.ItemTypes.of_frame;
                ofFrame.Mass = 250;
                ofFrame.ToolPod = true;
                ofFrame.ToolPodSingular = true;

                ofFrame.Research = new ResearchItem(Enums.ItemTypes.of_frame, 6, 1);
                ofFrame.Research.Locked = false;

                ofFrame.Locked = true;
                ofFrame.OrbitOnly = false;
                ofFrame.BuildRequirements = new List<BuildRequirement>();
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 55));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 80));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 50));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 25));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 40));

                StaticGameData.ItemList.Add(ofFrame);

                var supplyPod = new Item();
                supplyPod.FullName = "Supply Pod";
                supplyPod.ShortName = "Supply Pod";
                supplyPod.ItemCategory = Enums.ItemCategory.item;
                supplyPod.ItemType = Enums.ItemTypes.supply_pod;
                supplyPod.Mass = 4;

                supplyPod.Research = new ResearchItem(Enums.ItemTypes.supply_pod, 7, 1);
                supplyPod.Research.Locked = false;

                supplyPod.Locked = true;
                supplyPod.OrbitOnly = false;
                supplyPod.BuildRequirements = new List<BuildRequirement>();
                supplyPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                supplyPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                supplyPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                StaticGameData.ItemList.Add(supplyPod);

                var toolPod = new Item();
                toolPod.FullName = "Tool and Equipment Mounting";
                toolPod.ShortName = "Tool Pod";
                toolPod.ItemCategory = Enums.ItemCategory.item;
                toolPod.ItemType = Enums.ItemTypes.tool_pod;
                toolPod.Mass = 4;

                toolPod.Research = new ResearchItem(Enums.ItemTypes.tool_pod, 8, 1);
                toolPod.Research.Locked = false;

                toolPod.Locked = true;
                toolPod.OrbitOnly = false;
                toolPod.BuildRequirements = new List<BuildRequirement>();
                toolPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                toolPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                toolPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                StaticGameData.ItemList.Add(toolPod);

                var cryoPod = new Item();
                cryoPod.FullName = "Cryogenic Holding Pod";
                cryoPod.ShortName = "Cryo Pod";
                cryoPod.ItemCategory = Enums.ItemCategory.item;
                cryoPod.ItemType = Enums.ItemTypes.cryo_pod;
                cryoPod.Mass = 4;

                cryoPod.Research = new ResearchItem(Enums.ItemTypes.cryo_pod, 9, 1);
                cryoPod.Research.Locked = false;

                cryoPod.Locked = true;
                cryoPod.OrbitOnly = false;
                cryoPod.BuildRequirements = new List<BuildRequirement>();
                cryoPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                cryoPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                cryoPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                StaticGameData.ItemList.Add(cryoPod);

                var pulseLaser = new Item();

                pulseLaser.FullName = "Pulse Blast Laser";
                pulseLaser.ShortName = "Blaser";
                pulseLaser.ItemCategory = Enums.ItemCategory.item;
                pulseLaser.ItemType = Enums.ItemTypes.pulse_blaster_laser;
                pulseLaser.Mass = 750;

                pulseLaser.Research = new ResearchItem(Enums.ItemTypes.pulse_blaster_laser, 10, 3);

                pulseLaser.Locked = true;
                pulseLaser.OrbitOnly = true;
                pulseLaser.BuildRequirements = new List<BuildRequirement>();
                pulseLaser.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 120));
                pulseLaser.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 30));
                pulseLaser.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.hed_fuel, 600));

                StaticGameData.ItemList.Add(pulseLaser);

                var iChassis = new Item();
                iChassis.FullName = "I.O.S Chassis";
                iChassis.ShortName = "I Chassis";
                iChassis.ItemCategory = Enums.ItemCategory.item;
                iChassis.ItemType = Enums.ItemTypes.i_chassis;
                iChassis.Mass = 650;

                iChassis.Research = new ResearchItem(Enums.ItemTypes.i_chassis, 11, 2);

                iChassis.Locked = true;
                iChassis.OrbitOnly = true;
                iChassis.BuildRequirements = new List<BuildRequirement>();
                iChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 100));
                iChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 250));
                iChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 175));
                iChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 50));
                iChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 75));

                StaticGameData.ItemList.Add(iChassis);

                var iDrive = new Item();
                iDrive.FullName = "I.O.S Drive Unit";
                iDrive.ShortName = "I Drive";
                iDrive.ItemCategory = Enums.ItemCategory.item;
                iDrive.ItemType = Enums.ItemTypes.i_drive;
                iDrive.Mass = 95;

                iDrive.Research = new ResearchItem(Enums.ItemTypes.i_drive, 12, 2);

                iDrive.Locked = true;
                iDrive.OrbitOnly = true;
                iDrive.BuildRequirements = new List<BuildRequirement>();
                iDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 30));
                iDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 50));
                iDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 15));

                StaticGameData.ItemList.Add(iDrive);

                var gChassis = new Item();
                gChassis.FullName = "S.C.G. Chassis";
                gChassis.ItemCategory = Enums.ItemCategory.item;
                gChassis.ItemType = Enums.ItemTypes.g_chassis;
                gChassis.Mass = 1685;

                gChassis.Research = new ResearchItem(Enums.ItemTypes.g_chassis, 13, 3);

                gChassis.Locked = true;
                gChassis.OrbitOnly = true;
                gChassis.BuildRequirements = new List<BuildRequirement>();
                gChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 250));
                gChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 600));
                gChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 400));
                gChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 185));
                gChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 100));
                gChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 100));
                gChassis.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 50));

                StaticGameData.ItemList.Add(gChassis);

                var starDrive = new Item();
                starDrive.FullName = "S.C.G. Drive Unit";
                starDrive.ItemCategory = Enums.ItemCategory.item;
                starDrive.ItemType = Enums.ItemTypes.star_drive;
                starDrive.Mass = 265;

                starDrive.Research = new ResearchItem(Enums.ItemTypes.star_drive, 14, 3);

                starDrive.Locked = true;
                starDrive.OrbitOnly = true;
                starDrive.BuildRequirements = new List<BuildRequirement>();
                starDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 50));
                starDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 100));
                starDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 30));
                starDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 50));
                starDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 25));
                starDrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 10));

                StaticGameData.ItemList.Add(starDrive);

                var acc = new Item();
                acc.FullName = "Auto Cargo Computer";
                acc.ShortName = "A.C.C.";
                acc.ItemCategory = Enums.ItemCategory.item;
                acc.ItemType = Enums.ItemTypes.a__c__c;
                acc.Mass = 8;
                acc.ToolPod = true;

                acc.Research = new ResearchItem(Enums.ItemTypes.a__c__c, 16, 3);

                acc.Locked = true;
                acc.OrbitOnly = false;
                acc.BuildRequirements = new List<BuildRequirement>();
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 1));
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                StaticGameData.ItemList.Add(acc);

                var aoc = new Item();
                aoc.FullName = "Auto Operations Computer";
                aoc.ShortName = "A.O.C.";
                aoc.ItemCategory = Enums.ItemCategory.item;
                aoc.ItemType = Enums.ItemTypes.a__o__c;
                aoc.Mass = 8;

                aoc.Research = new ResearchItem(Enums.ItemTypes.a__o__c, 17, 3);

                aoc.Locked = true;
                aoc.OrbitOnly = true;
                aoc.BuildRequirements = new List<BuildRequirement>();
                aoc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 4));
                aoc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                aoc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 2));
                aoc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 1));

                StaticGameData.ItemList.Add(aoc);

                var BandAid = new Item();
                BandAid.FullName = "Installation Repair Equip";
                BandAid.ShortName = "Bandaid";
                BandAid.ItemCategory = Enums.ItemCategory.item;
                BandAid.ItemType = Enums.ItemTypes.bandaid;
                BandAid.Mass = 150;
                BandAid.ToolPod = true;

                BandAid.Research = new ResearchItem(Enums.ItemTypes.bandaid, 18, 3);

                BandAid.Locked = true;
                BandAid.OrbitOnly = true;
                BandAid.BuildRequirements = new List<BuildRequirement>();
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 30));

                StaticGameData.ItemList.Add(BandAid);

                var SelfDestruct = new Item();
                SelfDestruct.FullName = "Self Destruct Mechanism";
                SelfDestruct.ShortName = "S.D.M.";
                SelfDestruct.ItemCategory = Enums.ItemCategory.item;
                SelfDestruct.ItemType = Enums.ItemTypes.s__d__m;
                SelfDestruct.Mass = 9;

                SelfDestruct.Research = new ResearchItem(Enums.ItemTypes.s__d__m, 19, 3);

                SelfDestruct.Locked = true;
                SelfDestruct.OrbitOnly = true;
                SelfDestruct.BuildRequirements = new List<BuildRequirement>();
                SelfDestruct.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 5));
                SelfDestruct.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));
                SelfDestruct.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 1));
                SelfDestruct.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 2));

                StaticGameData.ItemList.Add(SelfDestruct);

                var HydraulicGrapple = new Item();
                HydraulicGrapple.FullName = "Hydraulic Grapple";
                HydraulicGrapple.ShortName = "Grapple";
                HydraulicGrapple.ItemCategory = Enums.ItemCategory.item;
                HydraulicGrapple.ItemType = Enums.ItemTypes.grapple;
                HydraulicGrapple.Mass = 5;
                HydraulicGrapple.ToolPod = true;

                HydraulicGrapple.Research = new ResearchItem(Enums.ItemTypes.grapple, 20, 3);

                HydraulicGrapple.Locked = true;
                HydraulicGrapple.OrbitOnly = true;
                HydraulicGrapple.BuildRequirements = new List<BuildRequirement>();
                HydraulicGrapple.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 2));
                HydraulicGrapple.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                HydraulicGrapple.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                StaticGameData.ItemList.Add(HydraulicGrapple);

                var DFCC = new Item();
                DFCC.FullName = "Drone Fleet Control Computer";
                DFCC.ShortName = "D.F.C.C.";
                DFCC.ItemCategory = Enums.ItemCategory.item;
                DFCC.ItemType = Enums.ItemTypes.d__f__c__c;
                DFCC.Mass = 8;
                DFCC.ToolPod = true;

                DFCC.Research = new ResearchItem(Enums.ItemTypes.d__f__c__c, 21, 3);

                DFCC.Locked = true;
                DFCC.OrbitOnly = true;
                DFCC.BuildRequirements = new List<BuildRequirement>();
                DFCC.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                DFCC.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                DFCC.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 1));
                DFCC.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));
                DFCC.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 2));
                DFCC.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 1));

                StaticGameData.ItemList.Add(DFCC);

                var AMA = new Item();
                AMA.FullName = "Asteroid Mining Attachment";
                AMA.ShortName = "A.M.A.";
                AMA.ItemCategory = Enums.ItemCategory.item;
                AMA.ItemType = Enums.ItemTypes.a__m__a;
                AMA.Mass = 124;
                AMA.ToolPod = true;

                AMA.Research = new ResearchItem(Enums.ItemTypes.a__m__a, 22, 3);

                AMA.Locked = true;
                AMA.OrbitOnly = true;
                AMA.BuildRequirements = new List<BuildRequirement>();
                AMA.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 6));
                AMA.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 70));
                AMA.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 10));
                AMA.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 30));
                AMA.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 2));
                AMA.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 5));
                AMA.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 1));

                StaticGameData.ItemList.Add(AMA);

                var Hyperlight = new Item();
                Hyperlight.FullName = "Hyperlight Travel";
                Hyperlight.ShortName = "Hyperlight";
                Hyperlight.ItemCategory = Enums.ItemCategory.item;
                Hyperlight.ItemType = Enums.ItemTypes.hyperlight;
                Hyperlight.Mass = 124;

                Hyperlight.Research = new ResearchItem(Enums.ItemTypes.hyperlight, 23, 3);

                Hyperlight.Locked = true;
                Hyperlight.OrbitOnly = true;
                Hyperlight.BuildRequirements = null;

                StaticGameData.ItemList.Add(Hyperlight);

                var MTX = new Item();
                MTX.FullName = "Mass Tranceiver";
                MTX.ShortName = "M.T.X.";
                MTX.ItemCategory = Enums.ItemCategory.item;
                MTX.ItemType = Enums.ItemTypes.m__t__x;
                MTX.Mass = 722;

                MTX.Research = new ResearchItem(Enums.ItemTypes.m__t__x, 24, 3);

                MTX.Locked = true;
                MTX.OrbitOnly = true;
                MTX.BuildRequirements = new List<BuildRequirement>();
                MTX.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 500));
                MTX.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 82));
                MTX.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 100));
                MTX.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 40));

                StaticGameData.ItemList.Add(MTX);

                var MFL = new Item();
                MFL.FullName = "Methanoid Fusion Laser";
                MFL.ShortName = "FuzLaser";
                MFL.ItemCategory = Enums.ItemCategory.item;
                MFL.ItemType = Enums.ItemTypes.m__f__l;
                MFL.Mass = 25;

                MFL.Research = new ResearchItem(Enums.ItemTypes.m__f__l, 25, 3);

                MFL.Locked = true;
                MFL.OrbitOnly = true;
                MFL.BuildRequirements = new List<BuildRequirement>();
                MFL.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 5));
                MFL.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 10));
                MFL.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 10));

                StaticGameData.ItemList.Add(MFL);

                var RFrame = new Item();
                RFrame.FullName = "Resource Station Section";
                RFrame.ShortName = "R Frame";
                RFrame.ItemCategory = Enums.ItemCategory.item;
                RFrame.ItemType = Enums.ItemTypes.r_frame;
                RFrame.Mass = 200;
                RFrame.ToolPod = true;

                RFrame.Research = new ResearchItem(Enums.ItemTypes.r_frame, 26, 3);

                RFrame.Locked = true;
                RFrame.OrbitOnly = true;
                RFrame.BuildRequirements = new List<BuildRequirement>();
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 35));
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 50));
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 20));
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 15));
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 30));
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 25));
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 10));
                RFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silica, 15));

                StaticGameData.ItemList.Add(RFrame);

                var PrejudiceTorpedoLauncher = new Item();
                PrejudiceTorpedoLauncher.FullName = "Prejudice Torpedo Launcher";
                PrejudiceTorpedoLauncher.ShortName = "P.T.L.";
                PrejudiceTorpedoLauncher.ItemCategory = Enums.ItemCategory.item;
                PrejudiceTorpedoLauncher.ItemType = Enums.ItemTypes.prejudice_torpedo_launcher;
                PrejudiceTorpedoLauncher.Mass = 151;

                PrejudiceTorpedoLauncher.Research = new ResearchItem(Enums.ItemTypes.prejudice_torpedo_launcher, 27, 3);

                PrejudiceTorpedoLauncher.Locked = true;
                PrejudiceTorpedoLauncher.OrbitOnly = true;
                PrejudiceTorpedoLauncher.BuildRequirements = new List<BuildRequirement>();
                PrejudiceTorpedoLauncher.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 96));
                PrejudiceTorpedoLauncher.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 45));
                PrejudiceTorpedoLauncher.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 10));

                StaticGameData.ItemList.Add(PrejudiceTorpedoLauncher);

                var COMMSPOD = new Item();
                COMMSPOD.FullName = "Communication Adapter";
                COMMSPOD.ShortName = "CommsPod";
                COMMSPOD.ItemCategory = Enums.ItemCategory.item;
                COMMSPOD.ItemType = Enums.ItemTypes.commspod;
                COMMSPOD.Mass = 5;
                COMMSPOD.ToolPod = true;

                COMMSPOD.Research = new ResearchItem(Enums.ItemTypes.commspod, 28, 3);

                COMMSPOD.Locked = true;
                COMMSPOD.OrbitOnly = true;
                COMMSPOD.BuildRequirements = new List<BuildRequirement>();
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 2));
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 1));
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 1));

                StaticGameData.ItemList.Add(COMMSPOD);

                var IOSDrone = new Item();
                IOSDrone.FullName = "IOS Battle Drone";
                IOSDrone.ShortName = "IOS Drone";
                IOSDrone.ItemCategory = Enums.ItemCategory.item;
                IOSDrone.ItemType = Enums.ItemTypes.ios_drone;
                IOSDrone.Mass = 490;

                IOSDrone.Research = new ResearchItem(Enums.ItemTypes.ios_drone, 29, 3);

                IOSDrone.Locked = true;
                IOSDrone.OrbitOnly = true;
                IOSDrone.BuildRequirements = new List<BuildRequirement>();
                IOSDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 120));
                IOSDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 120));
                IOSDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 120));
                IOSDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 15));
                IOSDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 55));
                IOSDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 30));
                IOSDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 30));

                StaticGameData.ItemList.Add(IOSDrone);

                var StarDrone = new Item();
                StarDrone.FullName = "SCG Battle Drone";
                StarDrone.ShortName = "Star Drone";
                StarDrone.ItemCategory = Enums.ItemCategory.item;
                StarDrone.ItemType = Enums.ItemTypes.star_drone;
                StarDrone.Mass = 1015;

                StarDrone.Research = new ResearchItem(Enums.ItemTypes.star_drone, 30, 3);

                StarDrone.Locked = true;
                StarDrone.OrbitOnly = true;
                StarDrone.BuildRequirements = new List<BuildRequirement>();
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 300));
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 200));
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 300));
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 100));
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 90));
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 80));
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 95));
                StarDrone.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 50));

                StaticGameData.ItemList.Add(StarDrone);

                var PrisonPod = new Item();
                PrisonPod.FullName = "Prison Pod";
                PrisonPod.ShortName = "Prison Pod";
                PrisonPod.ItemCategory = Enums.ItemCategory.item;
                PrisonPod.ItemType = Enums.ItemTypes.prison_pod;
                PrisonPod.Mass = 7;
                PrisonPod.ToolPod = true;

                PrisonPod.Research = new ResearchItem(Enums.ItemTypes.prison_pod, 31, 3);

                PrisonPod.Locked = true;
                PrisonPod.OrbitOnly = true;
                PrisonPod.BuildRequirements = new List<BuildRequirement>();
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 2));

                StaticGameData.ItemList.Add(PrisonPod);

                var SonicBlaster = new Item();
                SonicBlaster.FullName = "Sonic Blaster";
                SonicBlaster.ShortName = "Blaster";
                SonicBlaster.ItemCategory = Enums.ItemCategory.item;
                SonicBlaster.ItemType = Enums.ItemTypes.sonic_blaster;
                SonicBlaster.Mass = 1065;
                SonicBlaster.ToolPod = true;

                SonicBlaster.Research = new ResearchItem(Enums.ItemTypes.sonic_blaster, 32, 3);

                SonicBlaster.Locked = true;
                SonicBlaster.Research.ResearchMultiplier = 16;
                SonicBlaster.Research.ResearchValue = 16;
                SonicBlaster.OrbitOnly = true;
                SonicBlaster.BuildRequirements = new List<BuildRequirement>();
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 1000));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1500));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 800));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 1200));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 3000));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 3000));

                StaticGameData.ItemList.Add(SonicBlaster);

                #endregion

                #region Materials

                var Iron = new Item();
                Iron.FullName = "Iron";
                Iron.ShortName = "Iron";
                Iron.ItemCategory = Enums.ItemCategory.resource;
                Iron.ItemType = Enums.ItemTypes.iron;
                Iron.Mass = 1;
                Iron.Production = false;

                StaticGameData.ItemList.Add(Iron);

                var Titanium = new Item();
                Titanium.FullName = "Titanium";
                Titanium.ShortName = "Titanium";
                Titanium.ItemCategory = Enums.ItemCategory.resource;
                Titanium.ItemType = Enums.ItemTypes.titanium;
                Titanium.Mass = 1;
                Titanium.Production = false;

                StaticGameData.ItemList.Add(Titanium);

                var Aluminium = new Item();
                Aluminium.FullName = "Aluminium";
                Aluminium.ShortName = "Aluminium";
                Aluminium.ItemCategory = Enums.ItemCategory.resource;
                Aluminium.ItemType = Enums.ItemTypes.aluminium;
                Aluminium.Mass = 1;
                Aluminium.Production = false;

                StaticGameData.ItemList.Add(Aluminium);

                var Carbon = new Item();
                Carbon.FullName = "Carbon";
                Carbon.ShortName = "Carbon";
                Carbon.ItemCategory = Enums.ItemCategory.resource;
                Carbon.ItemType = Enums.ItemTypes.carbon;
                Carbon.Mass = 1;
                Carbon.Production = false;

                StaticGameData.ItemList.Add(Carbon);

                var Copper = new Item();
                Copper.FullName = "Copper";
                Copper.ShortName = "Copper";
                Copper.ItemCategory = Enums.ItemCategory.resource;
                Copper.ItemType = Enums.ItemTypes.copper;
                Copper.Mass = 1;
                Copper.Production = false;

                StaticGameData.ItemList.Add(Copper);

                var Hydrogen = new Item();
                Hydrogen.FullName = "Hydrogen";
                Hydrogen.ShortName = "Hydrogen";
                Hydrogen.ItemCategory = Enums.ItemCategory.resource;
                Hydrogen.ItemType = Enums.ItemTypes.hydrogen;
                Hydrogen.Mass = 1;
                Hydrogen.Production = false;

                StaticGameData.ItemList.Add(Hydrogen);

                var Deuterium = new Item();
                Deuterium.FullName = "Deuterium";
                Deuterium.ShortName = "Deuterium";
                Deuterium.ItemCategory = Enums.ItemCategory.resource;
                Deuterium.ItemType = Enums.ItemTypes.deuterium;
                Deuterium.Mass = 1;
                Deuterium.Production = false;

                StaticGameData.ItemList.Add(Deuterium);

                var Methane = new Item();
                Methane.FullName = "Methane";
                Methane.ShortName = "Methane";
                Methane.ItemCategory = Enums.ItemCategory.resource;
                Methane.ItemType = Enums.ItemTypes.methane;
                Methane.Mass = 1;
                Methane.Production = false;

                StaticGameData.ItemList.Add(Methane);

                var Helium = new Item();
                Helium.FullName = "Helium";
                Helium.ShortName = "Helium";
                Helium.ItemCategory = Enums.ItemCategory.resource;
                Helium.ItemType = Enums.ItemTypes.helium;
                Helium.Mass = 1;
                Helium.Production = false;

                StaticGameData.ItemList.Add(Helium);

                var Paladium = new Item();
                Paladium.FullName = "Paladium";
                Paladium.ShortName = "Paladium";
                Paladium.ItemCategory = Enums.ItemCategory.resource;
                Paladium.ItemType = Enums.ItemTypes.paladium;
                Paladium.Mass = 1;
                Paladium.Production = false;

                StaticGameData.ItemList.Add(Paladium);

                var Platinum = new Item();
                Platinum.FullName = "Platinum";
                Platinum.ShortName = "Platinum";
                Platinum.ItemCategory = Enums.ItemCategory.resource;
                Platinum.ItemType = Enums.ItemTypes.platinum;
                Platinum.Mass = 1;
                Platinum.Production = false;

                StaticGameData.ItemList.Add(Platinum);

                var Silver = new Item();
                Silver.FullName = "Silver";
                Silver.ShortName = "Silver";
                Silver.ItemCategory = Enums.ItemCategory.resource;
                Silver.ItemType = Enums.ItemTypes.silver;
                Silver.Mass = 1;
                Silver.Production = false;

                StaticGameData.ItemList.Add(Silver);

                var Gold = new Item();
                Gold.FullName = "Gold";
                Gold.ShortName = "Gold";
                Gold.ItemCategory = Enums.ItemCategory.resource;
                Gold.ItemType = Enums.ItemTypes.gold;
                Gold.Mass = 1;
                Gold.Production = false;

                StaticGameData.ItemList.Add(Gold);

                var Silica = new Item();
                Silica.FullName = "Silica";
                Silica.ShortName = "Silica";
                Silica.ItemCategory = Enums.ItemCategory.resource;
                Silica.ItemType = Enums.ItemTypes.silica;
                Silica.Mass = 1;
                Silica.Production = false;

                StaticGameData.ItemList.Add(Silica);

                var mehFuel = new Item();
                mehFuel.FullName = "Hydrogen Methanol Fuel";
                mehFuel.ShortName = "MeH Fuel";
                mehFuel.ItemCategory = Enums.ItemCategory.resource;
                mehFuel.ItemType = Enums.ItemTypes.meh_fuel;
                mehFuel.Mass = 3;
                mehFuel.Production = false;
                mehFuel.AutoProduce = true;

                mehFuel.Research = new ResearchItem(Enums.ItemTypes.meh_fuel, 5, 1);
                mehFuel.Research.Locked = false;

                mehFuel.Locked = true;
                mehFuel.OrbitOnly = false;
                mehFuel.BuildRequirements = new List<BuildRequirement>();
                mehFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.hydrogen, 2));
                mehFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.methane, 2));

                StaticGameData.ItemList.Add(mehFuel);

                var hedFuel = new Item();
                hedFuel.FullName = "Helium Deuterium Fuel";
                hedFuel.ShortName = "HeD";
                hedFuel.ItemCategory = Enums.ItemCategory.resource;
                hedFuel.ItemType = Enums.ItemTypes.hed_fuel;
                hedFuel.Mass = 3;
                hedFuel.Production = false;
                hedFuel.AutoProduce = true;

                hedFuel.Research = new ResearchItem(Enums.ItemTypes.hed_fuel, 15, 3);

                hedFuel.Locked = true;
                hedFuel.OrbitOnly = true;
                hedFuel.BuildRequirements = new List<BuildRequirement>();
                hedFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.helium, 2));
                hedFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.deuterium, 2));

                StaticGameData.ItemList.Add(hedFuel);

                #endregion

                #endregion


                StaticGameData.Stars.Add(Enums.StellarBodies.the_sun, new Objects.Star(Enums.StellarBodies.the_sun)
                {
                    PlanetDistanceList = new List<int> { 37, 41, 53, 59, 68, 76, 85, 91, 98, 108, 112, 128, 128, 141, 146, 158, 162, 174, 181, 185, 197, 201 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.proxima, new Objects.Star(Enums.StellarBodies.proxima)
                {
                    PlanetDistanceList = new List<int> { 49, 63, 146, 158 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.centauri, new Objects.Star(Enums.StellarBodies.centauri)
                {
                    PlanetDistanceList = new List<int> { 37, 43, 82, 94, 132, 140, 162, 174 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.barnard, new Objects.Star(Enums.StellarBodies.barnard)
                {
                    PlanetDistanceList = new List<int> { 37, 41, 53, 59, 68, 76, 114, 126, 133, 139, 144, 160, 196, 204 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.lalande, new Objects.Star(Enums.StellarBodies.lalande)
                {
                    PlanetDistanceList = new List<int> { 50, 62, 81, 95 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.sirius, new Objects.Star(Enums.StellarBodies.sirius)
                {
                    PlanetDistanceList = new List<int> { 52, 60, 68, 76 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.cygni, new Objects.Star(Enums.StellarBodies.cygni)
                {
                    PlanetDistanceList = new List<int> { 37, 41, 53, 59, 67, 77, 84, 92, 114, 126, 128, 144, 146, 158, 179, 189 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.procyon, new Objects.Star(Enums.StellarBodies.procyon)
                {
                    PlanetDistanceList = new List<int> { 53, 59, 129, 143, 160, 173 }
                });

                StaticGameData.Stars.Add(Enums.StellarBodies.tau_ceti, new Objects.Star(Enums.StellarBodies.tau_ceti)
                {
                    PlanetDistanceList = new List<int> { 53, 57, 68, 76, 85, 91, 113, 127, 146, 158 }
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.mercury, new Objects.Planet(Enums.StellarBodies.mercury, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.venus, new Objects.Planet(Enums.StellarBodies.venus, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.lines
                });

                //TODO - Earth must be ordered as 0, but the order affects map layout.
                StaticGameData.Planets.Add(Enums.StellarBodies.earth, new Objects.Earth(Enums.StellarBodies.earth, 2)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    BaseBuildParts = 2,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1)
                        }
                    ),
                    MoonList = new List<int> { 2 },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.lines
                });

                //Special Earth setup for a new game
                ((Earth)StaticGameData.Planets[StellarBodies.earth]).Factory.Ground = true;
                StaticGameData.Planets[StellarBodies.earth].PlanetResources.Derricks = 1;

                var trainingData = new Objects.Training();

                trainingData.AvailableTrainees = 6000;
                trainingData.ResearcherTrainingTime = 24;
                trainingData.ProductionTrainingTime = 24;
                trainingData.MarinesTrainingTime = 24;

                trainingData.ResearcherMaxCount = 250;
                trainingData.ProductionMaxCount = 200;

                trainingData.ResearcherTrainingCount = 0;
                trainingData.ProductionTrainingCount = 0;
                trainingData.MarinesTrainingCount = 0;

                trainingData.ResearcherLocked = false;
                trainingData.ProductionLocked = false;
                trainingData.MarinesLocked = false;

                trainingData.ResearcherDayStart = 0;
                trainingData.ProductionDayStart = 0;
                trainingData.MarinesDayStart = 0;

                trainingData.ResearcherTrainingMax = 100;
                trainingData.ProductionTrainingMax = 100;
                trainingData.MarinesTrainingMax = 41;

                trainingData.ReferenceTeamSize = 250;
                trainingData.ReferenceLevelMultiplier = 1.0;
                trainingData.ReferenceDuration = 58.0;

                ((Earth)StaticGameData.Planets[StellarBodies.earth]).TrainingData = trainingData;

                StaticGameData.Planets.Add(Enums.StellarBodies.the_moon, new Objects.Planet(Enums.StellarBodies.the_moon, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.earth,
                    ParentStar = Enums.StellarBodies.the_sun,
                    BaseBuildParts = 2,
                    BaseDamaged = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                        new Objects.Material(Enums.ItemTypes.iron, 1),
                        new Objects.Material(Enums.ItemTypes.titanium, 1),
                        new Objects.Material(Enums.ItemTypes.aluminium, 1),
                        new Objects.Material(Enums.ItemTypes.carbon, 1),
                        new Objects.Material(Enums.ItemTypes.deuterium, 1),
                        new Objects.Material(Enums.ItemTypes.gold, 1),
                        new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.mars, new Objects.Planet(Enums.StellarBodies.mars, 3)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1)
                        }
                    ),
                    MoonList = new List<int> { 1, 8 },
                    PlanetColor = PlanetColor.red,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.phobos, new Objects.Planet(Enums.StellarBodies.phobos, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.mars,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                        new Objects.Material(Enums.ItemTypes.carbon, 1),
                        new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.deimos, new Objects.Planet(Enums.StellarBodies.deimos, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.mars,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                        new Objects.Material(Enums.ItemTypes.carbon, 1),
                        new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.asteroids, new Objects.Planet(Enums.StellarBodies.asteroids, 4)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                        new Objects.Material(Enums.ItemTypes.titanium, 1),
                        new Objects.Material(Enums.ItemTypes.aluminium, 1),
                        new Objects.Material(Enums.ItemTypes.carbon, 1),
                        new Objects.Material(Enums.ItemTypes.copper, 1),
                        new Objects.Material(Enums.ItemTypes.paladium, 1),
                        new Objects.Material(Enums.ItemTypes.platinum, 1),
                        new Objects.Material(Enums.ItemTypes.silver, 1),
                        new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.asteroid,
                    PlanetStyle = PlanetStyle.asteroid
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.jupiter, new Objects.Planet(Enums.StellarBodies.jupiter, 5)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1)
                        }
                    ),
                    MoonList = new List<int> { 1, 2, 3, 4, 6, 7, 8, 9, 10 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.giant
                }) ;

                StaticGameData.Planets.Add(Enums.StellarBodies.amalthea, new Objects.Planet(Enums.StellarBodies.amalthea, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.io, new Objects.Planet(Enums.StellarBodies.io, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.europa, new Objects.Planet(Enums.StellarBodies.europa, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.ganymede, new Objects.Planet(Enums.StellarBodies.ganymede, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.callisto, new Objects.Planet(Enums.StellarBodies.callisto, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.leda, new Objects.Planet(Enums.StellarBodies.leda, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.himalia, new Objects.Planet(Enums.StellarBodies.himalia, 6)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.elara, new Objects.Planet(Enums.StellarBodies.elara, 7)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.pasiphae, new Objects.Planet(Enums.StellarBodies.pasiphae, 8)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jupiter,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.saturn, new Objects.Planet(Enums.StellarBodies.saturn, 6)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1)
                        }
                    ),
                    MoonList = new List<int> { 0, 1, 3, 5, 6, 7, 8, 9, 10 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.rings
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.mimas, new Objects.Planet(Enums.StellarBodies.mimas, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.encaladus, new Objects.Planet(Enums.StellarBodies.encaladus, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.tethys, new Objects.Planet(Enums.StellarBodies.tethys, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.dione, new Objects.Planet(Enums.StellarBodies.dione, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.rhea, new Objects.Planet(Enums.StellarBodies.rhea, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.titan, new Objects.Planet(Enums.StellarBodies.titan, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.hyperion, new Objects.Planet(Enums.StellarBodies.hyperion, 6)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.iapetus, new Objects.Planet(Enums.StellarBodies.iapetus, 7)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.phoebe, new Objects.Planet(Enums.StellarBodies.phoebe, 8)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.saturn,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
)
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.uranus, new Objects.Planet(Enums.StellarBodies.uranus, 7)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    ),
                    MoonList = new List<int> { 1, 5, 6, 10 },
                    PlanetColor = PlanetColor.green,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.miranda, new Objects.Planet(Enums.StellarBodies.miranda, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.uranus,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.ariel, new Objects.Planet(Enums.StellarBodies.ariel, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.uranus,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.umbriel, new Objects.Planet(Enums.StellarBodies.umbriel, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.uranus,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.titania, new Objects.Planet(Enums.StellarBodies.titania, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.uranus,
                    ParentStar = Enums.StellarBodies.the_sun,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.oberon, new Objects.Planet(Enums.StellarBodies.oberon, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.uranus,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.neptune, new Objects.Planet(Enums.StellarBodies.neptune, 8)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1)
                        }
                    ),
                    MoonList = new List<int> { 1, 4, 6, 9 },
                    PlanetColor = PlanetColor.blue,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.triton, new Objects.Planet(Enums.StellarBodies.triton, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.neptune,
                    ParentStar = Enums.StellarBodies.the_sun,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.neried, new Objects.Planet(Enums.StellarBodies.neried, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.neptune,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.nthree, new Objects.Planet(Enums.StellarBodies.nthree, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.neptune,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.nfour, new Objects.Planet(Enums.StellarBodies.nfour, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.neptune,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.pluto, new Objects.Planet(Enums.StellarBodies.pluto, 9)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    ),
                    MoonList = new List<int> { 2 },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.charon, new Objects.Planet(Enums.StellarBodies.charon, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.pluto,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.decuria, new Objects.Planet(Enums.StellarBodies.decuria, 10)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.the_sun,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.atlantic, new Objects.Planet(Enums.StellarBodies.atlantic, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.proxima,
                    ActiveMethanoid = true,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1)
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.blue,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.pacific, new Objects.Planet(Enums.StellarBodies.pacific, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.proxima,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1)
                        }
                    ),
                    MoonList = new List<int> { 1, 6 },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.barent, new Objects.Planet(Enums.StellarBodies.barent, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.pacific,
                    ParentStar = Enums.StellarBodies.proxima,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.baltic, new Objects.Planet(Enums.StellarBodies.baltic, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.pacific,
                    ParentStar = Enums.StellarBodies.proxima,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1)
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.chiron, new Objects.Planet(Enums.StellarBodies.chiron, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cercops, new Objects.Planet(Enums.StellarBodies.cercops, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { 0, 3 },
                    PlanetColor = PlanetColor.red,
                    PlanetStyle = PlanetStyle.massive_rings
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.circe, new Objects.Planet(Enums.StellarBodies.circe, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cercops,
                    ParentStar = Enums.StellarBodies.centauri,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.chimaera, new Objects.Planet(Enums.StellarBodies.chimaera, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cercops,
                    ParentStar = Enums.StellarBodies.centauri,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cerberus, new Objects.Planet(Enums.StellarBodies.cerberus, 2)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { 2, 4, 5, 8 },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cronus, new Objects.Planet(Enums.StellarBodies.cronus, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cerberus,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.chloe, new Objects.Planet(Enums.StellarBodies.chloe, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cerberus,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.calchas, new Objects.Planet(Enums.StellarBodies.calchas, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cerberus,
                    ParentStar = Enums.StellarBodies.centauri,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cadmus, new Objects.Planet(Enums.StellarBodies.cadmus, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cerberus,
                    ParentStar = Enums.StellarBodies.centauri,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.creon, new Objects.Planet(Enums.StellarBodies.creon, 3)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { 1, 9 },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.massive
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cybele, new Objects.Planet(Enums.StellarBodies.cybele, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.creon,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cupid, new Objects.Planet(Enums.StellarBodies.cupid, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.creon,
                    ParentStar = Enums.StellarBodies.centauri,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),

                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.mycenae, new Objects.Planet(Enums.StellarBodies.mycenae, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.green,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.tyre, new Objects.Planet(Enums.StellarBodies.tyre, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 2 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.ur, new Objects.Planet(Enums.StellarBodies.ur, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.tyre,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.thebes, new Objects.Planet(Enums.StellarBodies.thebes, 2)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    ),
                    MoonList = new List<int> { 0, 2, 3, 4, 5, 7, 8, 10 },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.tanis, new Objects.Planet(Enums.StellarBodies.tanis, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.memphis, new Objects.Planet(Enums.StellarBodies.memphis, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.karnak, new Objects.Planet(Enums.StellarBodies.karnak, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.gizeh, new Objects.Planet(Enums.StellarBodies.gizeh, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.calah, new Objects.Planet(Enums.StellarBodies.calah, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.noria, new Objects.Planet(Enums.StellarBodies.noria, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.abydos, new Objects.Planet(Enums.StellarBodies.abydos, 6)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.saqqara, new Objects.Planet(Enums.StellarBodies.saqqara, 7)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.thebes,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.pompeii, new Objects.Planet(Enums.StellarBodies.pompeii, 3)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    ),
                    MoonList = new List<int> { 1, 9 },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.petra, new Objects.Planet(Enums.StellarBodies.petra, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.pompeii,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.palmyra, new Objects.Planet(Enums.StellarBodies.palmyra, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.pompeii,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.jericho, new Objects.Planet(Enums.StellarBodies.jericho, 4)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                        }
                    ),
                    MoonList = new List<int> { 3, 6, 10 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.babylon, new Objects.Planet(Enums.StellarBodies.babylon, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jericho,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.troy, new Objects.Planet(Enums.StellarBodies.troy, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jericho,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.carthage, new Objects.Planet(Enums.StellarBodies.carthage, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.jericho,
                    ParentStar = Enums.StellarBodies.barnard,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.crete, new Objects.Planet(Enums.StellarBodies.crete, 5)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 0, 2, 4, 5, 8, 9 },
                    PlanetColor = PlanetColor.green,
                    PlanetStyle = PlanetStyle.massive
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.knossos, new Objects.Planet(Enums.StellarBodies.knossos, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.crete,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.delphi, new Objects.Planet(Enums.StellarBodies.delphi, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.crete,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.ephesus, new Objects.Planet(Enums.StellarBodies.ephesus, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.crete,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.corinth, new Objects.Planet(Enums.StellarBodies.corinth, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.crete,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.athens, new Objects.Planet(Enums.StellarBodies.athens, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.crete,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.olympia, new Objects.Planet(Enums.StellarBodies.olympia, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.crete,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.mari, new Objects.Planet(Enums.StellarBodies.mari, 6)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                        }
                    ),
                    MoonList = new List<int> { 3 },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cuzco, new Objects.Planet(Enums.StellarBodies.cuzco, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.mari,
                    ParentStar = Enums.StellarBodies.barnard,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });


                StaticGameData.Planets.Add(Enums.StellarBodies.nero, new Objects.Planet(Enums.StellarBodies.nero, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.lalande,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.julius, new Objects.Planet(Enums.StellarBodies.julius, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.lalande,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 1, 4, 5, 9 },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.moon
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.septimus, new Objects.Planet(Enums.StellarBodies.septimus, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.julius,
                    ParentStar = Enums.StellarBodies.lalande,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.augustus, new Objects.Planet(Enums.StellarBodies.augustus, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.julius,
                    ParentStar = Enums.StellarBodies.lalande,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.claudius, new Objects.Planet(Enums.StellarBodies.claudius, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.julius,
                    ParentStar = Enums.StellarBodies.lalande,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.hadrian, new Objects.Planet(Enums.StellarBodies.hadrian, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.julius,
                    ParentStar = Enums.StellarBodies.lalande,
                    ActiveMethanoid = true,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.romulus, new Objects.Planet(Enums.StellarBodies.romulus, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.sirius,
                    ActiveMethanoid = true,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.remus, new Objects.Planet(Enums.StellarBodies.remus, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.sirius,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.helios, new Objects.Planet(Enums.StellarBodies.helios, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.white_green,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.lithos, new Objects.Planet(Enums.StellarBodies.lithos, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.burah, new Objects.Planet(Enums.StellarBodies.burah, 2)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 0, 8 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.alumen, new Objects.Planet(Enums.StellarBodies.alumen, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.burah,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.silex, new Objects.Planet(Enums.StellarBodies.silex, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.burah,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.sulfurum, new Objects.Planet(Enums.StellarBodies.sulfurum, 3)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 2, 7, 10 },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.chloros, new Objects.Planet(Enums.StellarBodies.chloros, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.sulfurum,
                    ParentStar = Enums.StellarBodies.the_sun,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.argos, new Objects.Planet(Enums.StellarBodies.argos, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.sulfurum,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.calx, new Objects.Planet(Enums.StellarBodies.calx, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.sulfurum,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.titanes, new Objects.Planet(Enums.StellarBodies.titanes, 4)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    ),
                    MoonList = new List<int> { 1, 3, 4, 5, 7, 9 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.moon
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.vanadis, new Objects.Planet(Enums.StellarBodies.vanadis, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.titanes,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.chronos, new Objects.Planet(Enums.StellarBodies.chronos, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.titanes,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.selene, new Objects.Planet(Enums.StellarBodies.selene, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.titanes,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.bromos, new Objects.Planet(Enums.StellarBodies.bromos, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.titanes,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.kryptos, new Objects.Planet(Enums.StellarBodies.kryptos, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.titanes,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.rubidos, new Objects.Planet(Enums.StellarBodies.rubidos, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.titanes,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.zargun, new Objects.Planet(Enums.StellarBodies.zargun, 5)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                        }
                    ),
                    MoonList = new List<int> { 1, 2, 4, 5, 7, 8, 10 },
                    PlanetColor = PlanetColor.green,
                    PlanetStyle = PlanetStyle.giant
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.niobe, new Objects.Planet(Enums.StellarBodies.niobe, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zargun,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.kadmeia, new Objects.Planet(Enums.StellarBodies.kadmeia, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zargun,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.tellus, new Objects.Planet(Enums.StellarBodies.tellus, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zargun,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.iodes, new Objects.Planet(Enums.StellarBodies.iodes, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zargun,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.xenos, new Objects.Planet(Enums.StellarBodies.xenos, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zargun,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.caesius, new Objects.Planet(Enums.StellarBodies.caesius, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zargun,
                    ParentStar = Enums.StellarBodies.cygni,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.rhenus, new Objects.Planet(Enums.StellarBodies.rhenus, 6)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zargun,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.osme, new Objects.Planet(Enums.StellarBodies.osme, 6)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    ),
                    MoonList = new List<int> { 0, 2, 4, 6, 8 },
                    PlanetColor = PlanetColor.white,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.iris, new Objects.Planet(Enums.StellarBodies.iris, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.osme,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.platina, new Objects.Planet(Enums.StellarBodies.platina, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.osme,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.aurum, new Objects.Planet(Enums.StellarBodies.aurum, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.osme,
                    ParentStar = Enums.StellarBodies.cygni,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.thallos, new Objects.Planet(Enums.StellarBodies.thallos, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.osme,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.astatos, new Objects.Planet(Enums.StellarBodies.astatos, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.osme,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.radius, new Objects.Planet(Enums.StellarBodies.radius, 7)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    ),
                    MoonList = new List<int> { 1, 3, 10 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.aktis, new Objects.Planet(Enums.StellarBodies.aktis, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.radius,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.protos, new Objects.Planet(Enums.StellarBodies.protos, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.radius,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.prasios, new Objects.Planet(Enums.StellarBodies.prasios, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.radius,
                    ParentStar = Enums.StellarBodies.cygni,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });


                StaticGameData.Planets.Add(Enums.StellarBodies.cambrian, new Objects.Planet(Enums.StellarBodies.cambrian, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.cainozoic, new Objects.Planet(Enums.StellarBodies.cainozoic, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { 0, 3, 4, 6, 7, 10 },
                    PlanetColor = PlanetColor.blue,
                    PlanetStyle = PlanetStyle.massive
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.tertiary, new Objects.Planet(Enums.StellarBodies.tertiary, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cainozoic,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.paleocene, new Objects.Planet(Enums.StellarBodies.paleocene, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cainozoic,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.eocene, new Objects.Planet(Enums.StellarBodies.eocene, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cainozoic,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.oligocene, new Objects.Planet(Enums.StellarBodies.oligocene, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cainozoic,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.miocene, new Objects.Planet(Enums.StellarBodies.miocene, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cainozoic,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.pliocene, new Objects.Planet(Enums.StellarBodies.pliocene, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.cainozoic,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.paleozoic, new Objects.Planet(Enums.StellarBodies.paleozoic, 2)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 6 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.massive_rings
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.silurian, new Objects.Planet(Enums.StellarBodies.silurian, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.paleozoic,
                    ParentStar = Enums.StellarBodies.procyon,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.alpha, new Objects.Planet(Enums.StellarBodies.alpha, 0)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    Segment = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    ),
                    MoonList = new List<int> { },
                    PlanetColor = PlanetColor.white_green,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.beta, new Objects.Planet(Enums.StellarBodies.beta, 1)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 2 },
                    PlanetColor = PlanetColor.white_blue,
                    PlanetStyle = PlanetStyle.whirl
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.delta, new Objects.Planet(Enums.StellarBodies.delta, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.beta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.gamma, new Objects.Planet(Enums.StellarBodies.gamma, 2)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                        }
                    ),
                    MoonList = new List<int> { 1, 4, 9 },
                    PlanetColor = PlanetColor.yellow,
                    PlanetStyle = PlanetStyle.lines
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.theta, new Objects.Planet(Enums.StellarBodies.theta, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.gamma,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.iota, new Objects.Planet(Enums.StellarBodies.iota, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.gamma,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.kappa, new Objects.Planet(Enums.StellarBodies.kappa, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.gamma,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new (Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.epsilon, new Objects.Planet(Enums.StellarBodies.epsilon, 3)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    ),
                    MoonList = new List<int> { 3, 4, 5, 7, 8, 10 },
                    PlanetColor = PlanetColor.red,
                    PlanetStyle = PlanetStyle.giant
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.lambda, new Objects.Planet(Enums.StellarBodies.lambda, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.epsilon,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.mu, new Objects.Planet(Enums.StellarBodies.mu, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.epsilon,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.nu, new Objects.Planet(Enums.StellarBodies.nu, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.epsilon,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.xi, new Objects.Planet(Enums.StellarBodies.xi, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.epsilon,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.omicron, new Objects.Planet(Enums.StellarBodies.omicron, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.epsilon,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.pi, new Objects.Planet(Enums.StellarBodies.pi, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.epsilon,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.zeta, new Objects.Planet(Enums.StellarBodies.zeta, 4)
                {
                    IsMoon = false,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.silver, 1),
                        }
                    ),
                    MoonList = new List<int> { 0, 2, 3, 4, 6, 7, 9 },
                    PlanetColor = PlanetColor.green,
                    PlanetStyle = PlanetStyle.massive_rings
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.rho, new Objects.Planet(Enums.StellarBodies.rho, 0)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zeta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                            new Objects.Material(Enums.ItemTypes.silica, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.sigma, new Objects.Planet(Enums.StellarBodies.sigma, 1)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zeta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.upsilon, new Objects.Planet(Enums.StellarBodies.upsilon, 2)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zeta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.aluminium, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.deuterium, 1),
                            new Objects.Material(Enums.ItemTypes.methane, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.phi, new Objects.Planet(Enums.StellarBodies.phi, 3)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zeta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.carbon, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.platinum, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.chi, new Objects.Planet(Enums.StellarBodies.chi, 4)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zeta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.psi, new Objects.Planet(Enums.StellarBodies.psi, 5)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zeta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.copper, 1),
                            new Objects.Material(Enums.ItemTypes.helium, 1),
                            new Objects.Material(Enums.ItemTypes.gold, 1),
                        }
                    )
                });

                StaticGameData.Planets.Add(Enums.StellarBodies.omega, new Objects.Planet(Enums.StellarBodies.omega, 6)
                {
                    IsMoon = true,
                    MoonParentPlanetId = Enums.StellarBodies.zeta,
                    ParentStar = Enums.StellarBodies.tau_ceti,
                    ActiveMethanoid = true,
                    PlanetResources = new Objects.PlanetResource(new List<Objects.Material>()
                        {
                            new Objects.Material(Enums.ItemTypes.iron, 1),
                            new Objects.Material(Enums.ItemTypes.titanium, 1),
                            new Objects.Material(Enums.ItemTypes.hydrogen, 1),
                            new Objects.Material(Enums.ItemTypes.paladium, 1),
                        }
                    )
                });

                //set up all methanoid owned planets
                foreach (IPlanet p in StaticGameData.Planets.Values)
                {
                    if (p.ActiveMethanoid)
                    {
                        p.Station.BuildParts = 8;
                        p.Station.Built = true;
                        p.Station.MtxInstalled = true;
                        p.Station.SdmInstalled = true;
                        p.Station.Factory.AOC = true;
                        p.BaseBuildParts = 2;
                        p.BaseDamaged = false;
                    }
                }

                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.iron] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.titanium] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.aluminium] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.carbon] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.copper] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.hydrogen] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.deuterium] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.methane] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.helium] = 4;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.paladium] = 1;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.platinum] = 2;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.silver] = 2;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.gold] = 3;
                StaticGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.silica] = 1;

                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.iron] = 2;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.titanium] = 2;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.aluminium] = 2;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.carbon] = 2;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.copper] = 2;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.hydrogen] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.deuterium] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.methane] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.helium] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.paladium] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.platinum] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.silver] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.gold] = 1;
                StaticGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.silica] = 2;

                var saveData = Utility.Serialization.WriteObject<Deuteros.Code.Objects.GameData.BaseData>(StaticGameData);
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\GameData.dat", saveData);
            }
            else
            {
                var fileData = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\GameData.dat");
                StaticGameData = Utility.Serialization.ReadObject<Deuteros.Code.Objects.GameData.BaseData>(fileData);
            }
        }
    }
}