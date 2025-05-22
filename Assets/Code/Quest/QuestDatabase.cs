using System.Collections.Generic;
using UnityEngine;

public static class QuestDatabase
{
    private static Dictionary<string, QuestData> questDict;

    static QuestDatabase()
    {
        questDict = new Dictionary<string, QuestData>();
        foreach (var quest in Resources.LoadAll<QuestData>("Quests"))
        {
            if (!questDict.ContainsKey(quest.questId))
            {
                questDict[quest.questId] = quest;
            }
        }
    }

    public static QuestData GetQuestById(string id)
    {
        if (questDict.TryGetValue(id, out var quest))
        {
            return quest;
        }

        Debug.LogWarning($"[QuestDatabase] Quest with ID '{id}' not found!");
        return null;
    }
}
