﻿namespace FateGrandOrderPOC.Shared
{
    public interface IBaseRelation
    {
        float GetAttackMultiplier(string attack);
        float GetAttackMultiplier(string attack, string defend);
    }
}
