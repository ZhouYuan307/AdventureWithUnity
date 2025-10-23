using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : Skill
{
    public override void UseSkiil()
    {
        base.UseSkiil();
        Debug.Log("create clone behind");
    }
}
