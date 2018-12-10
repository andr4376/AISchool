// -----------------------------------------------------------------------
// <copyright file="AIModifiers.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace AIFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class AIModifiers
    {
        public const int initialAgentCount = 20;
        public const float maxMeleeAttackRange = 10.0f;
		public const float maxProcreateRange = 10.0f;
        public const float maxFeedingRange = 10.0f;

        public const int maxAttributePoints = 250;
        public const float hungerIncrementPerSecond = 5.0f;
        public const float maxHungerBeforeHitpointsDamage = 100f;
        public const float hungerHitpointsDamagePerSecond = 2.0f;
        public const float hitpointRegenPerSecond = 1.0f;
        public const float hungerReductionPerFeeding = 20.0f;
        public const int baseChanceOfAttackSuccess = 50;
        public const int minChanceOfAttackSuccess = 10;
        public const float procreationReductionPerSecond = 1.0f;
        public const float initialProcreationCount = 20f;

        public static int initialPlants = (int)Math.Ceiling(initialAgentCount *0.5);
        public static int newPlantsPerSecond = (int)Math.Ceiling(initialAgentCount/8f);

        public const float maxRunningTimeInSeconds = 200;
    }
}
