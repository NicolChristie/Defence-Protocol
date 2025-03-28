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
    public float deviation = 0f; // Maximum deviation angle in degrees
    public float rotationSpeed = 180f;

    public bool homing = false; // Enables homing projectiles
    public float homingSpeed = 5f; // Speed at which projectiles home in

    private float nextFireTime = 0f;
    private bool isRotated = false;
    private GameObject currentTarget;
    private float baseRotation;
    private Vector3 initialForwardDirection; // The weapon's original forward direction
    private float stopDistance = 100f; // Distance at which projectiles stop

    void Start()
    {
        initialForwardDirection = transform.right; // Store the original forward direction
        baseRotation = transform.eulerAngles.z;  // Save initial weapon rotation
    }

    public void UpdateStats(float speedMultiplier, float damageMultiplier, float rangeMultiplier, float aoeMultiplier, int pierceIncrease)
    {
        fireRate *= speedMultiplier;
        projectileDamage = Mathf.RoundToInt(projectileDamage * damageMultiplier);
        range *= rangeMultiplier;
        aoeRadius *= aoeMultiplier;
        pierceCount += pierceIncrease;
    }

    void Update()
    {
        UpdateTarget();
        RotateTowardsTarget();

        if (isRotated && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + (1 / fireRate);
        }
    }

    // Update the target for the weapon based on line of sight and range
    public void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 weaponForward = Quaternion.Euler(0, 0, baseRotation) * Vector3.right; // Weapon’s original forward direction

        List<GameObject> validEnemies = enemies
        .Where(enemy =>
        {
            Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.SignedAngle(weaponForward, toEnemy, Vector3.forward);
            
            // Ensure the enemy is within the allowed rotation range
            return angleToEnemy >= -180f && angleToEnemy <= 180f;
        })
        .Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) <= range) // Apply range check here
        .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)) // Prioritize nearest within range
        .ToList();

        // Filter valid enemies further based on line of sight
        validEnemies = validEnemies.Where(enemy => IsLineOfSightClear(enemy)).ToList();

        currentTarget = validEnemies.FirstOrDefault(); // Get the closest valid target

        // Debugging: Draw the LoS to the target (if any)
        if (currentTarget != null)
        {
            Debug.DrawLine(transform.position, currentTarget.transform.position, Color.red, 0.1f);
        }
    }

    // Line of sight check with raycasting
    bool IsLineOfSightClear(GameObject target)
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);

        // Debugging: Draw the ray to show the path
        Debug.DrawLine(transform.position, hit.point, Color.green, 0.1f);  // If hit something
        Debug.DrawLine(transform.position, target.transform.position, Color.red, 0.1f);  // Always show target line

        // If we hit something and it's NOT the target or an enemy, LoS is blocked
        if (hit.collider != null && !hit.collider.CompareTag("Enemy") && hit.collider.gameObject != target)
        {
            return false; // Line of sight is blocked
        }

        return true; // Line of sight is clear
    }

    // Smoothly rotate the weapon towards the target
    void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 toTarget = (currentTarget.transform.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

        // Clamp the target angle within the allowed rotation range (360 degrees)
        float clampedAngle = Mathf.Clamp(targetAngle, -180f, 180f);

        // Smoothly rotate the weapon towards the target, but leave the node rotation unchanged
        float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, clampedAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle); // Rotate only the weapon, not the node

        // Check if rotation is close enough to be considered "locked"
        isRotated = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, clampedAngle)) < 1f;
    }

    // Fire the projectile towards the current target
    void FireProjectile()
    {
        if (currentTarget == null) return;

        // Get the weapon's rotation
        Quaternion weaponRotation = firePoint.rotation;

        // Apply deviation (randomly within the deviation angle range)
        float deviationAmount = Random.Range(-deviation, deviation);
        Quaternion deviationRotation = Quaternion.Euler(0, 0, deviationAmount);

        // Final rotation with deviation applied
        Quaternion finalRotation = weaponRotation * deviationRotation;

        // Instantiate the projectile with the final rotation
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, finalRotation);
        Projectile newProjectile = projectileInstance.GetComponent<Projectile>();

        // Set projectile properties (pass homing parameter as needed)
        newProjectile.SetProperties(firePoint, projectileDamage, range, aoeRadius, aoeDamageMultiplier, pierceCount, 
                                    stopDistance, homing, homingSpeed, currentTarget, deviation);
    }

    // Ensures projectiles rotate based on their movement direction
    private System.Collections.IEnumerator AdjustProjectileRotation(Projectile projectile)
    {
        yield return new WaitForFixedUpdate(); // Wait until projectile starts moving

        while (projectile != null)
        {
            Vector3 moveDirection = projectile.transform.right; // Assume projectile moves along its local right axis
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null; // Continue updating each frame
        }
    }
}
