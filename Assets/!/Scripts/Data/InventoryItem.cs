using UnityEngine;

/// <summary>
/// Scriptable Object representing a single item that can be stored in a character's inventory.
/// Supports both consumable and equipment item types with associated behaviors.
/// </summary>
[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Scriptable Objects/Inventory Item")]
public class InventoryItem: ScriptableObject
{
    /// <summary>Unique identifier for this inventory item</summary>
    public string ItemId;

    /// <summary>Display name of this item</summary>
    public string ItemName;

    /// <summary>Type of item (consumable or equipment)</summary>
    public enum ItemType { CONSUMABLE, EQUIPMENT };

    /// <summary>
    /// Called when the item is equipped.
    /// </summary>
    void OnEquip()
    {
    }

    /// <summary>
    /// Called when the item is unequipped.
    /// </summary>
    void OnUnequip()
    {
    }

    /// <summary>
    /// Called when a consumable item is used.
    /// </summary>
    void Consume()
    {
    }

    /// <summary>
    /// Called when the item is dropped from the inventory.
    /// </summary>
    void Drop()
    {

    }
}
