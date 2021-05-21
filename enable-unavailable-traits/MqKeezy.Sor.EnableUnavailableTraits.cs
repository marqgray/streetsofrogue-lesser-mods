using System;
using System.Linq;
using BepInEx;
using Google2u;
using HarmonyLib;
using mqKeezy_EnableUnavailableTraits.Properties;

namespace mqKeezy_EnableUnavailableTraits
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorEnableUnavailableTraits : BaseUnityPlugin
    {
        private static bool enabledAllTraits;

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
                        if (enabledAllTraits)
                        {
                            return;
                        }

                        foreach (Unlock unlock in
                            GameController.gameController.sessionDataBig.unlocks.Where(predicate: unlock =>
                                unlock.unlockType ==
                                "Trait"))
                        {
                            if (unlock.unavailable)
                            {
                                GameController.gameController.sessionDataBig.traitUnlocks.Add(unlock);

                                GameController.gameController.sessionDataBig.traitUnlocksCharacterCreation
                                    .Add(unlock);

                                Unlock.traitCount++;
                                Unlock.traitCountCharacterCreation++;
                                unlock.unlocked = true;
                                unlock.unavailable = false;
                                unlock.onlyInCharacterCreation = false;
                            }
                            else
                            {
                                if (unlock.onlyInCharacterCreation)
                                {
                                    GameController.gameController.sessionDataBig.traitUnlocks.Add(unlock);
                                    Unlock.traitCount++;
                                    unlock.onlyInCharacterCreation = false;
                                }

                                unlock.unlocked = true;
                                unlock.unavailable = false;
                            }
                        }

                        foreach (Unlock unlock in from traitId in Enum.GetNames(typeof(StatusEffectNameDB.rowIds))
                            let foundTrait =
                                GameController.gameController.sessionDataBig.traitUnlocks.Any(
                                    predicate: unlockedTrait =>
                                        unlockedTrait.unlockName == traitId)
                            where !foundTrait
                            select new Unlock(traitId, myType: "Trait", isUnlocked: true, myCost: 0, myCost2: 0,
                                myCost3: 0)
                            {
                                onlyInCharacterCreation = false, unavailable = false, unlocked = true
                            })
                        {
                            GameController.gameController.sessionDataBig.traitUnlocks.Add(unlock);
                            GameController.gameController.sessionDataBig.traitUnlocksCharacterCreation.Add(unlock);
                            Unlock.traitCount++;
                            Unlock.traitCountCharacterCreation++;
                        }

                        enabledAllTraits = true;
                    }
                }
            }
        }
    }
}