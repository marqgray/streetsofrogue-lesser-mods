using BepInEx.Configuration;
using UnityEngine;

namespace mqKeezy_Mutator_SimpleInjuries
{
    public class SimpleInjuriesConfig
    {
        private readonly ConfigEntry<int> injuredHealthPercentage;
        private readonly ConfigEntry<int> injuredHealthSpeedPercentageMin;
        private readonly ConfigEntry<int> injuredHealthSpeedPercentageMax;
        private readonly ConfigEntry<bool> affectPlayers;
        private readonly ConfigEntry<bool> affectNPCs;
        private readonly ConfigEntry<bool> bleedOutEnabled;
        private readonly ConfigEntry<int> bleedOutHealthPercentage;
        private readonly ConfigEntry<int> bleedOutDamagePercentage;
        private readonly ConfigEntry<bool> bloodEnabled;
        private readonly ConfigEntry<int> bleedHealthPercentage;
        private readonly ConfigEntry<float> bleedTimeMinSeconds;
        private readonly ConfigEntry<float> bleedTimeMaxSeconds;
        private readonly ConfigEntry<int> bleedScaleMinPercentage;
        private readonly ConfigEntry<int> bleedScaleMaxPercentage;

        private float InjuredHealthPercentage => injuredHealthPercentage.Value / 100f;
        private float InjuredHealthSpeedPercentageMin => injuredHealthSpeedPercentageMin.Value / 100f;
        private float InjuredHealthSpeedPercentageMax => injuredHealthSpeedPercentageMax.Value / 100f;
        public bool IsAffectPlayers => affectPlayers.Value;
        public bool IsAffectNPCs => affectNPCs.Value;
        public bool IsBleedOutEnabled => bleedOutEnabled.Value;
        private float BleedOutHealthPercentage => bleedOutHealthPercentage.Value / 100f;
        private float BleedOutDamagePercentage => bleedOutDamagePercentage.Value / 100f;
        public bool IsBloodEnabled => GameController.gameController.bloodEnabled && bloodEnabled.Value;
        private float BleedHealthPercentage => bleedHealthPercentage.Value / 100f;
        public float BleedTimeMinSeconds => bleedTimeMinSeconds.Value;
        public float BleedTimeMaxSeconds => bleedTimeMaxSeconds.Value;
        public float BleedScaleMinPercentage => bleedScaleMinPercentage.Value / 100f;
        public float BleedScaleMaxPercentage => bleedScaleMaxPercentage.Value / 100f;

        public SimpleInjuriesConfig(ConfigFile config)
        {
            injuredHealthPercentage = config.Bind(section: "General", key: "InjuredHealthPercentage",
                defaultValue: 40,
                description:
                "The percentage of health when a dweller becomes injured. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            injuredHealthSpeedPercentageMin = config.Bind(section: "General",
                key: "InjuredHealthSpeedPercentageMin",
                defaultValue: 10,
                description:
                "The minimum percentage speed a dweller moves when injured. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            injuredHealthSpeedPercentageMax = config.Bind(section: "General",
                key: "InjuredHealthSpeedPercentageMax",
                defaultValue: 80,
                description:
                "The maximum percentage speed a dweller moves when injured. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            affectPlayers = config.Bind(section: "General",
                key: "AffectPlayers",
                defaultValue: true,
                description:
                "Set this to true or false if you want the injuries to affect players.");

            affectNPCs = config.Bind(section: "General",
                key: "AffectNPCs",
                defaultValue: true,
                description:
                "Set this to true or false if you want the injuries to affect the other dwellers.");

            bleedOutEnabled = config.Bind(section: "General",
                key: "IsBleedOutEnabled",
                defaultValue: true,
                description:
                "Set this to true or false if you want dwellers to bleed out & slowly lose health til death when injured.");

            bleedOutHealthPercentage = config.Bind(section: "General",
                key: "BleedOutHealthPercentage",
                defaultValue: 20,
                description:
                "The percentage of health of a dweller that bleedout occurs. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            bleedOutDamagePercentage = config.Bind(section: "General",
                key: "BleedOutDamagePercentage",
                defaultValue: 1,
                description:
                "The percentage of health that a dweller gets damaged by during bleed out every second. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            bloodEnabled = config.Bind(section: "General",
                key: "IsBloodEnabled",
                defaultValue: true,
                description:
                "Set this to true or false if you want dwellers to bleed when injured.");

            bleedHealthPercentage = config.Bind(section: "General",
                key: "BleedHealthPercentage",
                defaultValue: 20,
                description:
                "The percentage of health when a dweller starts to bleed. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            bleedTimeMinSeconds = config.Bind(section: "General",
                key: "BleedTimeMinSeconds",
                defaultValue: 2f,
                description:
                "The minimum number of seconds required for a dweller to bleed.");

            bleedTimeMaxSeconds = config.Bind(section: "General",
                key: "BleedTimeMaxSeconds",
                defaultValue: 4f,
                description:
                "The maximum number of seconds required for a dweller to bleed.");

            bleedScaleMinPercentage = config.Bind(section: "General",
                key: "BleedScaleMin",
                defaultValue: 20,
                description:
                "The percentage value for the lower bound of blood splatter sizes. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");

            bleedScaleMaxPercentage = config.Bind(section: "General",
                key: "BleedScaleMax",
                defaultValue: 50,
                description:
                "The percentage value for the upper bound of blood splatter sizes. Ex: 20 = 20%, 30 = 30%, 50 = 50%.");
        }
        
        public float CalculateInjuredSpeedModifier(Agent agent)
        {
            float percentage = agent.health / (agent.healthMax * InjuredHealthPercentage);
            return Mathf.Lerp(InjuredHealthSpeedPercentageMin, InjuredHealthSpeedPercentageMax, percentage);
        }
        
        public float CalculateBleedOutDamage(Agent agent)
        {
            return agent.healthMax * BleedOutDamagePercentage;
        }
        
        public bool IsInjured(Agent agent)
        {
            return agent.health <= agent.healthMax * InjuredHealthPercentage;
        }
        
        public bool IsBleeding(Agent agent)
        {
            return agent.health < agent.healthMax * BleedHealthPercentage;
        }

        public bool IsBleedingOut(Agent agent)
        {
            return agent.health < agent.healthMax * BleedOutHealthPercentage;
        }
    }
}