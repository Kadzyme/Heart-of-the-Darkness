using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackPosition : MonoBehaviour
{
    [SerializeField] private ContactFilter2D filter = new();

    private void Start()
        => GetComponent<Collider2D>().isTrigger = true;

    public List<Health> CheckDamagableInRange()
    {
        List<Collider2D> collisions = new();
        Collider2D collider2D = GetComponent<Collider2D>();

        collider2D.Overlap(filter, collisions);

        List<Health> health = new();
        foreach (Collider2D collider in collisions)
        {
            if(collider.GetComponent<Health>() != null)
                health.Add(collider.GetComponent<Health>());
        }
        return health;
    }
}
