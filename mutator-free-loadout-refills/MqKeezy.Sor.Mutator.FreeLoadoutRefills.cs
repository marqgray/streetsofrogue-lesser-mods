using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_FreeLoadoutRefills.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_FreeLoadoutRefills
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorFreeLoadoutRefills : BaseUnityPlugin
    {
        public static CustomMutator Mutator;

        public void Awake()
        {
            Mutator = RogueLibs.CreateCustomMutator("mqKeezy.FreeLoadoutRefills",
                true,
                new CustomNameInfo("Free Loadout Refills"),
                new CustomNameInfo(""));

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private class Patches
        {
            private class PlayfieldObjectPatch
            {
                [HarmonyPatch(typeof(PlayfieldObject),
                    "determineMoneyCost",
                    typeof(InvItem),
                    typeof(int),
                    typeof(string))]
                private class DetermineMoneyCost
                {
                    [HarmonyPrefix]
                    private static bool Prefix(ref string transactionType)
                    {
                        return Mutator?.IsActive != true || transactionType != "LoadoutMachine";
                    }
                }
            }
        }
    }
}