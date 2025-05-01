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
    public GameObject aoeIndicatorPrefab; 

    public float speed = 10f;

    public void SetProperties(Transform weaponTransform, int dmg, float rng, float aoe, float aoeMultiplier, int pierce, float stopDist, bool homing, float homingSpeed, GameObject target, float deviationAmount = 0f)
    {
        damage = dmg;
        range = rng;
        aoeRadius = aoe;
        aoeDamageMultiplier = aoeMultiplier;
        pierceCount = pierce;
        startPosition = transform.position;
        stopDistance = stopDist;  
        this.homing = homing;
        this.homingSpeed = homingSpeed;
        this.target = target;


        if (homing && target != null)
        {
            direction = (target.transform.position - transform.position).normalized;

            if (deviationAmount != 0f)
            {

                float deviationAngle = Random.Range(-deviationAmount, deviationAmount);
                direction = Quaternion.Euler(0, 0, deviationAngle) * direction;
            }
        }
        else
        {
            direction = transform.right.normalized;
        }
    }

    void Update()
    {
        float distanceTraveled = Vector3.Distance(transform.position, startPosition);

        if (distanceTraveled >= range * 1.2f && distanceTraveled < stopDistance)
        {
            Destroy(gameObject);
            return;
        }

        if (distanceTraveled >= stopDistance)
        {
            return;
        }

        if (homing && target != null)
        {
            Vector3 targetDirection = (target.transform.position - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, homingSpeed * Time.deltaTime).normalized;
        }

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

            if (aoeRadius > 0)
            {
                ApplyAOE(collision.transform.position);
            }

            enemiesHit++;
            if (enemiesHit > pierceCount)
            {
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("Ship"))
        {
            Destroy(gameObject);
        }
    }

    void ApplyAOE(Vector3 explosionPoint)
    {
        Debug.Log("Applying AOE damage...");

        ShowAOEIndicator(explosionPoint);

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

    

   void ShowAOEIndicator(Vector3 position)
{
    GameObject aoeIndicator = Instantiate(aoeIndicatorPrefab, position, Quaternion.identity);

    AOEindicator indicatorScript = aoeIndicator.GetComponent<AOEindicator>();
    if (indicatorScript != null)
    {
        indicatorScript.aoeRadius = aoeRadius;  
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
