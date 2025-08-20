using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public GameObject ship; 
    public ShipHealthBar shipHealthBar;
    public float speed = 1f;
    public int maxHP = 4;
    public int currentHP;
    private Rigidbody2D rb;
    public HealthBar healthBar;
    public int damage = 10; 
    public int coinAmount = 0;

    private Vector3 spawnPosition;
    public float stopDistance = 200f;
    public float destroyDistance = 300f;

    private static List<EnemyManager> allEnemies = new List<EnemyManager>();
    public DamageFlashEffect damageFlashEffect; 


    void Start()
{
    rb = GetComponent<Rigidbody2D>();
    spawnPosition = transform.position;
    currentHP = (maxHP * 8);

    if (player == null)
        player = GameObject.FindWithTag("Player");

    if (ship == null)
        ship = GameObject.FindWithTag("Ship");

    if (shipHealthBar == null)
        shipHealthBar = Object.FindFirstObjectByType<ShipHealthBar>();

    if (healthBar != null)
        healthBar.UpdateHealthBar(currentHP, maxHP);

    if (player == null)
        Debug.LogError("Player reference is not assigned in EnemyManager!");
    if (ship == null)
        Debug.LogError("Ship reference is not assigned in EnemyManager!");
    if (shipHealthBar == null)
        Debug.LogError("ShipHealthBar script is not assigned in EnemyManager!");
    
    if (damageFlashEffect == null)
    damageFlashEffect = Object.FindFirstObjectByType<DamageFlashEffect>();


    allEnemies.Add(this);
}


    void OnDestroy()
    {
        allEnemies.Remove(this);
    }

    void FixedUpdate()
    {
        float distanceFromSpawn = Vector3.Distance(spawnPosition, transform.position);

        if (distanceFromSpawn >= destroyDistance)
        {
            Destroy(gameObject);
            return;
        }

        if (distanceFromSpawn >= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * (speed*0.5f);
        }
    }

    public void TakeDamage(int damage,Weaponprefab sourceWeapon = null)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHP, maxHP);
        }

        if (currentHP <= 0)
        {
            CoinManager.Instance.AddCoins(coinAmount);
            SoundFxManager.Instance.PlaySound("enemyDeath", transform, 1f);
            Destroy(gameObject); 
            Die(sourceWeapon);
        }
    }
    void Die(Weaponprefab sourceWeapon)
{
    if (sourceWeapon != null)
        sourceWeapon.RegisterKill();

    Destroy(gameObject);
}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ship != null && collision.gameObject == ship)
        {
            if (shipHealthBar != null)
            {
                shipHealthBar.TakeDamage(damage);
                if (damageFlashEffect != null)
                    damageFlashEffect.TriggerFlash();
            }
            SoundFxManager.Instance.PlaySound("enemyAttack", transform, 1f);
            Destroy(gameObject);
        }
    }

    public static List<EnemyManager> GetAllEnemies()
    {
        return new List<EnemyManager>(allEnemies); 
    }
}
