using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public float baseSpeed = 5f;         
    public float weaponMultiplier = 2f;   
    public float damageMultiplier = 2f;   
    public float rotationMultiplier = 2f; 

    private float currentSpeed;           
    private float currentWeaponMultiplier;
    private float currentDamageMultiplier;
    private float currentRotationMultiplier;

    void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            Destroy(gameObject); 
            return;
        }
        Debug.Log("PlayerManager instance created: " + gameObject.name);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

        currentSpeed = baseSpeed;
        currentWeaponMultiplier = weaponMultiplier;
        currentDamageMultiplier = damageMultiplier;
        currentRotationMultiplier = rotationMultiplier;
        Debug.Log("the current weapon multiplier is " + currentWeaponMultiplier + " the current damage multiplier is " + currentDamageMultiplier);
    }

    public void ResetWeaponBoost()
    {
        currentWeaponMultiplier = weaponMultiplier;
        currentDamageMultiplier = damageMultiplier;
        currentRotationMultiplier = rotationMultiplier;
        currentSpeed = baseSpeed; 
    }

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
    private bool canMove = true; // Add this at the top

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }

    public void ApplyUpgrade(string upgradeName, float multiplier)
    {
        switch (upgradeName)
        {
            case "Speed Boost":
                baseSpeed *= multiplier;
                currentSpeed = baseSpeed;
                Debug.Log($"New Speed: {baseSpeed}"); 
                break;

            case "Weapon Fire Rate":
                weaponMultiplier *= multiplier;
                currentWeaponMultiplier = weaponMultiplier; 
                Debug.Log($"New Weapon Fire Rate: {weaponMultiplier}"); 
                break;

            case "Damage Boost":
                damageMultiplier *= multiplier;
                currentDamageMultiplier = damageMultiplier; 
                Debug.Log($"New Damage Multiplier: {damageMultiplier}"); 
                break;

            case "Rotation Speed":
                rotationMultiplier *= multiplier;
                currentRotationMultiplier = rotationMultiplier; 
                Debug.Log($"New Rotation Speed: {rotationMultiplier}"); 
                break;

            default:
                Debug.LogWarning("Upgrade not recognized: " + upgradeName);
                break;
        }

        UpdateMovement();
    }

    void UpdateMovement()
    {
        Debug.Log($"Updated Speed: {currentSpeed}");
    }

    void Update()
    {
        if (!canMove) return; 
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(moveX, moveY).normalized * currentSpeed * Time.deltaTime;

        if (move != Vector2.zero)
            transform.position += (Vector3)move;
    }
}
