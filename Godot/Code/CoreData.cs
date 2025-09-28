using Deuteros.Code.Objects;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deuteros.Code
{
    [Serializable]
    public partial class CoreData
    {
        public Dictionary<Enums.Planetoids, Deuteros.Code.Objects.Interfaces.IPlanet> Planets { get; set; }

        public List<Item> ItemList { get; set; }

        //The number of days since day 0
        public uint CurrentDay { get; set; }

        public Dictionary<Enums.ItemTypes, int> ResourceLevels_Survey_Multiplier { get; set; }
        public Dictionary<Enums.ItemTypes, int> ResourceRate_Per_Derrick { get; set; }

        public bool TimeSkip { get; set; }
        public ulong TimeSkipStart { get; set; }
        public bool TimeSkipDay { get; set; }

        public Enums.Planetoids CurrentPlanet { get; set; }
        public List<Enums.Game_Stages> CompleteStages { get; set; }

        public Item GetItem(Enums.ItemTypes itemType)
        {
            return ItemList.First(T => T.ItemType == itemType);
        }

        public IEnumerable<Item> GetAllActiveItems()
        {
            return ItemList.Where(T => T.Locked == false && (T.Research == null || T.Research.Researched));
        }

        public static CoreData CreateNewGameFile()
        {
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data") || 1==1)
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Data");

                var newGameData = new CoreData();

                newGameData.CurrentPlanet = Enums.Planetoids.earth;
                newGameData.Planets = new Dictionary<Enums.Planetoids, Objects.Interfaces.IPlanet>();
                newGameData.ResourceLevels_Survey_Multiplier = new Dictionary<Enums.ItemTypes, int>();
                newGameData.ResourceRate_Per_Derrick = new Dictionary<Enums.ItemTypes, int>();
                newGameData.CompleteStages = new List<Enums.Game_Stages>();

                #region ItemList

                newGameData.ItemList = new List<Item>();

                #region Items

                var derrick = new Item();
                derrick.FullName = "Resource Mining Rig";
                derrick.ItemCategory = Enums.ItemCategory.item;
                derrick.ItemType = Enums.ItemTypes.derrick;
                derrick.Mass = 8;
                derrick.Locked = false;

                derrick.Research = new ResearchItem(Enums.ItemTypes.derrick, 2, 1);
                derrick.Research.Researched = true;
                derrick.Research.ResearchPercentageComplete = 100;
                derrick.Research.ResearchOrder = 1;

                derrick.Locked = false;
                derrick.OrbitOnly = false;
                derrick.BuildRequirements = new List<BuildRequirement>();
                derrick.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 3));
                derrick.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 4));
                derrick.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 1));

                newGameData.ItemList.Add(derrick);

                var shuttlechas = new Item();
                shuttlechas.FullName = "Shuttle Chassis";
                shuttlechas.ItemCategory = Enums.ItemCategory.item;
                shuttlechas.ItemType = Enums.ItemTypes.s_chassis;
                shuttlechas.Mass = 130;

                shuttlechas.Research = new ResearchItem(Enums.ItemTypes.s_chassis, 3, 1);
                shuttlechas.Research.ResearchPercentageComplete = 1;

                shuttlechas.Locked = true;
                shuttlechas.OrbitOnly = false;
                shuttlechas.BuildRequirements = new List<BuildRequirement>();
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 20));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 50));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 35));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 10));
                shuttlechas.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 15));

                newGameData.ItemList.Add(shuttlechas);

                var shuttledrive = new Item();
                shuttledrive.FullName = "Shuttle Drive Unit";
                shuttledrive.ItemCategory = Enums.ItemCategory.item;
                shuttledrive.ItemType = Enums.ItemTypes.s_drive;
                shuttledrive.Mass = 20;

                shuttledrive.Research = new ResearchItem(Enums.ItemTypes.s_drive, 4, 1);
                shuttledrive.Research.ResearchPercentageComplete = 1;
                shuttledrive.Research.ResearchMultiplier = 32;
                shuttledrive.Research.ResearchValue = 96;

                shuttledrive.Locked = true;
                shuttledrive.OrbitOnly = false;
                shuttledrive.BuildRequirements = new List<BuildRequirement>();
                shuttledrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 6));
                shuttledrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 10));
                shuttledrive.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 4));

                newGameData.ItemList.Add(shuttledrive);

                var ofFrame = new Item();
                ofFrame.FullName = "Orbital Factory Section";
                ofFrame.ItemCategory = Enums.ItemCategory.item;
                ofFrame.ItemType = Enums.ItemTypes.of_frame;
                ofFrame.Mass = 250;

                ofFrame.Research = new ResearchItem(Enums.ItemTypes.of_frame, 6, 1);
                ofFrame.Research.ResearchPercentageComplete = 1;

                ofFrame.Locked = true;
                ofFrame.OrbitOnly = false;
                ofFrame.BuildRequirements = new List<BuildRequirement>();
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 55));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 80));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 50));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 25));
                ofFrame.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 40));

                newGameData.ItemList.Add(ofFrame);

                var toolPod = new Item();
                toolPod.FullName = "Tool and Equipment Mounting";
                toolPod.ItemCategory = Enums.ItemCategory.item;
                toolPod.ItemType = Enums.ItemTypes.tool_pod;
                toolPod.Mass = 4;

                toolPod.Research = new ResearchItem(Enums.ItemTypes.tool_pod, 7, 1);
                toolPod.Research.ResearchPercentageComplete = 1;

                toolPod.Locked = true;
                toolPod.OrbitOnly = false;
                toolPod.BuildRequirements = new List<BuildRequirement>();
                toolPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                toolPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                toolPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                newGameData.ItemList.Add(toolPod);

                var supplyPod = new Item();
                supplyPod.FullName = "Supply Pod";
                supplyPod.ItemCategory = Enums.ItemCategory.item;
                supplyPod.ItemType = Enums.ItemTypes.supply_pod;
                supplyPod.Mass = 4;

                supplyPod.Research = new ResearchItem(Enums.ItemTypes.supply_pod, 8, 1);
                supplyPod.Research.ResearchPercentageComplete = 1;

                supplyPod.Locked = true;
                supplyPod.OrbitOnly = false;
                supplyPod.BuildRequirements = new List<BuildRequirement>();
                supplyPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                supplyPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                supplyPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                newGameData.ItemList.Add(supplyPod);

                var cryoPod = new Item();
                cryoPod.FullName = "Cryogenic Holding Pod";
                cryoPod.ItemCategory = Enums.ItemCategory.item;
                cryoPod.ItemType = Enums.ItemTypes.cryo_pod;
                cryoPod.Mass = 4;

                cryoPod.Research = new ResearchItem(Enums.ItemTypes.cryo_pod, 9, 1);
                cryoPod.Research.ResearchPercentageComplete = 1;

                cryoPod.Locked = true;
                cryoPod.OrbitOnly = false;
                cryoPod.BuildRequirements = new List<BuildRequirement>();
                cryoPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                cryoPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                cryoPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                newGameData.ItemList.Add(cryoPod);

                var iChassis = new Item();
                iChassis.FullName = "I.O.S Chassis";
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

                newGameData.ItemList.Add(iChassis);

                var iDrive = new Item();
                iDrive.FullName = "I.O.S Drive Unit";
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

                newGameData.ItemList.Add(iDrive);

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

                newGameData.ItemList.Add(gChassis);

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

                newGameData.ItemList.Add(starDrive);

                var acc = new Item();
                acc.FullName = "Auto Cargo Computer";
                acc.ItemCategory = Enums.ItemCategory.item;
                acc.ItemType = Enums.ItemTypes.a__c__c;
                acc.Mass = 8;

                acc.Research = new ResearchItem(Enums.ItemTypes.a__c__c, 16, 3);

                acc.Locked = true;
                acc.OrbitOnly = false;
                acc.BuildRequirements = new List<BuildRequirement>();
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 1));
                acc.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                newGameData.ItemList.Add(acc);

                var aoc = new Item();
                aoc.FullName = "Auto Operations Computer";
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

                newGameData.ItemList.Add(aoc);

                var BandAid = new Item();
                BandAid.FullName = "Installation Repair Equip";
                BandAid.ItemCategory = Enums.ItemCategory.item;
                BandAid.ItemType = Enums.ItemTypes.bandaid;
                BandAid.Mass = 150;

                BandAid.Research = new ResearchItem(Enums.ItemTypes.bandaid, 18, 3);

                BandAid.Locked = true;
                BandAid.OrbitOnly = true;
                BandAid.BuildRequirements = new List<BuildRequirement>();
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 30));
                BandAid.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 30));

                newGameData.ItemList.Add(BandAid);

                var SelfDestruct = new Item();
                SelfDestruct.FullName = "Self Destruct Mechanism";
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

                newGameData.ItemList.Add(SelfDestruct);

                var HydraulicGrapple = new Item();
                HydraulicGrapple.FullName = "Hydraulic Grapple";
                HydraulicGrapple.ItemCategory = Enums.ItemCategory.item;
                HydraulicGrapple.ItemType = Enums.ItemTypes.grapple;
                HydraulicGrapple.Mass = 5;

                HydraulicGrapple.Research = new ResearchItem(Enums.ItemTypes.grapple, 20, 3);

                HydraulicGrapple.Locked = true;
                HydraulicGrapple.OrbitOnly = true;
                HydraulicGrapple.BuildRequirements = new List<BuildRequirement>();
                HydraulicGrapple.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.iron, 2));
                HydraulicGrapple.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                HydraulicGrapple.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));

                newGameData.ItemList.Add(HydraulicGrapple);

                var DFCC = new Item();
                DFCC.FullName = "Drone Fleet Control Computer";
                DFCC.ItemCategory = Enums.ItemCategory.item;
                DFCC.ItemType = Enums.ItemTypes.d__f__c__c;
                DFCC.Mass = 8;

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

                newGameData.ItemList.Add(DFCC);

                var AMA = new Item();
                AMA.FullName = "Asteroid Mining Attachment";
                AMA.ItemCategory = Enums.ItemCategory.item;
                AMA.ItemType = Enums.ItemTypes.a__m__a;
                AMA.Mass = 124;

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

                newGameData.ItemList.Add(AMA);

                var Hyperlight = new Item();
                Hyperlight.FullName = "Asteroid Mining Attachment";
                Hyperlight.ItemCategory = Enums.ItemCategory.item;
                Hyperlight.ItemType = Enums.ItemTypes.hyperlight;
                Hyperlight.Mass = 124;

                Hyperlight.Research = new ResearchItem(Enums.ItemTypes.hyperlight, 23, 3);

                Hyperlight.Locked = true;
                Hyperlight.OrbitOnly = true;
                Hyperlight.BuildRequirements = null;

                newGameData.ItemList.Add(Hyperlight);

                var MTX = new Item();
                MTX.FullName = "Mass Tranceiver";
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

                newGameData.ItemList.Add(MTX);

                var MFL = new Item();
                MFL.FullName = "Mass Tranceiver";
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

                newGameData.ItemList.Add(MFL);

                var RFrame = new Item();
                RFrame.FullName = "Resource Station Section";
                RFrame.ItemCategory = Enums.ItemCategory.item;
                RFrame.ItemType = Enums.ItemTypes.r_frame;
                RFrame.Mass = 200;

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

                newGameData.ItemList.Add(RFrame);

                var PrejudiceTorpedoLauncher = new Item();
                PrejudiceTorpedoLauncher.FullName = "Prejudice Torpedo Launcher";
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

                newGameData.ItemList.Add(PrejudiceTorpedoLauncher);

                var COMMSPOD = new Item();
                COMMSPOD.FullName = "Communication Adapter";
                COMMSPOD.ItemCategory = Enums.ItemCategory.item;
                COMMSPOD.ItemType = Enums.ItemTypes.commspod;
                COMMSPOD.Mass = 5;

                COMMSPOD.Research = new ResearchItem(Enums.ItemTypes.commspod, 28, 3);

                COMMSPOD.Locked = true;
                COMMSPOD.OrbitOnly = true;
                COMMSPOD.BuildRequirements = new List<BuildRequirement>();
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 2));
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.carbon, 1));
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));
                COMMSPOD.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 1));

                newGameData.ItemList.Add(COMMSPOD);

                var IOSDrone = new Item();
                IOSDrone.FullName = "IOS Battle Drone";
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

                newGameData.ItemList.Add(IOSDrone);

                var StarDrone = new Item();
                StarDrone.FullName = "SCG Battle Drone";
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

                newGameData.ItemList.Add(StarDrone);

                var PrisonPod = new Item();
                PrisonPod.FullName = "Prison Pod";
                PrisonPod.ItemCategory = Enums.ItemCategory.item;
                PrisonPod.ItemType = Enums.ItemTypes.prison_pod;
                PrisonPod.Mass = 7;

                PrisonPod.Research = new ResearchItem(Enums.ItemTypes.prison_pod, 31, 3);

                PrisonPod.Locked = true;
                PrisonPod.OrbitOnly = true;
                PrisonPod.BuildRequirements = new List<BuildRequirement>();
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 2));
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1));
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 1));
                PrisonPod.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.platinum, 2));

                newGameData.ItemList.Add(PrisonPod);

                var SonicBlaster = new Item();
                SonicBlaster.FullName = "Sonic Blaster";
                SonicBlaster.ItemCategory = Enums.ItemCategory.item;
                SonicBlaster.ItemType = Enums.ItemTypes.sonic_blaster;
                SonicBlaster.Mass = 7;

                SonicBlaster.Research = new ResearchItem(Enums.ItemTypes.sonic_blaster, 32, 3);

                SonicBlaster.Locked = true;
                SonicBlaster.OrbitOnly = true;
                SonicBlaster.BuildRequirements = new List<BuildRequirement>();
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.titanium, 1000));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.aluminium, 1500));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.copper, 800));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.paladium, 1200));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.silver, 3000));
                SonicBlaster.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.gold, 3000));

                newGameData.ItemList.Add(SonicBlaster);

                #endregion

                #region Materials

                var Iron = new Item();
                Iron.FullName = "Iron";
                Iron.ItemCategory = Enums.ItemCategory.resource;
                Iron.ItemType = Enums.ItemTypes.iron;
                Iron.Mass = 1;

                newGameData.ItemList.Add(Iron);

                var Titanium = new Item();
                Titanium.FullName = "Titanium";
                Titanium.ItemCategory = Enums.ItemCategory.resource;
                Titanium.ItemType = Enums.ItemTypes.titanium;
                Titanium.Mass = 1;

                newGameData.ItemList.Add(Titanium);

                var Aluminium = new Item();
                Aluminium.FullName = "Aluminium";
                Aluminium.ItemCategory = Enums.ItemCategory.resource;
                Aluminium.ItemType = Enums.ItemTypes.aluminium;
                Aluminium.Mass = 1;

                newGameData.ItemList.Add(Aluminium);

                var Carbon = new Item();
                Carbon.FullName = "Carbon";
                Carbon.ItemCategory = Enums.ItemCategory.resource;
                Carbon.ItemType = Enums.ItemTypes.carbon;
                Carbon.Mass = 1;

                newGameData.ItemList.Add(Carbon);

                var Copper = new Item();
                Copper.FullName = "Copper";
                Copper.ItemCategory = Enums.ItemCategory.resource;
                Copper.ItemType = Enums.ItemTypes.copper;
                Copper.Mass = 1;

                newGameData.ItemList.Add(Copper);

                var Hydrogen = new Item();
                Hydrogen.FullName = "Hydrogen";
                Hydrogen.ItemCategory = Enums.ItemCategory.resource;
                Hydrogen.ItemType = Enums.ItemTypes.hydrogen;
                Hydrogen.Mass = 1;

                newGameData.ItemList.Add(Hydrogen);

                var Deuterium = new Item();
                Deuterium.FullName = "Deuterium";
                Deuterium.ItemCategory = Enums.ItemCategory.resource;
                Deuterium.ItemType = Enums.ItemTypes.deuterium;
                Deuterium.Mass = 1;

                newGameData.ItemList.Add(Deuterium);

                var Methane = new Item();
                Methane.FullName = "Methane";
                Methane.ItemCategory = Enums.ItemCategory.resource;
                Methane.ItemType = Enums.ItemTypes.methane;
                Methane.Mass = 1;

                newGameData.ItemList.Add(Methane);

                var Paladium = new Item();
                Paladium.FullName = "Paladium";
                Paladium.ItemCategory = Enums.ItemCategory.resource;
                Paladium.ItemType = Enums.ItemTypes.paladium;
                Paladium.Mass = 1;

                newGameData.ItemList.Add(Paladium);

                var Platinum = new Item();
                Platinum.FullName = "Platinum";
                Platinum.ItemCategory = Enums.ItemCategory.resource;
                Platinum.ItemType = Enums.ItemTypes.platinum;
                Platinum.Mass = 1;

                newGameData.ItemList.Add(Platinum);

                var Silver = new Item();
                Silver.FullName = "Silver";
                Silver.ItemCategory = Enums.ItemCategory.resource;
                Silver.ItemType = Enums.ItemTypes.silver;
                Silver.Mass = 1;

                newGameData.ItemList.Add(Silver);

                var Gold = new Item();
                Gold.FullName = "Gold";
                Gold.ItemCategory = Enums.ItemCategory.resource;
                Gold.ItemType = Enums.ItemTypes.gold;
                Gold.Mass = 1;

                newGameData.ItemList.Add(Gold);

                var Silica = new Item();
                Silica.FullName = "Silica";
                Silica.ItemCategory = Enums.ItemCategory.resource;
                Silica.ItemType = Enums.ItemTypes.silica;
                Silica.Mass = 1;

                newGameData.ItemList.Add(Silica);

                var mehFuel = new Item();
                mehFuel.FullName = "MeH Fuel";
                mehFuel.ItemCategory = Enums.ItemCategory.resource;
                mehFuel.ItemType = Enums.ItemTypes.meh_fuel;
                mehFuel.Mass = 3;
                mehFuel.Production = true;
                mehFuel.AutoProduce = true;

                mehFuel.Research = new ResearchItem(Enums.ItemTypes.meh_fuel, 5, 1);
                mehFuel.Research.ResearchPercentageComplete = 1;

                mehFuel.Locked = false;
                mehFuel.OrbitOnly = false;
                mehFuel.BuildRequirements = new List<BuildRequirement>();
                mehFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.hydrogen, 2));
                mehFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.methane, 2));

                newGameData.ItemList.Add(mehFuel);

                var hedFuel = new Item();
                hedFuel.FullName = "Helium Deuterium Fuel";
                hedFuel.ItemCategory = Enums.ItemCategory.resource;
                hedFuel.ItemType = Enums.ItemTypes.hed_fuel;
                hedFuel.Mass = 3;
                hedFuel.Production = true;
                hedFuel.AutoProduce = true;

                hedFuel.Research = new ResearchItem(Enums.ItemTypes.hed_fuel, 15, 3);

                hedFuel.Locked = true;
                hedFuel.OrbitOnly = true;
                hedFuel.BuildRequirements = new List<BuildRequirement>();
                hedFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.helium, 2));
                hedFuel.BuildRequirements.Add(new BuildRequirement(Enums.ItemTypes.deuterium, 2));

                newGameData.ItemList.Add(hedFuel);

                #endregion

                #endregion

                var newEarth = new Objects.Earth();
                newEarth.PlanetId = Enums.Planetoids.earth;
                newEarth.IsMoon = false;
                newEarth.ParentStar = Enums.Stars.the_sun;
                newEarth.Stores = new Objects.Store();
                newEarth.ActiveMethanoid = false;
                newEarth.Station = new SpaceStation();
                newEarth.Factory = new Factory();

                newEarth.TrainingData = new Objects.Training();

                newEarth.TrainingData.AvailableTrainees = 6000;
                newEarth.TrainingData.ResearcherTrainingTime = 24;
                newEarth.TrainingData.ProductionTrainingTime = 24;
                newEarth.TrainingData.MarinesTrainingTime = 24;

                newEarth.TrainingData.ResearcherMaxCount = 250;
                newEarth.TrainingData.ProductionMaxCount = 200;

                newEarth.TrainingData.ResearcherTrainingCount = 0;
                newEarth.TrainingData.ProductionTrainingCount = 0;
                newEarth.TrainingData.MarinesTrainingCount = 0;

                newEarth.TrainingData.ResearcherLocked = false;
                newEarth.TrainingData.ProductionLocked = false;
                newEarth.TrainingData.MarinesLocked = false;

                newEarth.TrainingData.ResearcherDayStart = 0;
                newEarth.TrainingData.ProductionDayStart = 0;
                newEarth.TrainingData.MarinesDayStart = 0;

                newEarth.TrainingData.ResearcherTrainingMax = 100;
                newEarth.TrainingData.ProductionTrainingMax = 100;
                newEarth.TrainingData.MarinesTrainingMax = 41;

                newEarth.TrainingData.ReferenceTeamSize = 250;
                newEarth.TrainingData.ReferenceLevelMultiplier = 1.0;
                newEarth.TrainingData.ReferenceDuration = 58.0;

                var earthMaterials = new List<Objects.Material>();
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.iron, 2));
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.titanium, 2));
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.aluminium, 2));
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.carbon, 2));
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.copper, 2));
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.hydrogen, 2));
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.deuterium, 2));
                earthMaterials.Add(new Objects.Material(Enums.ItemTypes.methane, 2));
                newEarth.PlanetResources = new Objects.PlanetResource(earthMaterials);
                newEarth.PlanetResources.Derricks = 1;
                newEarth.Stores = new Objects.Store();
                newEarth.PlanetId = Enums.Planetoids.earth;

                newGameData.Planets.Add(Enums.Planetoids.earth, newEarth);

                var newMars = new Objects.Planet();
                newMars.PlanetId = Enums.Planetoids.mars;
                newMars.IsMoon = false;
                newMars.ParentStar = Enums.Stars.the_sun;
                newMars.Stores = new Objects.Store();

                var marsMaterials = new List<Objects.Material>();
                marsMaterials.Add(new Objects.Material(Enums.ItemTypes.iron, 1));
                marsMaterials.Add(new Objects.Material(Enums.ItemTypes.gold, 1));
                newMars.PlanetResources = new Objects.PlanetResource(marsMaterials);
                newMars.Stores = new Objects.Store();
                newMars.PlanetResources.PlanetId = Enums.Planetoids.mars;
                newMars.Station = new SpaceStation();

                newGameData.Planets.Add(Enums.Planetoids.mars, newMars);

                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.iron] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.titanium] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.aluminium] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.carbon] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.copper] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.hydrogen] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.deuterium] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.methane] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.helium] = 4;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.paladium] = 1;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.platinum] = 2;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.silver] = 2;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.gold] = 3;
                newGameData.ResourceLevels_Survey_Multiplier[Enums.ItemTypes.silica] = 1;

                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.iron] = 2;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.titanium] = 2;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.aluminium] = 2;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.carbon] = 2;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.copper] = 2;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.hydrogen] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.deuterium] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.methane] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.helium] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.paladium] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.platinum] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.silver] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.gold] = 1;
                newGameData.ResourceRate_Per_Derrick[Enums.ItemTypes.silica] = 2;


                var saveData = Utility.Serialization.WriteObject<Deuteros.Code.CoreData>(newGameData);
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\GameData.dat", saveData);

                return newGameData;
            }
            else
            {
                var fileData = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Data\\GameData.dat");
                return Utility.Serialization.ReadObject<Deuteros.Code.CoreData>(fileData);
            }
        }
    }
}