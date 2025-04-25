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
        Debug.Log("the current weapi multiplier is " + currentWeaponMultiplier + " the current damage multiplier is " + currentDamageMultiplier);
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

    void Update()
{
    float moveX = Input.GetAxisRaw("Horizontal");
    float moveY = Input.GetAxisRaw("Vertical");

    Vector2 move = new Vector2(moveX, moveY).normalized * currentSpeed * Time.deltaTime;

    if (move != Vector2.zero)
        Debug.Log("Moving: " + move);

    transform.position += (Vector3)move;
}

}
