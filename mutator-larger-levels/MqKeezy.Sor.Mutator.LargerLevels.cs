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
            Mutator = RogueLibs.CreateCustomMutator(id: "mqKeezy.LargerLevels",
                unlockedFromStart: true,
                new CustomNameInfo(english: "Larger Levels"),
                new CustomNameInfo(english: ""));

            configLevelSizePercentage = Config.Bind(section: "General", key: "LevelSizePercentage", defaultValue: 200,
                description:
                "The size of all levels will be multiplied by this percentage. You can configure this to your liking, but be careful of setting these values too high or low or there may be issues. Ex: 100 = 100%, 50 = 50%, 300 = 300%. There are safety min & max caps on the maps to prevent errors. A higher value such as 400 would guarantee max level size for every area.");

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        public static int ModifyMaxLevelSize(int value)
        {
            if (Mutator?.IsActive == true)
            {
                return (int) Mathf.Clamp(value * (configLevelSizePercentage.Value / 100f), min: 10, max: 64);
            }

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
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction t = codes[i];
                if (i > 3 && codes[i - 1].OperandIs(levelSizeMaxInfoField) &&
                    codes[i - 2].opcode == OpCodes.Ldc_I4_S && codes[i - 3].opcode == OpCodes.Ldarg_0
                )
                {
                    foreach (CodeInstruction codeInstruction in InsertMaxLevelSizeInstructions())
                    {
                        yield return codeInstruction;
                    }
                }

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