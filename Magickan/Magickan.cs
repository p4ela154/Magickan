using BepInEx;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.GUI;
using Jotunn.Managers;
using Jotunn.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Jotunn.Logger;
using static EffectList;

//Thanks to MarcoPogo!

namespace Magickan
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    [BepInDependency("cinnabun.backpacks-v1.0.0", BepInDependency.DependencyFlags.SoftDependency)]
    internal class ShardOfGungnir : BaseUnityPlugin
    {
        // BepInEx' plugin metadata
        public const string PluginGUID = "com.jotunn.Magickan";
        public const string PluginName = "Magickan";
        public const string PluginVersion = "1.0.0";
        public static Skills.SkillType magickan_skill = 0;
        private ButtonConfig WoodStaffSpecialButton;


        private void Awake()
        {

            // Test adding a skill with a texture
            magickan_skill = SkillManager.Instance.AddSkill(new SkillConfig
            {
                Identifier = "com.jotunn.JotunnModExample.testskill",
                Name = "$magickan_skill",
                Description = "$magickan_skill_description",
                Icon = AssetUtils.LoadSprite("Magickan/Assets/Magicka_skill.png"),
                IncreaseStep = 1f
            });

            AddCustomItems(); // Load, create and init your custom mod stuff
            PrefabManager.OnVanillaPrefabsAvailable += AddCustomItems; // Add custom items cloned from vanilla items
        }
        ///
        ///
 // Called every frame
        private void Update()
        {
            // Since our Update function in our BepInEx mod class will load BEFORE Valheim loads,
            // we need to check that ZInput is ready to use first.
            if (ZInput.instance != null)
            {

                // Use the name of the ButtonConfig to identify the button pressed
                // without knowing what key the user bound to this button in his configuration.
                // Our button is configured to block all other input, so we just want to query
                // ZInput when our custom item is equipped.
                if (WoodStaffSpecialButton != null && MessageHud.instance != null &&
                    Player.m_localPlayer != null && Player.m_localPlayer.m_visEquipment.m_rightItem == "woodstaff")
                {
                    if (ZInput.GetButton(WoodStaffSpecialButton.Name) && MessageHud.instance.m_msgQeue.Count == 0)
                    {
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$woodstaff_beevilmessage");
                    }
                }
            }
        }
        ///
        ///
        private void AddInputs()
        {
            WoodStaffSpecialButton = new ButtonConfig
            {
                Name = "AllySpawner",
                Key = KeyCode.B,
                HintToken = "$AllySpawnerHint",        // Displayed KeyHint
                BlockOtherInputs = false   // Blocks all other input for this Key / Button
            };
            InputManager.Instance.AddButton(PluginGUID, WoodStaffSpecialButton);
        }
        ///
        ///
        private void KeyHintsWoodStaff()
        {
            // Create custom KeyHints for the item
            KeyHintConfig KHC = new KeyHintConfig
            {
                Item = "woodstaff",
                ButtonConfigs = new[]
                {
                    WoodStaffSpecialButton
                }
            };
            KeyHintManager.Instance.AddKeyHint(KHC);
        }
        ///
        ///       
        private void AddSpecialEffects(GameObject SE_GameObject, String SE_TEXT)
        {
            //  Try using TargetParentPath = "attach
            print(SE_TEXT);
            Debug.Log(SE_TEXT);
            KitbashObject kitbashObject = KitbashManager.Instance.AddKitbash(SE_GameObject, new KitbashConfig
            {
                KitbashSources = new List<KitbashSourceConfig>
                    {
                        new KitbashSourceConfig
                        {
                            Name = "SE",
                            SourcePrefab = "fx_Lightning",
                            SourcePath = "Sparcs",
                            TargetParentPath = "attach",
                            Position = new Vector3(0, 0, -0.75f),
                            Rotation = Quaternion.Euler(-0, 0, 0),
                            Scale = new Vector3(0.1f, 0.1f, 0.1f)
                        }
                    }

            }
            );
            kitbashObject.OnKitbashApplied += () =>
            {
                // We've added a CapsuleCollider to the skeleton, this is no longer needed
                Destroy(kitbashObject.Prefab.transform.Find("Bow/Attach/default").GetComponent<Mesh>());
                Destroy(kitbashObject.Prefab.transform.Find("Bow/Attach/default").GetComponent<MeshFilter>());
            };
        }
        // Implementation of cloned items
        private void AddCustomItems()
        {
            try {
                CustomItem CI = new CustomItem("woodstaff", "Bow");
                //AddSpecialEffects(CI.ItemDrop.gameObject, String.Concat("KitBashStart: ", CI.ItemPrefab.name));
                ItemManager.Instance.AddItem(CI);
                var itemDrop = CI.ItemDrop;
                itemDrop.m_itemData.m_shared.m_name = "$item_wood_staff";
                itemDrop.m_itemData.m_shared.m_damages.m_pierce = 15f;
                itemDrop.m_itemData.m_shared.m_holdDurationMin = 1f;
                itemDrop.m_itemData.m_shared.m_skillType = magickan_skill;
                itemDrop.m_itemData.m_shared.m_attack.m_attackAnimation = "spear_throw";
                AddSpecialEffects(CI.ItemPrefab, "STAFF");
                //itemDrop.m_itemData.m_shared.m_holdAnimationState = "spear_throw";
                Recipewoodstaff(CI.ItemDrop, CI.ItemPrefab.name, 1);
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CustomItem CI_A = new CustomItem("staffmagic", "ArrowFire");
                //AddSpecialEffects(CI.ItemDrop.gameObject, String.Concat("KitBashStart: ", CI.ItemPrefab.name));
                ItemManager.Instance.AddItem(CI_A);
                var itemDrop_A = CI_A.ItemDrop;
                itemDrop_A.m_itemData.m_shared.m_name = "$item_staffmagic";
                itemDrop_A.m_itemData.m_shared.m_damages.m_fire = 25f;
                Recipewoodstaff(CI_A.ItemDrop, CI_A.ItemPrefab.name, 20);
                KeyHintsWoodStaff();
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CustomItem CI_AP = new CustomItem("staffmagic_projectile", "bow_projectile_fire");
                //AddSpecialEffects(CI.ItemDrop.gameObject, String.Concat("KitBashStart: ", CI.ItemPrefab.name));
                ItemManager.Instance.AddItem(CI_AP);
                var itemDrop_AP = CI_AP.ItemDrop;
                itemDrop_AP.m_itemData.m_shared.m_name = "$item_staffmagic_projectile";
            }
            catch (Exception ex) {Jotunn.Logger.LogError($"Error while adding cloned item: {ex.Message}");}
            finally { PrefabManager.OnVanillaPrefabsAvailable -= AddCustomItems; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Implementation of assets via using manual recipe creation and prefab cache's
        private void Recipewoodstaff(ItemDrop itemDrop, String SE_TEXT, int QNTY)
        {
            // Create and add a recipe for the copied item
            Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Recipe_" + SE_TEXT;
            recipe.m_item = itemDrop;
            recipe.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>("piece_workbench"); //forge
            recipe.m_repairStation = PrefabManager.Cache.GetPrefab<CraftingStation>("piece_workbench"); //forge
            recipe.m_minStationLevel = 1;
            recipe.m_amount = QNTY;
            recipe.m_resources = new Piece.Requirement[]
                {
                    new Piece.Requirement() { m_resItem = PrefabManager.Cache.GetPrefab<ItemDrop>("Wood"), m_amount = 1 }
                };
            // Since we got the vanilla prefabs from the cache, no referencing is needed
            CustomRecipe CR = new CustomRecipe(recipe, fixReference: false, fixRequirementReferences: false);
            ItemManager.Instance.AddRecipe(CR);
        }

        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    }
}
