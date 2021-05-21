using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_FreeAmmoRefills.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_FreeAmmoRefills
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorAmmoRefills : BaseUnityPlugin
    {
        public static CustomMutator Mutator;

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomMutator(id: "mqKeezy.FreeAmmoRefills",
                unlockedFromStart: true,
                new CustomNameInfo(english: "Free Ammo Dispenser Refills"),
                new CustomNameInfo(english: ""));

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private class Patches
        {
            private class PlayfieldObjectPatch
            {
                [HarmonyPatch(typeof(PlayfieldObject), methodName: "determineMoneyCost", typeof(int), typeof(string))]
                private class DetermineMoneyCost
                {
                    [HarmonyPrefix]
                    private static bool Prefix(ref string transactionType)
                    {
                        return Mutator?.IsActive != true || transactionType != "AmmoDispenser";
                    }
                }
            }
        }
    }
}