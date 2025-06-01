using UnityEngine;

public class PamanNPC : MonoBehaviour, IInteractable
{
    public QuestData questData;
    private QuestManager questManager;
    private Inventory inventory;
    private UIObjective uiObjective;
    private DialogManager dialogManager;
    private QuestIconController questIcon;
    private PamanAnimator pamanAnimator;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        inventory = FindObjectOfType<Inventory>();
        uiObjective = FindObjectOfType<UIObjective>();
        dialogManager = DialogManager.Instance;
        questIcon = GetComponent<QuestIconController>();
        pamanAnimator = GetComponent<PamanAnimator>();

        UpdateQuestIcon();
    }

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
        if (questManager == null || dialogManager == null) return;

        if (pamanAnimator != null)
            pamanAnimator.PlayAnim("jualBeli", 2f); // animasi saat mulai interaksi

        if (questManager.IsQuestInProgress() && questManager.currentQuestData == questData)
        {
            if (questManager.HasRequiredItems())
            {
                dialogManager.StartSimpleDialog("Kamu sudah mengumpulkan beras. Terima kasih, misi selesai!", "Paman");
                questManager.CompleteQuest();
                uiObjective.Hide();
                UpdateQuestIcon();

                if (pamanAnimator != null)
                    pamanAnimator.PlayAnim("bawaBarang", 2f); // animasi beri item
            }
            else
            {
                pamanAnimator.PlayAnim("jualBeli", 2f);
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
                    UpdateQuestIcon();

                    if (pamanAnimator != null)
                        pamanAnimator.PlayAnim("jualBeli", 3f); // animasi tanam saat mulai quest
                },
                () =>
                {
                    pamanAnimator.PlayAnim("jualBeli", 2f);
                    dialogManager.StartSimpleDialog("Paman: Baiklah, kalau berubah pikiran bilang ya!", "Paman");
                },
                "Paman"
            );
        }
        else
        {
            pamanAnimator.PlayAnim("jualBeli", 2f);
            dialogManager.StartSimpleDialog("Sedang ada quest lain yang harus diselesaikan dulu.", "Paman");
        }
    }

    public string GetInteractionPrompt()
    {
        return "Tekan [E] untuk berbicara dengan Paman";
    }
}
