using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackPosition : MonoBehaviour
{
    private List<Collider2D> colliders = new();

    private void Start()
        => GetComponent<Collider2D>().isTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
        => colliders.Add(collision);

    public List<Health> CheckDamagableInRange()
    {
        List<Health> health = new();
        foreach (Collider2D collider in colliders)
        {
            if(collider.GetComponent<Health>() != null)
                health.Add(collider.GetComponent<Health>());
        }
        return health;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (colliders.Contains(collision))
            colliders.Remove(collision);
    }
}
