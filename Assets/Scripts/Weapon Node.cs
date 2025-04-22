using UnityEngine;
using System.Collections;

public class WeaponNode : MonoBehaviour
{
    public Transform carryLocation;
    public GameObject storedWeapon;
    public static GameObject playerWeapon;

    private Weaponprefab storedWeaponPrefab;
    private static Weaponprefab playerWeaponPrefab;

    private bool isPlayerInside = false;
    public bool mergedWeapon = false;
    private bool boostApplied = false;

    void Update()
{
    if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeSelf)
        return;

    if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
    {
            // Weapon was purchased, so handle pickup/drop/merge
            Debug.Log($"the player is carrying {playerWeaponPrefab} and the stored weapon is {storedWeaponPrefab}");
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

            // Ensure storedWeaponPrefab is assigned correctly to the weapon the player is carrying
            if (playerWeapon != null)
            {
                playerWeaponPrefab = playerWeapon.GetComponent<Weaponprefab>();
                if (playerWeaponPrefab != null && !boostApplied)
                {
                    playerWeaponPrefab.ApplyBoost(
                        player.GetWeaponMultiplier(),
                        player.GetDamageMultiplier(),
                        player.GetRotationMultiplier(),
                        false
                    );
                    boostApplied = true;
                }
            } else if (storedWeapon != null)
            {
                if (storedWeaponPrefab != null && !boostApplied)
                {
                    storedWeaponPrefab.ApplyBoost(
                        player.GetWeaponMultiplier(),
                        player.GetDamageMultiplier(),
                        player.GetRotationMultiplier(),
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

                if (storedWeaponPrefab != null && boostApplied)
                {
                    storedWeaponPrefab.RemoveBoost(
                        player.GetWeaponMultiplier(),
                        player.GetDamageMultiplier(),
                        player.GetRotationMultiplier()
                    );
                    boostApplied = false;
                }
            }
        }
    }

    private void HandleWeaponPickupOrDrop()
{
    if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeSelf)
        return;

    if (carryLocation == null){
        Debug.LogWarning("Carry location not assigned!");
        return;
    }

    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null)
    {
        Debug.LogWarning("Player not found!");
        return;
    }

