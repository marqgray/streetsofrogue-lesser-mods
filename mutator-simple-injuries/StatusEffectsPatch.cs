using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine.Networking;

#pragma warning disable 618

namespace mqKeezy_Mutator_SimpleInjuries
{
    [PublicAPI]
    [HarmonyPatch(declaringType: typeof(StatusEffects))]
    public static class StatusEffectsPatch
    {
        private static readonly Dictionary<Agent, Agent> agentLastDamagedBy = new Dictionary<Agent, Agent>();

        private static SimpleInjuriesConfig SIConfig => MqkSorMutatorSimpleInjuries.config;

        public static Agent GetAgentLastDamageSource(Agent agent)
        {
            agentLastDamagedBy.TryGetValue(agent, out Agent result);
            return result;
        }

        [HarmonyPatch(
            methodName: nameof(StatusEffects.ChangeHealth),
            argumentTypes: new[] { typeof(float), typeof(PlayfieldObject), typeof(NetworkInstanceId), typeof(float), typeof(string), typeof(byte) })]
        [HarmonyPostfix]
        private static void ChangeHealth_Postfix(ref StatusEffects __instance, PlayfieldObject damagerObject)
        {
            if (!MqkSorMutatorSimpleInjuries.IsMutatorEnabled || !SIConfig.IsInjured(__instance.agent))
            {
                return;
            }

            Agent attacker = null;
            if (damagerObject != null)
            {
                switch (damagerObject)
                {
                    case Bullet bullet:
                        attacker = bullet.agent;
                        break;
                    case Melee melee:
                        attacker = melee.agent;
                        break;
                    case Noise noise:
                        attacker = noise.agent;
                        break;
                    case Explosion explosion:
                        attacker = explosion.agent;
                        break;
                    case Fire fire:
                        attacker = fire.agent;
                        break;
                    case Gas gas:
                        attacker = gas.agent;
                        break;
                }
            }

            if (attacker != null)
            {
                agentLastDamagedBy[__instance.agent] = attacker;
            }
        }
    }
}