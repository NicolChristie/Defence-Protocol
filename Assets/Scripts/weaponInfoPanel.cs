using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class WeaponInfoPanel : MonoBehaviour
{
    public static WeaponInfoPanel Instance;
    [Header("UI References")]
    public GameObject panel; // Assign your panel once
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;

    [Header("Settings")]
    public string weaponTag = "Weapon"; // make sure all your weapons have this tag

    [Header("Targeting UI")]
    public TextMeshProUGUI targetingText;
    public UnityEngine.UI.Button leftButton;
    public UnityEngine.UI.Button rightButton;

    public Vector2 startPos = new Vector2(0, -800);
    public Vector2 endPos = new Vector2(0, 0);
    public float animationDuration = 1f;
    private Weaponprefab currentWeapon;
    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
        
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ShowWeaponInfoUnderMouse();
            }
        }
    }
    void ShowWeaponInfoUnderMouse()
{
    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

    if (hit.collider != null && hit.collider.CompareTag(weaponTag))
    {
        Weaponprefab weapon = hit.collider.GetComponent<Weaponprefab>();
        if (weapon != null)
        {
            // If the panel is already showing this weapon, do nothing
            if (panel.activeSelf && currentWeapon == weapon)
            {
                Debug.Log("Panel is already showing this weapon.");
                return;
            }

            Debug.Log($"Showing info for weapon: {weapon.name}");
            ShowInfo(weapon);
        }
    }
    else
    {
        HideInfo();
        Debug.Log("No weapon found under mouse cursor.");
    }
}

    
    void UpdateTargetingText(Weaponprefab weapon)
    {
        if (targetingText != null && weapon.targetOptions != null && weapon.targetOptions.Length > 0)
        {
            targetingText.text = $"<b>Target:</b> {weapon.targetOptions[0]}";
        }
    }
    void SwitchTargeting(Weaponprefab weapon, int direction)
    {
        if (weapon.targetOptions == null || weapon.targetOptions.Length == 0) return;

        // Rotate the array so that the first element changes
        int len = weapon.targetOptions.Length;
        string[] newOptions = new string[len];
        for (int i = 0; i < len; i++)
        {
            newOptions[i] = weapon.targetOptions[(i - direction + len) % len];
        }
        weapon.targetOptions = newOptions;

        // Update display
        UpdateTargetingText(weapon);
    }

    private Coroutine currentAnimation;

    void ShowInfo(Weaponprefab weapon)
    {
        if (panel == null) return;

        panel.SetActive(true);
        string cleanName = weapon.name.Replace("(Clone)", "").Trim();
        nameText.text = cleanName;

        string stats =
            $"<b>Damage:</b> {weapon.projectileDamage}\n" +
            $"<b>Fire Rate:</b> {weapon.fireRate:F2}\n" +
            $"<b>Range:</b> {weapon.originalRange:F1}\n";

        if (weapon.pierceCount > 0f)
            stats += $"<b>Pierce:</b> {weapon.pierceCount}\n";
        if (weapon.aoeRadius > 0f)
            stats += $"<b>AOE:</b> {weapon.aoeRadius} (x{weapon.aoeDamageMultiplier})\n";
        stats += $"<b>Kills:</b> {weapon.enemiesKilled}\n";

        statsText.text = stats;
        UpdateTargetingText(weapon);

        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(() => SwitchTargeting(weapon, -1));
        rightButton.onClick.AddListener(() => SwitchTargeting(weapon, 1));

        // Start open animation
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimatePanel(startPos, endPos));
        currentWeapon = weapon;
}

public void HideInfo()
{
    if (panel == null) return;

    currentWeapon = null; // clear the reference
    if (currentAnimation != null) StopCoroutine(currentAnimation);
    currentAnimation = StartCoroutine(ClosePanel());
}

IEnumerator AnimatePanel(Vector2 from, Vector2 to)
{
    float elapsed = 0f;
    while (elapsed < animationDuration)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(elapsed / animationDuration);
        panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(from, to, t);
        yield return null;
    }
    panel.GetComponent<RectTransform>().anchoredPosition = to;
}

IEnumerator ClosePanel()
{
    yield return AnimatePanel(endPos, startPos);
    panel.SetActive(false);
}

}
