using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    [Header("Names of animator variables")]
    [SerializeField] private string attack = "Attack";
    [SerializeField] private string isWalking = "IsWalking";
    [SerializeField] private string isGrounded = "IsGrounded";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string recover = "Recover";
    [SerializeField] private string stun = "Stun";
    [SerializeField] private string isStunned = "IsStunned";
    [SerializeField] private string isAttacking = "IsAttacking";

    private Animator animator;

    private void Awake()
        => animator = GetComponent<Animator>();

    public void PlayAttackAnim(int combo)
        => animator.SetTrigger(attack + combo);

    public void PlayWalkAnim(bool amount)
        => animator.SetBool(isWalking, amount);

    public void PlayJumpAnim()
        => animator.SetTrigger(jump);

    public void PlayRecoverAnim()
        => animator.SetTrigger(recover);

    public void IsAttacking(bool value = true)
       => animator.SetBool(isAttacking, value);

    public void IsAttackingFalse()
       => animator.SetBool(isAttacking, false);

    public void ChangeAnimatorSpeed(float newAnimatorSpeed)
        => animator.speed = newAnimatorSpeed;

    public void SetIsGrounded(bool amount)
        => animator.SetBool(isGrounded, amount);

    public void SetIsStunned(bool amount)
        => animator.SetBool(isStunned, amount);

    public void PauseAnim(float time)
    {
        float currentAnimatorSpeed = animator.speed;

        animator.speed = 0f;

        StartCoroutine(ContinueAnim(time, currentAnimatorSpeed));
    }

    public void PlayStunAnim()
        => animator.Play(stun);

    private IEnumerator ContinueAnim(float time, float newAnimatorSpeed = 1f)
    {
        yield return new WaitForSeconds(time);

        animator.speed = newAnimatorSpeed;
    }
}
