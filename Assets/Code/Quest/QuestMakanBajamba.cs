using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestMakanBajamba", menuName = "Quest/MakanBajamba")]
public class QuestMakanBajambaData : QuestData
{
    [System.Serializable]
    public class QuestRequirement
    {
        public Item item;
        public int requiredAmount;
    }

    // Ganti nama properti agar tidak bentrok dengan QuestData.requiredItems
    public List<QuestRequirement> modularRequiredItems;
    public int cooldownDays = 3;
}
