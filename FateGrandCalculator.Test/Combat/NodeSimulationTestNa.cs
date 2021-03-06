﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Autofac;

using FateGrandCalculator.AtlasAcademy.Json;
using FateGrandCalculator.Enums;
using FateGrandCalculator.Models;

using FateGrandCalculator.Test.Utility;
using FateGrandCalculator.Test.Utility.AutofacConfig;
using FateGrandCalculator.Test.Utility.Fixture;

using FluentAssertions;
using FluentAssertions.Execution;

using Xunit;
using Xunit.Abstractions;

namespace FateGrandCalculator.Test.Combat
{
    public class NodeSimulationTestNa : IClassFixture<WireMockFixture>
    {
        private readonly WireMockFixture _wireMockFixture;
        private readonly WireMockUtility _wireMockUtility;
        private readonly IContainer _container;
        private readonly ITestOutputHelper _output;

        public NodeSimulationTestNa(WireMockFixture wireMockFixture, ITestOutputHelper output)
        {
            _wireMockFixture = wireMockFixture;
            _output = output;
            _wireMockUtility = new WireMockUtility("NA");
            _container = ContainerBuilderInit.Create("NA");
        }

        [Fact]
        public async Task FlamingMansionLB2Dantes()
        {
            _wireMockFixture.CheckIfMockServerInUse();
            _wireMockUtility.AddStubs(_wireMockFixture);

            using (var scope = _container.BeginLifetimeScope())
            {
                ScopedClasses resolvedClasses = AutofacUtility.ResolveScope(scope);
                ConstantExportJson constantExportJson = await FrequentlyUsed.GetConstantExportJsonAsync(resolvedClasses.AtlasAcademyClient);
                List<PartyMember> party = new List<PartyMember>();

                EquipBasicJson equipBasicJson = constantExportJson.ListEquipBasicJson.Find(c => c.Id.ToString() == WireMockUtility.KSCOPE_CE);
                CraftEssence chaldeaSuperscope = FrequentlyUsed.GetCraftEssence(equipBasicJson, 100, true);

                MysticCode mysticCode = new MysticCode
                {
                    MysticCodeLevel = 4,
                    MysticCodeInfo = await resolvedClasses.AtlasAcademyClient.GetMysticCodeInfo(WireMockUtility.ARTIC_ID)
                };

                #region Party Data
                // Dantes
                ServantBasicJson basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.DANTES_AVENGER);
                PartyMember partyMemberAttacker = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 1, false, chaldeaSuperscope);
                party.Add(partyMemberAttacker);

