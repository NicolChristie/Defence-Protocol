using System.Collections;
using UnityEngine;

public class AOEindicator : MonoBehaviour
{
    public float aoeRadius;
    public float expansionDuration = 0.5f;
    public float shrinkDuration = 1.0f;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on AOE Indicator.");
            return;
        }

        transform.localScale = Vector3.zero;
        StartCoroutine(ExpandAndShrink());
    }

    private IEnumerator ExpandAndShrink()
    {
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < expansionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / expansionDuration);
            float scaleFactor = Mathf.Lerp(originalScale.x, aoeRadius * 2f, t);
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            yield return null;
        }

        elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);
            float scaleFactor = Mathf.Lerp(aoeRadius * 2f, 0f, t);
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            float alpha = Mathf.Lerp(originalColor.a, 0f, t);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }
}
