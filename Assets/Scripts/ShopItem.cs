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
 
    public ShopItem(string name, int cost, string desc, GameObject weapon, Texture2D image = null, int level = 0)
    {
        itemName = name;
        price = cost;
        description = desc;
        itemType = ItemType.Weapon;
        weaponPrefab = weapon;
        itemImage = image;
        upgradeValue = 0;  
        itemRarity = Rarity.Common;  
        unlockLevel = level;         
    }

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
