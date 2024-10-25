using UnityEngine;

public enum EffectType
{
    instaDamage,
    instaHeal,
    stun
}

[System.Serializable]
public class Effect
{
    public EffectType effectType;
    public float amount;

    public float duringTime;
    [HideInInspector] public float currentDuringTime;

    public bool isEndless;
}
