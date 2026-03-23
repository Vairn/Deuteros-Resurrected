using Deuteros.Code;
using Deuteros.Code.Objects;
using Deuteros.Code.Objects.Interfaces;
using Deuteros.Code.Platform.Base;
using Godot;
using Newtonsoft.Json;
using System;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using static Deuteros.Code.Enums;

public partial class Bulletins : BaseSubScene
{
	RichTextLabel BulletinLabel { get; set; }

	[Export] public AudioStreamPlayer TypeSound;

	public int LetterDelayMs { get; set; }



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LetterDelayMs = 75;

		BulletinLabel = GetNode<RichTextLabel>("Labels/BulletinLabel");
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private async Task TypeText(RichTextLabel label, string fullText)
	{
		label.Text = "";

		for (int i = 0; i < fullText.Length; i++)
		{
			if (fullText[i] == '@') continue;
			if (i > 0 && fullText[i - 1] == '@')
			{
				switch (fullText[i])
				{
					case 'R':
						label.Text += "[color=#ff0000]";
						break;
					case 'W':
						label.Text += "[color=#ffffff]";
						break;

				}
				continue;
			}

			label.Text += fullText[i];

			// Optional: don't blip on spaces
			if (fullText[i] != ' ' && TypeSound != null)
			{
				TypeSound.Stop(); // restarts the sound cleanly
				TypeSound.Play();
			}

			await WaitMs(LetterDelayMs);
		}
	}

	private async Task WaitMs(int ms)
	{
		await ToSignal(
			GetTree().CreateTimer(ms / 1000.0),
			SceneTreeTimer.SignalName.Timeout
		);
	}

	public async void DisplayBulletin(BulletinTypes bulletin)
	{
		string bulletinText = String.Empty;

		switch (bulletin)
		{
			case BulletinTypes.ios:
				bulletinText = 
				"We've made some preliminary\n" +
				"designs for an Interplanetary\n" +
				"Operations Spacecraft (IOS).\n" +
				"Unfortunately, it has quite a\n" +
				"large chassis and will not be\n" +
				"capable of landing on any moon\n" +
				"or planet.\n"+
				" \n" +
				"We have also started to design\n" +
				"attachments for the IOS. These\n" +
				"will follow when the chassis\n" +
				"has been fully designed.";
				break;
			case BulletinTypes.ios_attachments:
				bulletinText =
				"As promised, we now have some\n" +
				"basic designs for IOS tools.\n" +
				" \n" +
				"I should warn you that a few\n" +
				"of these items may require\n" +
				"rare materials and apologise\n" +
				"if this causes problems.";
				break;
			case BulletinTypes.methanoid_laser:
				bulletinText =
				"This Methanoid Laser you gave\n" +
				"us is virtually useless.\n" +
				" \n" +
				"There is absolutely no way we\n" +
				"can fit it to our IOS !      .\n" +
				" \n" +
				"We will have to use an adapted\n" +
				"chassis and build the laser in\n" +
				"to it. These 'DRONE' ships can\n" +
				"be controlled via a computer\n" +
				"fitted to a stadard IOS.\n" +
				"However, this computer will\n" +
				"occupy all the ship's cargo\n" +
				"space.";
				break;
		}

		bulletinText = "@RSpecial Bulletin.\n" +
			"@WFrom: \n" +
			GameCore.Earth.ResearchStaff.Leader+"\n"+
			"Head of research.\n \n" + bulletinText+"\n \nMessage ends.";
		await TypeText(BulletinLabel, bulletinText);
	}
}
