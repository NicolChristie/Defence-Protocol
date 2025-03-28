using UnityEngine;

public class ShipHealthBar : MonoBehaviour
{
    public GameObject healthBar; // Assign the Health Bar GameObject in the Inspector
    public int maxHP = 100;
    private int currentHP;
    private Vector3 fullScale = new Vector3(20f, 0.2f, 1f); // Default size of 20 by 0.5

    void Start()
    {
        currentHP = maxHP;
        if (healthBar != null)
        {
            healthBar.transform.localScale = fullScale;
        }
        else
        {
            Debug.LogError("ShipHealthBar: healthBar GameObject is not assigned!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHealthBar();
        Debug.Log("Ship took " + damage + " damage. Current HP: " + currentHP);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercent = Mathf.Clamp01((float)currentHP / maxHP);
            healthBar.transform.localScale = new Vector3(fullScale.x * healthPercent, fullScale.y, fullScale.z);
        }
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
}