    // Pickup Weapon
    if (playerWeapon == null && storedWeapon != null)
    {
        PickupWeapon(player);
        return;
    }
    // Drop Weapon
    else if (playerWeapon != null && storedWeapon == null)
    {
        DropWeapon(player);
        return;
    }
    // Swap or Merge Weapons
    else if (playerWeapon != null && storedWeapon != null)
    {
        SwapOrMergeWeapons(player);
        return;
    } else{
        Debug.LogWarning("No valid weapon to pick up or drop!");
        return;
    }

}


    private void PickupWeapon(GameObject player)
    {
        Debug.Log("Picking up weapon...");
        playerWeapon = storedWeapon;
        playerWeaponPrefab = playerWeapon.GetComponent<Weaponprefab>();
        storedWeapon = null;
        storedWeaponPrefab = null;

        playerWeapon.transform.SetParent(player.transform);
        playerWeapon.transform.position = carryLocation.position;
        playerWeapon.transform.localRotation = Quaternion.identity;
        playerWeapon.transform.localScale = Vector3.one * 0.66f;
    }

    private void DropWeapon(GameObject player)
    {
        Debug.Log("Dropping weapon...");
        storedWeapon = playerWeapon;
        storedWeaponPrefab = playerWeaponPrefab;
        playerWeapon = null;
        playerWeaponPrefab = null;

        storedWeapon.transform.SetParent(transform);
        storedWeapon.transform.position = transform.position;
        storedWeapon.transform.localRotation = Quaternion.identity;
        storedWeapon.transform.localScale = Vector3.one;

        CharacterManager currentPlayer = player.GetComponent<CharacterManager>();
        if (currentPlayer != null && storedWeaponPrefab != null)
        {
            storedWeaponPrefab.ResetToBaseStats();
        }

        // Check if weapon was purchased recently
        if (storedWeaponPrefab != null && storedWeaponPrefab.wasPurchased)
        {
            Debug.Log("Recently purchased weapon detected. Opening shop...");
            storedWeaponPrefab.wasPurchased = false;
            StartCoroutine(ShowShopWithDelay(0.5f)); // Open shop after placing weapon
        }
        else
        {
            Debug.Log("No recently purchased weapon detected. No shop to open.");
        }
    }

    private void SwapOrMergeWeapons(GameObject player)
    {
        Debug.Log($"trying to swap or merge weapons... {playerWeaponPrefab} and {storedWeaponPrefab}");
        if (playerWeaponPrefab == null)
            playerWeaponPrefab = playerWeapon.GetComponent<Weaponprefab>();
        if (storedWeaponPrefab == null)
            storedWeaponPrefab = storedWeapon.GetComponent<Weaponprefab>();

        if (playerWeaponPrefab != null && storedWeaponPrefab != null &&
            playerWeaponPrefab.originalPrefab == storedWeaponPrefab.originalPrefab)
        {
            Weaponprefab result = WeaponMergeManager.Instance.GetMergeResult(
                playerWeaponPrefab,
                storedWeaponPrefab
            );

            if (result != null)
            {
                MergeWeapons(result);
                return;
            }
        }

        // Swap weapons if merging is not possible
        SwapWeapons(player);
    }

    private void MergeWeapons(Weaponprefab result)
{
    Destroy(storedWeapon);
    storedWeapon = null;
    storedWeaponPrefab = null;

    GameObject newWeapon = Instantiate(result.gameObject, transform.position, Quaternion.identity, transform);
    storedWeapon = newWeapon;
    storedWeaponPrefab = storedWeapon.GetComponent<Weaponprefab>();

    // ✅ Assign the correct original prefab here
    storedWeaponPrefab.originalPrefab = result.originalPrefab != null ? result.originalPrefab : result.gameObject;

    // Transfer the wasPurchased flag
    if (playerWeaponPrefab.wasPurchased)
    {
        storedWeaponPrefab.wasPurchased = true;
    }

    Destroy(playerWeapon);
    playerWeaponPrefab = null;
    playerWeapon = null;

    if (storedWeaponPrefab != null)
    {
        storedWeaponPrefab.InitialiseBaseStats();
        storedWeaponPrefab.ResetToBaseStats();
        storedWeaponPrefab.transform.localScale = Vector3.one;
        mergedWeapon = true;
    }

    WeaponNode newWeaponNode = newWeapon.GetComponent<WeaponNode>() ?? newWeapon.AddComponent<WeaponNode>();
    newWeaponNode.boostApplied = false;

    Debug.Log("Weapon merged. Reopening shop...");
    StartCoroutine(ShowShopWithDelay(0.5f)); // Open shop after merge
    storedWeaponPrefab.wasPurchased = false;
}


    private void SwapWeapons(GameObject player)
    {
        Debug.Log("Swapping weapons...");
        GameObject temp = storedWeapon;
        Weaponprefab tempPrefab = storedWeaponPrefab;

        storedWeapon = playerWeapon;
        storedWeaponPrefab = playerWeaponPrefab;

        playerWeapon = temp;
        playerWeaponPrefab = tempPrefab;

        storedWeapon.transform.SetParent(transform);
        storedWeapon.transform.position = transform.position;
        storedWeapon.transform.localRotation = Quaternion.identity;
        storedWeapon.transform.localScale = Vector3.one;

        playerWeapon.transform.SetParent(player.transform);
        playerWeapon.transform.position = carryLocation.position;
        playerWeapon.transform.localRotation = Quaternion.identity;
        playerWeapon.transform.localScale = Vector3.one * 0.66f;

        // Swap the wasPurchased flag between prefabs
        if (storedWeaponPrefab != null && playerWeaponPrefab != null)
        {
            bool tempWasPurchased = storedWeaponPrefab.wasPurchased;
            storedWeaponPrefab.wasPurchased = playerWeaponPrefab.wasPurchased;
            playerWeaponPrefab.wasPurchased = tempWasPurchased;
        }
    }

    private IEnumerator ShowShopWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShopManager.Instance.ShowShop();
    }
}
