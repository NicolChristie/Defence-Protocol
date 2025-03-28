using UnityEngine;

public class WeaponNode : MonoBehaviour
{
    public Transform carryLocation; // The player's weapon carry position (set in Inspector)
    
    private GameObject storedWeapon; // The weapon assigned to THIS node
    private static GameObject playerWeapon; // The weapon the player is holding (shared across all nodes)
    private bool isPlayerInside = false;
    private float originalFireRate;
    private int originalProjectileDamage;

    private void Update()
    {
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

                float weaponMultiplier = player.GetWeaponMultiplier();
                float damageMultiplier = player.GetDamageMultiplier();

                Debug.Log("Player entered weapon node.");

                // If this node has a weapon as its child, set it as storedWeapon
                if (storedWeapon == null && transform.childCount > 0)
                {
                    storedWeapon = transform.GetChild(0).gameObject;
                }

                if (storedWeapon != null)
                {
                    Weaponprefab weaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
                    if (weaponPrefab != null)
                    {
                        originalFireRate = weaponPrefab.fireRate;
                        originalProjectileDamage = weaponPrefab.projectileDamage;
                        weaponPrefab.UpdateStats(weaponMultiplier, damageMultiplier, 1f, 1f, 0);
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
                player.ResetWeaponBoost();

                // Reset the weapon stats when the player leaves
                if (storedWeapon != null)
                {
                    Weaponprefab weaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
                    if (weaponPrefab != null)
                    {
                        weaponPrefab.UpdateStats(1f / player.GetWeaponMultiplier(), 1f / player.GetDamageMultiplier(), 1f, 1f, 0);
                    }
                }
            }
        }
    }

    private void HandleWeaponPickupOrDrop()
    {
        if (carryLocation == null)
        {
            Debug.LogWarning("Carry Location not assigned in WeaponNode!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (playerWeapon == null && storedWeapon != null)
        {
            // ✅ Pick up the weapon from this node
            playerWeapon = storedWeapon;
            playerWeapon.transform.position = carryLocation.position;
            playerWeapon.transform.SetParent(player.transform);

            // Save and reset the rotation when picking up
            Quaternion weaponRotation = playerWeapon.transform.localRotation;
            playerWeapon.transform.localRotation = Quaternion.identity; // Prevent weapon from rotating with the node

            storedWeapon = null; // This node no longer holds a weapon
            playerWeapon.transform.localRotation = weaponRotation; // Restore the original weapon rotation
            Debug.Log("Weapon picked up.");
        }
        else if (playerWeapon != null && storedWeapon == null)
        {
            // ✅ Place the weapon onto this node
            storedWeapon = playerWeapon;
            storedWeapon.transform.position = transform.position;
            storedWeapon.transform.SetParent(transform);

            // Save and reset the rotation when placing down
            Quaternion weaponRotation = storedWeapon.transform.localRotation;
            storedWeapon.transform.localRotation = Quaternion.identity; // Prevent weapon from rotating with the node

            playerWeapon = null; // Player no longer holding a weapon
            storedWeapon.transform.localRotation = weaponRotation; // Restore the original weapon rotation
            Debug.Log("Weapon placed down.");
        }
        else
        {
            Debug.Log("No valid action to perform.");
        }
    }
}
