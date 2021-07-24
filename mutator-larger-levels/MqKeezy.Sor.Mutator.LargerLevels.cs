using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using mqKeezy_Mutator_LargerLevels.Properties;
using RogueLibsCore;
using UnityEngine;

namespace mqKeezy_Mutator_LargerLevels
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorLargerLevels : BaseUnityPlugin
    {
        private static ConfigEntry<int> configLevelSizePercentage;
        private static UnlockBuilder Mutator;

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomUnlock(new MutatorUnlock(
                    name: "mqKeezy.LargerLevels",
                    unlockedFromStart: true
                ))
                .WithName(new CustomNameInfo(english: "Larger Levels"))
                .WithDescription(new CustomNameInfo(english: ""));

            configLevelSizePercentage = Config.Bind(section: "General", key: "LevelSizePercentage", defaultValue: 200,
                description:
                "The size of all levels will be multiplied by this percentage. You can configure this to your liking, but be careful of setting these values too high or low or there may be issues. Ex: 100 = 100%, 50 = 50%, 300 = 300%. There are safety min & max caps on the maps to prevent errors. A higher value such as 400 would guarantee max level size for every area.");

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        public static int ModifyMaxLevelSize(int value)
        {
            return Mutator?.Unlock.IsEnabled == true
                ? (int) Mathf.Clamp(value * (configLevelSizePercentage.Value / 100f), min: 10, max: 64)
                : value;
        }
    }
}