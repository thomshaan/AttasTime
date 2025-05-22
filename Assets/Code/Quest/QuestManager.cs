using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest")]
    public QuestData currentQuestData;
    public QuestState currentQuestState = QuestState.NotStarted;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Inventory inventory;

    public delegate void QuestUpdated();
    public event QuestUpdated OnQuestChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartQuest(QuestData quest)
    {
        currentQuestData = quest;
        currentQuestState = QuestState.InProgress;
        OnQuestChanged?.Invoke();
    }

    public bool IsQuestInProgress()
    {
        return currentQuestData != null && currentQuestState == QuestState.InProgress;
    }

    public bool HasRequiredItems()
    {
        if (inventory == null || currentQuestData == null) return false;

        foreach (Item item in currentQuestData.requiredItems)
        {
            if (inventory.CountOf(item) < currentQuestData.requiredItemAmount)
                return false;
        }

        return true;
    }

    public void CompleteQuest()
    {
        if (currentQuestData == null || currentQuestState != QuestState.InProgress)
            return;

        currentQuestState = QuestState.Completed;

        // Grant rewards
        if (playerStats != null)
        {
            playerStats.AddCoins(currentQuestData.rewardCoins);
            playerStats.AddXP(currentQuestData.rewardXP);
        }

        if (currentQuestData.rewardItem != null)
        {
            inventory.SendMessage("AddItem", currentQuestData.rewardItem);
        }

        OnQuestChanged?.Invoke();
    }

    public (string questId, string questState) GetQuestStateData()
    {
        if (currentQuestData != null)
            return (currentQuestData.questId, currentQuestState.ToString());
        else
            return ("", "NotStarted");
    }

    public void LoadQuestStateData(string questId, string questState)
    {
        if (string.IsNullOrEmpty(questId)) return;

        QuestData loadedQuest = QuestDatabase.GetQuestById(questId); // You need this helper
        currentQuestData = loadedQuest;
        currentQuestState = (QuestState)System.Enum.Parse(typeof(QuestState), questState);
        OnQuestChanged?.Invoke();
    }

}
