using UnityEngine;

public class SellerNPC : MonoBehaviour, IInteractable
{
    private enum SellerState { Idle, Offer, Buying, Selling, OutOfStock }
    private SellerState currentState = SellerState.Idle;

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

        currentState = (stock > 0) ? SellerState.Offer : SellerState.OutOfStock;
    }

    public void Interact()
    {
        if (item == null || dialogData == null)
        {
            Debug.LogWarning("[SellerNPC] Interact gagal, item/dialogData kosong.");
            return;
        }

        switch (currentState)
        {
            case SellerState.Offer:
                animHandler?.PlayAnim("jualBeli", 2f);
                DialogManager.Instance.StartChoiceDialog(
                    $"Aku menjual {item.name}, mau beli?",
                    () => ChangeState(SellerState.Buying),
                    () => ChangeState(SellerState.Selling),
                    "Penjual"
                );
                break;

            case SellerState.Buying:
                animHandler?.PlayAnim("bawaBarang", 2f);
                TryBuyItem();
                break;

            case SellerState.Selling:
                TrySellItem();
                break;

            case SellerState.OutOfStock:
                DialogManager.Instance.StartSimpleDialog("Maaf, stok sudah habis.", "Penjual");
                break;

            case SellerState.Idle:
            default:
                DialogManager.Instance.StartSimpleDialog("Selamat datang!", "Penjual");
                break;
        }
    }

    private void ChangeState(SellerState next)
    {
        currentState = next;

        // Langsung jalankan behavior setelah transisi state
        switch (next)
        {
            case SellerState.Buying:
                Interact(); // panggil ulang untuk lanjut beli
                break;

            case SellerState.Selling:
                DialogManager.Instance.StartChoiceDialog(
                    "Apa kamu mau jual item ini ke penjual?",
                    TrySellItem,
                    () =>
                    {
                        DialogManager.Instance.StartSimpleDialog("Baiklah, semoga harimu menyenangkan.", "Penjual");
                        currentState = SellerState.Offer;
                    },
                    "Penjual"
                );
                break;
        }
    }

    private void TryBuyItem()
    {
        if (playerStats.coins < item.price)
        {
            DialogManager.Instance.StartSimpleDialog("Koin kamu kurang.", "Penjual");
            currentState = SellerState.Offer;
            return;
        }

        bool success = playerStats.SpendCoins(item.price);
        if (success)
        {
            inventory.AddItem(item);
            stock--;
            DebugLogManager.Instance.ShowLog($"[SellerNPC] Item {item.name} berhasil dibeli. Stok tersisa: {stock}");
            DialogManager.Instance.StartSimpleDialog("Terima kasih!", "Penjual");

            // Update state jika stok habis
            currentState = (stock > 0) ? SellerState.Offer : SellerState.OutOfStock;
        }
        else
        {
            DebugLogManager.Instance.ShowLog("[SellerNPC] Transaksi gagal saat SpendCoins.");
            DialogManager.Instance.StartSimpleDialog("Transaksi gagal.", "Penjual");
            currentState = SellerState.Offer;
        }
    }

    private void TrySellItem()
    {
        animHandler?.PlayAnim("bawaBarang", 2f);

        if (inventory.CountOf(item) <= 0)
        {
            DialogManager.Instance.StartSimpleDialog($"Kamu tidak punya {item.name} untuk dijual.", "Penjual");
            currentState = SellerState.Offer;
            return;
        }

        int sellPrice = Mathf.Max(1, item.price - 5);
        bool removed = inventory.RemoveItem(item, 1);
        if (removed)
        {
            playerStats.AddCoins(sellPrice);
            DialogManager.Instance.StartSimpleDialog($"Terima kasih! {item.name} berhasil dijual seharga {sellPrice} koin.", "Penjual");
        }
        else
        {
            DialogManager.Instance.StartSimpleDialog($"Gagal menjual {item.name}.", "Penjual");
        }

        currentState = SellerState.Offer;
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
