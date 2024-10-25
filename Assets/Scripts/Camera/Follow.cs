using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private float additionalY;
    [SerializeField] private float additionalX;
    [SerializeField] private float changingY;

    private float delayForChangingX = 3f;
    private float currentTime;

    private float normalZ;

    private float playerScaleX;

    private void Start()
    {
        normalZ = transform.position.z;
        playerScaleX = Global.currentPlayer.localScale.x;
    }

    private void Update()
    {
        if (Global.currentPlayer == null)
            return;

        currentTime += Time.deltaTime;

        if (playerScaleX != Global.currentPlayer.localScale.x)
        {
            playerScaleX = Global.currentPlayer.localScale.x;
            currentTime = 0;
        }

        float currentAdditionalY = additionalY;

        float verticalInput = Input.GetAxis("Vertical");
        currentAdditionalY += verticalInput * changingY;

        Vector2 currentPos = transform.position;
        Vector2 playerCurrentPos = Global.currentPlayer.position;

        float currentAdditionalX = 0;

        if (currentTime > delayForChangingX)
        {
            float delay = currentTime - delayForChangingX;

            if (delay > 1)
                delay = 1;

            currentAdditionalX = additionalX * delay;
        }

        Transform player = Global.currentPlayer;
        if (player.localScale.x * player.GetComponent<Stats>().normalXMultiplicator > 0)
            currentAdditionalX *= -1;

        playerCurrentPos.x += currentAdditionalX;
        playerCurrentPos.y += currentAdditionalY;

        if (Vector2.Distance(currentPos, playerCurrentPos) > maxDistance)
        {
            transform.position = 
                Vector2.Lerp(transform.position, playerCurrentPos, Time.deltaTime * Vector2.Distance(currentPos, playerCurrentPos) * 0.75f);
            Vector3 temp = new(0, 0, normalZ);
            transform.position += temp;
        }
    }
}
