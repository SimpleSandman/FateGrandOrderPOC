﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using FateGrandOrderPOC.Shared;
using FateGrandOrderPOC.Shared.AtlasAcademy;
using FateGrandOrderPOC.Shared.Enums;
using FateGrandOrderPOC.Shared.Models;

namespace FateGrandOrderPOC
{
    public class Program
    {
        private readonly ICombatFormula _combatFormula;
        private readonly IServantSkillActivation _skillActivation;
        private readonly IAtlasAcademyClient _aaClient;
        private List<PartyMember> _party = new List<PartyMember>();

        #region Servant IDs
        const string ARTORIA_PENDRAGON_SABER = "100100";
        const string MHX_ASSASSIN = "601800";
        const string TOMOE_GOZEN_ARCHER = "202100";
        const string SUMMER_USHI_ASSASSIN = "603400";
        const string KIARA_ALTER_EGO = "1000300";
        const string ABBY_FOREIGNER = "2500100";
        const string MHXX_FOREIGNER = "2500300";
        const string LANCELOT_BERSERKER = "700200";
        const string DANTES_AVENGER = "1100200";
        const string SKADI_CASTER = "503900";
        const string PARACELSUS_CASTER = "501000";
        const string MORDRED_SABER = "100900";
        #endregion

        #region Craft Essence IDs
        const string KSCOPE_CE = "9400340";
        const string AERIAL_DRIVE_CE = "9402750";
        const string BLACK_GRAIL_CE = "9400480";
        #endregion

        #region Mystic Code IDs
        const string PLUGSUIT_ID = "20";
        const string FRAGMENT_ID = "100";
        const string ARTIC_ID = "110";
        #endregion

        public Program()
        {
            _aaClient = new AtlasAcademyClient("https://api.atlasacademy.io", "NA");
            _skillActivation = new ServantSkillActivation();
            _combatFormula = new CombatFormula(_aaClient);
        }

        static void Main()
        {
            Program program = new Program();
            Task.WaitAll(program.Calculations());
        }

        /// <summary>
        /// Main calculations
        /// </summary>
        private async Task Calculations()
        {
            /* Party data */
            #region Party Member
            CraftEssence chaldeaKscope = new CraftEssence
            {
                CraftEssenceLevel = 100,
                Mlb = true,
                CraftEssenceInfo = await _aaClient.GetCraftEssenceInfo(KSCOPE_CE)
            };

            Servant chaldeaAttackServant = new Servant
            {
                ServantLevel = 90,
                NpLevel = 1,
                FouHealth = 1000,
                FouAttack = 1000,
                SkillLevels = new int[3] { 10, 10, 10 },
                IsSupportServant = false,
                ServantInfo = await _aaClient.GetServantInfo(DANTES_AVENGER)
            };

            PartyMember partyMemberAttacker = _combatFormula.AddPartyMember(_party, chaldeaAttackServant, chaldeaKscope);
            _combatFormula.ApplyCraftEssenceEffects(partyMemberAttacker);

            _party.Add(partyMemberAttacker);
            #endregion

            #region Party Member 2
            Servant chaldeaCaster = new Servant
            {
                ServantLevel = 90,
                NpLevel = 1,
                FouHealth = 1000,
                FouAttack = 1000,
                SkillLevels = new int[3] { 10, 10, 10 },
                IsSupportServant = false,
                ServantInfo = await _aaClient.GetServantInfo(SKADI_CASTER)
            };

            PartyMember partyMemberCaster = _combatFormula.AddPartyMember(_party, chaldeaCaster);

            _party.Add(partyMemberCaster);
            #endregion

            #region Party Member Support
            Servant supportCaster = new Servant
            {
                ServantLevel = 90,
                NpLevel = 1,
                FouHealth = 1000,
                FouAttack = 1000,
                SkillLevels = new int[3] { 10, 10, 10 },
                IsSupportServant = true,
                ServantInfo = await _aaClient.GetServantInfo(SKADI_CASTER)
            };

            PartyMember partyMemberSupportCaster = _combatFormula.AddPartyMember(_party, supportCaster);

            _party.Add(partyMemberSupportCaster);
            #endregion

            MysticCode mysticCode = new MysticCode
            {
                MysticCodeLevel = 4,
                MysticCodeInfo = await _aaClient.GetMysticCodeInfo(ARTIC_ID)
            };

            /* Enemy node data */
            #region First Wave
            EnemyMob enemyMob1 = new EnemyMob
            {
                Id = 0,
                Name = "Walkure",
                ClassName = ClassRelationEnum.Rider,
                AttributeName = AttributeRelationEnum.Sky,
                Gender = GenderRelationEnum.Female,
                WaveNumber = WaveNumberEnum.First,
                Health = 13933.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Divine", "Humanoid", "Female"
                }
            };

            EnemyMob enemyMob2 = new EnemyMob
            {
                Id = 1,
                Name = "Walkure",
                ClassName = ClassRelationEnum.Rider,
                AttributeName = AttributeRelationEnum.Sky,
                Gender = GenderRelationEnum.Female,
                WaveNumber = WaveNumberEnum.First,
                Health = 14786.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Divine", "Humanoid", "Female"
                }
            };

