using Deuteros.Code;
using Deuteros.Code.Objects;
using Deuteros.Code.Objects.Interfaces;
using Deuteros.Code.Platform.Base;
using Deuteros.Code.Platform.Screens;
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

    AudioStreamPlayer TypeSound { get; set; }

    public int LetterDelayMs { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LetterDelayMs = 75;

        BulletinLabel = GetNode<RichTextLabel>("Labels/BulletinLabel");
        TypeSound = GetNode<AudioStreamPlayer>("TypeSound");

        base._Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    private async Task TypeText(RichTextLabel label, string fullText)
    {
        GameCore.LockScreen();
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
        GameCore.UnLockScreen();
    }

    private async Task WaitMs(int ms)
    {
        await ToSignal(
            GetTree().CreateTimer(ms / 1000.0),
            SceneTreeTimer.SignalName.Timeout
        );
    }

    public async void AlienTransmission()
    {
        string bulletinText =
        "greetings human.\n" +
        "we are monitoring all of your\n" +
        "transmissions in an attempt to\n" +
        "understand your language\n" +
        " \n" +
        "this message shall be repeated\n" +
        "until we are able to communicate\n" +
        "fluently\n" +
        " \n" +
        "there is a subject of great\n" +
        "importance we must discuss\n" +
        "with you";

        await TypeText(BulletinLabel, bulletinText);

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
                "or planet.\n" +
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
                "us is virtually useless...\n" +
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
                "space..!";
                break;

            case BulletinTypes.self_destruct:
                bulletinText =
                "These Methanoids are more\n" +
                "cunning than we thought...\n" +
                " \n" +
                "Their factories are fitted\n" +
                "with self-destruct mechanisms.\n" +
                " \n" +
                "Give us the time and we will\n" +
                "try to emulate their system.\n" +
                " \n" +
                "There should be an on - off\n" +
                "switch some where...";
                break;

            case BulletinTypes.matter_transmitter:
                bulletinText =
                "The Methanoids are too clever\n" +
                "for comfort...\n" +
                " \n" +
                "Their storerooms are fitted\n" +
                "with what appears to be a\n" +
                "Matter Transmitter / Receiver.\n" +
                " \n" +
                "I assume that it requires a\n" +
                "target transmitter with which\n" +
                "to communicate.\n" +
                " \n" +
                "It's mechanism is beyond my\n" +
                "comprehension but we CAN copy\n" +
                "it...s";
                break;

            case BulletinTypes.sol_cleared:
                bulletinText =
                "We are certain that there are\n" +
                "no Methanoid colonies left in\n" +
                "the Solar System although a\n" +
                "few warships may remain...\n" +
                " \n" +
                "We have discovered a design\n" +
                "for a Star Class Galleon(SCG)\n" +
                "in the Methanoid production\n" +
                "records !!!\n" +
                " \n" +
                "If they have built starships\n" +
                "then this is not the last\n" +
                "we shall see of them.....";
                break;

            case BulletinTypes.scg_drone:
                bulletinText =
                "We have designed an adaptation\n" +
                "of the Galleon chassis for use\n" +
                "as a battle drone.\n" +
                " \n" +
                "This is more powerful than the\n" +
                "IOS Drone and is capable of\n" +
                "interstellar flight...";
                break;

            case BulletinTypes.hyperlight_speed:
                bulletinText =
                "Fascinating....\n" +
                "I've been taking a closer look\n" +
                "at the Star Drive design and I\n" +
                "am certain, with a little more\n" +
                "research, it could be made to\n" +
                "exceed light speed.\n" +
                " \n" +
                "It should be quite simple. No\n" +
                "changes to the drive will be\n" +
                "needed. I am convinced that the\n" +
                "Methanoids already have this\n" +
                "capability....";
                break;

            case BulletinTypes.fuel_weapon:
                bulletinText =
                "This Helium fuel mixture is\n" +
                "hot stuff !\n" +
                " \n" +
                "Using a controlled ignition of\n" +
                "some fuel, we could direct the\n" +
                "resulting energy through a\n" +
                "platinum tube.This would fuse\n" +
                "anything in its path !!\n" +
                " \n" +
                "The only drawback is that the\n" +
                "explosion will also destroy\n" +
                "the laser and its carrier..";
                break;

            case BulletinTypes.drone_ships:
                bulletinText =

                "Dont't Worry, Chief.\n" +
                "These Methanoids Can't Scare\n" +
                "Us!\n" +
                " \n" +
                "I've Come Up With An Idea To\n" +
                "Defend Ourselves But You'll\n" +
                "Have To Give Us The Time To\n" +
                "Develop It.\n" +
                " \n" +
                "We Can Build Small DRONE Ships\n" +
                "And Control Them As A Battle\n" +
                "Fleet Via A Computer Mounted\n" +
                "On An I.O.S.Chassis.\n" +
                "However, This Computer Will\n" +
                "Occupy All The Ship's Cargo\n" +
                "Space...!";
                break;


            case BulletinTypes.rogue_ship:
                bulletinText =

                "We've Tried Our Best To Find A\n" +
                "Solution To Your Rogue Ship.\n" +
                " \n" +
                "All We've Come Up With Is A\n" +
                "Lockable Cryo Pod.Once The\n" +
                "Pirate Crew Is Inside There Is\n" +
                "No Way They Can Escape.\n" +
                " \n" +
                "Although, I Have No Idea How\n" +
                "You Will Persuade Them Into\n" +
                "It..\n" +
                "Traitors!";
                break;

            case BulletinTypes.mutiny:
                bulletinText =
                "M U T I N Y!\n" +
                " \n" +
                " \n" +
                "One Of Our Warlords Has Stolen\n" +
                "A Starship And, As Far As We\n" +
                "Can Tell, Is Taking It To Our\n" +
                "Enemies!\n" +
                "\n" +
                "He Has Disconnected All Remote\n" +
                "Control Circuits And Refuses\n" +
                "To Respond To Orders.\n" +
                "We Do Not Know His Intentions\n" +
                "But They Can't Be Good\n" +
                " \n" +
                "Can They ?";
                break;

            case BulletinTypes.transmission1:
                bulletinText =

                "We Have Received A Very\n" +
                "Strange Transmission.\n" +
                "\n" +
                "It Is Repeated At Regular\n" +
                "Intervals But We Cannot Make\n" +
                "Anything From It.The Language\n" +
                "Is Certainly NOT Methanoid So\n" +
                "It Must Be From Someone Else.\n" +
                "\n" +
                "Take  A Look For Your self...";
                break;

            case BulletinTypes.transmission2:
                bulletinText =
                "We Have Another Transmission\n" +
                "For You...";
                break;

            case BulletinTypes.storm:
                bulletinText =
                "We Have Observed A Very Strange\n" +
                "Event On {0}\n" +
                "It Seems To Be An Immense Storm\n" +
                "On Its Surface Which Is Having\n" +
                "Drastic Effects Upon The Star's\n" +
                "Activity.\n" +
                " \n" +
                "The Energy Output From The Star\n" +
                "Is Now Too Low To Power Our\n" +
                "Factories And All Functions\n" +
                "Have Ground To A Halt.\n" +
                " \n" +
                "We Cannot Predict When It Will\n" +
                "Subside.If Ever!";
                break;

            case BulletinTypes.storm_over:
                bulletinText =
                "The Storm On {0} Has\n" +
                "Almost Disappeared And All\n" +
                "Factories Are Now Operational\n" +
                " \n" +
                "We Shall Study This Phenomenon\n" +
                "In Detail And Inform You When\n" +
                "It Is Likely To Occur Again.";
                break;

            case BulletinTypes.mining_dump:
                bulletinText =
                "We Have Found An Old Methanoid\n" +
                "Mining Dump On {0} !\n" +
                "\n" +
                "It Holds 10000 tonnes of\n" +
                "{1}\n" +
                "This Has Now Been Transfered\n" +
                "To The Surface Stores.";
                break;

            case BulletinTypes.meteor_warning:
                bulletinText =
                "WARNING!\n" +
                " \n" +
                "We Are Tracking A Meteor Of\n" +
                "Great Mass Heading Directly For\n" +
                "{0}\n" +
                "At Present We Cannot Predict\n" +
                "The Exact Time And Location\n" +
                "Of Impact But I Suggest You\n" +
                "Evacuate The Factory\n" +
                "Immediately...";
                break;

            case BulletinTypes.meteor_strike:
                bulletinText =
                "The Meteor Has Destoyed The\n" +
                "Factory At {0} !\n" +
                "However, It Failed To Impact\n" +
                "On The Surface And Has Veered\n" +
                "Off Into A Larger Orbit.\n" +
                " \n" +
                "We Are Continuing To Track Its\n" +
                "Course...";
                break;

            case BulletinTypes.sonic_weapon:
                bulletinText =
                "When {0} Was Captured\n" +
                "We Found An Old Blueprint In\n" +
                "The Production Library.At The\n" +
                "Time It Seemed Unimportant So\n" +
                "We Did Not Disturb You.\n" +
                " \n" +
                "After Inspection The Drawings\n" +
                "Seem To Be For A Super Weapon,\n" +
                "Powered By Fusion And Releases\n" +
                "Some Kind Of Sonic Pulses.\n" +
                " \n" +
                "We Are Ready To Design It...";
                break;

            case BulletinTypes.eureka:
                bulletinText =
                "E U R E K A!\n" +
                " \n" +
                "I've Been Working On A Little\n" +
                "Something For Last Few Months\n" +
                "And Have Finally Reached The\n" +
                "Design Stage.\n" +
                " \n" +
                "The Controls Will Fit Into\n" +
                "A Cockpit In The Same Fashion\n" +
                "As The A.C.C.While The\n" +
                "Hardware Must Be Attached To\n" +
                "The D.F.C.C.\n" +
                " \n" +
                "I'm So Clever !";
                break;

        }

        bulletinText = "@RSpecial Bulletin.\n" +
            "@WFrom: \n" +
            GameCore.Earth.ResearchStaff.Leader + "\n" +
            "Head of research.\n \n" + bulletinText + "\n \nMessage ends.";
        await TypeText(BulletinLabel, bulletinText);
    }
}


