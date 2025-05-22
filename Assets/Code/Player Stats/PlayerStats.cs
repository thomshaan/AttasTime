using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI xpText;

    [Header("Starting Values")]
    [SerializeField] private int startingCoins = 100;
    [SerializeField] private int startingXP = 50;

    [Header("Runtime Stats")]
    public int coins;
    public int xp;

    void Start()
    {
        coins = startingCoins;
        xp = startingXP;
        UpdateUI();
    }

    // New method for loading saved stats
    public void SetStats(int loadedCoins, int loadedXP)
    {
        coins = loadedCoins;
        xp = loadedXP;
        UpdateUI();
    }

    // Add Coins
    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log($"[PlayerStats] 💰 Coins added: +{amount} → Total: {coins}");
        UpdateUI();
    }

    // Spend Coins
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log($"[PlayerStats] 💸 Coins spent: -{amount} → Remaining: {coins}");
            UpdateUI();
            return true;
        }

        Debug.LogWarning("[PlayerStats] ❌ Not enough coins!");
        return false;
    }

    // Add XP
    public void AddXP(int amount)
    {
        xp += amount;
        Debug.Log($"[PlayerStats] ✨ XP gained: +{amount} → Total: {xp}");
        UpdateUI();
    }

    // Getters
    public int GetCoins() => coins;
    public int GetXP() => xp;

    // UI Sync
    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();

        if (xpText != null)
            xpText.text = xp.ToString();
    }
}
