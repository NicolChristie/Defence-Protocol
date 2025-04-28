using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public float baseSpeed = 5f;          // Base speed of the player
    public float weaponMultiplier = 2f;   // Weapon multiplier (affects fire rate)
    public float damageMultiplier = 2f;   // Damage multiplier (affects projectile damage)
    public float rotationMultiplier = 2f; // Rotation multiplier (affects rotation speed)

    private float currentSpeed;           // Current player speed after applying boosts
    private float currentWeaponMultiplier;
    private float currentDamageMultiplier;
    private float currentRotationMultiplier;

    void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            Destroy(gameObject); // Avoid duplicates
            return;
        }
        Debug.Log("PlayerManager instance created: " + gameObject.name);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Initialize speed and multipliers
        currentSpeed = baseSpeed;
        currentWeaponMultiplier = weaponMultiplier;
        currentDamageMultiplier = damageMultiplier;
        currentRotationMultiplier = rotationMultiplier;
        Debug.Log("the current weapon multiplier is " + currentWeaponMultiplier + " the current damage multiplier is " + currentDamageMultiplier);
    }

    // Called when leaving the weapon node to reset multipliers
    public void ResetWeaponBoost()
    {
        currentWeaponMultiplier = weaponMultiplier;
        currentDamageMultiplier = damageMultiplier;
        currentRotationMultiplier = rotationMultiplier;
        currentSpeed = baseSpeed;  // Reset speed back to the base value
    }

    // Get the current multipliers for the weapon node to use
    public float GetWeaponMultiplier()
    {
        return currentWeaponMultiplier;
    }

    public float GetDamageMultiplier()
    {
        return currentDamageMultiplier;
    }

    public float GetRotationMultiplier()
    {
        return currentRotationMultiplier;
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }

    // This method is called to apply upgrades from the PlayerShipUpgradeManager
    public void ApplyUpgrade(string upgradeName, float multiplier)
    {
        switch (upgradeName)
        {
            case "Speed Boost":
                baseSpeed *= multiplier;
                currentSpeed = baseSpeed; // Update current speed immediately
                Debug.Log($"New Speed: {baseSpeed}"); // Log new speed
                break;

            case "Weapon Fire Rate":
                weaponMultiplier *= multiplier;
                currentWeaponMultiplier = weaponMultiplier; // Update fire rate immediately
                Debug.Log($"New Weapon Fire Rate: {weaponMultiplier}"); // Log new fire rate
                break;

            case "Damage Boost":
                damageMultiplier *= multiplier;
                currentDamageMultiplier = damageMultiplier; // Update damage immediately
                Debug.Log($"New Damage Multiplier: {damageMultiplier}"); // Log new damage multiplier
                break;

            case "Rotation Speed":
                rotationMultiplier *= multiplier;
                currentRotationMultiplier = rotationMultiplier; // Update rotation speed immediately
                Debug.Log($"New Rotation Speed: {rotationMultiplier}"); // Log new rotation speed
                break;

            default:
                Debug.LogWarning("Upgrade not recognized: " + upgradeName);
                break;
        }

        // After applying upgrade, update movement or fire rate behavior accordingly
        UpdateMovement();
    }

    // Optional: Update movement behavior (e.g., with speed or fire rate)
    void UpdateMovement()
    {
        // Code for movement, which depends on the currentSpeed
        Debug.Log($"Updated Speed: {currentSpeed}");
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(moveX, moveY).normalized * currentSpeed * Time.deltaTime;

        if (move != Vector2.zero)
            transform.position += (Vector3)move;
    }
}
