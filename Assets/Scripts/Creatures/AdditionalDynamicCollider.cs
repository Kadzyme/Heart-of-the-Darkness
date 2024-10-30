using UnityEngine;

public class AdditionalDynamicCollider : MonoBehaviour
{
    [SerializeField]private Collider2D neededCollider;

    private void Start()
        => DisableCollider();

    public void EnableCollider() 
        => neededCollider.enabled = true;

    public void DisableCollider()
        => neededCollider.enabled = false;
}
