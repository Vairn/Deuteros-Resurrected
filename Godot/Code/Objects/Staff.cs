using Godot;
using System;
using System.Linq;

namespace Deuteros.Code.Objects
{
    [Serializable]
    public partial class Staff
    {
        public string Leader { get; set; }
        public int ActionsTaken { get; set; }
        public int Count { get; set; }
        public Enums.StaffType Type { get; set; }

        public Staff()
        {
            ActionsTaken = 0;
        }

        public int GetLevel()
        {
            if (Type == Enums.StaffType.Research)
            {
                if (ActionsTaken >= 6 && ActionsTaken < 9)
                    return (int)Enums.StaffLevel_Researcher.Doctor;
                else if (ActionsTaken >= 9)
                    return (int)Enums.StaffLevel_Researcher.Professor;
                else
                    return (int)Enums.StaffLevel_Researcher.Technician;
            }
            else if (Type == Enums.StaffType.Production)
            {
                if (ActionsTaken >= 6 && ActionsTaken < 12)
                    return (int)Enums.StaffLevel_Production.Engineer;
                else if (ActionsTaken >= 12)
                    return (int)Enums.StaffLevel_Production.Expert;
                else
                    return (int)Enums.StaffLevel_Production.Apprentice;
            }
            else if (Type == Enums.StaffType.Marines)
            {
                if (ActionsTaken >= 10 && ActionsTaken < 30)
                    return (int)Enums.StaffLevel_Marines.Captain;
                else if (ActionsTaken >= 30)
                    return (int)Enums.StaffLevel_Marines.Admiral;
                else
                    return (int)Enums.StaffLevel_Marines.Pilot;
            }

            return 0;
        }
    }
}