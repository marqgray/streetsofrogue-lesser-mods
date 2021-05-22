using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using mqKeezy_Mutator_CrowdedLevels.Properties;
using RogueLibsCore;

namespace mqKeezy_Mutator_CrowdedLevels
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorCrowdedLevels : BaseUnityPlugin
    {
        public static ConfigEntry<int> configCrowdedLevelScale;
        public static CustomMutator Mutator;

        public static MethodInfo agentMultiplierPropertyInfo = AccessTools.PropertyGetter(
            typeof(MqkSorMutatorCrowdedLevels),
            nameof(AgentMultiplier));

        public static int AgentMultiplier
        {
            get => Mutator?.IsActive == true ? configCrowdedLevelScale.Value / 100 : 1;
            private set => throw new NotImplementedException();
        }

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomMutator(id: "mqKeezy.CrowdedLevels",
                unlockedFromStart: true,
                new CustomNameInfo(english: "Crowded Levels"),
                new CustomNameInfo(english: ""));

            configCrowdedLevelScale = Config.Bind(section: "General", key: "CrowdedLevelScale", defaultValue: 300,
                description:
                "The number of dwellers in all levels will be multiplied by this percentage. Ex: 100 = 100%, 50 = 50%, 300 = 300%.");

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private class Patches
        {
            private class LoadLevelPatch
            {
                [HarmonyPatch]
                private class SetupMore3_3
                {
                    [HarmonyTargetMethod]
                    private static MethodBase TargetMethod()
                    {
                        Type predicateClass = typeof(LoadLevel).GetNestedTypes(AccessTools.all)
                            .FirstOrDefault(predicate: t => t.FullName?.Contains(value: "d__148") == true);
                        return predicateClass?.GetMethods(AccessTools.all)
                            .FirstOrDefault(predicate: m => m.Name.Contains(value: "MoveNext"));
                    }

                    [HarmonyTranspiler]
                    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                    {
                        List<CodeInstruction> codeInstructions = instructions.ToList();
                        bool isStringLoadingSlumDwellersFound = false;
                        bool isOpCodeStfldFound = false;
                        bool isPatched = false;
                        CodeInstruction bigTriesInstruction = null;
                        foreach (CodeInstruction instruction in codeInstructions)
                        {
                            if (!isPatched)
                            {
                                if (!isStringLoadingSlumDwellersFound)
                                {
                                    if (instruction.opcode == OpCodes.Ldstr &&
                                        ((string) instruction.operand).Contains(value: "Loading Slum Dwellers"))
                                    {
                                        isStringLoadingSlumDwellersFound = true;
                                        yield return instruction;
                                        continue;
                                    }
                                }
                                else if (!isOpCodeStfldFound)
                                {
                                    if (instruction.opcode == OpCodes.Stfld && instruction.operand != null)
                                    {
                                        isOpCodeStfldFound = true;
                                        bigTriesInstruction = instruction;
                                        yield return instruction;
                                        continue;
                                    }
                                }
                                else
                                {
                                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                                    yield return new CodeInstruction(OpCodes.Ldfld, bigTriesInstruction.operand);
                                    yield return new CodeInstruction(OpCodes.Call, agentMultiplierPropertyInfo);
                                    yield return new CodeInstruction(OpCodes.Mul);
                                    yield return new CodeInstruction(OpCodes.Stfld, bigTriesInstruction.operand);
                                    isPatched = true;
                                    yield return instruction;
                                    continue;
                                }
                            }

                            yield return instruction;
                        }
                    }
                }
            }
        }
    }
}