using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for a sequence of dialog lines used in quests and cutscenes.
/// </summary>
[CreateAssetMenu(menuName = "Quest/Dialogue Data", fileName = "NewDialogData")]
public class DialogData : ScriptableObject
{
    [Tooltip("List of dialog lines to display in sequence.")]
    public List<DialogLine> lines = new List<DialogLine>();
}
