using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace mqKeezy_Mutator_LargerLevels
{
    [PublicAPI]
    [HarmonyPatch(typeof(LoadLevel))]
    public static class LoadLevelPatch
    {
        private static readonly FieldInfo levelSizeMaxInfoField =
            AccessTools.Field(typeof(LoadLevel), nameof(LoadLevel.levelSizeMax));

        private static readonly MethodInfo modifyMaxLevelSizeField =
            AccessTools.Method(typeof(MqkSorMutatorLargerLevels), nameof(MqkSorMutatorLargerLevels.ModifyMaxLevelSize));

        [HarmonyPatch("CreateInitialMap")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for (int i = 0; i < codes.Count; i++)
            {
                if (i > 3 && codes[i - 1].OperandIs(levelSizeMaxInfoField) &&
                    codes[i - 2].opcode == OpCodes.Ldc_I4_S && codes[i - 3].opcode == OpCodes.Ldarg_0)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, levelSizeMaxInfoField);
                    yield return new CodeInstruction(OpCodes.Call, modifyMaxLevelSizeField);
                    yield return new CodeInstruction(OpCodes.Stfld, levelSizeMaxInfoField);
                }

                yield return codes[i];
            }
        }
    }
}