using System.Collections;
using UnityEngine;

public class AOEindicator : MonoBehaviour
{
    public float aoeRadius;  // Radius of the AOE
    public float expansionDuration = 0.5f;  // Time for expansion
    public float shrinkDuration = 1.0f;  // Time for shrinking and fading

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on AOE Indicator.");
            return;
        }

        // Set initial size to zero (or a small value)
        transform.localScale = Vector3.zero;
        
        // Start the expanding and shrinking effect
        StartCoroutine(ExpandAndShrink());
    }

    private IEnumerator ExpandAndShrink()
    {
        // Expansion Phase
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < expansionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / expansionDuration);

            // Gradually expand the AOE indicator to the full size
            float scaleFactor = Mathf.Lerp(originalScale.x, aoeRadius * 2f, t);
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            yield return null;
        }

        // Reset elapsed time for shrinking phase
        elapsed = 0f;

        // Shrink and Fade Phase
        Color originalColor = sr.color;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);

            // Shrink the AOE indicator
            float scaleFactor = Mathf.Lerp(aoeRadius * 2f, 0f, t);
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            // Fade out the color
            float alpha = Mathf.Lerp(originalColor.a, 0f, t);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // Final cleanup, ensure it's completely gone
        transform.localScale = Vector3.zero;
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Destroy the AOE indicator object after the animation
        Destroy(gameObject);
    }
}
