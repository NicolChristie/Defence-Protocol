using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public List<ShopItem> availableItems = new List<ShopItem>(); 
    private List<ShopItem> currentShopItems = new List<ShopItem>();

    public TextMeshProUGUI[] itemNameTexts;
    public TextMeshProUGUI[] itemPriceTexts;
    public TextMeshProUGUI[] itemDescriptionTexts;
    public RawImage[] itemImageSlots;
    public Button[] buyButtons;
    public GameObject shopPanel;
    private bool isPurchasing = false;


    public static event System.Action OnWeaponPurchased;
    private List<bool> purchasedFlags = new List<bool>();


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GenerateShop();
    }

    public void GenerateShop()
{
    currentShopItems.Clear();
    purchasedFlags.Clear();

    if (availableItems.Count < 3)
    {
        Debug.LogError("Not enough items in the availableItems list!");
        return;
    }

    List<int> chosenIndexes = new List<int>();
    while (chosenIndexes.Count < 3)
    {
        // Roll for rarity first
        ShopItem.Rarity rarity = GetRandomRarity();

        // Get items of that rarity
        List<ShopItem> itemsOfRarity = availableItems.FindAll(item => item.itemRarity == rarity);

        // Ensure there are items of that rarity to choose from
        if (itemsOfRarity.Count > 0)
        {
            int randomIndex = Random.Range(0, itemsOfRarity.Count);
            if (!chosenIndexes.Contains(randomIndex))
            {
                chosenIndexes.Add(randomIndex);
                currentShopItems.Add(itemsOfRarity[randomIndex]);
                purchasedFlags.Add(false);  // Initialize the flag as false (not purchased)
            }
        }
    }

    for (int slot = 0; slot < 3; slot++)
    {
        ShopItem currentItem = currentShopItems[slot];

        itemNameTexts[slot].text = currentItem.itemName;
        itemPriceTexts[slot].text = "Price: " + currentItem.price;
        itemDescriptionTexts[slot].text = currentItem.description;

        // Change the name color based on rarity
        switch (currentItem.itemRarity)
        {
            case ShopItem.Rarity.Common:
                itemNameTexts[slot].color = Color.black; // Black for common
                break;
            case ShopItem.Rarity.Rare:
                itemNameTexts[slot].color = Color.green; // Green for rare
                break;
            case ShopItem.Rarity.UltraRare:
                itemNameTexts[slot].color = Color.blue; // Blue for ultra rare
                break;
            case ShopItem.Rarity.Legendary:
                itemNameTexts[slot].color = Color.red; // Red for legendary
                break;
        }

        // Set item image for RawImage UI
        if (currentItem.itemImage != null)
        {
            itemImageSlots[slot].texture = currentItem.itemImage;
            itemImageSlots[slot].enabled = true;
        }
        else
        {
            itemImageSlots[slot].enabled = false;
        }

        int slotNumber = slot;
        buyButtons[slot].onClick.RemoveAllListeners();
        Debug.Log("Adding listener for button slot: " + slotNumber); // Add debug to track listener addition
        buyButtons[slot].onClick.AddListener(() => BuyItem(slotNumber));
    }
}



    // Method to roll for a rarity
    private ShopItem.Rarity GetRandomRarity()
    {
        float roll = Random.Range(0f, 100f);

        if (roll < 1f)  // 1% chance for Legendary
        {
            return ShopItem.Rarity.Legendary;
        }
        else if (roll < 5f)  // 4% chance for Ultra Rare
        {
            return ShopItem.Rarity.UltraRare;
        }
        else if (roll < 25f)  // 20% chance for Rare
        {
            return ShopItem.Rarity.Rare;
        }
        else  // 75% chance for Common
        {
            return ShopItem.Rarity.Common;
        }
    }

  public void BuyItem(int shopSlot)
{
    if (shopSlot < 0 || shopSlot >= currentShopItems.Count)
    {
        Debug.LogError("Invalid shop slot selected!");
        return;
    }

    if (isPurchasing)  // Prevent purchase if already in progress
    {
        Debug.LogWarning("Purchase already in progress, skipping this request.");
        return;
    }

    isPurchasing = true;  // Set flag to true when purchase starts

    ShopItem itemToBuy = currentShopItems[shopSlot];

    // Disable the button to prevent double clicking
    buyButtons[shopSlot].interactable = false;

    if (CoinManager.Instance.SpendCoins(itemToBuy.price))
    {
        Debug.Log("Purchased: " + itemToBuy.itemName);

        if (itemToBuy.itemType == ShopItem.ItemType.Weapon)
        {
            EquipWeapon(itemToBuy);
            HideShop();
        }
        else if (itemToBuy.itemType == ShopItem.ItemType.PlayerUpgrade || itemToBuy.itemType == ShopItem.ItemType.ShipUpgrade)
        {
            PlayerShipUpgradeManager.Instance.ApplyUpgrade(itemToBuy);
        }

        // Optionally, delay re-enabling the button to avoid repeated clicks
        StartCoroutine(ReenableButtonAfterPurchase(shopSlot));
    }
    else
    {
        Debug.Log("Not enough coins to buy: " + itemToBuy.itemName);
        // Re-enable the button if purchase fails
        buyButtons[shopSlot].interactable = true;
        isPurchasing = false;  // Reset the flag if purchase fails
    }
    }
    private IEnumerator ReenableButtonAfterPurchase(int shopSlot)
    {
    // Wait for 1 second before re-enabling the button
    yield return new WaitForSeconds(1f);

    // Re-enable the button after the wait
    buyButtons[shopSlot].interactable = true;

    // Reset the purchasing flag
    isPurchasing = false;
    }





    void EquipWeapon(ShopItem item)
{
    if (item.weaponPrefab == null)
    {
        Debug.LogError("No weapon prefab assigned!");
        return;
    }

    if (WeaponNode.playerWeapon != null)
    {
        Debug.Log("Player is already holding a weapon. Cannot equip another.");
        return;
    }

    GameObject weaponInstance = Instantiate(item.weaponPrefab);

    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null)
    {
        Debug.LogError("Player not found!");
        Destroy(weaponInstance);
        return;
    }

    Transform carryLocation = player.transform.Find("carryLocation");
    if (carryLocation == null)
    {
        Debug.LogError("Carry Location not found on player!");
        Destroy(weaponInstance);
        return;
    }

    Debug.Log("Equipping weapon to player...");

    weaponInstance.transform.SetParent(carryLocation);
    weaponInstance.transform.localPosition = Vector3.zero;
    weaponInstance.transform.localRotation = Quaternion.identity;
    weaponInstance.transform.localScale = Vector3.one * 1f;

    WeaponNode.playerWeapon = weaponInstance;

    WeaponNode weaponNode = weaponInstance.AddComponent<WeaponNode>();
    weaponNode.SetRecentlyPurchasedWeapon();

    Debug.Log("Weapon equipped: " + weaponInstance.name);
}


    public void ShowShop()
    {
        shopPanel.SetActive(true);
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
    }
}