                // Skadi
                basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.SKADI_CASTER);
                PartyMember partyMemberCaster = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses);
                party.Add(partyMemberCaster);

                // Skadi Support
                PartyMember partyMemberSupportCaster = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 1, true);
                party.Add(partyMemberSupportCaster);
                #endregion

                /* Enemy node data */
                #region First Wave
                EnemyMob walkure1 = new EnemyMob
                {
                    Id = 0,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 13933.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure2 = new EnemyMob
                {
                    Id = 1,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 14786.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob muspell1 = new EnemyMob
                {
                    Id = 2,
                    Name = "Muspell",
                    ClassName = ClassRelationEnum.Berserker,
                    AttributeName = AttributeRelationEnum.Earth,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 23456.0f,
                    Traits = new List<string>
                    {
                        "giant", "humanoid", "genderMale", "superGiant"
                    }
                };
                #endregion

                #region Second Wave
                EnemyMob muspell2 = new EnemyMob
                {
                    Id = 3,
                    Name = "Muspell",
                    ClassName = ClassRelationEnum.Berserker,
                    AttributeName = AttributeRelationEnum.Earth,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 25554.0f,
                    Traits = new List<string>
                    {
                        "giant", "humanoid", "genderMale", "superGiant"
                    }
                };

                EnemyMob walkure3 = new EnemyMob
                {
                    Id = 4,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 19047.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob muspell3 = new EnemyMob
                {
                    Id = 5,
                    Name = "Muspell",
                    ClassName = ClassRelationEnum.Berserker,
                    AttributeName = AttributeRelationEnum.Earth,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 26204.0f,
                    Traits = new List<string>
                    {
                        "giant", "humanoid", "genderMale", "superGiant"
                    }
                };
                #endregion

                #region Third Wave
                EnemyMob walkure4 = new EnemyMob
                {
                    Id = 6,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 42926.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure5 = new EnemyMob
                {
                    Id = 7,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 180753.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob muspell4 = new EnemyMob
                {
                    Id = 8,
                    Name = "Muspell",
                    ClassName = ClassRelationEnum.Berserker,
                    AttributeName = AttributeRelationEnum.Earth,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 61289.0f,
                    Traits = new List<string>
                    {
                        "giant", "humanoid", "genderMale", "superGiant"
                    }
                };
                #endregion

                List<EnemyMob> enemyMobs = new List<EnemyMob>
                {
                    walkure1, walkure2, muspell1, // 1st wave
                    muspell2, walkure3, muspell3, // 2nd wave
                    walkure4, walkure5, muspell4  // 3rd wave
                };

                /* Simulate node combat */
                // Fight 1/3
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberCaster, 1, party, enemyMobs, 1); // Skadi quick up buff
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 1, party, enemyMobs, 1); // Skadi (support) quick up buff
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberAttacker, 2, party, enemyMobs, 1); // Dante's 2nd skill

                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyMemberAttacker).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.First, constantExportJson);

                _output.WriteLine($"{partyMemberAttacker.Servant.ServantBasicInfo.Name} has {partyMemberAttacker.NpCharge}% charge after the 1st fight");

                // Fight 2/3
                resolvedClasses.ServantSkillActivation.AdjustSkillCooldowns(party);
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberCaster, 3, party, enemyMobs, 1); // Skadi NP buff

                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyMemberAttacker).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.Second, constantExportJson);

                _output.WriteLine($"{partyMemberAttacker.Servant.ServantBasicInfo.Name} has {partyMemberAttacker.NpCharge}% charge after the 2nd fight");

                // Fight 3/3
                resolvedClasses.ServantSkillActivation.AdjustSkillCooldowns(party);
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 3, party, enemyMobs, 1); // Skadi (support) NP buff
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 2, party, enemyMobs, 1); // Skadi (support) enemy defense down
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberCaster, 2, party, enemyMobs, 1); // Skadi enemy defense down
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberAttacker, 1, party, enemyMobs, 1); // Dante's 1st skill
                resolvedClasses.ServantSkillActivation.SkillActivation(mysticCode, 2, party, enemyMobs, 1); // Artic mystic code ATK and NP damage up

                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyMemberAttacker).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.Third, constantExportJson);

                foreach (EnemyMob enemyMob in enemyMobs.FindAll(e => e.Health > 0.0f))
                {
                    _output.WriteLine($"{enemyMob.Name} has {enemyMob.Health} HP left");
                }

                _output.WriteLine($"{partyMemberAttacker.Servant.ServantBasicInfo.Name} has {partyMemberAttacker.NpCharge}% charge after the 3rd fight");

                using (new AssertionScope())
                {
                    enemyMobs.Count.Should().Be(1);
                    enemyMobs.Find(e => e.Health > 0.0f).Health.Should().Be(47025.5f);
                    partyMemberAttacker.NpCharge.Should().Be(52);
                }
            }
        }

        [Fact]
        public async Task CastleSnowIceLB2ArashZerkalotJack()
        {
            _wireMockFixture.CheckIfMockServerInUse();
            _wireMockUtility.AddStubs(_wireMockFixture);

            using (var scope = _container.BeginLifetimeScope())
            {
                ScopedClasses resolvedClasses = AutofacUtility.ResolveScope(scope);
                ConstantExportJson constantExportJson = await FrequentlyUsed.GetConstantExportJsonAsync(resolvedClasses.AtlasAcademyClient);
                List<PartyMember> party = new List<PartyMember>();

                MysticCode mysticCode = new MysticCode
                {
                    MysticCodeLevel = 10,
                    MysticCodeInfo = await resolvedClasses.AtlasAcademyClient.GetMysticCodeInfo(WireMockUtility.FRAGMENT_2004_ID)
                };

                #region Craft Essence Data
                EquipBasicJson equipBasicJson = constantExportJson.ListEquipBasicJson.Find(c => c.Id.ToString() == WireMockUtility.KSCOPE_CE);
                CraftEssence chaldeaSuperscope = FrequentlyUsed.GetCraftEssence(equipBasicJson, 100, true);
                CraftEssence chaldeaMlbKscope = FrequentlyUsed.GetCraftEssence(equipBasicJson, 30, true);

                equipBasicJson = constantExportJson.ListEquipBasicJson.Find(c => c.Id.ToString() == WireMockUtility.IMAGINARY_ELEMENT_CE);
                CraftEssence chaldeaImaginaryElement = FrequentlyUsed.GetCraftEssence(equipBasicJson, 36, true);
                #endregion

                #region Party Data
                // Lancelot Berserker
                ServantBasicJson basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.LANCELOT_BERSERKER);
                PartyMember partyLancelot = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 5, false, chaldeaSuperscope);
                party.Add(partyLancelot);

                // Arash
                basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.ARASH_ARCHER);
                PartyMember partyArash = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 5, false, chaldeaImaginaryElement);
                party.Add(partyArash);

                // Jack
                basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.JACK_ASSASSIN);
                PartyMember partyJack = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 1, false, chaldeaMlbKscope);
                party.Add(partyJack);

                // Skadi Support
                basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.SKADI_CASTER);
                PartyMember partyMemberSupportCaster = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 1, false);
                party.Add(partyMemberSupportCaster);
                #endregion

                /* Node data */
                #region First Wave
                EnemyMob walkure1 = new EnemyMob
                {
                    Id = 0,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 9884.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure2 = new EnemyMob
                {
                    Id = 1,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 10889.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure3 = new EnemyMob
                {
                    Id = 2,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 10664.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };
                #endregion

                #region Second Wave
                EnemyMob walkure4 = new EnemyMob
                {
                    Id = 3,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 30279.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure5 = new EnemyMob
                {
                    Id = 4,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 24599.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure6 = new EnemyMob
                {
                    Id = 5,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 33264.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };
                #endregion

                #region Third Wave
                EnemyMob walkure7 = new EnemyMob
                {
                    Id = 6,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 41136.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure8 = new EnemyMob
                {
                    Id = 7,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 49586.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };

                EnemyMob walkure9 = new EnemyMob
                {
                    Id = 8,
                    Name = "Walkure",
                    ClassName = ClassRelationEnum.Rider,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 180432.0f,
                    Traits = new List<string>
                    {
                        "divine", "humanoid", "genderFemale"
                    }
                };
                #endregion

                List<EnemyMob> enemyMobs = new List<EnemyMob>
                {
                    walkure1, walkure2, walkure3, // 1st wave
                    walkure4, walkure5, walkure6, // 2nd wave
                    walkure7, walkure8, walkure9  // 3rd wave
                };

                /* Simulate node combat */
                // Fight 1/3
                resolvedClasses.ServantSkillActivation.SkillActivation(partyArash, 3, party, enemyMobs, 1); // Arash NP charge

                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyArash).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.First, constantExportJson);

                // Fight 2/3
                resolvedClasses.ServantSkillActivation.AdjustSkillCooldowns(party);
                resolvedClasses.ServantSkillActivation.SkillActivation(partyLancelot, 3, party, enemyMobs, 1); // Zerkalot NP gain up
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 1, party, enemyMobs, 1); // Skadi quick buff up

                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyLancelot).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.Second, constantExportJson);

                _output.WriteLine($"{partyLancelot.Servant.ServantBasicInfo.Name} has {partyLancelot.NpCharge}% charge after the 2nd fight");

                // Fight 3/3
                resolvedClasses.ServantSkillActivation.AdjustSkillCooldowns(party);
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 2, party, enemyMobs, 1); // Skadi (support) enemy defense down
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 3, party, enemyMobs, 1); // Skadi (support) NP buff
                resolvedClasses.ServantSkillActivation.SkillActivation(mysticCode, 1, party, enemyMobs, 1); // Fragment of 2004's NP strength buff
                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyLancelot).Should().BeTrue();
                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyJack).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.Third, constantExportJson);

                _output.WriteLine($"{partyLancelot.Servant.ServantBasicInfo.Name} has {partyLancelot.NpCharge}% charge after the 3rd fight");
                _output.WriteLine($"{partyJack.Servant.ServantBasicInfo.Name} has {partyJack.NpCharge}% charge after the 3rd fight");

                using (new AssertionScope())
                {
                    enemyMobs.Count.Should().Be(0);
                    partyLancelot.NpCharge.Should().Be(27);
                }
            }
        }

        [Fact]
        public async Task PlugsuitWaverAstolfoDailyDoors()
        {
            _wireMockFixture.CheckIfMockServerInUse();
            _wireMockUtility.AddStubs(_wireMockFixture);

            using (var scope = _container.BeginLifetimeScope())
            {
                ScopedClasses resolvedClasses = AutofacUtility.ResolveScope(scope);
                ConstantExportJson constantExportJson = await FrequentlyUsed.GetConstantExportJsonAsync(resolvedClasses.AtlasAcademyClient);
                List<PartyMember> party = new List<PartyMember>();

                MysticCode mysticCode = new MysticCode
                {
                    MysticCodeLevel = 10,
                    MysticCodeInfo = await resolvedClasses.AtlasAcademyClient.GetMysticCodeInfo(WireMockUtility.PLUGSUIT_ID)
                };

                #region Craft Essence Data
                EquipBasicJson equipBasicJson = constantExportJson.ListEquipBasicJson.Find(c => c.Id.ToString() == WireMockUtility.HOLY_NIGHT_SUPPER_CE);
                CraftEssence chaldeaHolyNightSupper = FrequentlyUsed.GetCraftEssence(equipBasicJson, 33, true);

                equipBasicJson = constantExportJson.ListEquipBasicJson.Find(c => c.Id.ToString() == WireMockUtility.KSCOPE_CE);
                CraftEssence chaldeaSuperscope = FrequentlyUsed.GetCraftEssence(equipBasicJson, 100, true);
                #endregion

                #region Party Data
                // Waver
                ServantBasicJson basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.WAVER_CASTER);
                PartyMember partyWaver = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 2);
                party.Add(partyWaver);

                // Astolfo Rider
                basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.ASTOLFO_RIDER);
                PartyMember partyAstolfo = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 5, false, chaldeaHolyNightSupper);
                partyAstolfo.Servant.ServantLevel = 100;

                party.Add(partyAstolfo);

                // Spartacus Berserker
                basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.SPARTACUS_BERSERKER);
                PartyMember partySpartacus = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 5, false, chaldeaSuperscope);
                party.Add(partySpartacus);

                // Waver Support
                basicJson = constantExportJson.ListServantBasicJson.Find(s => s.Id.ToString() == WireMockUtility.WAVER_CASTER);
                PartyMember partyMemberSupportCaster = await FrequentlyUsed.PartyMemberAsync(basicJson, party, resolvedClasses, 1, true);
                party.Add(partyMemberSupportCaster);
                #endregion

                /* Node data */
                #region First Wave
                EnemyMob doorSaint1 = new EnemyMob
                {
                    Id = 0,
                    Name = "Door of the Saint",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 14464.0f,
                    Traits = new List<string>()
                };

                EnemyMob doorChampion1 = new EnemyMob
                {
                    Id = 1,
                    Name = "Door of the Champion",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 23716.0f,
                    Traits = new List<string>()
                };

                EnemyMob doorSaint2 = new EnemyMob
                {
                    Id = 2,
                    Name = "Door of the Saint",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.First,
                    Health = 14464.0f,
                    Traits = new List<string>()
                };
                #endregion

                #region Second Wave
                EnemyMob doorSaint3 = new EnemyMob
                {
                    Id = 3,
                    Name = "Door of the Saint",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 14464.0f,
                    Traits = new List<string>()
                };

                EnemyMob doorChampion2 = new EnemyMob
                {
                    Id = 4,
                    Name = "Door of the Champion",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 23716.0f,
                    Traits = new List<string>()
                };

                EnemyMob doorChampion3 = new EnemyMob
                {
                    Id = 5,
                    Name = "Door of the Champion",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Second,
                    Health = 23716.0f,
                    Traits = new List<string>()
                };
                #endregion

                #region Third Wave
                EnemyMob doorChampion4 = new EnemyMob
                {
                    Id = 6,
                    Name = "Door of the Champion",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 29596.0f,
                    Traits = new List<string>()
                };

                EnemyMob doorChampion5 = new EnemyMob
                {
                    Id = 7,
                    Name = "Door of the Champion",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 23716.0f,
                    Traits = new List<string>()
                };

                EnemyMob doorChampion6 = new EnemyMob
                {
                    Id = 8,
                    Name = "Door of the Champion",
                    ClassName = ClassRelationEnum.Caster,
                    AttributeName = AttributeRelationEnum.Sky,
                    WaveNumber = WaveNumberEnum.Third,
                    Health = 23716.0f,
                    Traits = new List<string>()
                };
                #endregion

                List<EnemyMob> enemyMobs = new List<EnemyMob>
                {
                    doorSaint1, doorChampion1, doorSaint2,       // wave 1
                    doorSaint3, doorChampion2, doorChampion3,    // wave 2
                    doorChampion4, doorChampion5, doorChampion6  // wave 3
                };

                /* Simulate node combat */
                // Fight 1/3
                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partySpartacus).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.First, constantExportJson);

                // Fight 2/3
                resolvedClasses.ServantSkillActivation.AdjustSkillCooldowns(party);
                resolvedClasses.ServantSkillActivation.SkillActivation(partyAstolfo, 3, party, enemyMobs, 1); // Astolfo NP charge & crit stars & crit damage

                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyAstolfo).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.Second, constantExportJson);

                // Fight 3/3
                resolvedClasses.ServantSkillActivation.AdjustSkillCooldowns(party);
                resolvedClasses.ServantSkillActivation.SkillActivation(partyWaver, 1, party, enemyMobs, 2); // Waver crit damage on Astolfo with 30% charge
                resolvedClasses.ServantSkillActivation.SkillActivation(partyWaver, 2, party, enemyMobs, 2); // Waver defense up to party with 10% charge
                resolvedClasses.ServantSkillActivation.SkillActivation(partyWaver, 3, party, enemyMobs, 2); // Waver attack up to party with 10% charge

                _output.WriteLine("--- Before party swap ---");
                foreach (PartyMember partyMember in party)
                {
                    _output.WriteLine(partyMember.Servant.ServantBasicInfo.Name);
                }

                resolvedClasses.ServantSkillActivation.SkillActivation(mysticCode, 3, party, enemyMobs, 3, 3, 4); // Swap spartacus with Waver

                _output.WriteLine("\n--- After party swap ---");
                foreach (PartyMember partyMember in party)
                {
                    _output.WriteLine(partyMember.Servant.ServantBasicInfo.Name);
                }

                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 1, party, enemyMobs, 2); // Waver (support) crit damage on Astolfo with 30% charge
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 2, party, enemyMobs, 2); // Waver (support) defense up to party with 10% charge
                resolvedClasses.ServantSkillActivation.SkillActivation(partyMemberSupportCaster, 3, party, enemyMobs, 2); // Waver (support) attack up to party with 10% charge
                resolvedClasses.ServantSkillActivation.SkillActivation(mysticCode, 2, party, enemyMobs, 2, 3); // Stun 3rd enemy on the field with plugsuit

                resolvedClasses.CombatFormula.AddPartyMemberToNpChain(party, partyAstolfo).Should().BeTrue();
                resolvedClasses.CombatFormula.NoblePhantasmChainSimulator(party, enemyMobs, WaveNumberEnum.Third, constantExportJson);

                using (new AssertionScope())
                {
                    enemyMobs.Count.Should().Be(0);
                    party.IndexOf(partyMemberSupportCaster).Should().Be(2);
                    party.IndexOf(partySpartacus).Should().Be(3);
                }
            }
        }
    }
}
