using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum State
{
    idle,
    fight
}

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimatorController))]
[RequireComponent(typeof(Stats))]
public class AI : MonoBehaviour
{
    [SerializeField] private float timeToStay;
    private float currentTimeToStay;

    [SerializeField] private float maxFollowTime;
    private float currentFollowTime;

    [SerializeField] private float viewRange;
    [SerializeField] private float viewRangeOnDamage;

    [SerializeField] private Transform borderSensor;

    [SerializeField] private bool patrool = true;

    [SerializeField] private GroundSensor wallSensor;

    [SerializeField] private float optimalDistanceToEnemy;
    [SerializeField] private float maxDistanceFromOptimal = 0.4f;

    private float normalRightX = 1;

    private State currentState;

    private bool isMovingRight;
    private GameObject target;

    private Stats stats;
    private AnimatorController animController;
    private Rigidbody2D rb;

    private float currentSpeed;

    private float currentCooldownForTurnAround = 0;

    private float currentViewRange;

    private bool haveToTurnAround = false;

    private void Start()
    {
        GetComponents();

        int random = Random.Range(-1, 2);
        isMovingRight = random > 0;

        currentViewRange = viewRange;

        if (viewRangeOnDamage < viewRange)
            viewRangeOnDamage = viewRange * 2;

        SubscribeOnEvents();

        normalRightX = Mathf.Abs(transform.localScale.x) * stats.normalXMultiplicator;

        ChangeState(State.idle); 
        Patrool();
    }

    private void GetComponents()
    {
        stats = GetComponent<Stats>();
        animController = GetComponent<AnimatorController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void SubscribeOnEvents()
    {
        UnityAction action = new(TryFindNewEnemy);
        action += MultiplyViewRange;

        GetComponent<Health>().BeforeDamageRecevingEvent.AddListener(action);
    }

    private void FixedUpdate()
    {
        if (stats.isStunned)
            return;

        float deltaTime = Time.fixedDeltaTime;

        currentCooldownForTurnAround -= deltaTime;

        animController.PlayWalkAnim(!(rb.linearVelocity.x == 0));

        CheckState(deltaTime);

        if (target != null)
            return;

        TryFindNewEnemy();
    }

    private void CheckState(float deltaTime)
    {
        switch (currentState)
        {
            case State.idle:
                if (!patrool || currentSpeed <= 0)
                    return;

                currentTimeToStay -= deltaTime;

                if (currentTimeToStay < 0)
                {
                    if (haveToTurnAround)
                        TurnAround();

                    Patrool();
                }
                break;
            case State.fight:
                if (target == null || target.GetComponent<Health>() == null 
                    || stats.weapon == null || !stats.weapon.HavePossibitilyToAttack())
                {
                    ChangeState(State.idle);
                    return;
                }

                if (Vector2.Distance(transform.position, target.transform.position) > currentViewRange)
                {
                    currentFollowTime -= deltaTime;

                    if (currentFollowTime < 0)
                    {
                        currentFollowTime = maxFollowTime * Random.Range(0.8f, 1.5f);
                        ChangeState(State.idle);
                        return;
                    }
                }

                Vector2 currentPos = transform.position;
                currentPos.y = 0;

                Vector2 targetPos = target.transform.position;
                targetPos.y = 0;

                if (stats.weapon.IsEnemyInRange(target.GetComponent<Health>()))
                {
                    stats.TryAttack();
                }
                else if (stats.CanMove())
                {
                    if (Math.Abs(Vector2.Distance(currentPos, targetPos) - optimalDistanceToEnemy) > maxDistanceFromOptimal &&
                       CanMoveForwards())
                    {
                        bool isEnemyRight;

                        float distance1 = Vector2.Distance(new Vector2(currentPos.x + optimalDistanceToEnemy, 0), targetPos);
                        float distance2 = Vector2.Distance(new Vector2(currentPos.x - optimalDistanceToEnemy, 0), targetPos);

                        if (distance1 < distance2)
                            isEnemyRight = targetPos.x > currentPos.x + optimalDistanceToEnemy;
                        else
                            isEnemyRight = targetPos.x > currentPos.x - optimalDistanceToEnemy;

                        Move(isEnemyRight);
                    }
                    else
                    {
                        if ((targetPos.x > currentPos.x && transform.localScale.x < 0) ||
                            (targetPos.x < currentPos.x && transform.localScale.x > 0))
                            TurnAround();
                    }
                }
                break;
        }
    }

    private void Patrool()
    {
        if (!CanMoveForwards())
        {
            currentTimeToStay = timeToStay * Random.Range(0.8f, 1.2f);
            isMovingRight = !isMovingRight;
            haveToTurnAround = true;
        }
        else if (stats.CanMove())
        {
            Move(isMovingRight);
        }
    }

    private bool CanMoveForwards()
    {
        RaycastHit2D hitBottom = Physics2D.Raycast(borderSensor.position, Vector2.down, 0.2f, Global.groundLayer);

        return hitBottom.collider != null && !wallSensor.State();
    }

    private void TurnAround()
    {
        if (currentCooldownForTurnAround > 0)
            return;

        float currentX = transform.localScale.x;
        transform.localScale = new Vector2(currentX * -1, transform.localScale.y);

        currentCooldownForTurnAround = 1f;

        haveToTurnAround = false;
    }

    private void MultiplyViewRange()
    {
        currentViewRange = viewRangeOnDamage;
        StartCoroutine(SetViewRangeToNormal());
    }

    private IEnumerator SetViewRangeToNormal()
    {
        yield return new WaitForSeconds(10f);

        currentViewRange = viewRange;
    }

    private void TryFindNewEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentViewRange, Global.unitsLayer);

        foreach(Collider2D other in colliders)
        {
            Stats otherStats = other.GetComponent<Stats>();

            if (otherStats != null && Global.IsEnemy(stats.fraction, otherStats.fraction))
            {
                target = other.gameObject;
                ChangeState(State.fight);
            }
        }
    }

    private void Move(bool plus)
    {
        if (!stats.CanMove() || !stats.isGrounded)
            return;

        float distance = plus ? currentSpeed : -currentSpeed;
        float newNormalX = plus ? -normalRightX : normalRightX;

        rb.linearVelocity = new(distance, rb.linearVelocity.y);
        transform.localScale = new Vector2(newNormalX, transform.localScale.y);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        switch (newState)
        {
            case State.idle:
                currentSpeed = stats.currentMovementSpeed / Random.Range(1.5f, 2f);
                target = null;
                break;
            case State.fight:
                currentFollowTime = maxFollowTime * Random.Range(0.9f, 1.3f);
                currentSpeed = stats.currentMovementSpeed * Random.Range(0.9f, 1.2f);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, viewRangeOnDamage);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + optimalDistanceToEnemy, transform.position.y), maxDistanceFromOptimal);
        Gizmos.DrawWireSphere(new Vector2(transform.position.x - optimalDistanceToEnemy, transform.position.y), maxDistanceFromOptimal);
    }
}
