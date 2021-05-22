using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_SimpleInjuries.Properties;

namespace mqKeezy_Mutator_SimpleInjuries
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorSimpleInjuries : BaseUnityPlugin
    {
        private void Awake()
        {
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}