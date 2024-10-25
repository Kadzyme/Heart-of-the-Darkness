using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFillWithDelay : MonoBehaviour
{
    [SerializeField] private Image amountBar;

    private bool reduceFill = false;

    private float currentFillAmount;

    private void Start()
    {
        currentFillAmount = amountBar.fillAmount;
    }

    private void Update()
    {
        if(reduceFill)
        {
            currentFillAmount -= (1 - amountBar.fillAmount) * 0.25f * Time.deltaTime;
        }

        if (currentFillAmount < amountBar.fillAmount)
        {
            currentFillAmount = amountBar.fillAmount;
            reduceFill = false;
        }

        if (currentFillAmount != amountBar.fillAmount && !reduceFill)
        {
            StartCoroutine(ChangeFillWithTimeOffset());
        }

        GetComponent<Image>().fillAmount = currentFillAmount;
    }

    private IEnumerator ChangeFillWithTimeOffset()
    {
        yield return new WaitForSeconds(1f);

        reduceFill = true;
    }
}
