using System.Xml.Schema;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats")]
    public Stat strength; //increase damage
    public Stat agility; //increase evasion
    public Stat intelligence; //increase magic damage and magic resistance
    public Stat vitality; //increase health

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;


    public Stat damage;


    [SerializeField]private int currentHealth;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
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
            return true;
        }
        return false;
    }

    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;
        Debug.Log(_damage);

        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {

    }
}
