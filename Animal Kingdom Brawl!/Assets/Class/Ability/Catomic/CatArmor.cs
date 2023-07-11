﻿using System.Collections.Generic;

public class CatArmor : Ability
{
    public override AbilityType abilityType { get; set; } = AbilityType.NonTargetable;
    public override string description { get; set; } = "";

    public override void ActivateAbility(Player caster, List<Player> targetPlayers)
    {
        caster.shield++;
        caster.actionPoint++;
    }
}