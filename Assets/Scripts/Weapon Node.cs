using UnityEngine;
using System.Collections;

public class WeaponNode : MonoBehaviour
{
    public Transform carryLocation;
    public GameObject storedWeapon;
    public static GameObject playerWeapon;

    public Weaponprefab storedWeaponPrefab;
    public GameObject Outline;
    private static Weaponprefab playerWeaponPrefab;

    private bool isPlayerInside = false;
    public bool mergedWeapon = false;
    private bool boostApplied = false;

    void Start()
    {
        if (carryLocation == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                carryLocation = player.transform.Find("carryLocation");
                if (carryLocation == null)
                {
                    Debug.LogError("Carry location not found on the player!");
                }
            }
            else
            {
                Debug.LogError("Player object not found!");
            }
        }

        if (carryLocation == null)
        {
            Debug.LogError("No carryLocation assigned in WeaponNode or found on Player.");
            return;
        }
    }

    void Update()
    {
        if (ShopManager.Instance == null || ShopManager.Instance.shopPanel == null)
            return;

        if (ShopManager.Instance.shopPanel.activeSelf)
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
                Outline.SetActive(true);

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
                }
                else if (storedWeapon != null)
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
                Outline.SetActive(false);

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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (carryLocation == null)
        {
            if (player != null)
            {
                carryLocation = player.transform.Find("carryLocation");
                if (carryLocation == null)
                    return;
            }
        }

        if (player == null)
            return;

        if (playerWeapon == null && storedWeapon != null)
        {
            PickupWeapon(player);
            return;
        }
        else if (playerWeapon != null && storedWeapon == null)
        {
            DropWeapon(player);
            return;
        }
        else if (playerWeapon != null && storedWeapon != null)
        {
            SwapOrMergeWeapons(player);
            return;
        }
        else
        {
            return;
        }
    }

    private void PickupWeapon(GameObject player)
    {
        playerWeapon = storedWeapon;
        playerWeaponPrefab = playerWeapon.GetComponent<Weaponprefab>();
        storedWeapon = null;
        storedWeaponPrefab = null;

        playerWeapon.transform.SetParent(player.transform);
        playerWeapon.transform.position = carryLocation.position;
        playerWeapon.transform.localRotation = Quaternion.identity;
        playerWeapon.transform.localScale = Vector3.one * 0.5f;
    }

    private void DropWeapon(GameObject player)
    {
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
            storedWeaponPrefab.CalibrateRange();
        }
        boostApplied = false;

        if (storedWeaponPrefab != null && storedWeaponPrefab.wasPurchased)
        {
            storedWeaponPrefab.wasPurchased = false;
            StartCoroutine(ShowShopWithDelay(0.5f));
        }
    }

    private void SwapOrMergeWeapons(GameObject player)
    {
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

        SwapWeapons(player);
    }

    private void MergeWeapons(Weaponprefab result)
    {
        Destroy(storedWeapon);
        storedWeapon = null;
        storedWeaponPrefab = null;

        GameObject newWeapon = Instantiate(result.gameObject, transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        storedWeapon = newWeapon;
        storedWeaponPrefab = storedWeapon.GetComponent<Weaponprefab>();
        storedWeaponPrefab.originalRange = result.originalRange > 0 ? result.originalRange : storedWeaponPrefab.range;
storedWeaponPrefab.ManualInit();

        storedWeaponPrefab.ManualInit();
        storedWeaponPrefab.originalPrefab = result.originalPrefab != null ? result.originalPrefab : result.gameObject;

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
            storedWeaponPrefab.CalibrateRange();
            storedWeaponPrefab.transform.localScale = Vector3.one;
            boostApplied = false;
            mergedWeapon = true;
        }

        WeaponNode newWeaponNode = newWeapon.GetComponent<WeaponNode>() ?? newWeapon.AddComponent<WeaponNode>();
        newWeaponNode.boostApplied = false;
        if (storedWeaponPrefab.wasPurchased)
        {
            StartCoroutine(ShowShopWithDelay(0.5f));
            storedWeaponPrefab.wasPurchased = false;
        }
    }

    private void SwapWeapons(GameObject player)
    {
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
