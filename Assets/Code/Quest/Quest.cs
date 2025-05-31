using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questID;
    public string description;
    public int targetItemID; // ID beras
    public int targetAmount;
    public int currentAmount;
    public float timeLimit; // 1 hari waktu dalam game (misal dalam jam atau detik)
    public float startTime; // waktu mulai quest
    public bool isActive;
    public bool isCompleted;
}
