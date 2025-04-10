using UnityEngine;
using System.Collections;

public class WeaponNode : MonoBehaviour
{
    public Transform carryLocation;
    public GameObject storedWeapon;
    public static GameObject playerWeapon;
    private bool isPlayerInside = false;
    public bool recentlyPurchasedWeapon = false;
    public bool mergedWeapon = false;

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
            Debug.Log(recentlyPurchasedWeapon);
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
        Debug.Log("Picking up stored weapon.");

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
        Debug.Log("Dropping player weapon into empty node.");

        storedWeapon = playerWeapon;
        playerWeapon = null;

        storedWeapon.transform.SetParent(transform);
        storedWeapon.transform.position = transform.position;
        storedWeapon.transform.localRotation = Quaternion.identity;
        storedWeapon.transform.localScale = Vector3.one;

        CharacterManager currentPlayer = player.GetComponent<CharacterManager>();
        if (currentPlayer != null && storedWeapon != null)
        {
            Weaponprefab weaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
            if (weaponPrefab != null)
            {
                Debug.Log("Resetting dropped weapon to base stats.");
                weaponPrefab.ResetToBaseStats();
            }
        }

        WeaponNode weaponNode = storedWeapon.GetComponent<WeaponNode>();
        if (weaponNode != null && weaponNode.recentlyPurchasedWeapon)
        {
            Debug.Log("Stored weapon was recently purchased. Reopening shop...");
            weaponNode.recentlyPurchasedWeapon = false;
            StartCoroutine(ShowShopWithDelay(0.5f));
        }

        return;
    }
    else if (playerWeapon != null && storedWeapon != null)
    {
    Weaponprefab playerWeaponPrefab = playerWeapon.GetComponent<Weaponprefab>();
    Weaponprefab storedWeaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
    Debug.Log(recentlyPurchasedWeapon);
    if (playerWeaponPrefab != null && storedWeaponPrefab != null)
    {

        if (playerWeaponPrefab.originalPrefab == storedWeaponPrefab.originalPrefab)
        {
            Debug.Log("Original prefabs match. Attempting to merge...");

            Weaponprefab result = WeaponMergeManager.Instance.GetMergeResult(
                playerWeaponPrefab,
                storedWeaponPrefab
            );

            if (result != null)
            {
                Debug.Log("Merge result found! Replacing both weapons.");

                Destroy(playerWeapon);
                Destroy(storedWeapon);
                playerWeapon = null;
                storedWeapon = null;

                GameObject newWeapon = Instantiate(result.gameObject, transform.position, Quaternion.identity, transform);
                storedWeapon = newWeapon;

                Weaponprefab newWeaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
                if (newWeaponPrefab != null)
                {
                    Debug.Log("Resetting merged weapon to base stats.");
                    newWeaponPrefab.ResetToBaseStats();
                    newWeaponPrefab.transform.localScale = Vector3.one * 1f;
                    mergedWeapon = true;  // Set the flag to indicate a merge happened
                }

                WeaponNode weaponNode = storedWeapon.GetComponent<WeaponNode>();
                if (weaponNode != null)
                {
                    weaponNode.boostApplied = false;
                }

                // Open the shop after a successful merge
                if (weaponNode != null && weaponNode.recentlyPurchasedWeapon)
                {
                    Debug.Log("Merged weapon was recently purchased. Reopening shop...");
                    recentlyPurchasedWeapon = false;  // Reset the flag
                    StartCoroutine(ShowShopWithDelay(0.5f));  // Open shop with a delay
                }else{ 
                    Debug.Log("Merged weapon was not recently purchased. No shop to open.");
                    Debug.Log(recentlyPurchasedWeapon);
                }

                return;
            }
            else
            {
                Debug.Log("No valid merge result. Swapping instead.");
            }
        }
        else
        {
            Debug.Log("Weapons are not identical. Skipping merge and swapping instead.");
        }

        // Perform swap if no merge happened
        Debug.Log($"Swapping weapons: {playerWeaponPrefab.name} <--> {storedWeaponPrefab.name}");

        GameObject temp = storedWeapon;
        storedWeapon = playerWeapon;
        playerWeapon = temp;

        storedWeapon.transform.SetParent(transform);
        storedWeapon.transform.position = transform.position;
        storedWeapon.transform.localRotation = Quaternion.identity;
        storedWeapon.transform.localScale = Vector3.one;

        playerWeapon.transform.SetParent(player.transform);
        playerWeapon.transform.position = carryLocation.position;
        playerWeapon.transform.localRotation = Quaternion.identity;
        playerWeapon.transform.localScale = Vector3.one * 0.66f;

        return;
    }

    Debug.LogWarning("One of the weapons is missing a Weaponprefab component!");
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
        Debug.Log("Setting recently purchased weapon flag.");  
        recentlyPurchasedWeapon = true;
    }
}
