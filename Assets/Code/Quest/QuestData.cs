using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    [TextArea] public string description;

    public List<Item> requiredItems;
    public int requiredItemAmount = 1;

    public Item rewardItem;
    public int rewardCoins;
    public int rewardXP;
}
