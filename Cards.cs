using SquirrelBombMod.Abilities;
using SquirrelBombMod.Spamton;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod
{
    public partial class Plugin
    {
        public static void AddCards()
        {
            CardManager.New("morebosses", "Pipis", "Pipis", 0, 1, "Pipis.")
                .SetPortrait(LoadTexture("portrait_pipis.png"))
                .AddAbilities(FindRegisteredAbility<Pipis>());

            CardManager.New("morebosses", "FlyingHead", "Flying Head", 1, 1, "The most important part.")
                .SetPortrait(LoadTexture("portrait_flyinghead.png"))
                .AddAbilities(FindRegisteredAbility<PassThrough>())
                .SetTribes(Tribe.Squirrel);

            CardManager.New("morebosses", "SpamCart", "Spam Cart", 0, 3, "Filled with spam mail. Absorbs all kinds of attacks.")
                .SetPortrait(LoadTexture("portrait_spamcart.png"))
                .AddAbilities(Ability.StrafeSwap, Ability.PreventAttack);

            CardManager.New("morebosses", "SpamMail", "Spam Mail", 0, 2, "Has all kinds of unforgettable deals.")
                .SetPortrait(LoadTexture("portrait_spammail.png"))
                .AddAbilities(Ability.WhackAMole, Ability.Sharp, Ability.Reach)
                .SetTribes(Tribe.Squirrel);

            CardManager.New("morebosses", "BigShooter", "Big Shooter", 2, 1, "Shoots big shots.")
                .SetPortrait(LoadTexture("portrait_bigshooter.png"))
                .AddAbilities(Ability.Brittle)
                .AddTraits(Trait.Terrain)
                .AddAppearances(CardAppearanceBehaviour.Appearance.TerrainBackground);

            CardManager.New("morebosses", "ChainHeart", "Heart on a Chain", 1, 9, "Beautiful.")
                .SetPortrait(LoadTexture("portrait_chainheart.png"))
                .AddAbilities(Ability.Strafe, Ability.SplitStrike);

            CardManager.New("morebosses", "Spamgel", "Spamgel", 1, 2, "The result of pressing F1.")
                .SetPortrait(LoadTexture("portrait_spamgel.png"))
                .AddAbilities(FindRegisteredAbility<DieHeal>())
                .SetTribes(Tribe.Squirrel);

            CardManager.New("morebosses", "HandPhoneLeft", "Hand Phone", 0, 5, "Do you really need anyone else?")
                .SetPortrait(LoadTexture("portrait_leftphonehand.png"))
                .AddAbilities(FindRegisteredAbility<CreateSpamMailConduit>());

            CardManager.New("morebosses", "HandPhoneRight", "Hand Phone", 0, 5, "Do you really need anyone else?")
                .SetPortrait(LoadTexture("portrait_rightphonehand.png"))
                .AddAbilities(Ability.ConduitBuffAttack);

            //giantcard
            var giantSpamtonPrefab = assets.LoadAsset<GameObject>("SneoGiantPortrait");
            giantSpamtonPrefab.AddComponent<NeoAnimatedPortrait>();
            foreach (var t in giantSpamtonPrefab.GetComponentsInChildren<Transform>())
                t.gameObject.layer = LayerMask.NameToLayer("CardOffscreen");

            CardManager.New("morebosses", "!GIANTCARD_NEO", "THE POWER OF NEO!!!", 1, 40, "Don't you wanna be a big shot?")
                .AddAppearances(CardAppearanceBehaviour.Appearance.GiantAnimatedPortrait)
                .AddTraits(Trait.Giant, Trait.Uncuttable)
                .AddSpecialAbilities(SpecialTriggeredAbility.GiantCard)
                .AddAbilities(FindRegisteredAbility("NeoStrike"), Ability.Reach, Ability.MadeOfStone)
                .animatedPortrait = giantSpamtonPrefab;

            //ending
            CardManager.New("morebosses", "Receiver", "Receiver", -999, 99999, "The voice runs out eventually.")
                .SetPortrait(LoadTexture("portrait_receiver.png"))
                .AddTraits(Trait.Terrain)
                .AddAbilities(FindRegisteredAbility("HelpCall"))
                .AddAppearances(CardAppearanceBehaviour.Appearance.TerrainBackground)
                .hideAttackAndHealth = true;

            //bear challenge
            CardManager.New("morebosses", "BearPipis", "Grizzlipis", 0, 1, "Grizzlipis.")
                .SetPortrait(LoadTexture("portrait_bearpipis.png"))
                .AddAbilities(FindRegisteredAbility<Pipis>(), FindRegisteredAbility<Pipis>())
                .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardManager.New("morebosses", "BearCart", "Bear Cart", 2, 4, "Filled with bears. Absorbs all kinds of attacks.")
                .SetPortrait(LoadTexture("portrait_bearcart.png"))
                .AddAbilities(Ability.StrafeSwap, Ability.PreventAttack, Ability.Reach)
                .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardManager.New("morebosses", "BearMail", "Spam Mail", 0, 3, "Mail for bears.")
                .SetPortrait(LoadTexture("portrait_bearmail.png"))
                .AddAbilities(Ability.WhackAMole, Ability.Sharp, Ability.DeathShield, Ability.Reach)
                .SetTribes(Tribe.Squirrel)
                .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardManager.New("morebosses", "BearHeart", "Bear on a Chain", 1, 10, "Bearutiful.")
                .SetPortrait(LoadTexture("portrait_bearheart.png"))
                .AddAbilities(Ability.Strafe, Ability.TriStrike, Ability.MadeOfStone)
                .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardManager.New("morebosses", "HandBearLeft", "Bear Hand", 0, 5, "Do you really need anyone else?")
                .SetPortrait(LoadTexture("portrait_leftbearhand.png"))
                .AddAbilities(Ability.ConduitBuffAttack, Ability.Reach, Ability.MadeOfStone)
                .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardManager.New("morebosses", "HandBearRight", "Bear Hand", 0, 5, "Do you really need anyone else?")
                .SetPortrait(LoadTexture("portrait_rightbearhand.png"))
                .AddAbilities(FindRegisteredAbility<CreateSpamMailConduit>(), Ability.Reach, Ability.MadeOfStone)
                .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);
        }
    }
}
