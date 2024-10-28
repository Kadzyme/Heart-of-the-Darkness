using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bonfire : MonoBehaviour
{
    [SerializeField] private Transform newCheckpointPos;

    private Collider2D currentCollider;
    private bool isReadyForUse = false;
    private bool isActivated = false;

    private void Start()
    {
        currentCollider = GetComponent<Collider2D>();
        currentCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isReadyForUse = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isReadyForUse = false;
    }

    private void Update()
    {
        if (isReadyForUse && Input.GetKeyDown(KeyCode.E))
        {
            if (isActivated)
            {
                Global.checkpointPos = newCheckpointPos.position;
                Global.OnReplaceEvent.Invoke();
                Global.ReviveObjects();
            }
            else
            {
                isActivated = true;
            }
        }
    }
}
