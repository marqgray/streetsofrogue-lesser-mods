using BepInEx;
using HarmonyLib;
using mqKeezy_MaxCustomCharacterItemCount.Properties;

namespace mqKeezy_MaxCustomCharacterItemCount
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMaxCustomCharacterItemCount : BaseUnityPlugin
    {
        private void Awake()
        {
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private class Patches
        {
            private class InvDatabasePatch
            {
                [HarmonyPatch(typeof(InvDatabase), methodName: "AddItemPlayerStart")]
                private class AddItemPlayerStart
                {
                    [HarmonyPrefix]
                    private static bool Prefix(ref InvDatabase __instance, ref string itemName, ref int itemCount)
                    {
                        InvItem item = __instance.AddItem(itemName, itemCount);
                        if (__instance.agent.agentName == "Custom" && item.rechargeAmount <= 0 &&
                            item.itemType != "Money" && (item.stackable ||
                                                         item.itemType == "WeaponProjectile" ||
                                                         item.itemType == "WeaponMelee" ||
                                                         item.itemType == "WeaponThrown" ||
                                                         item.itemType == "Wearable"))
                        {
                            itemCount = 99999;
                        }

                        __instance.DestroyItem(item);
                        item = null;

                        return true;
                    }
                }
            }
        }
    }
}