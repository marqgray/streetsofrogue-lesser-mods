using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_CrowdedLevels.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_CrowdedLevels
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorCrowdedLevels : BaseUnityPlugin
    {
        public static CustomMutator Mutator;

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomMutator(id: "mqKeezy.CrowdedLevels",
                unlockedFromStart: true,
                new CustomNameInfo(english: "Crowded Levels"),
                new CustomNameInfo(english: ""));

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}