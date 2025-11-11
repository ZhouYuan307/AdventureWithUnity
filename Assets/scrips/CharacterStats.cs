using System.Xml.Schema;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats")]
    public Stat strength; //increase damage
    public Stat agility; //increase evasion
    public Stat intelligence; //increase magic damage and magic resistance
    public Stat vitality; //increase health

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;  //50% by default


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    private bool isMiss;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;  //do damage over time
    public bool isChilled;  //reduce armor by 20%
    public bool isShocked;  //reduce accuracy by 20%

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = .5f;
    private float igniteDamageTimer;
    private int igniteDamage;


    [SerializeField]private int currentHealth;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
        critPower.SetDefaultValue(150);
        isMiss = false;
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }

        if (chilledTimer < 0)
        {
            isChilled = false;
        }

        if(shockedTimer < 0)
        {
            isShocked = false;
        }

        if (igniteDamageTimer < 0 && isIgnited)
        {
            Debug.Log("burn!" + igniteDamage);
            TakeDamage(igniteDamage);
            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public virtual void DoDamage(CharacterStats _targetStats)
    {

        //apply evasion
        if (TryAvoidAttack(_targetStats))
        {
            return;
        }

        //DoPhysicalDamage(_targetStats);
        DoMagicalDamage(_targetStats);
    }

    private void DoPhysicalDamage(CharacterStats _targetStats)
    {
        //apply strength
        int totalDamage = damage.GetValue() + strength.GetValue();

        //check crit
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        //apply armor
        totalDamage = ApplyArmorReduction(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);
    }
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicDamage = (totalMagicDamage - (_targetStats.intelligence.GetValue()*3));
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        //apply magic resistance
        totalMagicDamage = Mathf.RoundToInt((float)totalMagicDamage * (1 - 0.01f * _targetStats.magicResistance.GetValue()));
        _targetStats.TakeDamage(totalMagicDamage);



        int maxDamage = Mathf.Max(_fireDamage, _iceDamage, _lightningDamage);
        if (maxDamage <= 0)
        {
            return;
        }

        bool canApplyIgnite =  _fireDamage == maxDamage;
        bool canApplyChill = !canApplyIgnite  && _iceDamage == maxDamage;
        bool canApplyShock = !canApplyIgnite && !canApplyChill && _lightningDamage == maxDamage;

        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private int ApplyArmorReduction(CharacterStats _targetStats, int totalDamage)
    {
        int armor = _targetStats.armor.GetValue();

        if (_targetStats.isChilled) 
        {
            armor = Mathf.RoundToInt(armor * .8f);
        }
        
        int leastDamage = (int)(((float)totalDamage) * 0.05f);
        if (totalDamage >= (armor + leastDamage))
        {
            totalDamage -= armor;
        }
        else
        {
            totalDamage = leastDamage;
        }

        return totalDamage;
    }

    private bool TryAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20;
        }


        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Miss");
            _targetStats.SetMissState(true);
            return true;
        }
        return false;
    }


    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {

        if (isIgnited || isChilled || isShocked)
        {
            //apply ailments only if target dont have any ailments
            
            return;
        }

        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = 4;
        }

        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = 2;
        }

        if (_shock)
        {
            isShocked = _shock;
            shockedTimer = 3;
        }


    }
    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {

    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0,100) <= totalCriticalChance)
        {
            return true;
        }
        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;
        float critDamage = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    public void SetMissState(bool _isMiss)
    {
        isMiss = _isMiss;
        return;
    }

    public bool GetMissState() { return isMiss; }
}
