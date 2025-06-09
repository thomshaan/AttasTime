using System.Collections;
using UnityEngine;

public class NPCIbu : MonoBehaviour, IInteractable
{
    public QuestMakanBajambaData questData;
    private QuestManager questManager;
    private Inventory inventory;
    private DialogManager dialogManager;
    private IbuAnimator ibuAnimator;

    private bool isCooking = false;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        inventory = FindObjectOfType<Inventory>();
        dialogManager = DialogManager.Instance;
        ibuAnimator = GetComponent<IbuAnimator>();
    }

    public void Interact()
    {
        if (questManager.currentQuestData != questData)
        {
            dialogManager.StartSimpleDialog("Hai, aku sedang tidak memasak apa-apa.", "Ibu");
            return;
        }

        if (isCooking)
        {
            dialogManager.StartSimpleDialog("Sedang memasak, mohon tunggu.", "Ibu");
            return;
        }

        if (questManager.currentQuestState == QuestState.InProgress)
        {
            // Pastikan modularRequiredItems diakses dengan casting
            if (questData != null && questData is QuestMakanBajambaData makanBajambaQuest)
            {
                if (questManager.HasAllItemsModular(makanBajambaQuest.modularRequiredItems, inventory))
                {
                    dialogManager.StartSimpleDialog("Mari kita mulai memasak!", "Ibu");
                    StartCoroutine(CookingProcess(makanBajambaQuest));
                }
                else
                {
                    dialogManager.StartSimpleDialog("Kamu belum membawa semua bahan masakan.", "Ibu");
                }
            }
        }
        else
        {
            dialogManager.StartSimpleDialog("Tidak ada yang harus dimasak sekarang.", "Ibu");
        }
    }

    private IEnumerator CookingProcess(QuestMakanBajambaData makanBajambaQuest)
    {
        isCooking = true;
        ibuAnimator.PlayAnim("masakRendang", 5f);

        foreach (var req in makanBajambaQuest.modularRequiredItems)
        {
            inventory.RemoveItem(req.item, req.requiredAmount);
        }

        yield return new WaitForSeconds(5f);

        Item makanBajambaItem = ItemDatabase.GetItemById("MakanBajamba"); // pakai GetItemById
        if (makanBajambaItem != null)
            inventory.AddItem(makanBajambaItem);

        dialogManager.StartSimpleDialog("Makan bajamba sudah siap! Silakan antar ke Rumah Gadang.", "Ibu");

        questManager.UpdateQuestState(QuestState.CookingDone);

        isCooking = false;
    }

    public string GetInteractionPrompt()
    {
        return "Tekan [E] untuk berbicara dengan Ibu";
    }
}
