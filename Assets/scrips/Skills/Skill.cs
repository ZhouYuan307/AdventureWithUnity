using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool TryUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkiil();
            cooldownTimer = cooldown;
            return true;
        }

        Debug.Log("skill is on cooldown");
        return false;
    }

    public virtual void UseSkiil()
    {

    }
}
