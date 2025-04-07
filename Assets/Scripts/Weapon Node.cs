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

        CharacterManager currentPlayer = player.GetComponent<CharacterManager>();
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
        // Debug: Log the names of both weapons to confirm they are identical
        Debug.Log($"Player Weapon: {playerWeaponPrefab.name}, Stored Weapon: {storedWeaponPrefab.name}");

        // Check if the names are identical before attempting to merge
        if (playerWeaponPrefab.name == storedWeaponPrefab.name)
        {
            Debug.Log("Weapons have the same name. Attempting to merge...");

            Weaponprefab result = WeaponMergeManager.Instance.GetMergeResult(
                playerWeaponPrefab,
                storedWeaponPrefab
            );

            if (result != null)
            {
                Debug.Log("Merging weapons from merge database!");

                // Destroy the old weapons
                Destroy(playerWeapon);
                Destroy(storedWeapon);
                playerWeapon = null;
                storedWeapon = null;

                // Instantiate the merged weapon
                GameObject newWeapon = Instantiate(result.gameObject, transform.position, Quaternion.identity, transform);
                storedWeapon = newWeapon;

                // Reset the relevant properties of the new weapon (such as removing any boosts or applying default stats)
                Weaponprefab newWeaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
                if (newWeaponPrefab != null)
                {
                    // Reset any boosts or stats here, if needed
                    newWeaponPrefab.ResetToBaseStats();
                }

                // Optionally reset the WeaponNode of the new weapon
                WeaponNode newWeaponNode = storedWeapon.GetComponent<WeaponNode>();
                if (newWeaponNode != null)
                {
                    newWeaponNode.boostApplied = false; // Reset boost state for new weapon
                }

                return;
            }
            else
            {
                Debug.Log("No valid merge result found. Swapping weapons.");

                // No valid merge, swap weapons
                Debug.Log($"Swapping weapons: {playerWeaponPrefab.name} and {storedWeaponPrefab.name}");

                // Swap the player and stored weapons
                GameObject temp = storedWeapon;
                storedWeapon = playerWeapon;
                playerWeapon = temp;

                // Reposition and reset the swapped weapons
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
        }
        else
        {
            Debug.Log("Weapons have different names. No merge attempt.");
            // No merge attempt, just swap them
            Debug.Log($"Swapping weapons: {playerWeaponPrefab.name} and {storedWeaponPrefab.name}");

            // Swap the player and stored weapons
            GameObject temp = storedWeapon;
            storedWeapon = playerWeapon;
            playerWeapon = temp;

            // Reposition and reset the swapped weapons
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
    }

    Debug.LogWarning("Weaponprefab component not found on player or stored weapon!");
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
