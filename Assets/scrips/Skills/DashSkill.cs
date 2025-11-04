using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : Skill
{
    [Header("Crystal instead of clone")]
    [SerializeField] private bool crystalInsteadOfMirage;

    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;

    public override void UseSkiil()
    {
        base.UseSkiil();
        
    }

    public void doDashSkillOnStart()
    {
        if (createCloneOnDashStart)
        {
            if (crystalInsteadOfMirage)
            {
                player.skill.crystal.CreateCrystal();
            }
            else
            {
                player.skill.clone.CreateClone(player.transform, Vector3.zero, false);
            }
        }

    }

    public void doDashSkillOnFinish() 
    {
        if (createCloneOnDashOver)
        {
            if (crystalInsteadOfMirage)
            {
                player.skill.crystal.CreateCrystal();
            }
            else
            {
                player.skill.clone.CreateClone(player.transform, Vector3.zero, false);
            }
        }
    }
}
