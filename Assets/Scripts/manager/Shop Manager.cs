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
    public Button goToShip;
    public Button goToShop;
    public GameObject shopPanel;
    private bool isPurchasing = false;
    private Dictionary<ShopItem, int> weaponPurchaseCounts = new Dictionary<ShopItem, int>(); //remove

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

        if (availableItems.Count < 3)
        {
            Debug.LogError("Not enough items in the availableItems list!");
            return;
        }

        HashSet<ShopItem> chosenItems = new HashSet<ShopItem>();
        while (chosenItems.Count < 3)
        {
            ShopItem.Rarity rarity = GetRandomRarity();
            List<ShopItem> itemsOfRarity = availableItems.FindAll(item => item.itemRarity == rarity);
            if (itemsOfRarity.Count > 0)
            {
                ShopItem candidate = itemsOfRarity[Random.Range(0, itemsOfRarity.Count)];
                if (!chosenItems.Contains(candidate))
                {
                    chosenItems.Add(candidate);
                    currentShopItems.Add(candidate);
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

            if (currentItem.itemImage != null)
            {
                itemImageSlots[slot].texture = currentItem.itemImage;
                itemImageSlots[slot].enabled = true;
                AdjustRawImageAspect(itemImageSlots[slot], currentItem.itemImage); // 🔥 ADD THIS LINE
            }
            else
            {
                itemImageSlots[slot].enabled = false;
            }

            int slotNumber = slot;
            buyButtons[slot].onClick.RemoveAllListeners();
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

        // Track and increase price if it's a weapon
        if (itemToBuy.itemType == ShopItem.ItemType.Weapon)
        {
            if (!weaponPurchaseCounts.ContainsKey(itemToBuy))
                weaponPurchaseCounts[itemToBuy] = 1;
            else
                weaponPurchaseCounts[itemToBuy]++;

            itemToBuy.price += 1; // Increase the price by 1 after each purchase

            EquipWeapon(itemToBuy);
            HideShop();
            GenerateShop(); // Regenerate the shop after purchase
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

    public void EquipWeapon(ShopItem item)
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

        // Instantiate and store the original prefab reference
        GameObject weaponInstance = Instantiate(item.weaponPrefab);
        Weaponprefab weaponScript = weaponInstance.GetComponent<Weaponprefab>();
        if (weaponScript != null)
        {
            weaponScript.originalPrefab = item.weaponPrefab; // ✅ Set originalPrefab here
        }

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
        Weaponprefab weaponPrefab = weaponInstance.GetComponent<Weaponprefab>();
        if (weaponPrefab != null)
        {
            weaponPrefab.wasPurchased = true;
            Debug.Log("Weapon purchased! Setting wasPurchased flag.");
        }
        // Check if the weapon was merged
        if (weaponNode.mergedWeapon)
        {
            Debug.Log("Merged weapon detected. Handling merge...");
            weaponNode.mergedWeapon = false; // Reset flag after handling the merged weapon
            StartCoroutine(ShowShopWithDelay(0.5f)); // Open the shop after merge
        }

        Debug.Log("Weapon equipped: " + weaponInstance.name);
    }

    private void AdjustRawImageAspect(RawImage rawImage, Texture texture)
    {
        if (texture == null || rawImage == null) return;

        float textureWidth = texture.width;
        float textureHeight = texture.height;
        float aspectRatio = textureWidth / textureHeight;

        RectTransform rt = rawImage.rectTransform;
        float currentHeight = rt.rect.height;
        float newWidth = currentHeight * aspectRatio;

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }


    private IEnumerator ShowShopWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowShop();
    }

    public void ShowShop()
    {
        shopPanel.SetActive(true);
        goToShop.gameObject.SetActive(false);
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
    }

    public void ReturnToShip(){
        Debug.Log("Returning to ship...");
        goToShop.gameObject.SetActive(true);
        shopPanel.SetActive(false);

    }

    public void ReturnToShop(){
        Debug.Log("Going to shop...");
        shopPanel.SetActive(true);
        goToShop.gameObject.SetActive(false);
    }
}