﻿using FateGrandOrderPOC.Shared.AtlasAcademy.Json;

namespace FateGrandOrderPOC.Shared.Models
{
    public class Servant
    {
        public int ServantLevel { get; set; }
        public int SkillLevel1 { get; set; }
        public int SkillLevel2 { get; set; }
        public int SkillLevel3 { get; set; }
        public int NpLevel { get; set; }
        public int FouAttack { get; set; }
        public int FouHealth { get; set; }
        public bool IsSupportServant { get; set; }
        public ServantNiceJson ServantInfo { get; set; }
    }
}