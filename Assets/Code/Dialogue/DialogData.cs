using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for a sequence of dialog lines used in quests and cutscenes.
/// </summary>
[CreateAssetMenu(menuName = "Quest/Dialogue Data", fileName = "NewDialogData")]
public class DialogData : ScriptableObject
{
    public List<DialogLine> lines = new List<DialogLine>();

    [System.Serializable]
    public class DialogLine
    {
        [Tooltip("Name of the speaker.")]
        public string speaker;

        [Tooltip("The text to display.")]
        [TextArea(2, 5)]
        public string text;

        [Tooltip("Optional image to show with this line.")]
        public Sprite image;

        [Tooltip("If this line presents a choice to the player.")]
        public bool isChoice;

        [Tooltip("List of choices if isChoice is true.")]
        public List<ChoiceOption> choices;
    }

    [System.Serializable]
    public class ChoiceOption
    {
        [Tooltip("Text of the choice.")]
        public string choiceText;

        [Tooltip("Callback ID or trigger info for branching logic.")]
        public string choiceKey;
    }
}
