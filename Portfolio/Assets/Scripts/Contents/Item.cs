using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    public int itemCode;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public ItemType itemType;
    public int weaponCode;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public int attack;
    public int defence;

    public int giveHp;
    public int giveMp;
    public int giveSp;

    public int buyCost;
    public int sellCost;

    public enum ItemType
    {
        Weapon,
        Armor,
        Accessory,
        Equipment,
        Used,
        Ingredient,
        ETC
    }
}
