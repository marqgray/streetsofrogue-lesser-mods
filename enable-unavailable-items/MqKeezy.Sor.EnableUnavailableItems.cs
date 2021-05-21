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
                [HarmonyPatch(typeof(Unlocks), methodName: "LoadInitialUnlocks")]
                private class LoadInitialUnlocks
                {
                    [HarmonyPostfix]
                    private static void Postfix()
                    {
                        if (enabledAllItems)
                        {
                            return;
                        }

                        foreach (Unlock unlock in
                            GameController.gameController.sessionDataBig.unlocks.Where(predicate: unlock =>
                                unlock.unlockType ==
                                "Item"))
                        {
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
                        }

                        foreach (Unlock unlock in from itemId in Enum.GetNames(typeof(ItemNameDB.rowIds))
                            let foundItem =
                                GameController.gameController.sessionDataBig.itemUnlocks.Any(predicate: unlockedItem =>
                                    unlockedItem.unlockName == itemId)
                            where !foundItem
                            select new Unlock(itemId, myType: "Item", isUnlocked: true, myCost: 0, myCost2: 0,
                                myCost3: 0)
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