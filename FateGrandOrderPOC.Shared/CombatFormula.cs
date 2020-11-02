﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FateGrandOrderPOC.Shared.AtlasAcademy;
using FateGrandOrderPOC.Shared.AtlasAcademy.Calculations;
using FateGrandOrderPOC.Shared.AtlasAcademy.Json;
using FateGrandOrderPOC.Shared.Enums;
using FateGrandOrderPOC.Shared.Extensions;
using FateGrandOrderPOC.Shared.Models;

using Newtonsoft.Json.Linq;

namespace FateGrandOrderPOC.Shared
{
    public class CombatFormula : ICombatFormula
    {
        private readonly IAtlasAcademyClient _aaClient;
        private readonly AttributeRelation _attributeRelation = new AttributeRelation();
        private readonly ClassRelation _classRelation = new ClassRelation();
        private readonly ClassAttackRate _classAttackRate;
        private readonly ConstantRate _constantRate;

        public CombatFormula(IAtlasAcademyClient client)
        {
            _aaClient = client;
            _constantRate = new ConstantRate(client);
            _classAttackRate = new ClassAttackRate(client);
        }

        /// <summary>
        /// Simulate noble phantasms against a wave of enemies
        /// </summary>
        /// <param name="party"></param>
        /// <param name="enemyMobs"></param>
        /// <param name="waveNumber"></param>
        public async Task NoblePhantasmChainSimulator(List<PartyMember> party, List<EnemyMob> enemyMobs, WaveNumberEnum waveNumber)
        {
            List<PartyMember> npChainList = party
                .FindAll(p => p.NpChainOrder != NpChainOrderEnum.None)
                .OrderBy(p => p.NpChainOrder)
                .ToList();

            // Go through each party member's NP attack in the order of the NP chain provided
            foreach (PartyMember partyMember in npChainList)
            {
                // Check if any enemies are alive
                if (enemyMobs.Count == 0)
                {
                    Console.WriteLine("Node cleared!!");
                    return;
                }

                int npChainPosition = 0; // used to calculate overcharge

                // Determine party member buffs
                float totalNpRefund = 0.0f, cardNpTypeUp = 0.0f, attackUp = 0.0f, powerModifier = 0.0f, npGainUp = 0.0f;
                SetStatusEffects(partyMember, ref cardNpTypeUp, ref attackUp, ref powerModifier, ref npGainUp);

                // Go through each enemy mob grouped by their wave number
                for (int i = 0; i < enemyMobs.FindAll(e => e.WaveNumber == waveNumber).Take(3).Count(); i++)
                {
                    EnemyMob enemyMob = enemyMobs[i];

                    powerModifier += SpecialAttackUp(partyMember, enemyMob);

                    // Determine enemy debuffs
                    float defenseDownModifier = 0.0f, cardDefenseDebuffModifier = 0.0f;
                    SetStatusEffects(enemyMob, partyMember, ref defenseDownModifier, ref cardDefenseDebuffModifier);

                    //Console.WriteLine(">>>>>>>> Stats <<<<<<<<");
                    //Console.WriteLine($"Attribute Multiplier ({enemyMob.Name}): {AttributeModifier(partyMember, enemyMob)}x");
                    //Console.WriteLine($"Class Advantage Multiplier ({enemyMob.Name}): {ClassModifier(partyMember, enemyMob)}x");

                    float baseNpDamage = await BaseNpDamage(partyMember, enemyMob, npChainPosition);
                    //Console.WriteLine($"{partyMember.Servant.ServantInfo.Name}'s base NP damage: {baseNpDamage}");

                    float totalPowerDamageModifier = (1.0f + attackUp + defenseDownModifier + cardDefenseDebuffModifier) 
                        * (1.0f + cardNpTypeUp) 
                        * (1.0f + powerModifier);
                    //Console.WriteLine($"Total power damage modifier: {totalPowerDamageModifier}");

                    float modifiedNpDamage = baseNpDamage * totalPowerDamageModifier;
                    //Console.WriteLine($"Modified NP damage: {modifiedNpDamage}\n");

                    float npDamageForEnemyMob = await AverageNpDamage(partyMember, enemyMob, modifiedNpDamage);
                    //Console.WriteLine($"Average NP damage towards {enemyMob.Name}: {npDamageForEnemyMob}");

                    List<float> npDistributionPercentages = NpDistributionPercentages(partyMember);

                    // Grab NP refund from current enemy
                    totalNpRefund += NpGainedFromEnemy(partyMember, enemyMob, npGainUp, cardNpTypeUp, npDamageForEnemyMob, npDistributionPercentages);

                    // Check health of enemy
                    float chanceToKillEnemyMob = await ChanceToKill(partyMember, enemyMob, modifiedNpDamage);
                    enemyMob.Health = HealthRemaining(enemyMob, npDamageForEnemyMob);

                    Console.WriteLine($"Chance to kill {enemyMob.Name}: {chanceToKillEnemyMob}%\n");
                    Console.WriteLine($"Health remaining: {enemyMob.Health}\n");
                }

                // Replace old charge with newly refunded NP
                totalNpRefund /= 100.0f;
                if (totalNpRefund > 300.0f)
                {
                    totalNpRefund = 300.0f; // set max charge
                }
                partyMember.NpCharge = (float)Math.Floor(totalNpRefund);
                partyMember.NpChainOrder = NpChainOrderEnum.None;

                enemyMobs.RemoveAll(e => e.Health <= 0.0f); // remove dead enemies in preparation for next NP
                npChainPosition++;

                Console.WriteLine($"Total NP refund for {partyMember.Servant.ServantInfo.Name}: {partyMember.NpCharge}%\n");
            }

            return;
        }

