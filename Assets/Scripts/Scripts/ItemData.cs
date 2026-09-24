using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public float growthRate = 1f;

    public float hungerRate = 10f;
    public float abilityRate = 0.9f;
    public AbilityType abilityType = AbilityType.None;

}

public enum AbilityType
{
    None,
    Fire,
    Electric,
    Mist,
    Explosive
}