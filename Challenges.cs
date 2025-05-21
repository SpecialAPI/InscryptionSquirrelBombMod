using InscryptionAPI.Dialogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod
{
    public partial class Plugin
    {
        public static AscensionChallenge DentistryChallenge;
        public static AscensionChallenge ArchivedChallenge;

        public void AddChallenges()
        {
            //challenges
            ChallengeManager.AddSpecific(GUID,
                "Squirrel Bomb", //name
                "Squirrels explode when killed, dealing 10 damage to adjacent cards.", //description
                30, //points
                LoadTexture("ascensionicon_squirrelbomb"), //texture
                LoadTexture("ascensionicon_activated_squirrelbomb"), //activated texture
                typeof(SquirrelBomb), //type
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Dance Party", //name
                "Each turn, all of your cards will move in a direction. The cards that can't move die.", //description
                30, //points
                LoadTexture("ascensionicon_dance"), //texture
                LoadTexture("ascensionicon_activated_dance"), //activated texture
                typeof(DanceParty),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "???", //name
                "CHALLENGE LOCKED", //description
                20, //points
                LoadTexture("ascensionicon_sus"), //texture
                LoadTexture("ascensionicon_activated_sus"), //activated texture
                typeof(Sus),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Again", //name
                "All opponent cards attack twice.", //description
                60, //points
                LoadTexture("ascensionicon_again"), //texture
                LoadTexture("ascensionicon_activated_again"), //activated texture
                typeof(Again),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Starving", //name
                "Pray.", //description
                2, //points
                LoadTexture("ascensionicon_starve"), //texture
                ChallengeManager.DEFAULT_ACTIVATED_SPRITE, //activated texture
                typeof(Starving),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Dogfish", //name
                "Each battle starts with a Bait Bucket that has Guardian.", //description
                30, //points
                LoadTexture("ascensionicon_dogfish"), //texture
                ChallengeManager.DEFAULT_ACTIVATED_SPRITE, //activated texture
                typeof(Dogfish),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Annoying Totems", //name
                "Start each battle with an Annoying totem for your most used tribe.", //description
                25, //points
                LoadTexture("ascensionicon_annoyingtotems"), //texture
                LoadTexture("ascensionicon_activated_annoyingtotems"), //activated texture
                typeof(AnnoyingTotems),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Painful Draws", //name
                "Drawing from the main deck deals 1 damage.", //description
                30, //points
                LoadTexture("ascensionicon_painfulmain"), //texture
                LoadTexture("ascensionicon_activated_painfulmain"), //activated texture
                typeof(PainfulDraws),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Painful Squirrels", //name
                "Drawing from the side deck deals 1 damage.", //description
                30, //points
                LoadTexture("ascensionicon_painfulside"), //texture
                LoadTexture("ascensionicon_activated_painfulside"), //activated texture
                typeof(PainfulSidedecks),
                0); //unlock level

            ChallengeManager.AddSpecific(GUID,
                "Runes",
                "All enemy cards get random sigils when queued or spawned.",
                30,
                LoadTexture("ascensionicon_runes"),
                ChallengeManager.DEFAULT_ACTIVATED_SPRITE,
                typeof(Runes));

            DentistryChallenge = ChallengeManager.AddSpecific(GUID,
                "Dentistry",
                "Item events and Trinket Bearer cards only give pliers.",
                15,
                LoadTexture("ascensionicon_pliers"),
                LoadTexture("ascensionicon_activated_pliers"),
                0).Challenge.challengeType;

            ArchivedChallenge = ChallengeManager.AddSpecific(GUID,
                "Archived",
                "A random card choice is hidden in every card choice event.",
                20,
                LoadTexture("ascensionicon_archived"),
                LoadTexture("ascensionicon_activated_archived"),
                0).Challenge.challengeType;

            ChallengeManager.AddSpecific(GUID,
                "Foundation",
                "All cards have 1 less attack. Side deck cards have Leader.",
                25,
                LoadTexture("ascensionicon_foundation"),
                LoadTexture("ascensionicon_activated_foundation"),
                typeof(Foundation));

            //ChallengeManager.AddSpecific(GUID,
            //    "Tricky Rares", //name
            //    "Post-boss rare choices are replaced by rare tribe or cost choices.", //description
            //    30, //points
            //    LoadTexture("ascensionicon_painfulside"), //texture
            //    LoadTexture("ascensionicon_activated_painfulside"), //activated texture
            //    typeof(PainfulSidedecks),
            //    0); //unlock level
            // 🤔

            //dialogue
            DialogueManager.GenerateEvent(GUID, "DancePartyDie", new()
            {
                "You must dance! Don't stop dancing!!"
            }, new()
            {
                new()
                {
                    "Whoever stops dancing at my party will suffer the consequences."
                },
                new()
                {
                    "Is that mechanic annoying?",
                    "Perfect. That's the point of it."
                },
                new()
                {
                    "Dance my puppet! Daaaance."
                },
                new()
                {
                    "You must dance! Don't stop dancing!!"
                }
            });
        }
    }
}
