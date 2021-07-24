using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_FreeAmmoRefills.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_FreeAmmoRefills
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorAmmoRefills : BaseUnityPlugin
    {
        public static UnlockBuilder Mutator;

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomUnlock(new MutatorUnlock(
                    name: "mqKeezy.FreeAmmoRefills",
                    unlockedFromStart: true
                ))
                .WithName(new CustomNameInfo(english: "Free Ammo Dispenser Refills"))
                .WithDescription(new CustomNameInfo(english: ""));

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}