using HarmonyLib;
using JetBrains.Annotations;

namespace mqKeezy_Mutator_FreeAmmoRefills
{
    [PublicAPI]
    [HarmonyPatch(declaringType: typeof(PlayfieldObject))]
    public static class PlayfieldObjectPatch
    {
        [HarmonyPatch(methodName: "determineMoneyCost", argumentTypes: new[] { typeof(int), typeof(string) })]
        [HarmonyPrefix]
        private static bool Prefix(string transactionType)
        {
            return MqkSorMutatorAmmoRefills.Mutator?.Unlock.IsEnabled != true || transactionType != "AmmoDispenser";
        }
    }
}