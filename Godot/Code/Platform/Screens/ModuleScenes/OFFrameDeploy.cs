using Deuteros.Code.Platform.Base;
using Godot;
using System.Threading.Tasks;

namespace Deuteros.Code.Platform.Screens.ModuleScenes
{
	public partial class OFFrameDeploy : BaseSubScene
	{
		public Label Text01 { get; set; }
		public Label Text02 { get; set; }
		public Label Text03 { get; set; }
		public Label Text04 { get; set; }
		public Label WindowNumber { get; set; }

		[Export] public AudioStreamPlayer TypeSound;

		public string Text1 = "Equipment Pod Activating.";
		public string Text2 = "Positioning Orbital\r\nFactory Section No. XXX of 8";
		public string Text3 = "Sequence Complete.";
		public string Text4 = "Factory Is Now Operational.";

		public string RFrameText1 = "Equipment Pod Activating.";
		public string RFrameText2 = "Station Section  XXX Of 2";
		public string RFrameText3 = ".  .  .  DEPLOYED  .  .  .";
		public string RFrameText4 = "Station Is Now Operational.";

		public string MethanoidText = "Wujic Flakra, Tarran Tak\nGablak, Spligh Caboon.\n\nApologeek Dasmin Pag Fo\nOwst Meethane Stolt.\n\nSha-Sha.";
        public int PauseBetweenLabelsMs { get; set; }
		public int LetterDelayMs { get; set; }
        public int WordDelayMs { get; set; }

        public override void _Ready()
		{
			Text01 = GetNode<Label>("Window/Text01");
			Text02 = GetNode<Label>("Window/Text02");
			Text03 = GetNode<Label>("Window/Text03");
			Text04 = GetNode<Label>("Window/Text04");

			WindowNumber = GetNode<Node2D>("Window").GetNode<Label>("Background/Number");

			PauseBetweenLabelsMs = 750;
			LetterDelayMs = 75;
			WordDelayMs = 200;

			base._Ready();
		}

		public async Task PlayThreeLabels(int frameCount)
		{
			Text01.Text = "";
			Text02.Text = "";
			Text03.Text = "";
			Text04.Text = "";

			await TypeText(Text01, Text1);

			await WaitMs(PauseBetweenLabelsMs);
			await TypeText(Text02, Text2.Replace("XXX", frameCount.ToString()));

			await WaitMs(PauseBetweenLabelsMs);
			await TypeText(Text03, Text3);

			await WaitMs(PauseBetweenLabelsMs);

			if (frameCount == 8)
			{
				await TypeText(Text04, Text4);
				await WaitMs(PauseBetweenLabelsMs);
			}
		}
		public async Task PlayRFrameThreeLabels(int frameCount)
		{
			Text01.Text = "";
			Text02.Text = "";
			Text03.Text = "";
			Text04.Text = "";

			await TypeText(Text01, RFrameText1);

			await WaitMs(PauseBetweenLabelsMs);
			await TypeText(Text02, RFrameText2.Replace("XXX", frameCount.ToString()));

			await WaitMs(PauseBetweenLabelsMs);
			await TypeText(Text03, RFrameText3);

			await WaitMs(PauseBetweenLabelsMs);

			if (frameCount == 2)
			{
				await TypeText(Text04, RFrameText4);
				await WaitMs(PauseBetweenLabelsMs);
			}

		}

		public async Task PlayMethanoidText()
		{
            Text01.Text = "";
            Text02.Text = "";
            Text03.Text = "";
            Text04.Text = "";

            await TypeText(Text01, MethanoidText,true);
            await WaitMs(5000);
        }

        private async Task TypeText(Label label, string fullText, bool wordDelayOnly = false)
		{
			label.Text = "";

			for (int i = 0; i < fullText.Length; i++)
			{
				label.Text += fullText[i];

				if (wordDelayOnly == false)
				{
                    // Optional: don't blip on spaces
                    if (fullText[i] != ' ' && TypeSound != null)
                    {
                        TypeSound.Stop(); // restarts the sound cleanly
                        TypeSound.Play();
                    }
                    
					await WaitMs(LetterDelayMs);
				}
				else
				{
					if (fullText[i] == ' ')
                        await WaitMs(WordDelayMs);
					if (fullText[i] == '\n')
                        await WaitMs(WordDelayMs*2);
                }
            }
		}

		private async Task WaitMs(int ms)
		{
			await ToSignal(
				GetTree().CreateTimer(ms / 1000.0),
				SceneTreeTimer.SignalName.Timeout
			);
		}
	}
}
