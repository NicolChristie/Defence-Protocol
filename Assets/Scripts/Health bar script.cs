using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar; // Assign the Health Bar GameObject in the Inspector
    private Vector3 fullScale; // Static full scale (no need to calculate dynamically)

    void Start()
    {
        if (healthBar != null)
        {
            // Set the fullScale to a fixed value (like 1) for the x-axis, with a constant y scale of 0.1
            fullScale = new Vector3(1f, 0.1f, healthBar.transform.localScale.z);

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
            // Calculate the health percentage as a float
            float healthPercent = Mathf.Clamp01((float)currentHP / (float)maxHP);

            // Check if the parent scale is affecting the health bar
            Vector3 parentScale = healthBar.transform.parent ? healthBar.transform.parent.localScale : Vector3.one;


            // If parent scale is not (1, 1, 1), consider applying it to the local scale calculation
            if (parentScale != Vector3.one)
            {
                // Adjust the localScale based on the parent's scale
                healthBar.transform.localScale = new Vector3(fullScale.x * healthPercent / parentScale.x, fullScale.y, fullScale.z);
            }
            else
            {
                // Just set the scale normally
                healthBar.transform.localScale = new Vector3(fullScale.x * healthPercent, fullScale.y, fullScale.z);
            }
        }
        else
        {
            Debug.LogError("HealthBar: healthBar GameObject is not assigned!");
        }
    }
}
