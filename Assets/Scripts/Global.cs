using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Global : MonoBehaviour
{
    public static UnityEvent OnReplaceEvent = new();
    public static List<GameObject> objectsToRevive = new();

    public static Vector2 checkpointPos = Vector2.zero;
    [SerializeField] private Transform startCheckpointPos;

    public static Transform currentPlayer;

    private static Transform staticPlayerPrefab;
    [SerializeField] private Transform playerPrefab;

    public static int groundLayer;
    [SerializeField] private int groundLayerNum;

    public static int unitsLayer;
    [SerializeField] private int unitsLayerNum;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        staticPlayerPrefab = playerPrefab;
        checkpointPos = startCheckpointPos.position;

        groundLayer = 1 << groundLayerNum;
        unitsLayer = 1 << unitsLayerNum;

        CreateNewHero();
    }

    private void FixedUpdate()
    {
        if (currentPlayer == null)
            CreateNewHero();
    }

    public static void CreateNewHero()
    {
        currentPlayer = Instantiate(staticPlayerPrefab);
        OnReplaceEvent.Invoke();
        ReviveObjects();
    }

    public static void ReviveObjects()
    {
        foreach (GameObject objectToRevive in objectsToRevive)
        {
            objectToRevive?.SetActive(true);
        }
    }

    public static bool IsEnemy(Fraction obj1, Fraction obj2)
        => obj1 != obj2 && obj2 != Fraction.passive;
}