        /// <summary>
        /// Find special damage up buff versus an enemy trait
        /// </summary>
        /// <param name="partyMember"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public float SpecialAttackUp(PartyMember partyMember, EnemyMob enemy)
        {
            float powerModifier = 0.0f;
            const float STATUS_EFFECT_DENOMINATOR = 1000.0f;

            foreach (ActiveStatus activeStatus in partyMember.ActiveStatuses)
            {
                Buff buff = activeStatus.StatusEffect.Buffs[0];

                if (buff.Type == "upDamage" 
                    && buff.Vals.Any(f => f.Name == "buffPowerModStrUp") 
                    && buff.CkOpIndv.Any(f => f.Name == buff.Tvals.First().Name) 
                    && enemy.Traits.Contains(buff.Tvals.First().Name))
                {
                    powerModifier += activeStatus.StatusEffect.Svals[activeStatus.AppliedSkillLevel - 1].Value / STATUS_EFFECT_DENOMINATOR;
                }
            }

            return powerModifier;
        }

        /// <summary>
        /// Set necessary party member status effects for NP damage
        /// </summary>
        /// <param name="partyMember"></param>
        /// <param name="cardNpTypeUp"></param>
        /// <param name="attackUp"></param>
        /// <param name="powerModifier"></param>
        /// <param name="npGainUp"></param>
        public void SetStatusEffects(PartyMember partyMember, ref float cardNpTypeUp, ref float attackUp, ref float powerModifier, ref float npGainUp)
        {
            const float STATUS_EFFECT_DENOMINATOR = 1000.0f;

            foreach (ActiveStatus activeStatus in partyMember.ActiveStatuses)
            {
                Buff buff = activeStatus.StatusEffect.Buffs[0];

                if (buff.Type == "upCommandall" && buff.CkSelfIndv.Any(f => f.Name == ($"card{partyMember.NoblePhantasm.Card.ToUpperFirstChar()}")))
                {
                    // Calculate card buff for NP if same card type
                    cardNpTypeUp += activeStatus.StatusEffect.Svals[activeStatus.AppliedSkillLevel - 1].Value / STATUS_EFFECT_DENOMINATOR;
                }
                else if (buff.Type == "upAtk")
                {
                    attackUp += activeStatus.StatusEffect.Svals[activeStatus.AppliedSkillLevel - 1].Value / STATUS_EFFECT_DENOMINATOR;
                }
                else if (buff.Type == "upDropnp")
                {
                    npGainUp += activeStatus.StatusEffect.Svals[activeStatus.AppliedSkillLevel - 1].Value / STATUS_EFFECT_DENOMINATOR;
                }
                else if (buff.Type == "upNpdamage")
                {
                    powerModifier += activeStatus.StatusEffect.Svals[activeStatus.AppliedSkillLevel - 1].Value / STATUS_EFFECT_DENOMINATOR;
                }
            }
        }

