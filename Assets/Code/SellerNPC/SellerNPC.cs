using UnityEngine;

public class SellerNPC : MonoBehaviour, IInteractable
{
    private Item item;
    private int stock;
    private Inventory inventory;
    public PlayerStats playerStats;
    private SellerIconUI iconUI;
    private GameObject iconGO;
    public DialogData dialogData;

    private BaseCharacterAnimatorHandler animatorHandler;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
            Debug.LogWarning("[SellerNPC] Inventory tidak ditemukan.");

        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null)
            Debug.LogWarning("[SellerNPC] PlayerStats tidak ditemukan.");

        animatorHandler = GetComponent<BaseCharacterAnimatorHandler>();
    }

    public void InitializeSeller(Item newItem, int newStock)
    {
        item = newItem;
        stock = newStock;

        if (item != null && item.icon != null)
        {
            GameObject floatingIconPrefab = Resources.Load<GameObject>("UI/SellerIconUI");
            if (floatingIconPrefab != null)
            {
                iconGO = Instantiate(floatingIconPrefab);
                iconUI = iconGO.GetComponentInChildren<SellerIconUI>();
                iconUI.Initialize(transform, item.icon);
            }
            else
            {
                Debug.LogWarning("[SellerNPC] ❌ FloatingIconUI prefab tidak ditemukan.");
            }
        }
    }

    public void Interact()
    {
        if (item == null)
        {
            Debug.LogWarning("[SellerNPC] Item belum di-set.");
            return;
        }

        if (stock <= 0)
        {
            DialogManager.Instance.StartSimpleDialog("Maaf, stok sudah habis.", "Penjual");
            return;
        }

        // 🔁 Panggil animasi jualBeli (jika animatorHandler ada dan param tersedia)
        if (animatorHandler != null)
        {
            animatorHandler.PlayAnim("jualBeli", 2f); // durasi disesuaikan
        }

        if (dialogData != null)
        {
            DialogManager.Instance.StartChoiceDialog(
                $"Aku menjual {item.name}, mau beli?",
                OnBuyConfirmed,
                OnBuyDeclined,
                "Penjual");
        }
        else
        {
            Debug.LogWarning("[SellerNPC] DialogData belum di-set.");
        }
    }

    private void OnBuyConfirmed()
    {
        Debug.Log("OnBuyConfirmed triggered");

        if (playerStats == null || inventory == null)
        {
            Debug.LogWarning("[SellerNPC] PlayerStats atau Inventory belum di-assign.");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("[SellerNPC] Item null saat beli.");
            return;
        }

        if (playerStats.coins < item.price)
        {
            Debug.Log("[SellerNPC] Koin kurang untuk beli item.");
            DialogManager.Instance.StartSimpleDialog("Koin kamu kurang.", "Penjual");
            return;
        }

        bool success = playerStats.SpendCoins(item.price);
        if (success)
        {
            inventory.AddItem(item);
            stock--;
            DebugLogManager.Instance.ShowLog($"[SellerNPC] Item {item.name} berhasil dibeli. Stok tersisa: {stock}");
            DialogManager.Instance.StartSimpleDialog("Terima kasih!", "Penjual");
        }
        else
        {
            DebugLogManager.Instance.ShowLog("[SellerNPC] Transaksi gagal saat SpendCoins.");
            DialogManager.Instance.StartSimpleDialog("Transaksi gagal.", "Penjual");
        }
    }

    private void OnBuyDeclined()
    {
        DialogManager.Instance.StartSimpleDialog("Baiklah, semoga harimu menyenangkan.", "Penjual");
    }

    public string GetInteractionPrompt()
    {
        return item != null ? $"Buy {item.name} ({stock} left)" : "Seller";
    }

    private void OnDestroy()
    {
        if (iconGO != null)
        {
            Destroy(iconGO);
        }
    }
}
