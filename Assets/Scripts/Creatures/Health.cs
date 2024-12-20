using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Stats))]
public class Health : MonoBehaviour
{
    [HideInInspector] public UnityEvent BeforeDamageRecevingEvent;

    public AmountType health;

    [SerializeField] private AmountBar healthBar;

    private void Start()
    {
        Global.OnReplaceEvent.AddListener(RestoreHealth);

        BeforeDamageRecevingEvent ??= new();

        RestoreHealth();        
    }

    public void RestoreHealth()
    { 
        health.currentAmount = health.maxAmount;

        SetHealthBarAmount();
    }

    public void UpdateHealth(Damage damage)
    {
        if (damage.amount <= 0)
            return;

        switch (damage.type)
        {
            case DamageType.heal:
                Heal(damage.amount);
            break;
            case DamageType.damage:
                Damage(damage.amount);
            break;
        }

        SetHealthBarAmount();
    }

    private void SetHealthBarAmount()
    {
        if (healthBar != null)
            healthBar.SetAmount(health.currentAmount / health.maxAmount);
    }

    public void InstaKill()
    {   
        Damage(health.currentAmount + 1f);
        SetHealthBarAmount();
        Death();
    }

    public void DestroyObj(float offset)
        => Destroy(gameObject, offset);

    private void Heal(float amount)
    {
        health.currentAmount += amount;

        if (health.currentAmount > health.maxAmount)
            RestoreHealth();
    }

    private void Damage(float amount)
    {
        BeforeDamageRecevingEvent.Invoke();

        health.currentAmount -= amount; 

        if (health.currentAmount < 0)
            Death();
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
