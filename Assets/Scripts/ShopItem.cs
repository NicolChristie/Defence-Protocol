using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public string description;
    public ItemType itemType;
    public GameObject weaponPrefab;
    public Texture2D itemImage;
    public float upgradeValue;
    public Rarity itemRarity;
    public int unlockLevel;

    public enum ItemType
    {
        Weapon,
        ShipUpgrade,
        PlayerUpgrade
    }

    public enum Rarity
    {
        Common,
        Rare,
        UltraRare,
        Legendary
    }

    // Constructor for weapon items — auto-generates name and description
    public ShopItem(GameObject weapon, int cost, Texture2D image = null, int level = 0)
    {
        weaponPrefab = weapon;
        itemType = ItemType.Weapon;
        price = cost;
        itemImage = image;
        upgradeValue = 0;
        itemRarity = Rarity.Common;
        unlockLevel = level;

        if (weaponPrefab != null)
        {
            Weaponprefab weaponScript = weaponPrefab.GetComponent<Weaponprefab>();
            itemName = weaponPrefab.name;

            if (weaponScript != null)
            {
                description = $"Damage: {weaponScript.projectileDamage}\n" +
                              $"Fire Rate: {weaponScript.fireRate:F2} shots/sec\n" +
                              $"Range: {weaponScript.range:F1} units";
            }
            else
            {
                description = "No weapon stats available.";
            }
        }
        else
        {
            itemName = "Unknown Weapon";
            description = "Invalid weapon prefab.";
        }
    }

    // Constructor for upgrades (non-weapon items)
    public ShopItem(string name, int cost, string desc, ItemType type, float value, Texture2D image = null, int level = 0)
    {
        itemName = name;
        price = cost;
        description = desc;
        itemType = type;
        upgradeValue = value;
        itemImage = image;
        weaponPrefab = null;
        itemRarity = Rarity.Common;
        unlockLevel = level;
    }
}
