using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Weaponprefab : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public int projectileDamage = 2;
    public float range = 10f;
    public float aoeRadius = 0f;
    public float aoeDamageMultiplier = 0.5f;
    public int pierceCount = 0;
    public float deviation = 0f;
    public float rotationSpeed = 180f;
    private float stopDistance = 100f;
    public bool homing = false;
    public float homingSpeed = 5f;
    public bool wasPurchased = false;

    private SpriteRenderer spriteRenderer;

    private float baseFireRate;
    private int baseDamage;
    private float baseRotation;
    private float boostedFireRate;
    private int boostedDamage;
    private float boostedRotation;
    public float originalRange;


    private float nextFireTime = 0f;
    private GameObject currentTarget;
    private bool isRotated = false;
    public GameObject originalPrefab; 
    

 void Start()
{
    baseFireRate = fireRate;
    baseDamage = projectileDamage;
    baseRotation = rotationSpeed;
    boostedFireRate = fireRate;
    boostedDamage = projectileDamage;
    boostedRotation = rotationSpeed;

    spriteRenderer = GetComponent<SpriteRenderer>();

   if (originalRange <= 0f)
{
    originalRange = range;
    Debug.Log($"[Init] originalRange set to {originalRange}");
}
else
{
    Debug.Log($"[Init] originalRange already set to {originalRange}, not overridden");
}
    if (originalPrefab == null)
        originalPrefab = gameObject; 
}


    public void InitialiseBaseStats()
    {
        baseFireRate = fireRate;
        baseDamage = projectileDamage;
        baseRotation = rotationSpeed;

        boostedFireRate = fireRate;
        boostedDamage = projectileDamage;
        boostedRotation = rotationSpeed;
    }

    public void ApplyBoost(float speedMultiplier, float damageMultiplier,float rotationMultiplier, bool isFirstTime)
    {
        if (isFirstTime)
        {
            boostedFireRate = baseFireRate * speedMultiplier * speedMultiplier;
            boostedDamage = Mathf.RoundToInt(baseDamage * damageMultiplier * damageMultiplier);
            boostedRotation = baseRotation * rotationMultiplier * rotationMultiplier;
        }
        else
        {
            boostedFireRate *= speedMultiplier;
            boostedDamage = Mathf.RoundToInt(boostedDamage * damageMultiplier);
            boostedRotation *= rotationMultiplier;
        }

        UpdateStats();
    }

    public void ResetToBaseStats()
    {
        boostedFireRate = baseFireRate;
        boostedDamage = baseDamage;
        boostedRotation = baseRotation;
        UpdateStats();
    }

    public void RemoveBoost(float speedMultiplier, float damageMultiplier,float rotationMultiplier)
    {
        boostedFireRate /= speedMultiplier;
        boostedDamage = Mathf.RoundToInt(boostedDamage / damageMultiplier);
        boostedRotation /= rotationMultiplier;
        UpdateStats();
    }

    private void UpdateStats()
    {
        fireRate = boostedFireRate;
        projectileDamage = boostedDamage;
        rotationSpeed = boostedRotation;
        Debug.Log($"Updated stats: Fire Rate: {fireRate}, Damage: {projectileDamage}");
    }

    void Update()
    {
        UpdateTarget();
        RotateTowardsTarget();

        if (Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + (1 / fireRate);
        }
    }

    public void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 weaponForward = Quaternion.Euler(0, 0, baseRotation) * Vector3.right;

        List<GameObject> validEnemies = enemies
            .Where(enemy =>
            {
                Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
                float angleToEnemy = Vector3.SignedAngle(weaponForward, toEnemy, Vector3.forward);
                return angleToEnemy >= -180f && angleToEnemy <= 180f;
            })
            .Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) <= range)
            .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .ToList();

        validEnemies = validEnemies.Where(enemy => IsLineOfSightClear(enemy)).ToList();
        currentTarget = validEnemies.FirstOrDefault();
    }

    bool IsLineOfSightClear(GameObject target)
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);

        if (hit.collider != null && !hit.collider.CompareTag("Enemy") && hit.collider.gameObject != target)
        {
            return false;
        }
        return true;
    }

    void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 toTarget = (currentTarget.transform.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(targetAngle, -180f, 180f);
        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, clampedAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
        isRotated = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, clampedAngle)) < 1f;
    }

    void FireProjectile()
    {
        if (currentTarget == null) return;

        Quaternion weaponRotation = firePoint.rotation;
        float deviationAmount = Random.Range(-deviation, deviation);
        Quaternion deviationRotation = Quaternion.Euler(0, 0, deviationAmount);
        Quaternion finalRotation = weaponRotation * deviationRotation;

        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, finalRotation);
        Projectile newProjectile = projectileInstance.GetComponent<Projectile>();

        newProjectile.SetProperties(firePoint, projectileDamage, range, aoeRadius, aoeDamageMultiplier, pierceCount,
            stopDistance, homing, homingSpeed, currentTarget, deviation);
    }

    public bool getPurchased()
    {
        Debug.LogWarning("checking if the weapon was purchased and that is" + wasPurchased);
        return wasPurchased;
    }

    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, range);
}
    public void CalibrateRange()
{
    GameObject[] worldEdgeObjects = GameObject.FindGameObjectsWithTag("worldEdge");

    if (worldEdgeObjects.Length == 0)
    {
        Debug.LogWarning("No world edges found for range calibration.");
        return;
    }

    float minDistToEdge = float.MaxValue;
    Debug.Log($"[Calibration] Found {worldEdgeObjects.Length} world edge objects.");

    foreach (var edgeObject in worldEdgeObjects)
    {
        Debug.Log($"[Calibration] Checking world edge object: {edgeObject.name}.");
        Collider2D edgeCollider = edgeObject.GetComponent<Collider2D>();
        if (edgeCollider != null)
        {
            float distanceToEdge = Vector2.Distance(transform.position, edgeCollider.transform.position);
            if (distanceToEdge < minDistToEdge)
            {
                minDistToEdge = distanceToEdge;
                Debug.Log($"[Calibration] Closest edge found: {edgeObject.name} at distance {minDistToEdge}.");
            }
        }
    }

    if (minDistToEdge <= 0.01f)
{
    Debug.LogWarning("Weapon is too close to or outside world edges.");
    return;
}

range = (originalRange / 10f) * minDistToEdge;
Debug.Log($"[Calibration] Weapon range updated to: {range} (originalRange: {originalRange}, closest edge: {minDistToEdge})");

        
}
    public void ManualInit()
{
    Debug.Log("Manual Init called for weapon prefab.");
    if (spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();

    nextFireTime = Time.time + (1f / fireRate);
    InitialiseBaseStats();
    ResetToBaseStats();
    CalibrateRange();
}


}
