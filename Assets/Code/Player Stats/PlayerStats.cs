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
    private int xp;

    // Properti publik untuk akses aman coins dan xp
    public int Coins
    {
        get => coins;
        private set
        {
            coins = value;
            UpdateCoinUI();
        }
    }

    public int XP
    {
        get => xp;
        private set
        {
            xp = value;
            UpdateXPUI();
        }
    }

    // Public properties untuk UI agar bisa di-assign dari luar
    public TextMeshProUGUI CoinText
    {
        get => coinText;
        set
        {
            coinText = value;
            UpdateCoinUI();
        }
    }

    public TextMeshProUGUI XPText
    {
        get => xpText;
        set
        {
            xpText = value;
            UpdateXPUI();
        }
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Set nilai awal jika belum ada data (misal di new game)
        if (Coins == 0 && XP == 0)
        {
            Coins = startingCoins;
            XP = startingXP;
        }
        else
        {
            UpdateUI();
        }
    }

    // Fungsi untuk set data yang di-load dari save
    public void SetStats(int loadedCoins, int loadedXP)
    {
        Coins = loadedCoins;
        XP = loadedXP;
        Debug.Log($"[PlayerStats] SetStats called: coins={Coins}, xp={XP}");
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        Debug.Log($"[PlayerStats] 💰 Coins added: +{amount} → Total: {Coins}");
    }

    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            Debug.Log($"[PlayerStats] 💸 Coins spent: -{amount} → Remaining: {Coins}");
            return true;
        }

        Debug.LogWarning("[PlayerStats] ❌ Not enough coins!");
        return false;
    }

    public void AddXP(int amount)
    {
        XP += amount;
        Debug.Log($"[PlayerStats] ✨ XP gained: +{amount} → Total: {XP}");
    }

    // Update semua UI
    private void UpdateUI()
    {
        UpdateCoinUI();
        UpdateXPUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = Coins.ToString();
        }
    }

    private void UpdateXPUI()
    {
        if (xpText != null)
        {
            xpText.text = XP.ToString();
        }
    }
}