            EnemyMob enemyMob3 = new EnemyMob
            {
                Id = 2,
                Name = "Muspell",
                ClassName = ClassRelationEnum.Berserker,
                AttributeName = AttributeRelationEnum.Earth,
                Gender = GenderRelationEnum.Male,
                WaveNumber = WaveNumberEnum.First,
                Health = 23456.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Giant", "Humanoid", "Male", "Super Large"
                }
            };
            #endregion

            #region Second Wave
            EnemyMob enemyMob4 = new EnemyMob
            {
                Id = 3,
                Name = "Muspell",
                ClassName = ClassRelationEnum.Berserker,
                AttributeName = AttributeRelationEnum.Earth,
                Gender = GenderRelationEnum.Male,
                WaveNumber = WaveNumberEnum.Second,
                Health = 25554.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Giant", "Humanoid", "Male", "Super Large"
                }
            };

            EnemyMob enemyMob5 = new EnemyMob
            {
                Id = 4,
                Name = "Walkure",
                ClassName = ClassRelationEnum.Rider,
                AttributeName = AttributeRelationEnum.Sky,
                Gender = GenderRelationEnum.Female,
                WaveNumber = WaveNumberEnum.Second,
                Health = 19047.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Divine", "Humanoid", "Female"
                }
            };

            EnemyMob enemyMob6 = new EnemyMob
            {
                Id = 5,
                Name = "Muspell",
                ClassName = ClassRelationEnum.Berserker,
                AttributeName = AttributeRelationEnum.Earth,
                Gender = GenderRelationEnum.Male,
                WaveNumber = WaveNumberEnum.Second,
                Health = 26204.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Giant", "Humanoid", "Male", "Super Large"
                }
            };
            #endregion

            #region Third Wave
            EnemyMob enemyMob7 = new EnemyMob
            {
                Id = 6,
                Name = "Walkure",
                ClassName = ClassRelationEnum.Rider,
                AttributeName = AttributeRelationEnum.Sky,
                Gender = GenderRelationEnum.Female,
                WaveNumber = WaveNumberEnum.Third,
                Health = 42926.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Divine", "Humanoid", "Female"
                }
            };

            EnemyMob enemyMob8 = new EnemyMob
            {
                Id = 7,
                Name = "Walkure",
                ClassName = ClassRelationEnum.Rider,
                AttributeName = AttributeRelationEnum.Sky,
                Gender = GenderRelationEnum.Female,
                WaveNumber = WaveNumberEnum.Third,
                Health = 180753.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Divine", "Humanoid", "Female"
                }
            };

            EnemyMob enemyMob9 = new EnemyMob
            {
                Id = 8,
                Name = "Muspell",
                ClassName = ClassRelationEnum.Berserker,
                AttributeName = AttributeRelationEnum.Earth,
                Gender = GenderRelationEnum.Male,
                WaveNumber = WaveNumberEnum.Third,
                Health = 61289.0f,
                IsSpecial = false,
                Traits = new List<string>
                {
                    "Giant", "Humanoid", "Male", "Super Large"
                }
            };
            #endregion

            List<EnemyMob> enemyMobs = new List<EnemyMob> 
            { 
                enemyMob1, enemyMob2, enemyMob3, 
                enemyMob4, enemyMob5, enemyMob6,
                enemyMob7, enemyMob8, enemyMob9
            };

            /* Simulate node combat */
            Console.WriteLine(">>>>>> Fight 1/3 <<<<<<\n");
            _skillActivation.SkillActivation(partyMemberCaster, 1, _party, 1, enemyMobs, 1); // Skadi quick up buff
            _skillActivation.SkillActivation(partyMemberSupportCaster, 1, _party, 1, enemyMobs, 1); // Skadi (support) quick up buff
            _skillActivation.SkillActivation(partyMemberAttacker, 2, _party, 1, enemyMobs, 1); // Dante's 2nd skill

            _combatFormula.NpChargeCheck(_party, partyMemberAttacker);
            await _combatFormula.NoblePhantasmChainSimulator(_party, enemyMobs, WaveNumberEnum.First);

            Console.WriteLine("\n>>>>>> Fight 2/3 <<<<<<\n");
            _skillActivation.AdjustSkillCooldowns(_party);
            _skillActivation.SkillActivation(partyMemberCaster, 3, _party, 1, enemyMobs, 1); // Skadi NP buff

            _combatFormula.NpChargeCheck(_party, partyMemberAttacker);
            await _combatFormula.NoblePhantasmChainSimulator(_party, enemyMobs, WaveNumberEnum.Second);

            Console.WriteLine("\n>>>>>> Fatal 3/3 <<<<<<\n");
            _skillActivation.AdjustSkillCooldowns(_party);
            _skillActivation.SkillActivation(partyMemberSupportCaster, 3, _party, 1, enemyMobs, 1); // Skadi (support) NP buff
            _skillActivation.SkillActivation(partyMemberSupportCaster, 2, _party, 1, enemyMobs, 1); // Skadi (support) enemy defense down
            _skillActivation.SkillActivation(partyMemberCaster, 2, _party, 1, enemyMobs, 1); // Skadi enemy defense down
            _skillActivation.SkillActivation(partyMemberAttacker, 1, _party, 1, enemyMobs, 1); // Dante's 1st skill
            _skillActivation.SkillActivation(mysticCode, 2, _party, 1, enemyMobs, 1); // Artic mystic code ATK and NP damage up

            _combatFormula.NpChargeCheck(_party, partyMemberAttacker);
            await _combatFormula.NoblePhantasmChainSimulator(_party, enemyMobs, WaveNumberEnum.Third);

            Console.WriteLine("Simulation ended! ^.^");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Write("Press any key to exit...");
                Console.ReadKey(); // end program
            }
        }
    }
}
