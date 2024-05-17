using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Weapon,
    Consumable,
    Curative,
    SpeedModifier,
    FearModifier,
    FearIncrementReducer,
    General
}
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public Vector2Int size;
    public GameObject itemPrefab;
    public ItemType itemType;
    public int healingAmount;
    public float speedModifier;
    public float fearIncrementModifier;
    public float fearIncrementReductionTarget; // Valor específico para el incremento del miedo
    public float duration; // Duración del efecto en segundos

    public void Rotate()
    {
        size = new Vector2Int(size.y, size.x);
    }

    public void ApplyEffect(PlayerController player)
    {
        switch (itemType)
        {
            case ItemType.Curative:
                player.Heal(healingAmount);
                break;
            case ItemType.SpeedModifier:
                player.AdjustSpeed(speedModifier, duration);
                break;
            case ItemType.FearModifier:
                player.AdjustFearIncrement(fearIncrementModifier, duration);
                break;
            case ItemType.FearIncrementReducer:
                player.SetFearIncrement(fearIncrementReductionTarget, duration);
                break;
                // Add more cases as needed
        }
    }
}