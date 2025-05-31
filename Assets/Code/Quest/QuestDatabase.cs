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
            if (string.IsNullOrEmpty(quest.questId))
            {
                Debug.LogError($"[QuestDatabase] Quest '{quest.name}' memiliki questId yang null atau kosong!");
                continue; // Lewati quest ini
            }

            if (!questDict.ContainsKey(quest.questId))
            {
                questDict[quest.questId] = quest;
            }
            else
            {
                Debug.LogWarning($"[QuestDatabase] Duplikat questId ditemukan: {quest.questId}. Mengabaikan yang kedua.");
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
