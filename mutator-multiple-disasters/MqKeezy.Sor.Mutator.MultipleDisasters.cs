using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_MultipleDisasters.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_MultipleDisasters
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorMultipleDisasters : BaseUnityPlugin
    {
        public static CustomMutator Mutator;

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomMutator(id: "mqKeezy.MultipleDisasters",
                unlockedFromStart: true,
                new CustomNameInfo(english: "Multiple Disasters"),
                new CustomNameInfo(english: ""));

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}