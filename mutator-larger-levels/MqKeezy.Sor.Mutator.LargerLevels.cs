using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_LargerLevels.Properties;

namespace mqKeezy_Mutator_LargerLevels
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorLargerLevels : BaseUnityPlugin
    {
        private void Awake()
        {
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}