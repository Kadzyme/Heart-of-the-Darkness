using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class ParticlesOnDamage : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private Transform particlesPos;

    [Space]
    [SerializeField] private bool changeColor = true;
    [SerializeField] private Color colorToChange = Color.white;
    [SerializeField] private float time = 0.1f;

    private Color normalColor;

    private void Start()
    {
        UnityAction action = new(CreateParticles);

        if(changeColor)
            action += ChangeColor;

        normalColor = GetComponent<SpriteRenderer>().color;

        GetComponent<Health>().BeforeDamageRecevingEvent.AddListener(action);
    }

    private void CreateParticles()
    {       
        if (particles == null)
            return;

        GameObject newObj = Instantiate(particles, particlesPos.position, Quaternion.identity);

        newObj.SetActive(true);

        if (GetComponent<SpriteRenderer>() != null)
            newObj.GetComponent<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;

        Destroy(newObj, newObj.GetComponent<ParticleSystem>().main.duration);
    }

    private void ChangeColor()
    {
        if (GetComponent<SpriteRenderer>() == null)
            return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = colorToChange;

        StartCoroutine(ReturnColorToNormal());
    }

    private IEnumerator ReturnColorToNormal()
    {
        yield return new WaitForSeconds(time);

        GetComponent<SpriteRenderer>().color = normalColor;
    }
}
