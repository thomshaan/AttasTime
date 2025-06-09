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

    private BaseCharacterAnimatorHandler animHandler;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        playerStats = FindObjectOfType<PlayerStats>();
        animHandler = GetComponent<BaseCharacterAnimatorHandler>();

        if (inventory == null) Debug.LogWarning("[SellerNPC] Inventory tidak ditemukan.");
        if (playerStats == null) Debug.LogWarning("[SellerNPC] PlayerStats tidak ditemukan.");
        if (animHandler == null) Debug.LogWarning("[SellerNPC] SellerAnimatorHandler tidak ditemukan.");
    }

    public void InitializeSeller(Item newItem, int newStock)
    {
        item = newItem;
        stock = newStock;

        if (item != null && item.icon != null)
        {
            if (iconUI == null)
            {
                GameObject floatingIconPrefab = Resources.Load<GameObject>("UI/SellerIconUI");
                if (floatingIconPrefab != null)
                {
                    iconGO = Instantiate(floatingIconPrefab);
                    iconUI = iconGO.GetComponentInChildren<SellerIconUI>();
                }
                else
                {
                    Debug.LogWarning("[SellerNPC] ❌ FloatingIconUI prefab tidak ditemukan.");
                }
            }

            if (iconUI != null)
            {
                iconUI.Initialize(transform, item.icon);
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

        if (dialogData != null)
        {
            animHandler?.PlayAnim("jualBeli", 2f);

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
        animHandler?.PlayAnim("bawaBarang", 2f);

        if (playerStats == null || inventory == null) return;
        if (item == null) return;

        if (playerStats.coins < item.price)
        {
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
        if (iconUI != null)
        {
            iconUI.HideIcon();
            iconUI = null;
        }

        if (iconGO != null)
        {
            Destroy(iconGO);
            iconGO = null;
        }
    }
}
