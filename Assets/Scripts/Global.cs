using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Global : MonoBehaviour
{
    public static UnityEvent OnReplaceEvent = new();
    public static List<GameObject> objectsToRevive = new();

    private static Vector2 checkpointPos = Vector2.zero;
    [SerializeField] private Transform startCheckpointPos;

    public static Transform currentPlayer;

    private static Transform playerPrefab;
    [SerializeField] private Transform startPlayerPrefab;

    public static int groundLayer;
    [SerializeField] private int groundLayerNum;

    public static int unitsLayer;
    [SerializeField] private int unitsLayerNum;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        playerPrefab = startPlayerPrefab;

        groundLayer = 1 << groundLayerNum;
        unitsLayer = 1 << unitsLayerNum;

        SetNewCheckPointPos(startCheckpointPos.position);

        OnReplaceEvent.AddListener(MovePlayerToCheckPoint);

        CreateNewHero();
    }

    public static void SetNewCheckPointPos(Vector2 newPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(newPos, Vector2.down, 10f, groundLayer);

        checkpointPos = hit ? hit.point : newPos;
    }

    private void MovePlayerToCheckPoint()
       => currentPlayer.position = checkpointPos;

    private void FixedUpdate()
    {
        if (currentPlayer == null)
            CreateNewHero();
    }

    public static void CreateNewHero()
    {
        currentPlayer = Instantiate(playerPrefab);
        ReviveObjects();
        OnReplaceEvent.Invoke();
    }

    public static void ReviveObjects()
    {
        foreach (GameObject objectToRevive in objectsToRevive)
        {
            objectToRevive?.SetActive(true);
        }
    }

    public static bool IsEnemy(Fraction obj1, Fraction obj2)
        => obj1 != obj2;
}
