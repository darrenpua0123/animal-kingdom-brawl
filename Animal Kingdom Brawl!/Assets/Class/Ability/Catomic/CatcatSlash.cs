﻿using System.Collections.Generic;
using UnityEngine;

public class CatcatSlash : Ability
{
    public override List<AbilityType> abilityType { get; set; } = new List<AbilityType>() { AbilityType.SingleTargetable };
    public override string description { get; set; } = "";

    public override void ActivateAbility(Player caster, List<Player> targetPlayers)
    {
        int damage = 3;

        if (caster.activeEffects.Contains(ActiveEffect.Artifact_Expresso))
        {
            damage--;
        }

        foreach (var targetPlayer in targetPlayers)
        {
            if (targetPlayer.activeEffects.Contains(ActiveEffect.Trap_BookOfCorruption))
            {
                return;
            }

            if (targetPlayer.activeEffects.Contains(ActiveEffect.Piggion_ThickSkin) && targetPlayer.shield > 0)
            {
                if (caster.activeEffects.Contains(ActiveEffect.Catomic_PiercingClawOfPaw))
                {
                    targetPlayer.health -= damage;
                }
                else
                {
                    return;
                }
            }
            else
            {
                targetPlayer.TakeDamage(damage);
            }

            if (targetPlayer.health <= 0)
            {
                targetPlayer.isKnockedOut = true;
                caster.knockoutCounter++;
            }
        }
    }
}