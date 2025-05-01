using UnityEngine;

public class PlayerShipUpgradeManager : MonoBehaviour
{
    public static PlayerShipUpgradeManager Instance;

    private CharacterManager character;

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
                character.ApplyUpgrade("Speed Boost", upgrade.upgradeValue);
                break;

            case "Fire Rate Boost":
                character.ApplyUpgrade("Weapon Fire Rate", upgrade.upgradeValue);
                break;

            case "Damage Boost":
                character.ApplyUpgrade("Damage Boost", upgrade.upgradeValue);
                break;

            case "Rotation Boost":
                character.ApplyUpgrade("Rotation Speed", upgrade.upgradeValue);
                break;

            default:
                Debug.LogWarning("Upgrade not recognized: " + upgrade.itemName);
                break;
        }

        Debug.Log($"{upgrade.itemName} applied! New values: Speed={character.baseSpeed}, Fire Rate={character.weaponMultiplier}, Damage={character.damageMultiplier}");
    }

    private void ApplyShipUpgrade(ShopItem upgrade)
    {
        Debug.Log("Ship Upgrade functionality not implemented yet: " + upgrade.itemName);
    }
}
