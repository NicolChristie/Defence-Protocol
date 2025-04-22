using UnityEngine;

public class PlayerShipUpgradeManager : MonoBehaviour
{
    public static PlayerShipUpgradeManager Instance;

    private CharacterManager character;
    // Placeholder for ShipManager (to be implemented later)
    // private ShipManager ship;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
{
    character = FindFirstObjectByType<CharacterManager>(); // ✅ Updated method
    if (character == null)
    {
        Debug.LogError("CharacterManager not found!");
    }

    // ship = FindFirstObjectByType<ShipManager>();  // Future ship manager reference
}


    public void ApplyUpgrade(ShopItem upgrade)
    {
        if (upgrade.itemType == ShopItem.ItemType.PlayerUpgrade)
        {
            ApplyPlayerUpgrade(upgrade);
        }
        else if (upgrade.itemType == ShopItem.ItemType.ShipUpgrade)
        {
            ApplyShipUpgrade(upgrade);
        }
    }

    private void ApplyPlayerUpgrade(ShopItem upgrade)
    {
        if (character == null)
        {
            Debug.LogError("No CharacterManager assigned.");
            return;
        }

        Debug.Log($"Applying {upgrade.itemName} with multiplier: {upgrade.upgradeValue}");

        switch (upgrade.itemName)
        {
            case "Speed Boost":
                character.baseSpeed *= upgrade.upgradeValue; 
                Debug.Log($"New Speed: {character.baseSpeed}"); 
                break;

            case "Weapon Fire Rate":
                character.weaponMultiplier *= upgrade.upgradeValue;  
                Debug.Log($"New Fire Rate: {character.weaponMultiplier}");
                break;

            case "Damage Boost":
                character.damageMultiplier *= upgrade.upgradeValue; 
                Debug.Log($"New Damage: {character.damageMultiplier}"); 
                break;

            case "Rotation Speed":
                character.rotationMultiplier *= upgrade.upgradeValue; 
                Debug.Log($"New Rotation Speed: {character.rotationMultiplier}"); 
                break;

            default:
                Debug.LogWarning("Upgrade not recognized: " + upgrade.itemName);
                return;
        }

        Debug.Log($"{upgrade.itemName} applied! New values: Speed={character.baseSpeed}, Fire Rate={character.weaponMultiplier}, Damage={character.damageMultiplier}");
    }

    private void ApplyShipUpgrade(ShopItem upgrade)
    {
        // 🚀 Placeholder for ship upgrades (to be implemented when ShipManager is ready)
        Debug.Log("Ship Upgrade functionality not implemented yet: " + upgrade.itemName);
    }
}
