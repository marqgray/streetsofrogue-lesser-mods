using HarmonyLib;
using JetBrains.Annotations;

namespace mqKeezy_Mutator_FreeLoadoutRefills
{
    [PublicAPI]
    [HarmonyPatch(declaringType: typeof(PlayfieldObject))]
    public static class PlayfieldObjectPatch
    {
        [HarmonyPatch(
            methodName: nameof(PlayfieldObject.determineMoneyCost),
            argumentTypes: new[] { typeof(InvItem), typeof(int), typeof(string) })]
        [HarmonyPrefix]
        private static bool Prefix(string transactionType)
        {
            return MqkSorMutatorFreeLoadoutRefills.Mutator?.Unlock.IsEnabled != true || transactionType != "LoadoutMachine";
        }
    }
}