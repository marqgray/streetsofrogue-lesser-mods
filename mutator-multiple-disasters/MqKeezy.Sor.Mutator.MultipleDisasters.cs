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
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}