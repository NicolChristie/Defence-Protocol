using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;
    private Vector3 fullScale;

    void Start()
    {
        if (healthBar != null)
        {
            fullScale = new Vector3(0.25f, 0.02f, healthBar.transform.localScale.z);

            healthBar.transform.localScale = fullScale;

        }
        else
        {
            Debug.LogError("HealthBar: healthBar GameObject is not assigned!");
        }
    }

    public void UpdateHealthBar(int currentHP, int maxHP)
    {
        if (healthBar != null)
        {
            float healthPercent = Mathf.Clamp01((float)currentHP / (float)maxHP);

            Vector3 parentScale = healthBar.transform.parent ? healthBar.transform.parent.localScale : Vector3.one;


            if (parentScale != Vector3.one)
            {
                healthBar.transform.localScale = new Vector3(fullScale.x * healthPercent / parentScale.x, fullScale.y, fullScale.z);
            }
            else
            {
                healthBar.transform.localScale = new Vector3(fullScale.x * healthPercent, fullScale.y, fullScale.z);
            }
        }
        else
        {
            Debug.LogError("HealthBar: healthBar GameObject is not assigned!");
        }
    }
}