/*



"greetings once again, human.\n"+
" \n"+
"allow us to introduce ourselves.\n"+
"we are a peaceful race, similar\n"+
"to yourselves from a galaxy\n"+
"some 700000 parsecs distant.\n"+
" \n"+
"we have already made contact\n"+
"with a race in your galaxy and\n"+
"observe that you are both at\n"+
"war. this is to be expected.\n"+
" \n"+
"we too have found them to be\n"+
"dishonourable. this time we\n"+
"must be certain before placing\n"+
"our TRUST in YOU.\n"+
" \n"+
"CONTACT will follow when\n"+
"observations are complete.";



"GREETINGS, FRIEND.\n"+
" \n"+
"we BELIEVE we CAN TRUST YOU AND\n"+
"REQUEST your ASSISTANCE IN A\n"+
"project TO OUR MUTUAL BENEFIT.\n"+
" \n"+
"MANY land AGO, WE transmuted A\n"+
"gift TO the METHANOIDS.A GIFT\n"+
"OF great POWER AND imPORTANce.\n"+
" \n"+
"SADLY, THEY DISASSEMBLEd it IN\n"+
"AN ATTEMPT to understand THE\n"+
"TECHnologY, A GRAVE MISTaKE.\n"+
" \n"+
"OUR SCANNERS tell US THAT THE\n"+
"segments ARE SCATTERED amomg 8\n"+
"STARS in your GALAXY. we shall\n"+
"INFORM you of their exact\n"+
"LOCATION as we DETEct them.";


"GREETINGS, FRIEND.\n"+
" \n"+
"we have confirmation from our\n"+
"scanners that one segment of\n"+
"our apparatus is\n"+
"lying in orbit around\n"+
"{0}.\n"+
" \n"+
"please attempt to recover the\n"+
"segment and return it to any\n"+
"of your factories.\n"+
" \n"+
"good luck.\n";


"OUR COMPLIMENTS, FRIEND.\n"+
"YOU NOW HAVE ALL SEGMENTS OF\n"+
"OUR TRANSMITTER\n"+
" \n"+
"IF YOU WISH TO USE IT PLEASE\n"+
"FOLLOW THESE INSTRUCTIONS.\n"+
" \n"+
"1: CONSTRUCT THE TRANSMITTER\n"+
"   IN ANY OF YOUR FACTORIES\n"+
" \n"+
"2: FIT THIS TO ANY STARSHIP\n"+
"   IN YOUR FLEET\n"+
"3: ACTIVATE THE POD HOLDING\n"+
"   THE TRANSMITTER.\n"+
" \n"+
"WE WILL DO THE REST.\n"+
" \n"+
"SEE YOU SOON , HUMAN!\n";
					*/
