using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIObjective : MonoBehaviour
{
    public GameObject panelObjective; // Panel UI yang berisi info quest
    public TextMeshProUGUI textQuestName;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textProgress;
    private Inventory inventory;

    private QuestData currentQuest;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>(); // Sesuaikan akses inventory-mu
        panelObjective.SetActive(false);

        QuestManager.Instance.OnQuestChanged += UpdateUI;
    }

    public void ShowQuest(QuestData quest)
    {
        currentQuest = quest;
        panelObjective.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        panelObjective.SetActive(false);
        currentQuest = null;
    }

    private void UpdateUI()
    {
        if (currentQuest == null) return;

        textQuestName.text = currentQuest.questName;
        textDescription.text = currentQuest.description;

        int collected = 0;
        if (inventory != null)
        {
            foreach (var item in currentQuest.requiredItems)
            {
                collected += inventory.CountOf(item);
            }
        }

        textProgress.text = $"Beras: {collected}/{currentQuest.requiredItemAmount}";
        textProgress.color = collected >= currentQuest.requiredItemAmount ? Color.green : Color.red;
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestChanged -= UpdateUI;
    }
}
