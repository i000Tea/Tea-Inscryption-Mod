using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;

namespace CardLoaderMod
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "cyantist.inscryption.customcardexample";
        private const string PluginName = "CustomCardModExample";
        private const string PluginVersion = "1.0.0.0";

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");

            AddBears();
            AddAbility();
            ChangeWolf();
        }

        private void AddBears()
        {
            List<CardMetaCategory> metaCategories = new List<CardMetaCategory>();
            metaCategories.Add(CardMetaCategory.ChoiceNode);
            metaCategories.Add(CardMetaCategory.Rare);

            List<CardAppearanceBehaviour.Appearance> appearanceBehaviour = new List<CardAppearanceBehaviour.Appearance>();
            appearanceBehaviour.Add(CardAppearanceBehaviour.Appearance.RareCardBackground);

            byte[] imgBytes = System.IO.File.ReadAllBytes(Path.Combine(this.Info.Location.Replace("CardLoaderMod.dll", ""), "Artwork/eightfuckingbears.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgBytes);

            NewCard.Add("Eight_Bears", metaCategories, CardComplexity.Simple, CardTemple.Nature, "8 fucking bears!", 32, 48, description: "Kill this abomination please", cost: 3, appearanceBehaviour: appearanceBehaviour, tex: tex);
        }

        private NewAbility AddAbility()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.rulebookName = "Example Ability";
            info.rulebookDescription = "Example ability which adds a PiggyBank!";
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };

            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line();
            line.text = "New abilities? I didn't authorise homebrew!";
            lines.Add(line);
            info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            byte[] imgBytes = System.IO.File.ReadAllBytes(Path.Combine(this.Info.Location.Replace("CardLoaderMod.dll", ""), "Artwork/new.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgBytes);

            NewAbility ability = new NewAbility(info, typeof(NewTestAbility), tex, AbilityIdentifier.GetAbilityIdentifier(PluginGuid, info.rulebookName));
            NewTestAbility.ability = ability.ability;
            return ability;
        }

        private void ChangeWolf()
        {
            List<Ability> abilities = new List<Ability> { NewTestAbility.ability };
            new CustomCard("Wolf") { baseAttack = 10, abilities = abilities };
        }

        public class NewTestAbility : AbilityBehaviour
        {
            public override Ability Ability
            {
                get
                {
                    return ability;
                }
            }

            public static Ability ability;
            /// <summary>
            /// ��Ӧ�����������
            /// </summary>
            /// <returns></returns>
            public override bool RespondsToResolveOnBoard()
            {
                return true;
            }
            /// <summary>
            /// ���Ͼ���
            /// </summary>
            /// <returns></returns>
            public override IEnumerator OnResolveOnBoard()
            {
                yield return base.PreSuccessfulTriggerSequence();
                yield return new WaitForSeconds(0.2f);
                Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                yield return new WaitForSeconds(0.25f);
                if (RunState.Run.consumables.Count < 3)
                {
                    // ��Ǯ��
                    RunState.Run.consumables.Add("PiggyBank");
                    Singleton<ItemsManager>.Instance.UpdateItems(false);
                }
                else
                {
                    base.Card.Anim.StrongNegationEffect();
                    yield return new WaitForSeconds(0.2f);
                    Singleton<ItemsManager>.Instance.ShakeConsumableSlots(0f);
                }
                yield return new WaitForSeconds(0.2f);
                yield return base.LearnAbility(0f);
                yield break;
            }
        }
    }

}
