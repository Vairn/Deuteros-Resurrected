using Deuteros.Code.Objects;
using Godot;
using System.Collections.Generic;

namespace Deuteros.Code.Platform.Screens
{
    public partial class Training : Base.BaseSubScene
    {
        public AudioStream ButtonSound { get; set; }
        public AudioStream DoorSound { get; set; }
        private AudioStreamPlayer DoorButtonSound { get; set; }

        private List<TextureRect> ResearchDoors;
        private List<TextureRect> ProductionDoors;
        private List<TextureRect> MarinesDoors;

        private List<RepeatingButton> ResearchButtons;
        private List<RepeatingButton> ProductionButtons;
        private List<RepeatingButton> MarinesButtons;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            var researchButtonDown = GetNode<RepeatingButton>("Doors/Research/ResearchMinusButton");
            var researchButtonUp = GetNode<RepeatingButton>("Doors/Research/ResearchPlusButton");
            var productionButtonDown = GetNode<RepeatingButton>("Doors/Product/ProductionMinusButton");
            var productionButtonUp = GetNode<RepeatingButton>("Doors/Product/ProductionPlusButton");
            var marinesButtonDown = GetNode<RepeatingButton>("Doors/Marines/MarinesMinusButton");
            var marinesButtonUp = GetNode<RepeatingButton>("Doors/Marines/MarinesPlusButton");

            researchButtonDown.Connect("pressed", new Callable(this, nameof(ResearchMinusButton_Pressed)));
            researchButtonUp.Connect("pressed", new Callable(this, nameof(ResearchPlusButton_Pressed)));
            productionButtonDown.Connect("pressed", new Callable(this, nameof(ProductionMinusButton_Pressed)));
            productionButtonUp.Connect("pressed", new Callable(this, nameof(ProductionPlusButton_Pressed)));
            marinesButtonDown.Connect("pressed", new Callable(this, nameof(MarinesMinusButton_Pressed)));
            marinesButtonUp.Connect("pressed", new Callable(this, nameof(MarinesPlusButton_Pressed)));

            ResearchButtons = new List<RepeatingButton>
            {
                researchButtonDown,
                researchButtonUp
            };

            ProductionButtons = new List<RepeatingButton>
            {
                productionButtonDown,
                productionButtonUp
            };

            MarinesButtons = new List<RepeatingButton>
            {
                marinesButtonDown,
                marinesButtonUp
            };

            ResearchDoors = new List<TextureRect>
            {
                GetNode<TextureRect>("Doors/Research/LeftDoor"),
                GetNode<TextureRect>("Doors/Research/RightDoor")
            };

            ProductionDoors = new List<TextureRect>
            {
                GetNode<TextureRect>("Doors/Product/LeftDoor"),
                GetNode<TextureRect>("Doors/Product/RightDoor")
            };

            MarinesDoors = new List<TextureRect>
            {
                GetNode<TextureRect>("Doors/Marines/LeftDoor"),
                GetNode<TextureRect>("Doors/Marines/RightDoor")
            };

            ResearchDoors.ForEach(T => T.Visible = false);
            ProductionDoors.ForEach(T => T.Visible = false);
            MarinesDoors.ForEach(T => T.Visible = false);

            ButtonSound = (AudioStream)ResourceLoader.Load("res://Sounds/Button/sTrainingRoom_Button.wav");
            DoorSound = (AudioStream)ResourceLoader.Load("res://Sounds/Button/sTrainingRoom_Door.wav");

            DoorButtonSound = this.GetNode<AudioStreamPlayer>("SoundPlayer");
            DoorButtonSound.Stream = ButtonSound;
        }

        // Called every update.
        public override void _Draw()
        {
            DrawData();
        }

        //Triggered from gamecore
        protected override void DayTick(uint currentDay, uint nextDay)
        {
            QueueRedraw();
        }

        public void DrawData()
        {
            ResearchDoors.ForEach(T => T.Visible = GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked);
            ResearchButtons.ForEach(T => T.Disabled = GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked);

            ProductionDoors.ForEach(T => T.Visible = GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked);
            ProductionButtons.ForEach(T => T.Disabled = GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked);

            MarinesDoors.ForEach(T => T.Visible = GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked);
            MarinesButtons.ForEach(T => T.Disabled = GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked);

            GetNode<Label>("TraineeCountLabel").Text = (GameCore.SingletonInstance.Earth.TrainingData.AvailableTrainees - (GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount + GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount + GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount)).ToString();
            GetNode<Label>("Doors/Research/ResearchTrainingCountLabel").Text = GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount.ToString();
            GetNode<Label>("Doors/Product/ProductionTrainingCountLabel").Text = GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount.ToString();
            GetNode<Label>("Doors/Marines/MarinesTrainingCountLabel").Text = GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount.ToString();
        }

        public void ResearchMinusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked && GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount--;
            }

            DoorButtonSound.Play();

            QueueRedraw();
        }

        public void ResearchPlusButton_Pressed()
        {
            var earth = GameCore.SingletonInstance.GetPlanet<Earth>(Enums.Planetoids.earth);

            if (!GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked && GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingMax > GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount &&
            (
                earth.ResearchStaff == null
                ||
                (earth.ResearchStaff.Count + GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount) < GameCore.SingletonInstance.Earth.TrainingData.ResearcherMaxCount)
                )
            {
                GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount++;
            }

            DoorButtonSound.Play();

            QueueRedraw();
        }

        public void ProductionMinusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked && GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount--;
            }

            DoorButtonSound.Play();

            QueueRedraw();
        }

        public void ProductionPlusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked &&
                GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount < GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingMax &&
                (GameCore.SingletonInstance.Earth.Factory.Builder == null ? 0 : GameCore.SingletonInstance.Earth.Factory.Builder.Count) + GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount < GameCore.SingletonInstance.Earth.TrainingData.ProductionMaxCount)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount++;
            }
            DoorButtonSound.Play();
            QueueRedraw();
        }

        public void MarinesMinusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount--;
            }

            DoorButtonSound.Play();

            QueueRedraw();
        }

        public void MarinesPlusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingMax > GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount)
            {
                GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount++;
            }

            DoorButtonSound.Play();

            QueueRedraw();
        }
    }
}