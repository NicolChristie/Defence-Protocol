using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public float baseSpeed = 5f;          // Base speed of the player
    public float weaponMultiplier = 2f;   // Weapon multiplier (affects fire rate)
    public float damageMultiplier = 2f;   // Damage multiplier (affects projectile damage)

    private float currentSpeed;           // Current player speed after applying boosts
    private float currentWeaponMultiplier;
    private float currentDamageMultiplier;

    void Start()
    {
        // Initialize speed and multipliers
        currentSpeed = baseSpeed;
        currentWeaponMultiplier = weaponMultiplier;
        currentDamageMultiplier = damageMultiplier;
        Debug.Log("the current weapi multiplier is " + currentWeaponMultiplier + " the current damage multiplier is " + currentDamageMultiplier);
    }


    // Called when leaving the weapon node to reset multipliers
    public void ResetWeaponBoost()
    {
        currentWeaponMultiplier = weaponMultiplier;
        currentDamageMultiplier = damageMultiplier;
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

    public float GetSpeed()
    {
        return currentSpeed;
    }

    void Update()
    {
        // Player movement using the current speed
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(moveX, moveY).normalized * currentSpeed * Time.deltaTime;

        transform.position += (Vector3)move;
    }
}
