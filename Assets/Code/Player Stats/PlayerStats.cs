using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI xpText;

    [Header("Starting Values")]
    [SerializeField] private int startingCoins = 100;
    [SerializeField] private int startingXP = 50;

    [Header("Runtime Stats")]
    public int coins;
    public int xp;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Pastikan modal awal muncul saat game mulai
        if (coins == 0 && xp == 0)
        {
            coins = startingCoins;
            xp = startingXP;
        }
        UpdateUI();
    }

    // Dipanggil saat load data save
    public void SetStats(int loadedCoins, int loadedXP)
    {
        coins = loadedCoins;
        xp = loadedXP;
        Debug.Log($"[PlayerStats] SetStats called: coins={coins}, xp={xp}");
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

    // Update UI Text
    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
            Debug.Log($"[PlayerStats] Updated coin UI: {coinText.text}");
        }
        else
        {
            Debug.LogWarning("[PlayerStats] coinText UI reference is missing!");
        }

        if (xpText != null)
        {
            xpText.text = xp.ToString();
            Debug.Log($"[PlayerStats] Updated XP UI: {xpText.text}");
        }
        else
        {
            Debug.LogWarning("[PlayerStats] xpText UI reference is missing!");
        }
    }
}
