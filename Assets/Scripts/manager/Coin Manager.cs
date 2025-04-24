using UnityEngine;
using TMPro;  // Import the TextMeshPro namespace

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;  // Singleton instance
    private int coins = 0;  // Stores the current coin count
    public TextMeshProUGUI coinText;  // Reference to the TMP Text that will display coins

    void Awake()
    {
        // Set up Singleton pattern
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
        UpdateCoinDisplay();  // Initialize the coin text when the game starts
        setCoins(5); 
    }

    public void setCoins(int amount)
    {
        coins = amount;  // Set the coin count directly
        UpdateCoinDisplay();  // Update the UI to reflect the new coin count
    }

    // Method to add coins
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinDisplay();  // Update the UI whenever coins are added
    }

    // Method to subtract coins (for purchases)
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinDisplay();  // Update the UI whenever coins are spent
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }

    // Method to get current coin count
    public int GetCoins()
    {
        return coins;
    }

    // Method to update the coin text UI
    private void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins;  // Update the UI text to show the current coin count
        }
        else
        {
            Debug.LogError("CoinText UI element is not assigned in CoinManager!");
        }
    }
}
