using Godot;
using System;

namespace Deuteros.Code
{
    public partial class Enums
    {
        [Serializable]
        public enum Stars
        {
            the_sun,
            proxima,
            centauri,
            barnard,
            lalande,
            sirius,
            cygni,
            procyon,
            tau_ceti
        }

        [Serializable]
        public enum Planetoids
        {
            mercury,
            venus,
            earth,
            mars,
            asteroid,
            jupiter,
            saturn,
            uranus,
            neptune,
            pluto,
            decuria,
            atlantic,
            pacific,
            chiron,
            cecrops,
            cerberus,
            creon,
            mycenae,
            tyre,
            thebes,
            pompeii,
            jericon,
            crete,
            mari,
            nero,
            julius,
            romulus,
            remus,
            helios,
            lithos,
            burah,
            sulfurum,
            titanes,
            zargun,
            osme,
            radius,
            cambrian,
            cainozoic,
            paleozoic,
            alpha,
            beta,
            gamma,
            epsilon,
            zeta,
            //Moons
            moon,
            phobos,
            deimos,
            amalthea,
            io,
            europa,
            ganymede,
            calliston,
            leda,
            himalia,
            elra,
            pasiphae,
            mimas,
            encaladus,
            tethys,
            dione,
            rhea,
            titan,
            hyperion,
            iapet,
            phoebe,
            miranda,
            ariel,
            umbriel,
            titania,
            oberon,
            triton,
            neried,
            nthree,
            nfour,
            charon,
            barent,
            baltic,
            circa,
            chimaera,
            chronus,
            chloe,
            calchas,
            cadmus,
            cybele,
            cupid,
            ur,
            tanis,
            memphis,
            karnak,
            gizeh,
            calah,
            noria,
            abydos,
            saqqara,
            petra,
            palmyra,
            babylon,
            troy,
            carthage,
            knossos,
            delphi,
            ephesus,
            corinth,
            athens,
            olympia,
            cuzco,
            septimus,
            augustus,
            claudius,
            hadrian,
            alumen,
            silex,
            chloros,
            argosos,
            calx,
            vanadis,
            chronos,
            selene,
            bromos,
            niobe,
            kadmeia,
            tellus,
            iodes,
            xenos,
            caesius,
            rhenus,
            iris,
            platina,
            aurum,
            thallos,
            aktis,
            protos,
            prasios,
            tertiary,
            paleocene,
            eocene,
            oligocene,
            miocene,
            pliocene,
            silurian,
            delta,
            theta,
            iota,
            kappa,
            lambda,
            mu,
            nu,
            xi,
            omicron,
            pi,
            rho,
            sigma,
            upsilon,
            phi,
            chi,
            psi,
            omega
        }

        [Serializable]
        public enum StationType
        {
            earth = 1,
            sol = 2,
            milkyway = 3
        }

        [Serializable]
        public enum ItemTypes
        {
            none = 0,
            iron = 1,
            titanium = 2,
            aluminium = 3,
            carbon = 4,
            copper = 5,
            hydrogen = 6,
            deuterium = 7,
            methane = 8,
            helium = 9,
            paladium = 10,
            platinum = 11,
            silver = 12,
            gold = 13,
            silica = 14,
            meh_fuel = 15,
            hed_fuel = 16,
            derrick = 17,
            s_chassis = 18,
            s_drive = 19,
            of_frame = 20,
            supply_pod = 21,
            tool_pod = 22,
            cryo_pod = 23,
            i_chassis = 24,
            i_drive = 25,
            a__c__c = 26,
            a__o__c = 27,
            bandaid = 28,
            s__d__m = 29,
            grapple = 30,
            d__f__c__c = 31,
            a__m__a = 32,
            hyperlight = 33,
            m__t__x = 34,
            m__f__l = 35,
            r_frame = 36,
            prejudice_torpedo_launcher = 37,
            commspod = 38,
            ios_drone = 39,
            g_chassis = 40,
            star_drive = 41,
            p__t__l = 42,
            star_drone = 43,
            prison_pod = 44,
            sonic_blaster = 45,
            pulse_blaster_laser = 46,
        }

        [Serializable]
        public enum StaffType
        {
            Research,
            Production,
            Marines
        }

        [Serializable]
        public enum ItemCategory
        {
            resource,
            item,
            //Does not show up in stores at all
            hidden
        }

        //Underscores in scene names represent a folder
        [Serializable]
        public enum Scenes
        {
            Earth_Ground,
            Earth_Research,
            Earth_Training,
            GroundMaterials,
            Production,
            SaveScreen,
            News,
            Store,
            StoreMTX,
            ShipBay,
            Shuttle,
            None
        }

        //Variables to pass to scenes to notify them of button types
        //E.G. The shuttle button can be for the ground, or in orbit
        [Serializable]
        public enum SceneVariables
        {
            Ground,
            Orbit,
            Shuttle,
            Ship
        }

        [Serializable]
        public enum BackgroundSound
        {
            Earth_Ground,
            Earth_Training,
            Production,
            Resource,
            Research,
            ShuttleBay,
            Store
        }

        public enum StaffLevel_Researcher
        {
            Technician = 1,
            Doctor = 2,
            Professor = 3
        }

        public enum StaffLevel_Production
        {
            Apprentice = 1,
            Engineer = 2,
            Expert = 3
        }

        public enum StaffLevel_Marines
        {
            Pilot = 1,
            Captain = 2,
            Admiral = 3
        }

        public enum Game_Stages
        {
            Earth_Shuttle_Built = 1000,
            First_OrbitalFactory = 2000,
            Left_Earth = 3000,
            Recovered_Moon = 4000,
            Discovered_Methanoids = 5000,
            Traded_Methanoids = 6000,
            War_Methanoids = 7000,
            Left_Sol = 8000
        }

        public enum SidePanel_Button_State_Animations
        {
            Static_Locked = 100,
            Static_Red = 200,
            Static_Yellow = 300,
            Static_Green = 400,
            Red = 500,
            Yellow = 600,
            Green = 700
        }

        public enum Ship_Types
        {
            Shuttle = 100,
            IOS = 200,
            SCG = 300
        }

        public enum Module_Types
        {
            None = 0,
            Tool = 100,
            Supply = 200,
            Cryo = 300
        }

        public enum Fuel_Types
        {
            MEH_Fuel = 100,
            HED_Fuel = 200
        }

        public enum Menu_Buttons
        {
            Empty = 100,
            Production = 200,
            Research = 300,
            Shuttle = 400,
            GroundMaterials = 500,
            Planet_Left = 600,
            Planet_Right = 700,
            SDM = 800,
            Ship_Bay = 900,
            Shuttle_Bay = 1000,
            Station_Left = 1100,
            Station_Right = 1200,
            Store = 1300,
            Training = 1400
        }
    }
}