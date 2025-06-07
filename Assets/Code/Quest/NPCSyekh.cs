using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCSyekh : MonoBehaviour, IInteractable
{
    public QuestMakanBajambaData questData; // QuestMakanBajambaData digunakan untuk quest ini
    private QuestManager questManager;
    private Inventory inventory;
    private DialogManager dialogManager;
    private QuestIconController questIcon;
    private PlayerStats playerStats;
    private SyekhAnimator syekhAnimator;
    private UIObjective uiObjective;
    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        inventory = FindObjectOfType<Inventory>();
        dialogManager = DialogManager.Instance;
        questIcon = GetComponent<QuestIconController>();
        playerStats = FindObjectOfType<PlayerStats>();
        syekhAnimator = GetComponent<SyekhAnimator>();
        uiObjective = FindObjectOfType<UIObjective>();

        UpdateQuestIcon();
    }

    // Memperbarui ikon status quest
    public void UpdateQuestIcon()
    {
        if (questIcon == null || questManager == null) return;

        if (questManager.currentQuestData == questData)
        {
            switch (questManager.currentQuestState)
            {
                case QuestState.NotStarted:
                    questIcon.ShowNotStarted();
                    break;
                case QuestState.InProgress:
                    questIcon.ShowInProgress();
                    break;
                case QuestState.Completed:
                    questIcon.ShowCompleted();
                    break;
            }
        }
        else
        {
            questIcon.ShowNotStarted();
        }
    }

    public void Interact()
    {
        if (questManager == null || dialogManager == null)
        {
            Debug.LogError("QuestManager or DialogManager is not assigned.");
            return;
        }

        if (syekhAnimator != null)
            syekhAnimator.PlayAnim("doa", 2f);

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "WorldMain")
        {
            if (questManager.IsQuestOnCooldown(questData.questId))
            {
                dialogManager.StartSimpleDialog("Quest makan bajamba sedang cooldown, coba lagi nanti.", "Syekh");
                return;
            }

            if (!questManager.IsQuestInProgress())
            {
                dialogManager.StartChoiceDialog(
                    "Syekh: Mau bantu saya siapkan makan bajamba?",
                    () =>
                    {
                        questManager.StartQuest(questData);  // Mulai quest
                        UpdateQuestIcon();

                        // Tambahkan pengecekan uiObjective
                        if (uiObjective != null)
                        {
                            uiObjective.ShowQuest(questData);   // Tampilkan UI quest
                        }
                        else
                        {
                            Debug.LogError("UIObjective is not assigned or not found!");
                        }

                        dialogManager.StartSimpleDialog("Kumpulkan dulu bahan-bahannya ya!", "Syekh");
                    },
                    () =>
                    {
                        dialogManager.StartSimpleDialog("Kalau berubah pikiran bilang ya!", "Syekh");
                    },
                    "Syekh");
            }
            else if (questManager.currentQuestData == questData)
            {
                if (questData != null && questData is QuestMakanBajambaData makanBajambaQuest)
                {
                    if (questManager.HasAllItemsModular(makanBajambaQuest.modularRequiredItems, inventory))
                    {
                        dialogManager.StartSimpleDialog("Kamu sudah mengumpulkan semua bahan! Silakan lapor ke Ibu untuk memasak.", "Syekh");
                    }
                    else
                    {
                        dialogManager.StartSimpleDialog("Kamu belum mengumpulkan semua bahan, ayo teruskan!", "Syekh");
                    }
                }
            }
            else
            {
                dialogManager.StartSimpleDialog("Sedang ada quest lain yang harus diselesaikan dulu.", "Syekh");
            }
        }
        else
        {
            dialogManager.StartSimpleDialog("Tidak ada yang harus dilakukan di sini.", "Syekh");
        }
    }


    public string GetInteractionPrompt()
    {
        return "Tekan [E] untuk berbicara dengan Syekh";
    }
}
