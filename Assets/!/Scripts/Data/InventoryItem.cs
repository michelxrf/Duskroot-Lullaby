using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Scriptable Objects/Inventory Item")]
public class InventoryItem: ScriptableObject
{
    public string ItemId;
    public string ItemName;
    public enum ItemType { CONSUMABLE, EQUIPMENT };

    void OnEquip()
    {
    }

    void OnUnequip()
    {
    }

    void Consume()
    {
    }

    void Drop()
    {

    }
}
