using UnityEngine;
using TMPro;  

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;  
    private int coins = 0; 
    public TextMeshProUGUI coinText;  

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        coinText.gameObject.SetActive(true);  
        UpdateCoinDisplay();
        setCoins(5); 
    }

    public void setCoins(int amount)
    {
        coins = amount;  
        UpdateCoinDisplay();  
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinDisplay();  
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinDisplay(); 
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    private void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins; 
        }
        else
        {
            Debug.LogError("CoinText UI element is not assigned in CoinManager!");
        }
    }
}
