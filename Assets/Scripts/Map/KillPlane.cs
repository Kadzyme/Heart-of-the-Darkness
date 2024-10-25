using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;

    private void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(startPos, endPos, Global.unitsLayer);

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Health>() != null)
                collider.GetComponent<Health>().InstaKill();
            else
                Destroy(collider.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawLine(new Vector2(startPos.x, endPos.y), new Vector2(endPos.x, startPos.y));

        Gizmos.DrawLine(new Vector2(startPos.x, startPos.y), new Vector2(endPos.x, startPos.y));

        Gizmos.DrawLine(new Vector2(endPos.x, startPos.y), new Vector2(endPos.x, endPos.y));

        Gizmos.DrawLine(new Vector2(startPos.x, endPos.y), new Vector2(startPos.x, startPos.y));

        Gizmos.DrawLine(new Vector2(startPos.x, endPos.y), new Vector2(endPos.x, endPos.y));
    }
}
