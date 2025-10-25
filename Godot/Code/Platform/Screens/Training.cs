using Deuteros.Code.Objects;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deuteros.Code.Platform.Screens
{
    public partial class Training : Base.BaseSubScene
    {
        public AudioStream ButtonSound { get; set; }
        public AudioStream DoorSound { get; set; }
        private AudioStreamPlayer DoorButtonSound { get; set; }

        private AnimatedSprite2D ResearchDoorAnimation;
        private AnimatedSprite2D ProductionDoorAnimation;
        private AnimatedSprite2D MarinesDoorAnimation;

        private List<RepeatingButton> ResearchButtons;
        private List<RepeatingButton> ProductionButtons;
        private List<RepeatingButton> MarinesButtons;

        private const string Closed_Animation_Name = "closed";
        private const string Opening_Animation_Name = "opening";
        private const string Open_Animation_Name = "open";
        private const string Close_Animation_Name = "close";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

            var researchButtonDown = GetNode<RepeatingButton>("Doors/Research/MinusButton/ResearchMinusButton");
            var researchButtonUp = GetNode<RepeatingButton>("Doors/Research/PlusButton/ResearchPlusButton");
            var productionButtonDown = GetNode<RepeatingButton>("Doors/Production/MinusButton/ProductionMinusButton");
            var productionButtonUp = GetNode<RepeatingButton>("Doors/Production/PlusButton/ProductionPlusButton");
            var marinesButtonDown = GetNode<RepeatingButton>("Doors/Marines/MinusButton/MarinesMinusButton");
            var marinesButtonUp = GetNode<RepeatingButton>("Doors/Marines/PlusButton/MarinesPlusButton");

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

            ResearchDoorAnimation = GetNode<Node2D>("Doors/Research/TrainingDoors").GetNode<AnimatedSprite2D>("DoorAnimation");

            ProductionDoorAnimation = GetNode<Node2D>("Doors/Production/TrainingDoors").GetNode<AnimatedSprite2D>("DoorAnimation");

            MarinesDoorAnimation = GetNode<Node2D>("Doors/Marines/TrainingDoors").GetNode<AnimatedSprite2D>("DoorAnimation");

            ResearchDoorAnimation.Play(Closed_Animation_Name);
            ProductionDoorAnimation.Play(Closed_Animation_Name);
            MarinesDoorAnimation.Play(Closed_Animation_Name);

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
        protected override async void DayTick(uint currentDay, uint nextDay)
        {
            if (GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked && ResearchDoorAnimation.Animation == Open_Animation_Name)
            {
                ResearchDoorAnimation.Play(Close_Animation_Name);
            }
            if (GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked && ProductionDoorAnimation.Animation == Open_Animation_Name)
            {
                ProductionDoorAnimation.Play(Close_Animation_Name);
            }
            if (GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && MarinesDoorAnimation.Animation == Open_Animation_Name)
            {
                MarinesDoorAnimation.Play(Close_Animation_Name);
            }

            if (ResearchDoorAnimation.IsPlaying())
                await ToSignal(ResearchDoorAnimation, "animation_finished");
            if (ProductionDoorAnimation.IsPlaying())
                await ToSignal(ProductionDoorAnimation, "animation_finished");
            if (MarinesDoorAnimation.IsPlaying())
                await ToSignal(MarinesDoorAnimation, "animation_finished");

            QueueRedraw();
        }

        public async void DrawData()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked && ResearchDoorAnimation.Animation == Closed_Animation_Name)
                ResearchDoorAnimation.Play(Opening_Animation_Name);
            
            if (!GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked && ProductionDoorAnimation.Animation == Closed_Animation_Name)
                ProductionDoorAnimation.Play(Opening_Animation_Name);

            if (!GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && MarinesDoorAnimation.Animation == Closed_Animation_Name)
                MarinesDoorAnimation.Play(Opening_Animation_Name);

            if (ResearchDoorAnimation.IsPlaying())
                await ToSignal(ResearchDoorAnimation, "animation_finished");
            if (ProductionDoorAnimation.IsPlaying())
                await ToSignal(ProductionDoorAnimation, "animation_finished");
            if (MarinesDoorAnimation.IsPlaying())
                await ToSignal(MarinesDoorAnimation, "animation_finished");

            if (GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked)
                ResearchDoorAnimation.Play(Closed_Animation_Name);
            else
                ResearchDoorAnimation.Play(Open_Animation_Name);

            if (GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked)
                ProductionDoorAnimation.Play(Closed_Animation_Name);
            else
                ProductionDoorAnimation.Play(Open_Animation_Name);

            if (GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked)
                MarinesDoorAnimation.Play(Closed_Animation_Name);
            else
                MarinesDoorAnimation.Play(Open_Animation_Name);

            ProductionButtons.ForEach(T => T.Disabled = GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked);
            MarinesButtons.ForEach(T => T.Disabled = GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked);
            ResearchButtons.ForEach(T => T.Disabled = GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked);

            GetNode<Label>("TraineeCountLabel").Text = (GameCore.SingletonInstance.Earth.TrainingData.AvailableTrainees - (GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount + GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount + GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount)).ToString();
            GetNode<Label>("Doors/Research/ResearchTrainingCountLabel").Text = GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount.ToString();
            GetNode<Label>("Doors/Production/ProductionTrainingCountLabel").Text = GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount.ToString();
            GetNode<Label>("Doors/Marines/MarinesTrainingCountLabel").Text = GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount.ToString();
        }

        public void ResearchMinusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.ResearcherLocked && GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ResearcherTrainingCount--;
            }

            if (!ResearchButtons.Any(T => T.IsRepeating))
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

            if (!ResearchButtons.Any(T => T.IsRepeating))
                DoorButtonSound.Play();

            QueueRedraw();
        }

        public void ProductionMinusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.ProductionLocked && GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.ProductionTrainingCount--;
            }

            if (!ProductionButtons.Any(T => T.IsRepeating))
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

            if (!ProductionButtons.Any(T => T.IsRepeating))
                DoorButtonSound.Play();

            QueueRedraw();
        }

        public void MarinesMinusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && 
                GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount > 0)
            {
                GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount--;
            }

            if (!MarinesButtons.Any(T => T.IsRepeating))
                DoorButtonSound.Play();

            QueueRedraw();
        }

        public void MarinesPlusButton_Pressed()
        {
            if (!GameCore.SingletonInstance.Earth.TrainingData.MarinesLocked && 
                GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingMax > GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount &&
                GameCore.SingletonInstance.Earth.PlanetResources.Staff.Count < 4)
            {
                GameCore.SingletonInstance.Earth.TrainingData.MarinesTrainingCount++;
            }

            if (!MarinesButtons.Any(T => T.IsRepeating))
                DoorButtonSound.Play();

            QueueRedraw();
        }
    }
}