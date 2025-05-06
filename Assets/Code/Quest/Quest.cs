using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questName;
    public string description;
    public QuestState state = QuestState.NotStarted;

    public List<Item> requiredItems = new List<Item>();
    public Item rewardItem;
    public int rewardCoins;
    public int rewardXP;
}
