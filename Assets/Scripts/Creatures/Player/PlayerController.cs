using Unity.VisualScripting;
using UnityEngine;

public enum Direction
{
    left, 
    right, 
    up, 
    down
}

public enum DamageType
{
    damage,
    heal
}

[System.Serializable]
public struct Damage
{
    public DamageType type;
    public float amount;

    [HideInInspector] public GameObject owner;
}

[RequireComponent(typeof(AnimatorController))]
[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Stats))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GroundSensor rightSensor;
    [SerializeField] private GroundSensor leftSensor;
    private (bool, float) tryToRotate = (false, 0);
    private bool tryToAttack = false;

    private Stats stats;
    private Rigidbody2D rb;
    private AnimatorController animController;

    private float normalX = 1;

    private void Start()
    {
        animController = GetComponent<AnimatorController>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Stats>();
        normalX = Mathf.Abs(transform.localScale.x) * stats.normalXMultiplicator;

        SubscribeOnEvents();
    }

    private void SubscribeOnEvents()
    {
        stats.weapon.OnAttackStartEvent.AddListener(OnStartAttack);
        stats.weapon.OnAttackEndEvent.AddListener(OnEndAttack);
    }

    private void OnStartAttack()
        => stats.isAttacking = true;

    private void OnEndAttack()
        => stats.isAttacking = false;

    private void Update()
    {
        if (stats.isStunned)
            return;

        if ((Input.GetMouseButtonDown(0) || tryToAttack) && stats.isGrounded)
        {
            if (stats.TryToAttack())
                tryToAttack = false;

            if (stats.isAttacking && !tryToAttack)
                tryToAttack = true;
        }

        float inputX = Input.GetAxis("Horizontal");

        if (inputX != 0 && stats.CanChangePosition())
            tryToRotate = (true, inputX);

        if (!stats.isAttacking && tryToRotate.Item1)
        {
            TryToRotate(tryToRotate.Item2);
        }

        if (!stats.CanChangePosition())
        {
            animController.PlayWalkAnim(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        if (!stats.isGrounded)
        {
            float num = inputX * transform.localScale.x * stats.normalXMultiplicator;
            if ((num > 0 && leftSensor.State()) || (num < 0 && rightSensor.State()))
            {
                return;
            }
        }

        rb.linearVelocity = new Vector2(inputX * stats.currentMovementSpeed, rb.linearVelocity.y);

        if (stats.isGrounded)
        {
            animController.PlayWalkAnim(inputX != 0);
        }
        else
        {
            animController.PlayWalkAnim(false);
        }
    }

    private void TryToRotate(float inputX)
    {
        Vector3 newLocalScale = transform.localScale;

        if (inputX > 0)
            newLocalScale.x = -normalX;
        else
            newLocalScale.x = normalX;

        transform.localScale = newLocalScale;
    }

    private void TryJump()
    {
        if (!stats.isGrounded || !stats.CanChangePosition())
            return;

        stats.isGrounded = false;
        animController.PlayJumpAnim();
        stats.groundSensor.Disable(0.2f);
        rb.AddForce(100 * stats.jumpForce * transform.up);
    }
}
