using BepInEx;
using HarmonyLib;
using mqKeezy_UnlockEverything.Properties;

namespace mqKeezy_UnlockEverything
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorUnlockEverything : BaseUnityPlugin
    {
        private static bool _unlockedAll;

        private void Awake()
        {
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private class Patches
        {
            private class UnlocksPatch
            {
                [HarmonyPatch(typeof(Unlocks), "LoadInitialUnlocks")]
                private class LoadInitialUnlocks
                {
                    [HarmonyPostfix]
                    private static void Postfix()
                    {
                        if (_unlockedAll) return;

                        foreach (var unlock in GameController.gameController.sessionDataBig.unlocks)
                            unlock.unlocked = true;

                        _unlockedAll = true;
                    }
                }
            }
        }
    }
}