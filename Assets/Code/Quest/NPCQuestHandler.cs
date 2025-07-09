using System.Collections;
using UnityEngine;

public enum NPCRole { Syekh, Ibu }

/// <summary>
/// Unified handler for 'Makan Bajamba' quest interactions on both Syekh & Ibu NPCs.
/// Assign role, quest data, and dependencies in the Inspector.
/// </summary>
public class NPCQuestHandler : MonoBehaviour, IInteractable
{
    [Header("Configuration")]
    public NPCRole role;
    public QuestMakanBajambaData questData;

    [Header("References (assign in Inspector)")]
    public QuestManager questManager;
    public Inventory inventory;
    public DialogManager dialogManager;
    public Animator animator;
    public UIObjective uiObjective;

    [Header("Syekh Settings")]
    public QuestIconController questIcon;
    public CutsceneControllerMakanBajamba cutsceneController;

    [Header("Ibu Settings")]
    public float cookDuration = 5f;

    private bool isCooking = false;

    public void Interact()
    {
        if (questData == null || questManager == null) return;
        if (role == NPCRole.Syekh) InteractSyekh();
        else InteractIbu();
    }

    private void InteractSyekh()
    {
        animator?.SetTrigger("doa");

        // Update quest icon manually
        if (questIcon != null)
        {
            if (questManager.currentQuestData == questData)
            {
                switch (questManager.currentQuestState)
                {
                    case QuestState.NotStarted: questIcon.ShowNotStarted(); break;
                    case QuestState.InProgress: questIcon.ShowInProgress(); break;
                    case QuestState.CookingDone: questIcon.ShowInProgress(); break;
                    case QuestState.Completed: questIcon.ShowCompleted(); break;
                }
            }
            else
            {
                questIcon.ShowNotStarted();
            }
        }

        // Branch based on state
        QuestState state = questManager.currentQuestState;
        if (state == QuestState.NotStarted)
        {
            dialogManager.StartChoiceDialog(
                "Syekh: Mau bantu saya siapkan makan bajamba?",
                () => StartQuestFlow(),
                () => dialogManager.StartSimpleDialog("Kalau berubah pikiran bilang ya!", "Syekh"),
                "Syekh");
        }
        else if (state == QuestState.CookingDone)
        {
            dialogManager.StartSimpleDialog("Bagus! Ayo kita rayakan.", "Syekh");
            cutsceneController?.StartCutscene();
        }
        else if (state == QuestState.InProgress)
        {
            bool hasAll = questManager.HasAllItemsModular(questData.modularRequiredItems, inventory);
            dialogManager.StartSimpleDialog(
                hasAll ? "Kamu sudah mengumpulkan semua bahan! Silakan ke Ibu." : "Ayo kumpulkan bahan masakan terlebih dahulu.",
                "Syekh");
        }
        else
        {
            dialogManager.StartSimpleDialog("Terima kasih, tugas sudah selesai.", "Syekh");
        }
    }

    private void StartQuestFlow()
    {
        questManager.StartQuest(questData);
        uiObjective?.ShowQuest(questData);
        dialogManager.StartSimpleDialog("Kumpulkan dulu bahan-bahannya ya!", "Syekh");
    }

    private void InteractIbu()
    {
        QuestState state = questManager.currentQuestState;

        if (questManager.currentQuestData != questData)
        {
            dialogManager.StartSimpleDialog("Hai, aku sedang tidak memasak apa-apa.", "Ibu");
            return;
        }

        if (state == QuestState.Completed)
        {
            dialogManager.StartSimpleDialog("Terima kasih ya, jaga diri baik-baik.", "Ibu");
            return;
        }

        if (state == QuestState.ReadyToComplete)
        {
            dialogManager.StartSimpleDialog("Terima kasih sudah ikut Makan Bajamba, Nak. Ibu bangga!", "Ibu");

            // Finish quest after the dialog ends
            StartCoroutine(FinalizeQuestAfterDialog());
            return;
        }

        if (state != QuestState.InProgress)
        {
            dialogManager.StartSimpleDialog("Hai, aku sedang tidak memasak apa-apa.", "Ibu");
            return;
        }

        if (isCooking)
        {
            dialogManager.StartSimpleDialog("Sedang memasak, mohon tunggu.", "Ibu");
            return;
        }

        bool hasAll = questManager.HasAllItemsModular(questData.modularRequiredItems, inventory);
        if (hasAll)
        {
            dialogManager.StartSimpleDialog("Mari kita mulai memasak!", "Ibu");
            StartCoroutine(CookingRoutine());
        }
        else
        {
            dialogManager.StartSimpleDialog("Kamu belum membawa semua bahan.", "Ibu");
        }
    }

    private IEnumerator CookingRoutine()
    {
        isCooking = true;
        animator?.SetTrigger("masakRendang");

        foreach (var req in questData.modularRequiredItems)
            inventory.RemoveItem(req.item, req.requiredAmount);

        yield return new WaitForSeconds(cookDuration);

        var rendang = ItemDatabase.GetItemById("rendang");
        if (rendang != null)
            inventory.AddItem(rendang);

        dialogManager.StartSimpleDialog("Makanan sudah siap! Silakan antar ke Rumah Gadang.", "Ibu");
        questManager.UpdateQuestState(QuestState.CookingDone);
        isCooking = false;
    }

    public string GetInteractionPrompt()
    {
        return role == NPCRole.Syekh ? "Tekan [E] untuk berbicara dengan Syekh" : "Tekan [E] untuk berbicara dengan Ibu";
    }

    private IEnumerator FinalizeQuestAfterDialog()
    {
        yield return new WaitUntil(() => !dialogManager.dialogBox.activeSelf);

        questManager.CompleteQuest();
        questManager.StartQuestCooldown(questData.questId, 1f);
        SaveManager.Instance.SaveGame();
    }
}
