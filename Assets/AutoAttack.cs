using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [SerializeField] private bool attackAutomatically;
    [SerializeField] private bool attackOnStep;

    private Stats stats;

    private void Start()
        => stats = GetComponent<Stats>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Health>() != null && attackOnStep)
            stats.TryToAttack();
    }

    private void FixedUpdate()
    {
        if (attackAutomatically) 
            stats.TryToAttack();
    }
}
