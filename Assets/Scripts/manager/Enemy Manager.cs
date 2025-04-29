using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public GameObject ship; // Reference to the ship GameObject
    public ShipHealthBar shipHealthBar; // Reference to the health bar script
    public float speed = 1f;
    public int maxHP = 4;
    private int currentHP;
    private Rigidbody2D rb;
    public HealthBar healthBar;
    public int damage = 10; // Damage enemy deals when colliding with the ship
    public int coinAmount = 0;

    private Vector3 spawnPosition;
    public float stopDistance = 200f;
    public float destroyDistance = 300f;

    // Static list of all enemies in the game
    private static List<EnemyManager> allEnemies = new List<EnemyManager>();

    void Start()
{
    rb = GetComponent<Rigidbody2D>();
    spawnPosition = transform.position;
    currentHP = (maxHP * 4);

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

    allEnemies.Add(this);
}


    void OnDestroy()
    {
        // Remove this enemy from the static list when it is destroyed
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

    public void TakeDamage(int damage)
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
            Destroy(gameObject); // Calls OnDestroy and removes the enemy from the static list
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ship != null && collision.gameObject == ship) // Check if collided with the ship
        {
            if (shipHealthBar != null)
            {
                shipHealthBar.TakeDamage(damage); // Damage the ship's health bar
            }
            Destroy(gameObject); // Destroy enemy upon impact, also removes from allEnemies
        }
    }

    // Static method to get all active enemies
    public static List<EnemyManager> GetAllEnemies()
    {
        return new List<EnemyManager>(allEnemies); // Return a copy of the list to avoid modification
    }
}
