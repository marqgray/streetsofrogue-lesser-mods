using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace mqKeezy_Mutator_SimpleInjuries
{
    [PublicAPI]
    [HarmonyPatch(declaringType: typeof(Agent))]
    public static class AgentPatch
    {
        private static readonly Dictionary<Agent, float> nextBleedTime = new Dictionary<Agent, float>();
        private static readonly Dictionary<Agent, float> nextBleedOutTime = new Dictionary<Agent, float>();

        private static SimpleInjuriesConfig SIConfig => MqkSorMutatorSimpleInjuries.config;

        private static void HandleAgentBleeding(Agent agent)
        {
            float time = Time.time;
            if (!nextBleedTime.ContainsKey(agent) || time > nextBleedTime[agent])
            {
                nextBleedTime[agent] = time + Random.Range(SIConfig.BleedTimeMinSeconds, SIConfig.BleedTimeMaxSeconds);

                Vector3 decalPos = agent.tr.position;
                Vector3 position = new Vector3(
                    decalPos.x,
                    decalPos.y,
                    GameController.gameController.streamingWorld ? (decalPos.y - 1.28f) * 100f / 100000f : 0f);

                GameObject gameObject = GameController.gameController.spawnerMain.floorDecalPrefab.Spawn(position);
                gameObject.GetComponent<tk2dSprite>().SetSprite(spriteName: "BloodDecal");
                gameObject.transform.localScale *= Random.Range(SIConfig.BleedScaleMinPercentage, SIConfig.BleedScaleMaxPercentage);
                gameObject.transform.rotation = Quaternion.Euler(x: 0f, y: 0f, Random.Range(min: 0, max: 16) * 22.5f);
                GameController.gameController.floorDecalsList.Add(gameObject);
            }
        }

        private static void HandleAgentBleedOut(Agent agent)
        {
            if (!nextBleedOutTime.ContainsKey(agent))
            {
                // Started bleeding out in this tick, apply bleedOutDamage in 1 second (unless healed)
                nextBleedOutTime[agent] = 1f;
            }
            
            nextBleedOutTime[agent] -= Time.deltaTime;
            if (nextBleedOutTime[agent] < 0)
            {
                nextBleedOutTime[agent] = 1f;

                Agent lastDamagedAgent = StatusEffectsPatch.GetAgentLastDamageSource(agent);
                float bleedOutDamage = SIConfig.CalculateBleedOutDamage(agent);

                if (lastDamagedAgent != null)
                {
                    agent.justHitByAgent = lastDamagedAgent;
                    agent.justHitByAgent2 = lastDamagedAgent;
                    if (agent.health - bleedOutDamage <= 0)
                    {
                        agent.killedByAgent = lastDamagedAgent;
                    }
                }

                agent.statusEffects.ChangeHealth(-bleedOutDamage, lastDamagedAgent);
            }
        }

        [HarmonyPatch(methodName: nameof(Agent.GetCurSpeed), argumentTypes: new[] { typeof(bool) })]
        [HarmonyPostfix]
        private static void GetCurSpeed_Postfix(ref Agent __instance, ref float __result)
        {
            if (!MqkSorMutatorSimpleInjuries.IsMutatorEnabled || !SIConfig.IsInjured(__instance))
            {
                return;
            }

            if (__instance.name.Contains(value: "Player") ? !SIConfig.IsAffectPlayers : !SIConfig.IsAffectNPCs)
            {
                return;
            }

            __result *= SIConfig.CalculateInjuredSpeedModifier(__instance);
        }

        [HarmonyPatch(methodName: nameof(Agent.AgentFixedUpdate))]
        [HarmonyPostfix]
        private static void AgentFixedUpdate_Postfix(ref Agent __instance)
        {
            if (!MqkSorMutatorSimpleInjuries.IsMutatorEnabled || !SIConfig.IsInjured(__instance))
            {
                return;
            }

            if (SIConfig.IsBloodEnabled && SIConfig.IsBleeding(__instance) && !__instance.dead)
            {
                HandleAgentBleeding(__instance);
            }
            else
            {
                nextBleedTime.Remove(__instance);
            }

            if (SIConfig.IsBleedOutEnabled && SIConfig.IsBleedingOut(__instance) && !__instance.dead)
            {
                HandleAgentBleedOut(__instance);
            }
            else
            {
                nextBleedOutTime.Remove(__instance);
            }
        }
    }
}