using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GroundSensor : MonoBehaviour
{
    private float disableTime = 0;
    private List<Collider2D> colliders = new();

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanCountCollider(collision))
            return;

        colliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!CanCountCollider(collision))
            return;

        colliders.Remove(collision);
    }

    private bool CanCountCollider(Collider2D collision)
        => !collision.isTrigger;

    private void Update()
    {
        disableTime -= Time.deltaTime;
        foreach (Collider2D collider in colliders)
        {
            if (collider == null)
            {
                colliders.Remove(collider);
            }
        }
    }

    public void Disable(float time)
        =>disableTime = time;

    public bool State()
    {
        if (disableTime > 0)
            return false;
        return colliders.Count > 0;
    }
}
