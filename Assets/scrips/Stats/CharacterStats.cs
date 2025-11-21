using System.Xml.Schema;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

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

    //current ailment
    public bool isIgnited;  //do damage over time
    public bool isChilled;  //reduce armor by 20%
    public bool isShocked;  //reduce accuracy by 20%

    public float chillDuration;
    public float igniteDuration;
    public float shockDuration;

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = .5f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject thunderStrikePrefab;
    private int shockDamage;


    public int currentHealth;

    public System.Action onHealthChanged;
    protected bool isDead;


    public enum AttackType
    {
        // physical attack
        Physical,
        // magic attack
        Magical,
        // combined attack
        Combined
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
        currentHealth = GetMaxHealthValue();
        critPower.SetDefaultValue(150);
        isMiss = false;
        isDead = false;
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

        if (igniteDamageTimer < 0 && isIgnited && !isDead)
        {
            Debug.Log("burn!" + igniteDamage);
            TakeDamage(igniteDamage);
            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    #region init
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockDamage(int _damage) => shockDamage = _damage;
    #endregion


    public virtual void DoDamage(CharacterStats _targetStats, AttackType type)
    {

        //apply evasion
        if (TryAvoidAttack(_targetStats))
        {
            return;
        }

        if (type == AttackType.Physical)
        {
            DoPhysicalDamage(_targetStats);
        }
        else if (type == AttackType.Magical)
        {
            DoMagicalDamage(_targetStats);
        }
        else
        {
            DoMagicalDamage(_targetStats);
            DoPhysicalDamage(_targetStats);
        }
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


        if (canApplyShock)
        {
            _targetStats.SetupShockDamage(Mathf.RoundToInt(_lightningDamage * .1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    #region Ailments
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;
        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = igniteDuration;
            fx.IgniteFXFor(igniteDuration);
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = chillDuration;
            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, chillDuration);
            fx.ChillFXFor(chillDuration);
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                //first lightning attack do these
                ApplyShock(_shock);
            }
            else
            {
                //do ThunderStrike

                //at this time it wont work on player
                if (GetComponent<Player>() != null)
                {
                    return;
                }

                HitNearestTargetWithThunder();
            }

        }


    }
    public void ApplyShock(bool _shock)
    {
        if (isShocked)
        {
            return;
        }
        isShocked = _shock;
        shockedTimer = shockDuration;
        fx.ShockFXFor(shockDuration);
    }
    private void HitNearestTargetWithThunder()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && hit != this.GetComponent<Collider2D>())
            {
                float diatanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (diatanceToEnemy < closestDistance)
                {
                    closestDistance = diatanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy == null)
        {
            closestEnemy = this.transform;
        }
        GameObject newShockStrike = Instantiate(thunderStrikePrefab, transform.position, Quaternion.identity);
        newShockStrike.GetComponent<ThunderStrikeController>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
    }
    #endregion

    #region Physical Attack Caculate
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

    private int ApplyArmorReduction(CharacterStats _targetStats, int totalDamage)
    {
        int armor = _targetStats.armor.GetValue();

        if (_targetStats.isChilled) 
        {
            armor = Mathf.RoundToInt(armor * .8f);
        }
        
        int leastDamage = Mathf.Max((int)(((float)totalDamage) * 0.05f), 1);
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
    #endregion


    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        //change UI
        onHealthChanged?.Invoke();

        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
    }


    #region Getters and Setters
    public void SetMissState(bool _isMiss)
    {
        isMiss = _isMiss;
        return;
    }

    public bool GetMissState() { return isMiss; }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion
}
