using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Fraction
{
    player,
    enemy,
    passive
}

[System.Serializable]
public struct AmountType
{
    public float maxAmount;
    public float currentAmount;
}

public class Stats : MonoBehaviour
{
    #region variables
    [SerializeField] private float movementSpeed;
    [HideInInspector] public float currentMovementSpeed;
    public float jumpForce;

    [SerializeField] private bool blockMoveOnAttack = true;

    public Fraction fraction;

    public int normalXMultiplicator = 1;

    [SerializeField] private List<Effect> effects = new();

    public GroundSensor groundSensor;

    [SerializeField] private bool positionLocked = false;
    [SerializeField] private float currentStoppingTime = 0;

    [HideInInspector] public bool isGrounded;

    [HideInInspector] public Weapon weapon;
    private AnimatorController animatorController;

    [HideInInspector] public bool isStunned;
    [HideInInspector] public bool isAnimFixed;
    [HideInInspector] public bool isAttacking = false;

    [HideInInspector] public UnityEvent OnStunStartEvent;
    [HideInInspector] public UnityEvent OnStunEndEvent;
    #endregion

    private void Awake()
    {
        currentMovementSpeed = movementSpeed;

        GetComponents();

        SubscribeOnEvents();
    }

    private void GetComponents()
    {
        weapon ??= GetComponent<Weapon>();
        animatorController = GetComponent<AnimatorController>();
    }

    private void SubscribeOnEvents()
    {
        UnityAction action = new(weapon.EndAttackWeap);
        action += weapon.ResetCombo;
        action += animatorController.PlayStunAnim;
        action += FixAnim;
        action += StayStunned;
        OnStunStartEvent.AddListener(action);

        action = new(StayNotStunned);
        action += UnFixAnim;
        OnStunEndEvent.AddListener(action);

        action = new(UnlockPos);
        weapon.OnAttackEndEvent.AddListener(action);
    }

    private void FixAnim()
        => isAnimFixed = true;

    private void UnFixAnim()
        => isAnimFixed = false;

    private void StayStunned()
        => isStunned = true;

    private void StayNotStunned()
        => isStunned = false;

    private void Update()
    {
        currentStoppingTime -= Time.deltaTime;
        isGrounded = groundSensor.State();

        if (!CanMove())
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector3.zero;
        }

        UpdateEffects();

        animatorController.SetIsGrounded(isGrounded);
        animatorController.SetIsStunned(isStunned);
    }

    private void UpdateEffects()
    {
        List<Effect> effectsToRemove = new();

        foreach(Effect effect in effects)
        {
            if (effect == null || effect.isEndless)
                continue;

            effect.currentDuringTime -= Time.deltaTime;

            if (effect.currentDuringTime < 0)
            {
                switch (effect.effectType)
                {
                    case EffectType.stun:
                        OnStunEndEvent.Invoke();
                        break;
                }
                effectsToRemove.Add(effect);
            }
        }

        foreach (Effect effect in effectsToRemove)
        {
            effects.Remove(effect);
        }
    }

    public void AddEffect(Effect newEffect)
    {
        effects.Add(newEffect);

        newEffect.currentDuringTime = newEffect.duringTime;

        CheckEffect(newEffect);
    }

    public void CheckEffect(Effect effect)
    {
        switch (effect.effectType)
        {
            case EffectType.stun:
                OnStunStartEvent.Invoke();
                RemoveIdenticalEffects(effect);
                break;
            case EffectType.instaDamage:
            case EffectType.instaHeal:
                Damage newDamage = new()
                {
                    amount = effect.amount,
                    type = effect.effectType == EffectType.instaDamage ? DamageType.damage : DamageType.heal,
                };
                GetComponent<Health>().UpdateHealth(newDamage);
                effects.Remove(effect);
                break;
        }
    }

    private void RemoveIdenticalEffects(Effect effectToCheck)
    {
        List<Effect> effectsToRemove = new();

        foreach (Effect effect in effects)
        {
            if (effect.effectType == effectToCheck.effectType && effect != effectToCheck)
            {
                Effect effectToRemove = effect.currentDuringTime < effectToCheck.currentDuringTime ? effect : effectToCheck;

                effectToCheck = effectToRemove == effectToCheck ? effect : effectToCheck;

                effectsToRemove.Add(effectToRemove);
            }
        }

        foreach (Effect effect in effectsToRemove)
        {
            effects.Remove(effect);
        }
    }

    public bool CanMove()
        => !positionLocked && currentStoppingTime < 0 && !isStunned && !isAttacking;

    public bool TryAttack()
    {
        if (weapon.CanAttack() && !isStunned && weapon.CanActivateAnim())
        {
            if (blockMoveOnAttack)
                LockPos();

            weapon.ActivateAnim();

            return true;
        }
        return false;
    }

    private void LockPos()
        => positionLocked = true;

    private void UnlockPos()
        => positionLocked = false;
}
