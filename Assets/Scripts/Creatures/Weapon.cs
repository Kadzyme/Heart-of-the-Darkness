using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Attack
{
    public float damageMultiplicator;

    public AttackPosition attackPosition;

    public List<Effect> additionalEffects;
}

[RequireComponent(typeof(AnimatorController))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private Damage damage;

    [SerializeField] private Attack[] combo;

    private int currentCombo = 0;

    [SerializeField] private float maxComboDuration;
    private float currentComboDuration;

    private float currentCooldown = 0;

    [SerializeField] private float cooldown;

    [SerializeField] private Vector2 randomCooldownMultiplicator;

    private bool doDamage = false;
    private bool attackLocked = false;

    private List<Health> attackedObjectsHealth = new();

    private AnimatorController animatorController;

    [HideInInspector] public UnityEvent OnAttackStartEvent = new();
    [HideInInspector] public UnityEvent OnAttackEndEvent = new();

    public bool TryAttack()
    {
        if (CanAttack())
        {
            ActivateAnim();

            return true;
        }
        return false;
    }

    private bool CanAttack()
        => currentCooldown < 0 && !attackLocked && !doDamage;

    private void ActivateAnim()
    {
        attackLocked = true;

        animatorController.IsAttacking();
        animatorController.PlayAttackAnim(currentCombo);
    }

    private void Start()
    {
        damage.owner = gameObject;
        animatorController = GetComponent<AnimatorController>();

        SubscribeOnEvents();
    }

    private void SubscribeOnEvents()
    {
        OnAttackStartEvent.AddListener(OnStartAttack);

        OnAttackEndEvent.AddListener(OnEndAttack);
    }

    private void OnStartAttack()
    {
        doDamage = true;
        attackedObjectsHealth.Clear();
    }

    private void OnEndAttack()
    {
        animatorController.IsAttackingFalse();
        doDamage = false;
        UpdateCombo();
        attackLocked = false;
    }

    private void UpdateCombo()
    {
        currentComboDuration = maxComboDuration;

        currentCombo++;

        if (currentCombo >= combo.Length)
        {
            ResetCombo(); 
            SetComboCooldown();
        }
    }

    public void ResetCombo()
    {
        currentCombo = 0;
    }

    private void SetComboCooldown()
    {
        if (randomCooldownMultiplicator == Vector2.zero || randomCooldownMultiplicator.x > randomCooldownMultiplicator.y)
            currentCooldown = cooldown;
        else
            currentCooldown = cooldown * Random.Range(randomCooldownMultiplicator.x, randomCooldownMultiplicator.y);
    }

    public bool IsAttackPosDefined()
        => combo[currentCombo].attackPosition != null;

    public bool IsEnemyInRange(Health enemy)
    {
        List<Health> hitted = combo[currentCombo].attackPosition.CheckDamagableInRange();

        if (hitted.Contains(enemy))
            return true;
        return false;
    }

    public void StartAttack()
        => OnAttackStartEvent.Invoke();

    public void EndAttack()
        => OnAttackEndEvent.Invoke();

    private void Update()
    {
        currentCooldown -= Time.deltaTime;

        currentComboDuration -= Time.deltaTime;

        if (currentComboDuration < 0 && currentCombo != 0)
            currentCombo = 0;

        if (doDamage)
        {
            DoDamage();
        }
    }

    private void DoDamage()
    {
        Fraction ownerFraction = GetComponent<Stats>().fraction;
        List<Health> targets = combo[currentCombo].attackPosition.CheckDamagableInRange();

        if (targets.Count <= 0)
            return;

        foreach (Health target in targets)
        {
            if (target.TryGetComponent<Stats>(out var statsOfTarget))
            {
                if (Global.IsEnemy(ownerFraction, statsOfTarget.fraction)
                && !attackedObjectsHealth.Contains(target))
                {
                    Damage newDamage = damage;
                    newDamage.amount *= combo[currentCombo].damageMultiplicator;
                    attackedObjectsHealth.Add(target);
                    target.UpdateHealth(newDamage);
                
                    foreach (var effect in combo[currentCombo].additionalEffects)
                    {
                        statsOfTarget.AddEffect(effect);
                    }
                }
            }
        }
    }
}
