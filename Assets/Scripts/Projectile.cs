using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private float range;
    private float aoeRadius;
    private float aoeDamageMultiplier;
    private int pierceCount;
    private int enemiesHit = 0;
    private float stopDistance;
    private Vector3 startPosition;
    private Vector3 direction;

    private GameObject target;
    private bool homing;
    private float homingSpeed;

    public float speed = 10f;

public void SetProperties(Transform weaponTransform, int dmg, float rng, float aoe, float aoeMultiplier, int pierce, float stopDist, bool homing, float homingSpeed, GameObject target, float deviationAmount = 0f)
{
    damage = dmg;
    range = rng;
    aoeRadius = aoe;
    aoeDamageMultiplier = aoeMultiplier;
    pierceCount = pierce;
    startPosition = transform.position;
    stopDistance = stopDist;  // Save the stopDistance
    this.homing = homing;
    this.homingSpeed = homingSpeed;
    this.target = target;

    // Homing projectile follows the target
    if (homing && target != null)
    {
        direction = (target.transform.position - transform.position).normalized;

        // Apply deviation to homing direction
        if (deviationAmount != 0f)
        {
            // Randomly rotate the direction vector within the deviation angle range
            float deviationAngle = Random.Range(-deviationAmount, deviationAmount);
            direction = Quaternion.Euler(0, 0, deviationAngle) * direction;
        }
    }
    else
    {
        // Non-homing projectiles inherit weapon rotation
        direction = transform.right.normalized;
    }
}
    void Update()
    {
        float distanceTraveled = Vector3.Distance(transform.position, startPosition);

        // Destroy if it exceeds range but hasn't hit stop distance
        if (distanceTraveled >= range * 1.2f && distanceTraveled < stopDistance)
        {
            Destroy(gameObject);
            return;
        }

        // Stop if it reaches stop distance
        if (distanceTraveled >= stopDistance)
        {
            return;
        }

        // Homing behavior: Adjust direction toward target
        if (homing && target != null)
        {
            Vector3 targetDirection = (target.transform.position - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, homingSpeed * Time.deltaTime).normalized;
        }

        // Move in the assigned direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyManager enemy = collision.GetComponent<EnemyManager>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Apply AoE damage if applicable
            if (aoeRadius > 0)
            {
                ApplyAOE(collision.transform.position);
            }

            enemiesHit++;
            if (enemiesHit > pierceCount)
            {
                Destroy(gameObject);
            }
        }else if (collision.CompareTag("Ship"))
        {
            Destroy(gameObject);
        }
    }

    void ApplyAOE(Vector3 explosionPoint)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(explosionPoint, aoeRadius);
        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy") && hit.transform.position != explosionPoint)
            {
                EnemyManager enemy = hit.GetComponent<EnemyManager>();
                if (enemy != null)
                {
                    int aoeDamage = Mathf.RoundToInt(damage * aoeDamageMultiplier);
                    enemy.TakeDamage(aoeDamage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (aoeRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
    }
}