        /// <summary>
        /// Set necessary enemy status effects for NP damage
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="partyMember"></param>
        /// <param name="defenseDownModifier"></param>
        /// <param name="cardDefenseDownModifier"></param>
        public void SetStatusEffects(EnemyMob enemy, PartyMember partyMember, ref float defenseDownModifier, ref float cardDefenseDownModifier)
        {
            const float STATUS_EFFECT_DENOMINATOR = 1000.0f;

            foreach (ActiveStatus activeStatus in enemy.ActiveStatuses)
            {
                Buff buff = activeStatus.StatusEffect.Buffs[0];

                if (buff.Type == "downDefence")
                {
                    defenseDownModifier += activeStatus.StatusEffect.Svals[activeStatus.AppliedSkillLevel - 1].Value / STATUS_EFFECT_DENOMINATOR;
                }
                else if (buff.Type == "downDefencecommandall" && buff.CkOpIndv.Any(f => f.Name == ($"card{partyMember.NoblePhantasm.Card.ToUpperFirstChar()}")))
                {
                    cardDefenseDownModifier += activeStatus.StatusEffect.Svals[activeStatus.AppliedSkillLevel - 1].Value / STATUS_EFFECT_DENOMINATOR;
                }
            }
        }

        public float NpGainedFromEnemy(PartyMember partyMember, EnemyMob enemyMob, float npGainUp, float cardNpTypeUp,
            float npDamageForEnemyMob, List<float> npDistributionPercentages)
        {
            float effectiveHitModifier, npRefund = 0.0f;

            foreach (float npHitPerc in npDistributionPercentages)
            {
                if (0.9f * npDamageForEnemyMob * npHitPerc / 100.0f > enemyMob.Health)
                {
                    effectiveHitModifier = 1.5f; // overkill (includes killing blow)
                }
                else
                {
                    effectiveHitModifier = 1.0f; // regular hit
                }

                npRefund += (float)Math.Floor(effectiveHitModifier * CalculatedNpPerHit(partyMember, enemyMob, cardNpTypeUp, npGainUp));

                //Console.WriteLine($"NP Hit Perc: {npHitPerc}% || Effective hit: {effectiveHitModifier} || Accumulated NP refund: {npRefund / 100.0f}");
            }

            Console.WriteLine($"Total NP refund from {enemyMob.Name}: {npRefund / 100.0f}%");
            return npRefund;
        }

        public float CalculatedNpPerHit(PartyMember partyMember, EnemyMob enemyMob, float cardNpTypeUp, float npGainUp)
        {
            return EnemyClassModifier(enemyMob.ClassName.ToString())
                * partyMember.NoblePhantasm.NpGain.Np[partyMember.Servant.NpLevel - 1]
                * (1.0f + cardNpTypeUp)
                * (1.0f + npGainUp);
        }

        public List<float> NpDistributionPercentages(PartyMember partyMember)
        {
            float perc, lastNpHitPerc = 0.0f;
            List<float> percNpHitDistribution = new List<float>();

            /* Get NP distribution values and percentages */
            foreach (int npHit in partyMember.NoblePhantasm.NpDistribution)
            {
                perc = npHit + lastNpHitPerc;

                //Console.WriteLine($"NP Hit: {npHit}, Perc: {perc}%");
                percNpHitDistribution.Add(perc);

                lastNpHitPerc = perc;
            }

            return percNpHitDistribution;
        }

        public float HealthRemaining(EnemyMob enemyMob, float npDamageForEnemyMob)
        {
            float healthRemaining = enemyMob.Health - (npDamageForEnemyMob * 0.9f);
            if (healthRemaining < 0.0f)
            {
                healthRemaining = 0.0f;
            }

            return healthRemaining;
        }

        public async Task<float> BaseNpDamage(PartyMember partyMember, EnemyMob enemy, int npChainPosition)
        {
            Function npFunction = partyMember
                .NoblePhantasm
                .Functions
                .Find(f => f.FuncType.Contains("damageNp"));

            Sval svalNp = Overcharge(partyMember.NpCharge, npChainPosition) switch
            {
                5 => npFunction.Svals5[partyMember.Servant.NpLevel - 1],
                4 => npFunction.Svals4[partyMember.Servant.NpLevel - 1],
                3 => npFunction.Svals3[partyMember.Servant.NpLevel - 1],
                2 => npFunction.Svals2[partyMember.Servant.NpLevel - 1],
                _ => npFunction.Svals[partyMember.Servant.NpLevel - 1],
            };

            int npValue = svalNp.Value;
            int targetBonusNpDamage = 0;

            // Add additional NP damage if there is a special target ID
            if (svalNp.Target > 0)
            {
                JObject traits = await _aaClient.GetTraitEnumInfo();
                string traitName = traits.Property(svalNp.Target.ToString())?.Value.ToString() ?? "";
                if (enemy.Traits.Contains(traitName))
                {
                    targetBonusNpDamage = svalNp.Correction;
                }
            }

            //Console.WriteLine($"Total attack: {partyMember.TotalAttack}");
            //Console.WriteLine($"Class modifier: {_classAttackRate.GetAttackMultiplier(partyMember.Servant.ServantInfo.ClassName)}");
            //Console.WriteLine($"NP type modifier: {_constantRate.GetAttackMultiplier("enemy_attack_rate_" + partyMember.NoblePhantasm.Card)}");
            //Console.WriteLine($"NP value: {npValue / 1000.0f}");
            //Console.WriteLine($"Target Bonus NP damage: {targetBonusNpDamage / 1000.0f}");

            // Base NP damage = ATTACK_RATE * Servant total attack * Class modifier * NP type modifier * NP damage
            float baseNpDamage = await _constantRate.GetAttackMultiplier("ATTACK_RATE")
                * partyMember.TotalAttack
                * await _classAttackRate.GetAttackMultiplier(partyMember.Servant.ServantInfo.ClassName)
                * await _constantRate.GetAttackMultiplier($"ENEMY_ATTACK_RATE_{partyMember.NoblePhantasm.Card}")
                * (npValue / 1000.0f);

            if (targetBonusNpDamage != 0)
            {
                baseNpDamage *= targetBonusNpDamage / 1000.0f;
            }

            return baseNpDamage;
        }

