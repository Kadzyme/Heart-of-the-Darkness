using UnityEngine;
using UnityEngine.UI;

public class AmountBar : MonoBehaviour
{
    private Image slider;

    private void Awake()
    {
        slider = GetComponent<Image>();
    }

    public void SetAmount(float amount)
        => slider.fillAmount = amount;
}
