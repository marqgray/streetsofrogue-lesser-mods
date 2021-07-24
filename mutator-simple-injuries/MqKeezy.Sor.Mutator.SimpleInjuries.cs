using BepInEx;
using HarmonyLib;
using mqKeezy_Mutator_SimpleInjuries.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_SimpleInjuries
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorSimpleInjuries : BaseUnityPlugin
    {
        public static SimpleInjuriesConfig config;
        private static UnlockBuilder mutator;

        public static bool IsMutatorEnabled => mutator?.Unlock.IsEnabled == true;

        private void Awake()
        {
            config = new SimpleInjuriesConfig(Config);
            
            mutator = RogueLibs.CreateCustomUnlock(new MutatorUnlock(
                    name: "mqKeezy.SimpleInjuries",
                    unlockedFromStart: true
                ))
                .WithName(new CustomNameInfo(english: "Simple Injuries"))
                .WithDescription(new CustomNameInfo(english: ""));

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }
    }
}