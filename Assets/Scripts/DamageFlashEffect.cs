using UnityEngine;
using UnityEngine.UI; // Required for Image

public class DamageFlashEffect : MonoBehaviour
{
    public Image flashPanel; // The red panel you want to flash
    public float flashAlpha = 0.4f;
    public float fadeDuration = 0.3f;

    private Coroutine flashRoutine;
void Start()
{
    if (flashPanel != null)
    {
        Color c = flashPanel.color;
        c.a = 0f;
        flashPanel.color = c;
    }
}
    public void TriggerFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
    }

    private System.Collections.IEnumerator Flash()
    {
        Color color = flashPanel.color;
        color.a = flashAlpha;
        flashPanel.color = color;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(flashAlpha, 0f, t / fadeDuration);
            flashPanel.color = color;
            yield return null;
        }

        // Ensure it's fully transparent at the end
        color.a = 0f;
        flashPanel.color = color;
    }
}