        /// <summary>
        /// Return the overcharge in decimal (non-percent) format based on NP charge and the position in the NP chain
        /// </summary>
        /// <param name="npCharge"></param>
        /// <param name="npChainPosition"></param>
        /// <returns></returns>
        public int Overcharge(float npCharge, int npChainPosition)
        {
            int overcharge = 0;

            // Set overcharge based on the level of NP charge
            if (npCharge == 300.0f)
            {
                overcharge = 3;
            }
            else if (npCharge >= 200.0f)
            {
                overcharge = 2;
            }

            // Add additional overcharge depending on the position in the NP chain
            if (npChainPosition == 1) // 2nd NP in the chain
            {
                overcharge += 1;
            }
            else if (npChainPosition == 2) // 3rd NP in the chain
            {
                overcharge += 2;
            }

            return overcharge;
        }

        public async Task<float> AverageNpDamage(PartyMember partyMember, EnemyMob enemyMob, float modifiedNpDamage)
        {
            return modifiedNpDamage * await AttributeModifier(partyMember, enemyMob) * await ClassModifier(partyMember, enemyMob);
        }

        public async Task<float> ChanceToKill(PartyMember partyMember, EnemyMob enemyMob, float modifiedNpDamage)
        {
            if (0.9f * await AverageNpDamage(partyMember, enemyMob, modifiedNpDamage) > enemyMob.Health)
            {
                return 100.0f; // perfect clear, even with the worst RNG
            }
            else if (1.1f * await AverageNpDamage(partyMember, enemyMob, modifiedNpDamage) < enemyMob.Health)
            {
                return 0.0f; // never clear, even with the best RNG
            }
            else // show chance of success
            {
                return (1.0f - ((enemyMob.Health / await AverageNpDamage(partyMember, enemyMob, modifiedNpDamage) - 0.9f) / 0.2f)) * 100.0f;
            }
        }

        #region Private Methods
        /// <summary>
        /// NP gain modifier based on enemy class and special (server-side data)
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private float EnemyClassModifier(string className)
        {
            switch (className.ToLower())
            {
                case "berserker":
                    return 0.8f;
                case "assassin":
                    return 0.9f;
                case "saber":
                case "archer":
                case "lancer":
                case "ruler":
                case "avenger":
                case "alterEgo":
                case "foreigner":
                case "shielder":
                    return 1.0f;
                case "rider":
                    return 1.1f;
                case "caster":
                case "moonCancer":
                    return 1.2f;
                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Compares the relationship between two monster's attributes and shows the resulting modifier
        /// </summary>
        /// <param name="partyMember"></param>
        /// <param name="enemyMob"></param>
        /// <returns></returns>
        private async Task<float> AttributeModifier(PartyMember partyMember, EnemyMob enemyMob)
        {
            return await _attributeRelation.GetAttackMultiplier(partyMember.Servant.ServantInfo.Attribute, enemyMob.AttributeName.ToString().ToLower());
        }

        /// <summary>
        /// Compares the relationship between two monster's classes and shows the resulting modifier
        /// </summary>
        /// <param name="partyMember"></param>
        /// <param name="enemyMob"></param>
        /// <returns></returns>
        private async Task<float> ClassModifier(PartyMember partyMember, EnemyMob enemyMob)
        {
            return await _classRelation.GetAttackMultiplier(partyMember.Servant.ServantInfo.ClassName, enemyMob.ClassName.ToString().ToLower());
        }
        #endregion
    }
}
