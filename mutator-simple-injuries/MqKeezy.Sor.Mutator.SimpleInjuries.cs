using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using mqKeezy_Mutator_SimpleInjuries.Properties;
using RogueLibsCore;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

#pragma warning disable 618

namespace mqKeezy_Mutator_SimpleInjuries
{
    [BepInPlugin(ModInfo.BepInExPluginId, ModInfo.Title, ModInfo.Version)]
    public class MqkSorMutatorSimpleInjuries : BaseUnityPlugin
    {
        private static ConfigEntry<int> configInjuredHealthPercentage;
        private static ConfigEntry<int> configInjuredHealthSpeedPercentageMin;
        private static ConfigEntry<int> configInjuredHealthSpeedPercentageMax;
        private static ConfigEntry<bool> configAffectPlayers;
        private static ConfigEntry<bool> configAffectNPCs;
        private static ConfigEntry<bool> configBleedOutEnabled;
        private static ConfigEntry<int> configBleedOutHealthPercentage;
        private static ConfigEntry<int> configBleedOutDamagePercentage;
        private static ConfigEntry<bool> configBloodEnabled;
        private static ConfigEntry<int> configBleedHealthPercentage;
        private static ConfigEntry<float> configBleedTimeMinSeconds;
        private static ConfigEntry<float> configBleedTimeMaxSeconds;
        private static ConfigEntry<int> configBleedScaleMinPercentage;
        private static ConfigEntry<int> configBleedScaleMaxPercentage;
        public static CustomMutator Mutator;

        public static float InjuredHealthPercentage
        {
            get => configInjuredHealthPercentage.Value / 100f;
            private set => throw new NotImplementedException();
        }

        public static float InjuredHealthSpeedPercentageMinimum
        {
            get => configInjuredHealthSpeedPercentageMin.Value / 100f;
            private set => throw new NotImplementedException();
        }

        public static float InjuredHealthSpeedPercentageMaximum
        {
            get => configInjuredHealthSpeedPercentageMax.Value / 100f;
            private set => throw new NotImplementedException();
        }

        public static bool IsPlayersAffected
        {
            get => configAffectPlayers.Value;
            private set => throw new NotImplementedException();
        }

        public static bool IsNPCsAffected
        {
            get => configAffectNPCs.Value;
            private set => throw new NotImplementedException();
        }

        public static bool isBleedOutEnabled
        {
            get => configBleedOutEnabled.Value;
            private set => throw new NotImplementedException();
        }

        public static float BleedOutHealthPercentage
        {
            get => configBleedOutHealthPercentage.Value / 100f;
            private set => throw new NotImplementedException();
        }

        public static float BleedOutDamagePercentage
        {
            get => configBleedOutDamagePercentage.Value / 100f;
            private set => throw new NotImplementedException();
        }

        public static bool isBloodEnabled
        {
            get => GameController.gameController.bloodEnabled && configBloodEnabled.Value;
            private set => throw new NotImplementedException();
        }

        public static float BleedHealthPercentage
        {
            get => configBleedHealthPercentage.Value / 100f;
            private set => throw new NotImplementedException();
        }

        public static float BleedTimeMinimumSeconds
        {
            get => configBleedTimeMinSeconds.Value;
            private set => throw new NotImplementedException();
        }

        public static float BleedTimeMaximumSeconds
        {
            get => configBleedTimeMaxSeconds.Value;
            private set => throw new NotImplementedException();
        }

        public static float BleedScaleMinimumPercentage
        {
            get => configBleedScaleMinPercentage.Value / 100f;
            private set => throw new NotImplementedException();
        }

        public static float BleedScaleMaximumPercentage
        {
            get => configBleedScaleMaxPercentage.Value / 100f;
            private set => throw new NotImplementedException();
        }

