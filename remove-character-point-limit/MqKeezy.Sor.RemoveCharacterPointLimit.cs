using BepInEx;
using HarmonyLib;
using mqKeezy_RemoveCharacterPointLimit.Properties;

namespace mqKeezy_RemoveCharacterPointLimit
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorRemoveCharacterPointLimit : BaseUnityPlugin
    {
        private void Awake()
        {
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private class Patches
        {
            private class CharacterCreationPatch
            {
                [HarmonyPatch(typeof(CharacterCreation), methodName: "OpenCharacterCreation", typeof(bool))]
                private class OpenCharacterCreation
                {
                    [HarmonyPostfix]
                    private static void Postfix(ref CharacterCreation __instance)
                    {
                        __instance.totalPoints = 9999999;
                        __instance.itemLimit = 9999999;
                        __instance.traitLimit = 9999999;
                    }
                }
            }
        }
    }
}