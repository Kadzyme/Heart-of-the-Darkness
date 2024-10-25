using UnityEngine;

public class Global : MonoBehaviour
{
    public static Transform currentPlayer;
    [SerializeField] private Transform startPlayer;

    public static int groundLayer;
    [SerializeField] private int groundLayerNum;

    public static int unitsLayer;
    [SerializeField] private int unitsLayerNum;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        currentPlayer = startPlayer;
        groundLayer = 1 << groundLayerNum;
        unitsLayer = 1 << unitsLayerNum;
    }

    public static bool IsEnemy(Fraction obj1, Fraction obj2)
        => obj1 != obj2 && obj2 != Fraction.passive;
}
