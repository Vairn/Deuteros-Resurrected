using Deuteros.Code.Objects;
using Deuteros.Code.Objects.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deuteros.Code.Enums;

namespace Deuteros.Code.Platform.Screens.ShipBayScenes
{
    public partial class Cockpit : Control
    {
        public Control SpriteHolder { get; set; }

        public Staff[] StaffList { get; set; }

        public Label PilotNameLabel { get; set; }
        
        public TextureButton AddACC { get; set; }

        public Button Staff1Button { get; set; }
        public Button Staff2Button { get; set; }
        public Button Staff3Button { get; set; }
        public Button Staff4Button { get; set; }

        public Color ProductionStaffColor { get; set; } = new Color("#002288");
        public Color MarineStaffColor { get; set; } = new Color(255, 0, 0, 255);

        public IShip CurrentShip { get; set; }

        public delegate void AddACCDelegate();
        public event AddACCDelegate ACCAdded;

        public delegate Staff[] ChangePilotDelegate(Staff staff);
        public event ChangePilotDelegate PilotChanged;

        public delegate Staff[] ChangeProductionDelegate(Staff staff);
        public event ChangeProductionDelegate ProductionChanged;

        public override void _Ready()
        {
            StaffList = new Staff[4];

            SpriteHolder = GetNode<Control>("SpriteHolder");

            AddACC = GetNode<TextureButton>("Buttons/AddACC");
            
            Staff1Button = GetNode<Button>("Staff/Buttons/01");
            Staff2Button = GetNode<Button>("Staff/Buttons/02");
            Staff3Button = GetNode<Button>("Staff/Buttons/03");
            Staff4Button = GetNode<Button>("Staff/Buttons/04");

            PilotNameLabel = GetNode<Label>("Labels/PilotName");

            Staff1Button.Pressed += () => ButtonPress(0);
            Staff2Button.Pressed += () => ButtonPress(1);
            Staff3Button.Pressed += () => ButtonPress(2);
            Staff4Button.Pressed += () => ButtonPress(3);
        }

        public void ButtonPress(int staffID)
        {
            var staff = StaffList[staffID];

            if (staff == null || staff.Type == StaffType.Marines)
            {
                if (CurrentShip != null)
                {
                    StaffList = PilotChanged.Invoke(staff);
                }

                UpdateState();
            }
            else
            {
                StaffList = ProductionChanged.Invoke(staff);

                UpdateState();
            }
        }

        public void UpdateStaff(Staff[] staff)
        {
            StaffList = staff;

            UpdateState();
        }

        public void LoadShip(IShip ship)
        {
            CurrentShip = ship;
            
            UpdateState();
        }

        public void UpdateState()
        {
            if (CurrentShip == null)
            {
                PilotNameLabel.Text = "";
                SpriteHolder.Visible = false;
                AddACC.Visible = false;
            }
            else
            {
                PilotNameLabel.Text = CurrentShip.Pilot != null ? CurrentShip.Pilot.Leader : "";
                SpriteHolder.Visible = true;
                AddACC.Visible = true;
            }

            for (int i = 0; i < 4; i++)
            {
                var interfaceI = i+1;

                var staffBackground = GetNode<ColorRect>("Staff/Backgrounds/0" + interfaceI);
                var staffName = GetNode<Label>("Staff/Labels2/Staff" + interfaceI + "Name");
                var staffCount = GetNode<Label>("Staff/Labels2/Staff" + interfaceI + "Count");

                if (StaffList[i] != null)
                {
                    staffBackground.Visible = true;
                    staffName.Text = StaffList[i].Leader;
                    staffCount.Text = StaffList[i].Count.ToString();
                    if (StaffList[i].Type == StaffType.Marines)
                        staffBackground.Color = MarineStaffColor;
                    else if (StaffList[i].Type == StaffType.Production)
                        staffBackground.Color = ProductionStaffColor;
                }
                else
                {
                    staffBackground.Visible = false;
                    staffName.Text = "";
                    staffCount.Text = "";
                }
            }
        }
    }
}