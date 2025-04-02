using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Health>(out var collisionHealth))
            collisionHealth.InstaKill();
        else
            Destroy(collision.gameObject);
    }
}
