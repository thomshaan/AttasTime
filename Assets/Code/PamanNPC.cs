using UnityEngine;

public class PamanNPC : MonoBehaviour, IInteractable
{
    public QuestData questData;

    private QuestManager questManager;
    private Inventory inventory;
    private UIObjective uiObjective;
    private DialogManager dialogManager;
    private FloatingIconController iconController;
    public Sprite questAvailableIcon;
    public Sprite questInProgressIcon;

    private void Start()
    {
        iconController = GetComponent<FloatingIconController>();
        questManager = FindObjectOfType<QuestManager>();
        inventory = FindObjectOfType<Inventory>();
        uiObjective = FindObjectOfType<UIObjective>();
        dialogManager = DialogManager.Instance;
    }

    public void UpdateQuestIcon()
    {
        if (questManager.currentQuestData == questData && questManager.currentQuestState == QuestState.InProgress)
        {
            iconController.ShowIcon(questInProgressIcon, transform);
        }
        else if (questManager.currentQuestData != questData || questManager.currentQuestState == QuestState.NotStarted)
        {
            iconController.ShowIcon(questAvailableIcon, transform);
        }
        else
        {
            iconController.HideIcon();
        }
    }

    public void Interact()
    {
        if (questManager.IsQuestInProgress() && questManager.currentQuestData == questData)
        {
            if (questManager.HasRequiredItems())
            {
                dialogManager.StartSimpleDialog("Kamu sudah mengumpulkan beras. Terima kasih, misi selesai!", "Paman");
                questManager.CompleteQuest();
                uiObjective.Hide();
            }
            else
            {
                dialogManager.StartSimpleDialog("Kamu belum cukup beras, ayo lanjutkan mengumpulkan!", "Paman");
            }
        }
        else if (!questManager.IsQuestInProgress())
        {
            dialogManager.StartChoiceDialog(
                "Paman: Mau bantu saya mengumpulkan 3 beras?",
                () =>
                {
                    questManager.StartQuest(questData);
                    uiObjective.ShowQuest(questData);
                },
                () =>
                {
                    dialogManager.StartSimpleDialog("Paman: Baiklah, kalau berubah pikiran bilang ya!", "Paman");
                },
                "Paman"
            );
        }
        else
        {
            dialogManager.StartSimpleDialog("Sedang ada quest lain yang harus diselesaikan dulu.", "Paman");
        }
    }

    // Optional: implement GetInteractionPrompt jika diperlukan oleh IInteractable
    public string GetInteractionPrompt()
    {
        return "Tekan [E] untuk berbicara dengan Paman";
    }
}
