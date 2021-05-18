using System;
using System.Linq;
using BepInEx;
using Google2u;
using HarmonyLib;
using mqKeezy_EnableUnavailableItems.Properties;

namespace mqKeezy_EnableUnavailableItems
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorEnableUnavailableItems : BaseUnityPlugin
    {
        private static bool enabledAllItems;

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
                        if (enabledAllItems) return;

                        foreach (var unlock in
                            GameController.gameController.sessionDataBig.unlocks.Where(unlock => unlock.unlockType ==
                                "Item"))
                            if (unlock.unavailable)
                            {
                                GameController.gameController.sessionDataBig.itemUnlocks.Add(unlock);

                                GameController.gameController.sessionDataBig.itemUnlocksCharacterCreation
                                    .Add(unlock);

                                Unlock.itemCount++;
                                Unlock.itemCountCharacterCreation++;
                                unlock.unlocked = true;
                                unlock.unavailable = false;
                                unlock.onlyInCharacterCreation = false;
                            }
                            else
                            {
                                if (unlock.onlyInCharacterCreation)
                                {
                                    GameController.gameController.sessionDataBig.itemUnlocks.Add(unlock);
                                    Unlock.itemCount++;
                                    unlock.onlyInCharacterCreation = false;
                                }

                                unlock.unlocked = true;
                                unlock.unavailable = false;
                            }

                        foreach (var unlock in from itemId in Enum.GetNames(typeof(ItemNameDB.rowIds))
                            let foundItem =
                                GameController.gameController.sessionDataBig.itemUnlocks.Any(unlockedItem =>
                                    unlockedItem.unlockName == itemId)
                            where !foundItem
                            select new Unlock(itemId, "Item", true, 0, 0, 0)
                            {
                                onlyInCharacterCreation = false, unavailable = false, unlocked = true
                            })
                        {
                            GameController.gameController.sessionDataBig.itemUnlocks.Add(unlock);
                            GameController.gameController.sessionDataBig.itemUnlocksCharacterCreation.Add(unlock);
                            Unlock.itemCount++;
                            Unlock.itemCountCharacterCreation++;
                        }

                        enabledAllItems = true;
                    }
                }
            }
        }
    }
}