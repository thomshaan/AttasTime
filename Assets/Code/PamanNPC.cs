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

    private enum PamanState { Idle, OfferQuest, QuestInProgress, QuestCompleted }
    private PamanState currentState;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        inventory = FindObjectOfType<Inventory>();
        uiObjective = FindObjectOfType<UIObjective>();
        dialogManager = DialogManager.Instance;
        questIcon = GetComponent<QuestIconController>();
        pamanAnimator = GetComponent<PamanAnimator>();

        UpdateState();
        UpdateQuestIcon();
    }

    private void UpdateState()
    {
        if (questManager == null)
        {
            currentState = PamanState.Idle;
            return;
        }

        if (questManager.currentQuestData == questData)
        {
            switch (questManager.currentQuestState)
            {
                case QuestState.NotStarted:
                    currentState = PamanState.OfferQuest;
                    break;
                case QuestState.InProgress:
                    currentState = PamanState.QuestInProgress;
                    break;
                case QuestState.Completed:
                    currentState = PamanState.OfferQuest;
                    break;
            }
        }
        else
        {
            currentState = PamanState.OfferQuest;
        }
    }



    public void Interact()
    {
        if (dialogManager == null || questManager == null) return;

        pamanAnimator?.PlayAnim("jualBeli", 2f);

        switch (currentState)
        {
            case PamanState.OfferQuest:
                dialogManager.StartChoiceDialog(
                    "Paman: Mau bantu saya mengumpulkan 3 beras?",
                    () =>
                    {
                        questManager.StartQuest(questData);
                        uiObjective.ShowQuest(questData);
                        UpdateState();
                        UpdateQuestIcon();
                        pamanAnimator.PlayAnim("jualBeli", 3f);
                    },
                    () =>
                    {
                        dialogManager.StartSimpleDialog("Paman: Baiklah, kalau berubah pikiran bilang ya!", "Paman");
                    },
                    "Paman"
                );
                break;

            case PamanState.QuestInProgress:
                if (questManager.HasRequiredItems())
                {
                    dialogManager.StartSimpleDialog("Kamu sudah mengumpulkan beras. Terima kasih, misi selesai!", "Paman");
                    questManager.CompleteQuest();
                    uiObjective.Hide();
                    UpdateState();
                    UpdateQuestIcon();
                    pamanAnimator.PlayAnim("bawaBarang", 2f);
                }
                else
                {
                    dialogManager.StartSimpleDialog("Kamu belum cukup beras, ayo lanjutkan mengumpulkan!", "Paman");
                }
                break;

            case PamanState.QuestCompleted:
                dialogManager.StartSimpleDialog("Sedang ada quest lain yang harus diselesaikan dulu.", "Paman");
                break;

            default:
                dialogManager.StartSimpleDialog("Hai Atta!", "Paman");
                break;
        }
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

    public string GetInteractionPrompt()
    {
        return "Tekan [E] untuk berbicara dengan Paman";
    }
}
