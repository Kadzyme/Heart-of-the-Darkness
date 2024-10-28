using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class RevivableObject : MonoBehaviour
{
    [SerializeField] private bool isRevivable = true;

    private Vector2 positionToReplace;
    private GameObject duplicatedObj;

    private void Start()
    {
        if (isRevivable)
        {
            duplicatedObj = Instantiate(gameObject);
            duplicatedObj.SetActive(false);
        }

        positionToReplace = transform.position;
        Global.OnReplaceEvent.AddListener(Replace);
    }

    private void OnDestroy()
    {
        if (isRevivable)
            Global.objectsToRevive.Add(duplicatedObj);

        Global.objectsToRevive.Remove(gameObject);
    }

    private void Replace()
        =>transform.position = positionToReplace;
}
