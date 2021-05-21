using BepInEx;
using HarmonyLib;
using mqKeezy_NoGibbing.Properties;

namespace mqKeezy_NoGibbing
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorNoGibbing : BaseUnityPlugin
    {
        private static bool _placeholderDisappeared;
        private static bool _placeholderGibbed;

        public void Awake()
        {
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private class Patches
        {
            private class StatusEffectsPatch
            {
                [HarmonyPatch(typeof(StatusEffects), methodName: "NormalGib")]
                private class NormalGib
                {
                    [HarmonyPrefix]
                    private static bool Prefix(ref StatusEffects __instance)
                    {
                        _placeholderDisappeared = __instance.agent.disappeared;
                        _placeholderGibbed = __instance.agent.gibbed;
                        __instance.agent.disappeared = true;
                        __instance.agent.gibbed = true;

                        return true;
                    }

                    [HarmonyPostfix]
                    private static void Postfix(ref StatusEffects __instance)
                    {
                        __instance.agent.disappeared = _placeholderDisappeared;
                        __instance.agent.gibbed = _placeholderGibbed;
                    }
                }
            }
        }
    }
}