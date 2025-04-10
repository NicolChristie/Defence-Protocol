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
    public int level = 1;


    private SpriteRenderer spriteRenderer;

    private float baseFireRate;
    private int baseDamage;
    private float boostedFireRate;
    private int boostedDamage;

    private float nextFireTime = 0f;
    private GameObject currentTarget;
    private float baseRotation;
    private bool isRotated = false;
    public GameObject originalPrefab; // For prefab comparison in merge logic


    void Start()
    {
        baseFireRate = fireRate;
        baseDamage = projectileDamage;
        boostedFireRate = fireRate;
        boostedDamage = projectileDamage;

        baseRotation = transform.eulerAngles.z;

        spriteRenderer = GetComponent<SpriteRenderer>();
    
        if (originalPrefab == null)
    originalPrefab = gameObject; // fallback if not manually assigned

    }

    public void UpgradeWeapon()
    {

        fireRate *= 1.25f;
        projectileDamage += 1;

        UpdateStats();
    }

    public void ApplyBoost(float speedMultiplier, float damageMultiplier, bool isFirstTime)
    {
        if (isFirstTime)
        {
            boostedFireRate = baseFireRate * speedMultiplier * speedMultiplier;
            boostedDamage = Mathf.RoundToInt(baseDamage * damageMultiplier * damageMultiplier);
        }
        else
        {
            boostedFireRate *= speedMultiplier;
            boostedDamage = Mathf.RoundToInt(boostedDamage * damageMultiplier);
        }

        UpdateStats();
    }

    public void ResetToBaseStats()
    {
        boostedFireRate = baseFireRate;
        boostedDamage = baseDamage;
        UpdateStats();
    }

    public void RemoveBoost(float speedMultiplier, float damageMultiplier)
    {
        boostedFireRate /= speedMultiplier;
        boostedDamage = Mathf.RoundToInt(boostedDamage / damageMultiplier);

        UpdateStats();
    }

    private void UpdateStats()
    {
        fireRate = boostedFireRate;
        projectileDamage = boostedDamage;
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

        if (currentTarget != null)
        {
            Debug.DrawLine(transform.position, currentTarget.transform.position, Color.red, 0.1f);
        }
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
}
