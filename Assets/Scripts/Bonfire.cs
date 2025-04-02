using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bonfire : MonoBehaviour
{
    [SerializeField] private Transform newCheckpointPos;

    [SerializeField] private GameObject glow;

    private Collider2D currentCollider;
    private bool isReadyForUse = false;
    private bool isActivated = false;

    private void Start()
    {
        currentCollider = GetComponent<Collider2D>();
        currentCollider.isTrigger = true;
        glow.SetActive(false);
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
            Global.SetNewCheckPointPos(newCheckpointPos.position);

            if (isActivated)
            {
                Global.OnReplaceEvent.Invoke();
                Global.ReviveObjects();
            }
            else
            {
                isActivated = true;
                glow.SetActive(true);
            }
        }
    }
}
