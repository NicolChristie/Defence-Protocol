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
    public coverScript coverScript;

    private bool isPurchasing = false;

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
        RemovePlayerWeapon();
    }

    public void RemovePlayerWeapon()
    {
        if (WeaponNode.playerWeapon != null)
        {
            Destroy(WeaponNode.playerWeapon);
            WeaponNode.playerWeapon = null;
        }
    }

    public void GenerateShop()
    {
                coverScript.allowMouseZoom = false;
        coverScript.ForceZoomIn();
        currentShopItems.Clear();
        int finishedAmount = SaveManager.LoadFinishedAmount();
        List<ShopItem> unlockedItems = availableItems.FindAll(item => item.unlockLevel <= finishedAmount);

        if (unlockedItems.Count < 3)
            return;

        HashSet<ShopItem> chosenItems = new HashSet<ShopItem>();
        while (chosenItems.Count < 3)
        {
            ShopItem.Rarity rarity = GetRandomRarity();
            List<ShopItem> itemsOfRarity = unlockedItems.FindAll(item => item.itemRarity == rarity);

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

            switch (currentItem.itemRarity)
            {
                case ShopItem.Rarity.Common: itemNameTexts[slot].color = Color.black; break;
                case ShopItem.Rarity.Rare: itemNameTexts[slot].color = Color.green; break;
                case ShopItem.Rarity.UltraRare: itemNameTexts[slot].color = Color.blue; break;
                case ShopItem.Rarity.Legendary: itemNameTexts[slot].color = Color.red; break;
            }

            if (currentItem.itemImage != null)
            {
                itemImageSlots[slot].texture = currentItem.itemImage;
                itemImageSlots[slot].enabled = true;
                AdjustRawImageAspect(itemImageSlots[slot], currentItem.itemImage);
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

    private ShopItem.Rarity GetRandomRarity()
    {
        float roll = Random.Range(0f, 100f);
        if (roll < 1f) return ShopItem.Rarity.Legendary;
        if (roll < 5f) return ShopItem.Rarity.UltraRare;
        if (roll < 25f) return ShopItem.Rarity.Rare;
        return ShopItem.Rarity.Common;
    }

    public void BuyItem(int shopSlot)
    {
        if (shopSlot < 0 || shopSlot >= currentShopItems.Count || isPurchasing)
            return;

        isPurchasing = true;
        ShopItem itemToBuy = currentShopItems[shopSlot];
        buyButtons[shopSlot].interactable = false;

        if (CoinManager.Instance.SpendCoins(itemToBuy.price))
        {
            if (itemToBuy.itemType == ShopItem.ItemType.Weapon)
            {
                itemToBuy.price += 1;
                EquipWeapon(itemToBuy);
                HideShop();
            }
            else if (itemToBuy.itemType == ShopItem.ItemType.PlayerUpgrade || itemToBuy.itemType == ShopItem.ItemType.ShipUpgrade)
            {
                PlayerShipUpgradeManager.Instance.ApplyUpgrade(itemToBuy);
                GenerateShop();
            }

            StartCoroutine(ReenableButtonAfterPurchase(shopSlot));
        }
        else
        {
            buyButtons[shopSlot].interactable = true;
            isPurchasing = false;
        }
    }

    private IEnumerator ReenableButtonAfterPurchase(int shopSlot)
    {
        yield return new WaitForSeconds(1f);
        buyButtons[shopSlot].interactable = true;
        isPurchasing = false;
    }

    public void EquipWeapon(ShopItem item)
    {
        if (item.weaponPrefab == null || WeaponNode.playerWeapon != null)
            return;

        GameObject weaponInstance = Instantiate(item.weaponPrefab);
        Weaponprefab weaponScript = weaponInstance.GetComponent<Weaponprefab>();
        if (weaponScript != null)
            weaponScript.originalPrefab = item.weaponPrefab;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Destroy(weaponInstance);
            return;
        }

        Transform carryLocation = player.transform.Find("carryLocation");
        if (carryLocation == null)
        {
            Destroy(weaponInstance);
            return;
        }

        weaponInstance.transform.SetParent(carryLocation);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;
        weaponInstance.transform.localScale = Vector3.one;

        WeaponNode.playerWeapon = weaponInstance;

        WeaponNode weaponNode = weaponInstance.AddComponent<WeaponNode>();
        Weaponprefab weaponPrefab = weaponInstance.GetComponent<Weaponprefab>();
        if (weaponPrefab != null)
            weaponPrefab.wasPurchased = true;

        if (weaponNode.mergedWeapon)
        {
            weaponNode.mergedWeapon = false;
            StartCoroutine(ShowShopWithDelay(0.5f));
        }
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

    public void ReloadShop()
    {
        for (int slot = 0; slot < currentShopItems.Count; slot++)
        {
            ShopItem currentItem = currentShopItems[slot];

            itemNameTexts[slot].text = currentItem.itemName;
            itemPriceTexts[slot].text = "Price: " + currentItem.price;
            itemDescriptionTexts[slot].text = currentItem.description;

            switch (currentItem.itemRarity)
            {
                case ShopItem.Rarity.Common: itemNameTexts[slot].color = Color.black; break;
                case ShopItem.Rarity.Rare: itemNameTexts[slot].color = Color.green; break;
                case ShopItem.Rarity.UltraRare: itemNameTexts[slot].color = Color.blue; break;
                case ShopItem.Rarity.Legendary: itemNameTexts[slot].color = Color.red; break;
            }

            if (currentItem.itemImage != null)
            {
                itemImageSlots[slot].texture = currentItem.itemImage;
                itemImageSlots[slot].enabled = true;
                AdjustRawImageAspect(itemImageSlots[slot], currentItem.itemImage);
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

    private IEnumerator ShowShopWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowShop();
    }

    public void ShowShop()
    {
        GenerateShop();
        shopPanel.SetActive(true);
        goToShop.gameObject.SetActive(false);
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
    }

    public void ReturnToShip()
    {
        goToShop.gameObject.SetActive(true);
        shopPanel.SetActive(false);
    }

    public void ReturnToShop()
    {
        shopPanel.SetActive(true);
        goToShop.gameObject.SetActive(false);
    }
}
