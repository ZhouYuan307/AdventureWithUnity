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


    [SerializeField]private int currentHealth;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
        critPower.SetDefaultValue(150);
        isMiss = false;
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {

        //apply evasion
        if (TryAvoidAttack(_targetStats))
        {
            return;
        }

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

    private int ApplyArmorReduction(CharacterStats _targetStats, int totalDamage)
    {
        int armor = _targetStats.armor.GetValue();
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

    private static bool TryAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Miss");
            _targetStats.SetMissState(true);
            return true;
        }
        return false;
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
