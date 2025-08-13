using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class unlockText : MonoBehaviour
{
    public GameObject unlockPanel;
    public TextMeshProUGUI unlockTextUI;
    public RawImage unlockImageUI;

    public Button ExitButton;

    private void Start()
    {
        displayUnlockText();
    }

    public void displayUnlockText()
    {
        Scene mainScene = SceneManager.GetSceneByName("Main Scene");
        if (mainScene.isLoaded)
        {
            GameObject[] rootObjects = mainScene.GetRootGameObjects();

            int finishedAmount = -1;
            ShopManager shopManager = null;

            // --- Find LevelHandler and ShopManager ---
            foreach (GameObject go in rootObjects)
            {
                LevelHandler levelHandler = go.GetComponentInChildren<LevelHandler>();
                if (levelHandler != null)
                {
                    finishedAmount = levelHandler.GetFinishedAmount();
                }

                ShopManager sm = go.GetComponentInChildren<ShopManager>();
                if (sm != null)
                    shopManager = sm;
            }

            // --- If both were found, find matching items ---
            if (finishedAmount >= 0 && shopManager != null)
            {
                List<ShopItem> matchingItems = shopManager.availableItems
                    .FindAll(item => item.unlockLevel == finishedAmount);
                if (matchingItems.Count > 0)
                {
                    ShopItem firstItem = matchingItems[0];

                    // If finishedAmount == 1, override the name to "Upgrades"
                    string displayName = finishedAmount == 1 ? "Upgrades" : firstItem.itemName;

                    if (unlockTextUI != null)
                        unlockTextUI.text = $"{displayName}";

                    if (unlockImageUI != null && firstItem.itemImage != null)
                        unlockImageUI.texture = firstItem.itemImage;
                }
                else
                {
                    if (unlockTextUI != null)
                        unlockTextUI.text = $"No items unlock at level {finishedAmount}";
                }
            }
            else
            {
                Debug.LogWarning("Could not find LevelHandler or ShopManager in the scene.");
            }
        }
    }
    public void ExitScreen()
    {
        Debug.Log("Exiting unlock screen...");
        unlockPanel.SetActive(false);
    }
}
