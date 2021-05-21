using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using mqKeezy_Mutator_LargerLevels.Properties;
using RogueLibsCore;
using UnityEngine;

namespace mqKeezy_Mutator_LargerLevels
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorLargerLevels : BaseUnityPlugin
    {
        public static ConfigEntry<int> configLevelSizePercentage;
        public static CustomMutator Mutator;

        public static readonly FieldInfo levelSizeMaxInfoField =
            AccessTools.Field(typeof(LoadLevel), nameof(LoadLevel.levelSizeMax));

        public static readonly MethodInfo modifyMaxLevelSizeField =
            AccessTools.Method(typeof(MqkSorMutatorLargerLevels), nameof(ModifyMaxLevelSize));

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomMutator("mqKeezy.LargerLevels",
                true,
                new CustomNameInfo("Larger Levels"),
                new CustomNameInfo(""));

            configLevelSizePercentage = Config.Bind("General", "LevelSizePercentage", 200,
                "The size of all levels will be multiplied by this percentage. You can configure this to your liking, but be careful of setting these values too high or low or there may be issues. Ex: 100 = 100%, 50 = 50%, 300 = 300%. There are safety min & max caps on the maps to prevent errors. A higher value such as 400 would guarantee max level size for every area.");
            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        public static int ModifyMaxLevelSize(int value)
        {
            if (Mutator?.IsActive == true)
                return (int) Mathf.Clamp(value * (configLevelSizePercentage.Value / 100f), 10, 64);
            return value;
        }

        private static IEnumerable<CodeInstruction> InsertMaxLevelSizeInstructions()
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, levelSizeMaxInfoField);
            yield return new CodeInstruction(OpCodes.Call, modifyMaxLevelSizeField);
            yield return new CodeInstruction(OpCodes.Stfld, levelSizeMaxInfoField);
        }

        private static IEnumerable<CodeInstruction> TranspileFields(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            for (var i = 0; i < codes.Count; i++)
            {
                var t = codes[i];
                if (i > 3 && codes[i - 1].OperandIs(levelSizeMaxInfoField) &&
                    codes[i - 2].opcode == OpCodes.Ldc_I4_S && codes[i - 3].opcode == OpCodes.Ldarg_0
                )
                    foreach (var codeInstruction in InsertMaxLevelSizeInstructions())
                        yield return codeInstruction;

                yield return t;
            }
        }

        private static class Patches
        {
            private static class LoadLevelPatch
            {
                [HarmonyPatch(typeof(LoadLevel), nameof(LoadLevel.CreateInitialMap))]
                private static class CreateInitialMap
                {
                    [HarmonyTranspiler]
                    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                    {
                        return TranspileFields(instructions);
                    }
                }
            }
        }
    }
}