        private void Awake()
        {
            Mutator = RogueLibs.CreateCustomMutator(id: "mqKeezy.SimpleInjuries",
                unlockedFromStart: true,
                new CustomNameInfo(english: "Simple Injuries"),
                new CustomNameInfo(english: ""));

            configInjuredHealthPercentage = Config.Bind(section: "General", key: "InjuredHealthPercentage",
                defaultValue: 40,
                description:
                "The percentage of health when a dweller becomes injured. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            configInjuredHealthSpeedPercentageMin = Config.Bind(section: "General",
                key: "InjuredHealthSpeedPercentageMin",
                defaultValue: 10,
                description:
                "The minimum percentage speed a dweller moves when injured. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            configInjuredHealthSpeedPercentageMax = Config.Bind(section: "General",
                key: "InjuredHealthSpeedPercentageMax",
                defaultValue: 80,
                description:
                "The maximum percentage speed a dweller moves when injured. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            configAffectPlayers = Config.Bind(section: "General",
                key: "AffectPlayers",
                defaultValue: true,
                description:
                "Set this to true or false if you want the injuries to affect players.");

            configAffectNPCs = Config.Bind(section: "General",
                key: "AffectNPCs",
                defaultValue: true,
                description:
                "Set this to true or false if you want the injuries to affect the other dwellers.");

            configBleedOutEnabled = Config.Bind(section: "General",
                key: "BleedOutEnabled",
                defaultValue: true,
                description:
                "Set this to true or false if you want dwellers to bleed out & slowly lose health til death when injured.");

            configBleedOutHealthPercentage = Config.Bind(section: "General",
                key: "BleedOutHealthPercentage",
                defaultValue: 20,
                description:
                "The percentage of health of a dweller that bleedout occurs. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            configBleedOutDamagePercentage = Config.Bind(section: "General",
                key: "BleedOutDamagePercentage",
                defaultValue: 1,
                description:
                "The percentage of health that a dweller gets damaged by during bleed out every second. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            configBloodEnabled = Config.Bind(section: "General",
                key: "BloodEnabled",
                defaultValue: true,
                description:
                "Set this to true or false if you want dwellers to bleed when injured.");

            configBleedHealthPercentage = Config.Bind(section: "General",
                key: "BleedHealthPercentage",
                defaultValue: 20,
                description:
                "The percentage of health when a dweller starts to bleed. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            configBleedTimeMinSeconds = Config.Bind(section: "General",
                key: "BleedTimeMinSeconds",
                defaultValue: 2f,
                description:
                "The minimum number of seconds required for a dweller to bleed.");

            configBleedTimeMaxSeconds = Config.Bind(section: "General",
                key: "BleedTimeMaxSeconds",
                defaultValue: 4f,
                description:
                "The minimum number of seconds required for a dweller to bleed.");

            configBleedScaleMinPercentage = Config.Bind(section: "General",
                key: "BleedScaleMin",
                defaultValue: 20,
                description:
                "The percentage of health when a dweller starts to bleed. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            configBleedScaleMaxPercentage = Config.Bind(section: "General",
                key: "BleedScaleMax",
                defaultValue: 50,
                description:
                "The percentage of health when a dweller starts to bleed. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            new Harmony(ModInfo.BepInExHarmonyPatchesId).PatchAll();
        }

        private static bool IsInjured(Agent agent)
        {
            return agent.health <= agent.healthMax * InjuredHealthPercentage;
        }

        private static bool isBleeding(Agent agent)
        {
            return agent.health < agent.healthMax * BleedHealthPercentage;
        }

        private static bool isBleedingOut(Agent agent)
        {
            return agent.health < agent.healthMax * BleedOutHealthPercentage;
        }

        private static float CalculateInjuredHealthSpeed(Agent agent, int currentMaxSpeed)
        {
            float min = InjuredHealthSpeedPercentageMinimum;
            float max = InjuredHealthSpeedPercentageMaximum;
            float percentage = agent.health / (agent.healthMax * InjuredHealthPercentage);
            return currentMaxSpeed * (percentage * (max - min)) + min;
        }

        private static float CalculateBleedOutHealthChange(Agent agent)
        {
            return -(agent.healthMax * BleedOutDamagePercentage);
        }

        private static class Patches
        {
            private static readonly Dictionary<Agent, float>
                lastBleedTime = new Dictionary<Agent, float>();

            private static readonly Dictionary<Agent, float> lastBleedOutTime = new Dictionary<Agent, float>();

            private static readonly Dictionary<Agent, PlayfieldObject> agentLastDamagedBy =
                new Dictionary<Agent, PlayfieldObject>();

            private static int placeholderSpeedMax;

            private static class StatusEffectsPatch
            {
                [HarmonyPatch(typeof(StatusEffects), nameof(StatusEffects.ChangeHealth), typeof(float),
                    typeof(PlayfieldObject), typeof(NetworkInstanceId), typeof(float), typeof(string), typeof(byte))]
                private static class ChangeHealth
                {
                    [HarmonyPostfix]
                    // ReSharper disable once SuggestBaseTypeForParameter
                    private static void Postfix(ref StatusEffects __instance, PlayfieldObject damagerObject)
                    {
                        if (Mutator?.IsActive != true || !IsInjured(__instance.agent))
                        {
                            return;
                        }

                        Agent attacker = null;
                        if (damagerObject != null)
                        {
                            Bullet bullet = damagerObject as Bullet;
                            Melee melee = damagerObject as Melee;
                            Noise noise = damagerObject as Noise;
                            Explosion explosion = damagerObject as Explosion;
                            Fire fire = damagerObject as Fire;
                            Gas gas = damagerObject as Gas;
                            if (bullet != null)
                            {
                                attacker = bullet.agent;
                            }
                            else if (melee != null)
                            {
                                attacker = melee.agent;
                            }
                            else if (noise != null)
                            {
                                attacker = noise.agent;
                            }
                            else if (explosion != null)
                            {
                                attacker = explosion.agent;
                            }
                            else if (fire != null)
                            {
                                attacker = fire.agent;
                            }
                            else if (gas != null)
                            {
                                attacker = gas.agent;
                            }
                        }

                        if (!agentLastDamagedBy.ContainsKey(__instance.agent) && attacker != null)
                        {
                            agentLastDamagedBy.Add(__instance.agent, attacker);
                        }
                        else if (attacker != null)
                        {
                            agentLastDamagedBy[__instance.agent] = attacker;
                        }
                    }
                }
            }

            private static class AgentPatch
            {
                [HarmonyPatch(typeof(Agent), nameof(Agent.GetCurSpeed), typeof(bool))]
                private static class GetCurSpeed
                {
                    [HarmonyPrefix]
                    private static bool Prefix(ref Agent __instance)
                    {
                        if (Mutator?.IsActive != true || !IsInjured(__instance))
                        {
                            return true;
                        }

                        if (!IsPlayersAffected && __instance.name.Contains(value: "Player"))
                        {
                            return true;
                        }

                        if (!IsNPCsAffected && !__instance.name.Contains(value: "Player"))
                        {
                            return true;
                        }

                        placeholderSpeedMax = __instance.speedMax;
                        __instance.speedMax = (int) CalculateInjuredHealthSpeed(__instance, __instance.speedMax);

                        return true;
                    }

                    [HarmonyPostfix]
                    private static void Postfix(ref Agent __instance)
                    {
                        if (Mutator?.IsActive != true || placeholderSpeedMax == 0)
                        {
                            return;
                        }

                        __instance.speedMax = placeholderSpeedMax;
                        placeholderSpeedMax = 0;
                    }
                }

                [HarmonyPatch(typeof(Agent), nameof(Agent.AgentFixedUpdate))]
                private static class AgentFixedUpdate
                {
                    [HarmonyPostfix]
                    private static void Postfix(ref Agent __instance)
                    {
                        if (Mutator?.IsActive != true || !IsInjured(__instance))
                        {
                            return;
                        }

                        if (isBloodEnabled && isBleeding(__instance))
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                if (!lastBleedTime.ContainsKey(__instance))
                                {
                                    lastBleedTime.Add(__instance, Time.realtimeSinceStartup - BleedTimeMaximumSeconds);
                                }

                                if (__instance == null || __instance.dead)
                                {
                                    lastBleedTime.Remove(__instance);
                                    break;
                                }

                                if (!(Time.realtimeSinceStartup - lastBleedTime[__instance] > 0f))
                                {
                                    break;
                                }

                                lastBleedTime[__instance] = Time.realtimeSinceStartup +
                                                            Random.Range(BleedTimeMinimumSeconds,
                                                                BleedTimeMaximumSeconds);

                                Vector3 decalPos = __instance.tr.position;
                                Vector3 position = new Vector3(decalPos.x, decalPos.y, z: 0f);
                                if (GameController.gameController.streamingWorld)
                                {
                                    position = new Vector3(decalPos.x, decalPos.y,
                                        position.z + (decalPos.y - 1.28f) * 100f / 100000f);
                                }

                                GameObject gameObject =
                                    GameController.gameController.spawnerMain.floorDecalPrefab.Spawn(position);
                                tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                                GameController.gameController.floorDecalsList.Add(gameObject);
                                component.SetSprite(spriteName: "BloodDecal");
                                int num = Random.Range(min: 0, max: 16);
                                Vector3 scale = gameObject.transform.localScale;
                                scale *= Random.Range(BleedScaleMinimumPercentage, BleedScaleMaximumPercentage);
                                gameObject.transform.localScale = scale;
                                gameObject.transform.rotation = Quaternion.Euler(x: 0f, y: 0f, num * 22.5f);
                            }
                        }

                        if (!isBleedOutEnabled || !isBleedingOut(__instance))
                        {
                            return;
                        }

                        {
                            for (int i = 0; i < 1; i++)
                            {
                                if (!lastBleedOutTime.ContainsKey(__instance))
                                {
                                    lastBleedOutTime.Add(__instance, Time.realtimeSinceStartup + 1);
                                }

                                if (__instance == null || __instance.dead)
                                {
                                    lastBleedOutTime.Remove(__instance);
                                    break;
                                }

                                if (!(Time.realtimeSinceStartup - lastBleedOutTime[__instance] > 0f))
                                {
                                    break;
                                }

                                lastBleedOutTime[__instance] = Time.realtimeSinceStartup + 1f;
                                agentLastDamagedBy.TryGetValue(__instance, out PlayfieldObject lastDamagedAgent);
                                if (lastDamagedAgent != null)
                                {
                                    __instance.justHitByAgent = lastDamagedAgent as Agent;
                                    __instance.justHitByAgent2 = lastDamagedAgent as Agent;
                                    if (__instance.health - CalculateBleedOutHealthChange(__instance) <= 0)
                                    {
                                        __instance.killedByAgent = lastDamagedAgent as Agent;
                                    }

                                    __instance.statusEffects.ChangeHealth(CalculateBleedOutHealthChange(__instance),
                                        lastDamagedAgent as Agent);
                                }
                                else
                                {
                                    __instance.statusEffects.ChangeHealth(CalculateBleedOutHealthChange(__instance));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}