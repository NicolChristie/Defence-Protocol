using UnityEngine;
using System.Collections;

public class WeaponNode : MonoBehaviour
{
    public Transform carryLocation;
    public GameObject storedWeapon;
    public static GameObject playerWeapon;
    private bool isPlayerInside = false;
    private bool recentlyPurchasedWeapon = false;
    private bool boostApplied = false;

    private void OnEnable()
    {
        ShopManager.OnWeaponPurchased += HandleWeaponPurchased;
    }

    private void OnDisable()
    {
        ShopManager.OnWeaponPurchased -= HandleWeaponPurchased;
    }

    private void HandleWeaponPurchased()
    {
        recentlyPurchasedWeapon = true;
    }

    private void Update()
    {
        if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeSelf)
            return;

        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            HandleWeaponPickupOrDrop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterManager player = collision.GetComponent<CharacterManager>();
            if (player != null)
            {
                isPlayerInside = true;

                if (storedWeapon != null)
                {
                    Weaponprefab weaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
                    if (weaponPrefab != null && !boostApplied)
                    {
                        weaponPrefab.ApplyBoost(
                            player.GetWeaponMultiplier(),
                            player.GetDamageMultiplier(),
                            false
                        );
                        boostApplied = true;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterManager player = collision.GetComponent<CharacterManager>();
            if (player != null)
            {
                isPlayerInside = false;

                if (storedWeapon != null)
                {
                    Weaponprefab weaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
                    if (weaponPrefab != null && boostApplied)
                    {
                        weaponPrefab.RemoveBoost(
                            player.GetWeaponMultiplier(),
                            player.GetDamageMultiplier()
                        );
                        boostApplied = false;
                    }
                }
            }
        }
    }

    private void HandleWeaponPickupOrDrop()
{
    if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeSelf)
    {
        Debug.Log("Shop is open. Cannot pick up or drop weapons.");
        return;
    }

    if (carryLocation == null)
    {
        Debug.LogWarning("Carry Location not assigned in WeaponNode!");
        return;
    }

    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null)
    {
        Debug.LogWarning("Player not found!");
        return;
    }

    if (playerWeapon == null && storedWeapon != null)
    {
        playerWeapon = storedWeapon;
        storedWeapon = null;

        playerWeapon.transform.SetParent(player.transform);
        playerWeapon.transform.position = carryLocation.position;
        playerWeapon.transform.localRotation = Quaternion.identity;
        playerWeapon.transform.localScale = Vector3.one * 0.66f;
        return;
    }
    else if (playerWeapon != null && storedWeapon == null)
    {
        storedWeapon = playerWeapon;
        playerWeapon = null;

        storedWeapon.transform.SetParent(transform);
        storedWeapon.transform.position = transform.position;
        storedWeapon.transform.localRotation = Quaternion.identity;
        storedWeapon.transform.localScale = Vector3.one;

        CharacterManager currentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>();
        if (currentPlayer != null && storedWeapon != null)
        {
            Weaponprefab weaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
            if (weaponPrefab != null)
            {
                weaponPrefab.ResetToBaseStats();
            }
        }

        WeaponNode weaponNode = storedWeapon.GetComponent<WeaponNode>();
        if (weaponNode != null && weaponNode.recentlyPurchasedWeapon)
        {
            weaponNode.recentlyPurchasedWeapon = false;
            StartCoroutine(ShowShopWithDelay(0.5f));
        }

        return;
    }
    else if (playerWeapon != null && storedWeapon != null)
    {
        Weaponprefab playerWeaponPrefab = playerWeapon.GetComponent<Weaponprefab>();
        Weaponprefab storedWeaponPrefab = storedWeapon.GetComponent<Weaponprefab>();

        if (playerWeaponPrefab != null && storedWeaponPrefab != null)
        {
            if (playerWeaponPrefab.level == storedWeaponPrefab.level &&
                playerWeaponPrefab.levelSprites[playerWeaponPrefab.level - 1] == storedWeaponPrefab.levelSprites[storedWeaponPrefab.level - 1])
            {
                Debug.Log("Merging weapons!");

                Destroy(playerWeapon);
                playerWeapon = null;

                storedWeaponPrefab.UpgradeWeapon(); // 👈 Merge logic and placeholder for future upgrades
                WeaponNode weaponNode = storedWeapon.GetComponent<WeaponNode>();
                if (weaponNode != null && weaponNode.recentlyPurchasedWeapon)
                {
                    weaponNode.recentlyPurchasedWeapon = false;
                    StartCoroutine(ShowShopWithDelay(0.5f));
                }
                return;
            }
        }

        Debug.Log("Already holding a weapon. Cannot pick up another one.");
        return;
    }
}


    private IEnumerator ShowShopWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShopManager.Instance.ShowShop();
    }

    public void SetRecentlyPurchasedWeapon()
    {
        recentlyPurchasedWeapon = true;
    }
}
