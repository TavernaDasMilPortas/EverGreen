using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventário/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public bool consumable;
}