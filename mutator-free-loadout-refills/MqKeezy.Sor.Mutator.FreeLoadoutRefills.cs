using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_FreeLoadoutRefills.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_FreeLoadoutRefills
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorFreeLoadoutRefills : BaseUnityPlugin
    {
        public static UnlockBuilder Mutator;

        public void Awake()
        {
            Mutator = RogueLibs.CreateCustomUnlock(new MutatorUnlock(
                    name: "mqKeezy.FreeLoadoutRefills",
                    unlockedFromStart: true
                ))
                .WithName(new CustomNameInfo(english: "Free Loadout Refills"))
                .WithDescription(new CustomNameInfo(english: ""));

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}