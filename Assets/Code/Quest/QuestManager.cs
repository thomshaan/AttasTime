using System.Collections.Generic;
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
    private Dictionary<string, float> questCooldownTimers = new Dictionary<string, float>();


    private void Update()
    {
        float deltaTimeInDays = Time.deltaTime / 86400f; // Asumsi 1 hari = 86400 detik nyata
        List<string> keys = new List<string>(questCooldownTimers.Keys);
        foreach (var key in keys)
        {
            questCooldownTimers[key] -= deltaTimeInDays;
            if (questCooldownTimers[key] <= 0f)
                questCooldownTimers.Remove(key);
        }
    }

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

    public void StartQuestById(string questId)
    {
        QuestData quest = QuestDatabase.GetQuestById(questId);
        if (quest != null)
        {
            StartQuest(quest);
        }
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

        if (currentQuestData is QuestMakanBajambaData makanBajambaQuest)
        {
            foreach (var req in makanBajambaQuest.modularRequiredItems)
            {
                inventory.RemoveItem(req.item, req.requiredAmount);
            }
        }
        else
        {
            foreach (var item in currentQuestData.requiredItems)
            {
                inventory.RemoveItem(item, currentQuestData.requiredItemAmount);
            }
        }

        currentQuestState = QuestState.Completed;

        if (playerStats != null)
        {
            playerStats.AddCoins(currentQuestData.rewardCoins);
            playerStats.AddXP(currentQuestData.rewardXP);
        }

        if (currentQuestData.rewardItem != null)
        {
            inventory.AddItem(currentQuestData.rewardItem);
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

    public void UpdateQuestState(QuestState newState)
    {
        currentQuestState = newState;
        OnQuestChanged?.Invoke();
    }

    public void UpdateQuestProgress(Item item)
    {
        if (currentQuestData == null || currentQuestState != QuestState.InProgress) return;

        if (currentQuestData.requiredItems.Contains(item))
        {
            int count = inventory.CountOf(item);
            Debug.Log($"Progress quest {currentQuestData.questName}: {count}/{currentQuestData.requiredItemAmount} {item.name}");


            OnQuestChanged?.Invoke();

            if (count >= currentQuestData.requiredItemAmount)
            {
                Debug.Log("Quest selesai, item terkumpul!");
            }
        }
    }

    public bool HasAllItemsModular(List<QuestMakanBajambaData.QuestRequirement> requirements, Inventory inventory)
    {
        foreach (var req in requirements)
        {
            int playerAmount = inventory.CountOf(req.item);
            if (playerAmount < req.requiredAmount)
                return false;
        }
        return true;
    }

    public bool IsQuestOnCooldown(string questId)
    {
        return questCooldownTimers.TryGetValue(questId, out float remaining) && remaining > 0f;
    }

    public void StartQuestCooldown(string questId, float cooldownDays)
    {
        questCooldownTimers[questId] = cooldownDays;
    }


}
