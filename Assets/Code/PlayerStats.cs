using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI xpText;

    [Header("Values")]
    [SerializeField] private int startingCoins = 100;
    [SerializeField] private int startingXP = 0;

    private int coins;
    private int xp;

    void Start()
    {
        coins = startingCoins;
        xp = startingXP;
        UpdateUI();
    }
    // Coin Method
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }
    // Spend Coins
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            return true;
        }

        Debug.Log("Not enough coins!");
        return false;
    }

    // XP Addition
    public void AddXP(int amount)
    {
        xp += amount;
        UpdateUI();
    }

    public int GetCoins() => coins;
    public int GetXP() => xp;


    // Update UI 
    private void UpdateUI()
    {
        if (coinText != null) coinText.text = coins.ToString();
        if (xpText != null) xpText.text = xp.ToString();
    }
}
